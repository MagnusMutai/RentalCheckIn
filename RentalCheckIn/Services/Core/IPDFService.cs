namespace RentalCheckIn.Services.Core;
public interface IPDFService
{
    Task<MemoryStream> FillCheckInFormAsync(CheckInReservationDTO model, string culture);
}
