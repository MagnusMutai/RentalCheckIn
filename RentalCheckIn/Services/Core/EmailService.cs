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
        if (string.IsNullOrWhiteSpace(toEmail)) return;
        var mail = new MailMessage
        {
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mail.To.Add(toEmail);

        await SendMailMessageAsync(mail);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, Stream attachment, string attachmentName)
    {
        if (string.IsNullOrWhiteSpace(toEmail)) return;
        var mail = new MailMessage
        {
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mail.To.Add(toEmail);

        attachment.Position = 0;
        mail.Attachments.Add(new Attachment(attachment, "application/pdf") { Name = attachmentName });

        await SendMailMessageAsync(mail);
    }


    private async Task SendMailMessageAsync(MailMessage mail)
    {
        using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
        {
            Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
            EnableSsl = true
        };

        try
        {
            await smtp.SendMailAsync(mail);
        }
        catch (SmtpException ex)
        {
            logger.LogError(ex, "An SMTP error occurred while sending an email to {Recipients}. Subject: {Subject}",
                string.Join(", ", mail.To.Select(x => x.Address)), mail.Subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while sending an email to {Recipients}. Subject: {Subject}",
                string.Join(", ", mail.To.Select(x => x.Address)), mail.Subject);
        }

    }

}
