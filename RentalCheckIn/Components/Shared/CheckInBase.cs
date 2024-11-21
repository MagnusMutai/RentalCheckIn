using Microsoft.AspNetCore.Components;
using SignaturePad;

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
    }

    protected SignaturePadOptions _options = new SignaturePadOptions
    {
        LineCap = LineCap.Round,
        LineJoin = LineJoin.Round,
        LineWidth = 2,
        // Blue ink
    };
}
