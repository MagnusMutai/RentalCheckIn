using Microsoft.AspNetCore.Components;
using SignaturePad;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RentalCheckIn.Components.Shared;
public class CheckInBase : ComponentBase
{
    protected bool displaySignaturePad;

    [Parameter]
    public int Id { get; set; }

    protected CheckInReservationDTO checkInModel = new CheckInReservationDTO();
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private IReservationService ReservationService { get; set; }
    [Inject]
    private IPDFService PDFService { get; set; }

    // Nullable Agreement properties
    protected bool AgreeEnergyConsumption
    {
        get => checkInModel?.AgreeEnergyConsumption ?? false;
        set
        {
            if (checkInModel != null)
                checkInModel.AgreeEnergyConsumption = value;
        }
    }

    protected bool ReceivedKeys
    {
        get => checkInModel?.ReceivedKeys ?? false;
        set
        {
            if (checkInModel != null)
                checkInModel.ReceivedKeys = value;
        }
    }

    protected bool AgreeTerms
    {
        get => checkInModel?.AgreeTerms ?? false;
        set
        {
            if (checkInModel != null)
                checkInModel.AgreeTerms = value;
        }
    }

    // Signature property model
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
        displaySignaturePad = true;
    }
    protected async Task HandleValidSubmit()
    {
        // Calculate TotalPrice
        CalculateTotalPrice();

        await SaveData();
        SharePdf();
        NavigationManager.NavigateTo("/confirmation");
    }

    protected void CalculateTotalPrice()
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

    private void SharePdf()
    {
        // Implement PDF generation and sharing
        PDFService.FillCheckInFormAsync(checkInModel, SignatureBytes);
    }

    protected SignaturePadOptions _options = new SignaturePadOptions
    {
        LineCap = LineCap.Round,
        LineJoin = LineJoin.Round,
        LineWidth = 2,
        // Blue ink
    };
}
