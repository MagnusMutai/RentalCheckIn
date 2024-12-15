namespace RentalCheckIn.Services.UI;
public interface IDocumentService
{
    Task<OperationResult> GenerateAndSendCheckInFormAsync(CheckInReservationDTO model, string culture);
}
