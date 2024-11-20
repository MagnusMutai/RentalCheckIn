using Microsoft.AspNetCore.Components;
using SignaturePad;

namespace RentalCheckIn.Components.Shared;
public class CheckInBase : ComponentBase
{
    [Parameter]
    public int Id { get; set; }

    protected CheckInFormDTO checkInModel = new CheckInFormDTO();

    [Inject]
    private NavigationManager NavigationManager { get; set; }

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

    protected override void OnInitialized()
    {
        checkInModel.CheckInDateTime = DateTime.Now;
        checkInModel.Place = "Your Place Name"; // Set the place as needed
        checkInModel.CurrencySymbol = "$"; // Set currency symbol from database as needed
    }

    protected void HandleValidSubmit()
    {
        // Calculate TotalPrice
        CalculateTotalPrice();

        SaveData();
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

    private void SaveData()
    {
        // Implement database save logic here
    }

    private void SharePdf()
    {
        // Implement PDF generation and email sending here
    }

    protected SignaturePadOptions _options = new SignaturePadOptions
    {
        LineCap = LineCap.Round,
        LineJoin = LineJoin.Round,
        LineWidth = 2,
        // Blue ink
    };
}
