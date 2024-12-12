using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;
using SignaturePad;
using System.Drawing;
using System.Globalization;

namespace RentalCheckIn.Components.Shared;
public class CheckInBase : ComponentBase
{
    protected bool displaySignaturePad;
    protected string? message;
    protected string? signatureValidationError;
    protected string? displaySecretDepositValue;
    protected bool IsCheckingIn;
    protected bool checkAll;
    public string? DisplayModal { get; set; } = "d-block";
    protected bool IsSuccessToast { get; set; } = false;
    private static Color strokeColor = Color.FromArgb(0, 77, 230);
    protected CheckInReservationDTO checkInModel = new CheckInReservationDTO();

    [Parameter]
    public int Id { get; set; }
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private IReservationService ReservationService { get; set; }
    [Inject]
    private IDocumentService DocumentService{ get; set; }
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
            return string.IsNullOrEmpty(checkInModel.SignatureDataUrl)
                ? null
                : Convert.FromBase64String(checkInModel.SignatureDataUrl.Split(',')[1]); // Strip "data:image/png;base64,"
        }
        set
        {
            checkInModel.SignatureDataUrl = value == null
                ? null
                : "data:image/png;base64," + Convert.ToBase64String(value);
        }
    }

    protected DateTime CheckedInAt 
    {
        get; 
        //{
        //    return checkInModel.CheckedInAt
        //}
        set; 
        //{ 
        
        //}
    }


    protected override async Task OnInitializedAsync()
    {
        try
        {
            var reservationData = await ReservationService.GetCheckInReservationByIdAsync((uint)Id);
            
            if (reservationData != null)
            {
                checkInModel = reservationData;
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
                var authState = customAuthStateProvider.NotifyUserAuthentication(Constants.JWTToken);

                if (authState.User.Identity is { IsAuthenticated: true })
                {
                    // User is authenticated, retrieve claims
                    var hostIdClaim = authState.User.FindFirst("nameid");
                  
                    if (hostIdClaim != null && uint.TryParse(hostIdClaim.Value, out uint hostId))
                    {
                        // Assign the HostId to your model
                        checkInModel.LHostId = hostId;
                    }
                }
                else
                {
                    // User is not authenticated; redirect to login
                    NavigationManager.NavigateTo("/login", forceLoad: false);
                }
            }

            // Check the partial display issues even with this configuration specified
            displaySignaturePad = true;

        }
        catch (Exception ex)
        {
            message = Localizer["UnexpectedErrorOccurred"];
            DisplayModal = DisplayModal ?? "d-block";
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
            signatureValidationError = Localizer["Signature.Required"];
            return false;
        }
        else
        {
            signatureValidationError = null;
            return true;
        }
    }

    protected void OnDateChanged(ChangeEventArgs e)
    {
        if (checkInModel.CheckInDate != default && checkInModel.CheckOutDate != default)
        {
            // Convert DateOnly to DateTime for subtraction
            var checkInDateTime = checkInModel.CheckInDate.ToDateTime(TimeOnly.MinValue);
            var checkOutDateTime = checkInModel.CheckOutDate.ToDateTime(TimeOnly.MinValue);
            checkInModel.NumberOfNights = (checkOutDateTime - checkInDateTime).Days;
        }
    }

    // Keep it DRY by passing a string literal for identification
    protected void OnFeeChanged(decimal newValue)
    {
        checkInModel.ApartmentFee = newValue;
        CalculateTotalPrice();
    }
    protected void OnDepositChanged(decimal newValue)
    {
        checkInModel.SecurityDeposit = newValue;
        CalculateTotalPrice();
    }

    private void CalculateTotalPrice()
    {
        checkInModel.TotalPrice = checkInModel.ApartmentFee + checkInModel.SecurityDeposit;
        StateHasChanged();
    }

    private async Task SaveData()
    {
        try
        {
           var isSaved = await ReservationService.UpdateCheckInFormReservationAsync(checkInModel);
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
            bool sharedDocToEmail= await DocumentService.GenerateAndSendCheckInFormAsync(checkInModel, culture);
            if (sharedDocToEmail)
            {
                message = "Check-In form succesfully sent to customer by email.";
                DisplayModal = DisplayModal ?? "d-block";
                IsSuccessToast = true;
            }
            else
            {
                message = "Unable to send check-In form to guest's email. Please try again later";
                DisplayModal = DisplayModal ?? "d-block";
                IsSuccessToast = false;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An unexpected error occurred while trying to merge form data to the document template in CheckIn component.");
        }
    }
    
    protected SignaturePadOptions _options = new SignaturePadOptions
    {
        LineCap = LineCap.Round,
        LineJoin = LineJoin.Round,
        LineWidth = 2,
        StrokeStyle = strokeColor
    };

    protected void HandleCloseModal()
    {
        DisplayModal = null;
        message = null;

        if (IsSuccessToast)
        {
            NavigationManager.NavigateTo("/");
        }
    }

    protected void HandleBackButton()
    {
        NavigationManager.NavigateTo("/");
    }
}
