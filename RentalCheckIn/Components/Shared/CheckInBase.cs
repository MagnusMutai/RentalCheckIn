using Microsoft.AspNetCore.Components;
using SignaturePad;
using System.Drawing;

namespace RentalCheckIn.Components.Shared;
public class CheckInBase : ComponentBase
{
    protected bool displaySignaturePad;

    private string StatusMessage;

    protected string signatureValidationError;

    [Parameter]
    public int Id { get; set; }

    private static Color strokeColor = Color.FromArgb(0, 77, 230);

    protected CheckInReservationDTO checkInModel = new CheckInReservationDTO();
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private IReservationService ReservationService { get; set; }
    [Inject]
    private IDocumentService DocumentService{ get; set; }
    // Nullable Agreement properties
    protected bool AgreeEnergyConsumption = true;

    protected bool ReceivedKeys = true;

    protected bool AgreeTerms = true;

    public byte[]? SignatureBytes
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


    protected override async Task OnInitializedAsync()
    {
        var reservationData = await ReservationService.GetCheckInReservationByIdAsync((uint)Id);
        if (reservationData != null) 
        {
            checkInModel =  reservationData;
        }        
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Check the partial display issues even with this configuration specified
        displaySignaturePad = true;
    }
    protected async Task HandleValidSubmit()
    {

        bool isValid = CheckSignatureValidation();

        if (isValid)
        {
            CalculateTotalPrice();
            await SaveData();
            await SharePdf();
        }
    }


    private bool CheckSignatureValidation()
    {
        // Check custom field
        if (SignatureBytes == null || SignatureBytes.Length == 0)
        {
            signatureValidationError = "Signature is required.";
            return false;
        }
        else
        {
            signatureValidationError = null;
            return true;
        }
    }

    private void CalculateTotalPrice()
    {
        checkInModel.TotalPrice = checkInModel.ApartmentFee + checkInModel.SecurityDeposit;
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


    protected void OnFeeChanged(ChangeEventArgs e)
    {
        CalculateTotalPrice();
    }

    private async Task SaveData()
    {
        await ReservationService.UpdateCheckInFormReservationAsync(checkInModel);
    }

    private async Task SharePdf()
    {
        // Implement PDF generation and sharing
        try
        {
            StatusMessage = await DocumentService.GenerateAndSendCheckInFormAsync(checkInModel);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    
    protected SignaturePadOptions _options = new SignaturePadOptions
    {
        LineCap = LineCap.Round,
        LineJoin = LineJoin.Round,
        LineWidth = 2,
        StrokeStyle = strokeColor
        // Blue ink
    };
}
