using Microsoft.AspNetCore.Components;
using RentalCheckIn.Services.UI;
using SignaturePad;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RentalCheckIn.Components.Shared;
public class CheckInBase : ComponentBase
{
    protected bool displaySignaturePad;

    private string StatusMessage;

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
        await SharePdf();
        //NavigationManager.NavigateTo("/confirmation");
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
