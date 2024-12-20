namespace RentalCheckIn.Services.UI;
public interface IDocumentUIService
{
    Task<OperationResult<byte[]>> GenerateCheckInFormAsync(OperationRequest docRequest);
}
