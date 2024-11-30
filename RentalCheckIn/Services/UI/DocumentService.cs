namespace RentalCheckIn.Services.UI;
public class DocumentService : IDocumentService
{
    private readonly HttpClient httpClient;

    public DocumentService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<string> GenerateAndSendCheckInFormAsync(CheckInReservationDTO model, string culture)
    {
        try
        {
            var request = new OperationRequest
            {
                Model = model,
                Culture = culture
            };

            var response = await httpClient.PostAsJsonAsync("api/Document/GenerateAndSendCheckInForm", request);

            if (response.IsSuccessStatusCode)
            {
                return "Document sent successfully via WhatsApp.";
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to send document: {errorMessage}");
        }
        catch (Exception ex)
        {
            throw;
        }
    }

}
