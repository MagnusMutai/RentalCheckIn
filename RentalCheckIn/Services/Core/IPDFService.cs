namespace RentalCheckIn.Services.Core;
public interface IPDFService
{
    MemoryStream FillCheckInFormAsync(CheckInReservationDTO model, string culture);
}
