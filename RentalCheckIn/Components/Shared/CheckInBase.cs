using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;
using SignaturePad;
using System.Drawing;
using System.Globalization;

namespace RentalCheckIn.Components.Shared;
public class CheckInBase : ComponentBase
{
    protected bool ShouldDisplaySignaturePad { get; set; }
    protected string? SignatureValidationError { get; set; }
    protected bool IsCheckingIn {  get; set; }
    protected string? ModalMessage { get; set; }
    protected bool IsSuccessModal { get; set; }
    protected bool IsModalOpen { get; set; }
    private static Color StrokeColor { get; set; } = Color.FromArgb(0, 77, 230);
    protected CheckInReservationDTO CheckInModel { get; set; } = new CheckInReservationDTO();

    [Parameter]
    public int Id { get; set; }
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private IReservationService ReservationService { get; set; }
    [Inject]
    private IDocumentService DocumentService { get; set; }
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    private ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    private ILogger<CheckInBase> Logger { get; set; }
    [Inject]
    protected IStringLocalizer<Resource> Localizer { get; set; }

    protected byte[]? SignatureBytes
    {
        get
        {
            return string.IsNullOrEmpty(CheckInModel.SignatureDataUrl)
                ? null
                : Convert.FromBase64String(CheckInModel.SignatureDataUrl.Split(',')[1]); // Strip "data:image/png;base64,"
        }
        set
        {
            CheckInModel.SignatureDataUrl = value == null
                ? null
                : "data:image/png;base64," + Convert.ToBase64String(value);
        }
    }

    protected DateTime CheckedInAt
    {
        get
        {
            return CheckInModel.CheckedInAt ?? DateTime.UtcNow;
        }
    }


    protected override async Task OnInitializedAsync()
    {
        try
        {
            var reservationData = await ReservationService.GetCheckInReservationByIdAsync((uint)Id);

            if (reservationData != null)
            {
                CheckInModel = reservationData;
            }

        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An unexpected error occurred while trying to load check-In reservation data on OnInitializedAsync.");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            // Get the accessToken if it exists
            var response = await LocalStorage.GetAsync<string>("token");
            Constants.JWTToken = response.Success ? response.Value : "";

            if (AuthStateProvider is CustomAuthStateProvider customAuthStateProvider)
            {
                var authState = await customAuthStateProvider.GetAuthenticationStateAsync();

                if (authState.User.Identity is { IsAuthenticated: true })
                {
                    // User is authenticated, retrieve claims
                    var hostIdClaim = authState.User.FindFirst("nameid");

                    if (hostIdClaim != null && uint.TryParse(hostIdClaim.Value, out uint hostId))
                    {
                        // Assign the HostId to your model
                        CheckInModel.LHostId = hostId;
                    }
                }
                else
                {
                    // User is not authenticated; redirect to login
                    NavigationManager.NavigateTo("/login", forceLoad: false);
                }
            }

            // Check the partial display issues even with this configuration specified
            ShouldDisplaySignaturePad = true;

        }
        catch (Exception ex)
        {
            ModalMessage = Localizer["UnexpectedErrorOccurred"];
            IsSuccessModal = false;
            IsModalOpen = true;
            Logger.LogError(ex, "An unexpected error occurred while trying to authenticate user on OnAfterRenderAsync on CheckIn component.");
        }
    }

    protected async Task HandleValidSubmit()
    {
        IsCheckingIn = true;

        try
        {
            bool isValid = CheckSignatureValidation();

            if (isValid)
            {
                CalculateTotalPrice();
                await SaveData();
                await SharePdf();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An unexpected error occurred while updating check-In data to the db and sharing it via WhatsApp in CheckIn component HandleValidSubmit method.");
        }
        finally
        {
            IsCheckingIn = false;
        }
    }

    private bool CheckSignatureValidation()
    {
        // Check custom field
        if (SignatureBytes == null || SignatureBytes.Length == 0)
        {
            SignatureValidationError = Localizer["Signature.Required"];
            return false;
        }
        else
        {
            SignatureValidationError = null;
            return true;
        }
    }

    protected void OnDateChanged(ChangeEventArgs e)
    {
        if (CheckInModel.CheckInDate != default && CheckInModel.CheckOutDate != default)
        {
            // Convert DateOnly to DateTime for subtraction
            var checkInDateTime = CheckInModel.CheckInDate.ToDateTime(TimeOnly.MinValue);
            var checkOutDateTime = CheckInModel.CheckOutDate.ToDateTime(TimeOnly.MinValue);
            CheckInModel.NumberOfNights = (checkOutDateTime - checkInDateTime).Days;
        }
    }

    // Keep it DRY by passing a string literal for identification
    protected void OnFeeChanged(decimal newValue)
    {
        CheckInModel.ApartmentFee = newValue;
        CalculateTotalPrice();
    }
    protected void OnDepositChanged(decimal newValue)
    {
        CheckInModel.SecurityDeposit = newValue;
        CalculateTotalPrice();
    }

    private void CalculateTotalPrice()
    {
        CheckInModel.TotalPrice = CheckInModel.ApartmentFee + CheckInModel.SecurityDeposit;
        StateHasChanged();
    }

    private async Task SaveData()
    {
        try
        {
            var isSaved = await ReservationService.UpdateCheckInFormReservationAsync(CheckInModel);
            // Implement Result pattern to get more relevant and specific responses from the server.
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An unexpected error occurred while trying to update the user check-In information in CheckIn component.");
        }
    }

    private async Task SharePdf()
    {
        var culture = CultureInfo.CurrentCulture.Name;

        // PDF generation and sharing
        try
        {
            var result = await DocumentService.GenerateAndSendCheckInFormAsync(CheckInModel, culture);

            if (result.IsSuccess)
            {
                ModalMessage = "Check-In form succesfully sent to customer by email.";
                IsSuccessModal = true;
                IsModalOpen = true;
            }
            else
            {
                ModalMessage = result.Message;
                IsSuccessModal = false;
                IsModalOpen = true;
            }
        }
        catch (Exception ex)
        {
            ModalMessage = "Uexpected error has occurred. Unable to send check-In form to guest's email. Please try again later";
            IsSuccessModal = false;
            IsModalOpen = true;
            Logger.LogError(ex, "An unexpected error occurred while trying to merge form data to the document template in CheckIn component.");
        }
    }

    protected SignaturePadOptions _options = new SignaturePadOptions
    {
        LineCap = LineCap.Round,
        LineJoin = LineJoin.Round,
        LineWidth = 2,
        StrokeStyle = StrokeColor
    };

    protected void HandleModalClose()
    {
        IsModalOpen = false;
        ModalMessage = null;

        if (IsSuccessModal)
        {
            NavigationManager.NavigateTo("/");
        }
    }

    protected void HandleBackButton()
    {
        NavigationManager.NavigateTo("/");
    }
}
