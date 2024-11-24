namespace RentalCheckIn.Services.Core;
public interface IPDFService
{
    void FillCheckInFormAsync(CheckInReservationDTO model, byte[] sigImage);
}
