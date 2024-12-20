namespace RentalCheckIn.Services.UI;
public interface IEmailUIService
{
    Task<OperationResult> SendEmailWithAttachmentAsync(SendEmailRequest sendEmailRequest);
}
