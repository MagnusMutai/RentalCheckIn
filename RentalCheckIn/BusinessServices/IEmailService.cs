namespace RentalCheckIn.BusinessServices
{
    public interface IEmailService
    {
       Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
