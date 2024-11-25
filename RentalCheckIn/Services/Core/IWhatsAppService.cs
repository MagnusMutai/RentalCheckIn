namespace RentalCheckIn.Services.Core;
public interface IWhatsAppService
{
    Task SendDocumentAsync(string recipientPhoneNumber, string documentUrl, string caption);
}
