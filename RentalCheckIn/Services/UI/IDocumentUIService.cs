namespace RentalCheckIn.Services.UI;
public interface IDocumentUIService
{
    Task<OperationResult<byte[]>> GenerateCheckInFormAsync(OperationRequest docRequest);
    //Task<OperationResult> GenerateAndSendCheckInFormAsync(CheckInReservationDTO model, string culture);
}
