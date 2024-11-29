namespace RentalCheckIn.Services.Core;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        this.logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        // Replace with Result patter if necessary
        if (string.IsNullOrWhiteSpace(toEmail))
            return;

        var mail = new MailMessage()
        {
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        try
        {
            mail.To.Add(toEmail);

            using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mail);
        }
        catch (SmtpException ex)
        {
            logger.LogError(ex, "An SMTP error occurred while sending email.");
        }
        finally
        {
            mail.Dispose();
        }
    }
}
