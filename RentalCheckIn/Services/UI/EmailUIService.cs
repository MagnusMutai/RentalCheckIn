
using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;

namespace RentalCheckIn.Services.UI;

public class EmailUIService : IEmailUIService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<EmailUIService> logger;
    private readonly IStringLocalizer<Resource> localizer;

    public EmailUIService(HttpClient httpClient, ILogger<EmailUIService> logger, IStringLocalizer<Resource> localizer)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        this.localizer = localizer;
    }
    public async Task<OperationResult> SendEmailWithAttachmentAsync(SendEmailRequest sendEmailRequest)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/email/email-with-attachment", sendEmailRequest);

            if (response.IsSuccessStatusCode) 
            { 
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = localizer["UnexpectedErrorOccurred"]
                    };
                }

                return await response.Content.ReadFromJsonAsync<OperationResult>();
            }

            return new OperationResult
            {
                IsSuccess = false,
                Message = localizer["Error.TryAgainLater"]
            };
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "An unexpected error has occurred in EmailUIService while trying to send an email with an attachment.");

            return new OperationResult
            {
                IsSuccess = false,
                Message = localizer["UnexpectedErrorOccurred"]
            };
        }
    }
}
