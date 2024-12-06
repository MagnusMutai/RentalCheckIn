namespace RentalCheckIn.Services.UI;
public interface IDocumentService
{
    Task<bool> GenerateAndSendCheckInFormAsync(CheckInReservationDTO model, string culture);
}
