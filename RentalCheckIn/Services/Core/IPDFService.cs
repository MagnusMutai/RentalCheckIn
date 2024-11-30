namespace RentalCheckIn.Services.Core;
public interface IPDFService
{
    string FillCheckInFormAsync(CheckInReservationDTO model, string culture);
}
