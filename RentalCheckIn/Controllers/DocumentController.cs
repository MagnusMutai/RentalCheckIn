using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;

namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService documentService;
    private readonly ILogger<DocumentController> logger;
    private readonly IStringLocalizer<Resource> localizer;

    public DocumentController(IDocumentService documentService, ILogger<DocumentController> logger, IStringLocalizer<Resource> localizer)
    {
        this.documentService = documentService;
        this.logger = logger;
        this.localizer = localizer;
    }

    [HttpPost("generate-check-in-form")]
    public async Task<IActionResult> GenerateCheckInForm([FromBody] OperationRequest request)
    {
        try
        {
            // Generate the PDF in memory
            var pdfStream = await documentService.FillCheckInFormAsync(request.Model, request.Culture);

            if (pdfStream == null)
            {
                return NotFound(new OperationResult
                {
                    IsSuccess = false,
                    Message = "There's an error generating the Quest's Check-In Form document."
                });
            }

            // Ensure the stream is reset to the beginning so we don't have missing data
            pdfStream.Position = 0;
            // Return the PDF stream as a file format.
            return File(pdfStream, "application/pdf", "CheckInForm.pdf");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while generating the Check-In Form PDF.");
         
            return StatusCode(StatusCodes.Status500InternalServerError, new OperationResult
            {
                IsSuccess = false,
                Message = localizer["UnexpectedErrorOccurred"]
            });
        }
    }

}
