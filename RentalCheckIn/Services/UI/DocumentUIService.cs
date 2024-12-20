using Microsoft.Extensions.Localization;
using RentalCheckIn.Entities;
using RentalCheckIn.Locales;

namespace RentalCheckIn.Services.UI;
public class DocumentUIService : IDocumentUIService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<DocumentUIService> logger;
    private readonly IStringLocalizer<Resource> localizer;

    public DocumentUIService(HttpClient httpClient, ILogger<DocumentUIService> logger, IStringLocalizer<Resource> localizer)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        this.localizer = localizer;
    }


    public async Task<OperationResult<byte[]>> GenerateCheckInFormAsync(OperationRequest docRequest)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/Document/generate-check-in-form", docRequest);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return new OperationResult<byte[]>
                    {
                        IsSuccess = false,
                        Message = localizer["Error.TryAgainLater"]
                    };
                }

                var pdfData = await response.Content.ReadAsByteArrayAsync();

                return new OperationResult<byte[]>
                {
                    IsSuccess = true,
                    Data = pdfData
                };
            }

            return new OperationResult<byte[]>
            {
                IsSuccess = false,
                Message = localizer["Error.TryAgainLater"]
            };

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "n unexpected error has occurred in DocumentUIService while trying to generate check-in form.");

            return new OperationResult<byte[]>
            {
                IsSuccess = false,
                Message = localizer["UnexpectedErrorOccurred"]
            };
        }
    }
}
