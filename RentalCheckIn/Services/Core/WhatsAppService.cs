using RentalCheckIn.Configuration.WhatsApp;
using System.Net.Http.Headers;

namespace RentalCheckIn.Services.Core;
public class WhatsAppService : IWhatsAppService
{
    private readonly HttpClient httpClient;
    private readonly string accessToken;
    private readonly string phoneNumberId;
    private readonly ILogger<WhatsAppService> logger;

    public WhatsAppService(IHttpClientFactory httpClientFactory, IOptions<WhatsAppSettings> options, ILogger<WhatsAppService> logger)
    {
        httpClient = httpClientFactory.CreateClient();
        var settings = options.Value;
        accessToken = settings.AccessToken;
        phoneNumberId = settings.PhoneNumberId;
        this.logger = logger;
    }

    public async Task SendDocumentAsync(string recipientPhoneNumber, string documentUrl, string caption)
    {
        try
        {
            // Implement Result pattern to return meaningful response to the user.
            recipientPhoneNumber = "XXXXXXXXX";
            var requestUrl = $"https://graph.facebook.com/v13.0/{phoneNumberId}/messages";

            var payload = new
            {
                messaging_product = "whatsapp",
                to = recipientPhoneNumber,
                type = "document",
                document = new
                {
                    link = documentUrl,
                    caption = caption
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.PostAsync(requestUrl, requestContent);
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to send WhatsApp message: {responseContent}");
            }
        }
        catch (HttpRequestException ex) 
        {
            logger.LogError(ex, "An HTTP request error occurred in WhatsAppServicewhile trying to send a WhatsApp message to a Quest.");
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "An unexpected error occurred in WhatsAppServicewhile trying to send a WhatsApp message to a Quest.");
        }
    }
}
