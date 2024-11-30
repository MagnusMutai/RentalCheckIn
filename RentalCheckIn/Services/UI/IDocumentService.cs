namespace RentalCheckIn.Services.UI;
public interface IDocumentService
{
    Task<string> GenerateAndSendCheckInFormAsync(CheckInReservationDTO model, string culture);
}
