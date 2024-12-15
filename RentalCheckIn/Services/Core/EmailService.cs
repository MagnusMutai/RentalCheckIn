namespace RentalCheckIn.Services.Core;

public class EmailService : IEmailService
{
    private readonly EmailSettings emailSettings;
    private readonly ILogger<EmailService> logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        this.emailSettings = emailSettings.Value;
        this.logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(toEmail)) return;
        var mail = new MailMessage
        {
            From = new MailAddress(emailSettings.SenderEmail, emailSettings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mail.To.Add(toEmail);

        await SendMailMessageAsync(mail);
    }

    public async Task<OperationResult> SendEmailAsync(string toEmail, string subject, string body, Stream attachment, string attachmentName)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
        {
            return new OperationResult
            {
                IsSuccess = false,
                Message = "The email does not contain the recipient."
            };
        }
        var mail = new MailMessage
        {
            From = new MailAddress(emailSettings.SenderEmail, emailSettings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mail.To.Add(toEmail);

        attachment.Position = 0;
        mail.Attachments.Add(new Attachment(attachment, "application/pdf") { Name = attachmentName });

       return await SendMailMessageAsync(mail);
    }


    private async Task<OperationResult> SendMailMessageAsync(MailMessage mail)
    {
        using var smtp = new SmtpClient(emailSettings.SmtpServer, emailSettings.Port)
        {
            Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),
            EnableSsl = true
        };

        try
        {
            await smtp.SendMailAsync(mail);

            return new OperationResult
            {
                IsSuccess = true
            };
        }
        catch (SmtpException ex)
        {
            logger.LogError(ex, "An SMTP error occurred while sending an email to {Recipients}. Subject: {Subject}",
                string.Join(", ", mail.To.Select(x => x.Address)), mail.Subject);
            
            return new OperationResult
            {
                IsSuccess = false,
                Message = "Unable send Email. Check your newtwork connection and try again."
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while sending an email to {Recipients}. Subject: {Subject}",
                string.Join(", ", mail.To.Select(x => x.Address)), mail.Subject);
            
            return new OperationResult
            {
                IsSuccess = false,
                Message = "An unexpected error occurred while trying to send the email. Please try again later."
            };
        }

    }

}
