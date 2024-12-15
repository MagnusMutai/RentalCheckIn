namespace RentalCheckIn.Services.UI;
public class DocumentService : IDocumentService
{
    private readonly HttpClient httpClient;

    public DocumentService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<OperationResult> GenerateAndSendCheckInFormAsync(CheckInReservationDTO model, string culture)
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
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default;
                }

                return await response.Content.ReadFromJsonAsync<OperationResult>();
            }

            return new OperationResult
            { 
                IsSuccess = false,
                Message = "An error has occurred. Please try again later."
            };
            
        }
        catch (Exception ex)
        {
            // Log
            return new OperationResult
            {
                IsSuccess = false,
                Message = "An unexpected error has occurred. Please try again later."
            };
        }
    }

}
