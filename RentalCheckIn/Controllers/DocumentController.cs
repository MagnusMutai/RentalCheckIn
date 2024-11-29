
namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly IPDFService pdfService;
    private readonly IWhatsAppService whatsAppService;
    private readonly ILogger<DocumentController> logger;

    public DocumentController(IPDFService pdfService, IWhatsAppService whatsAppService, ILogger<DocumentController> logger
)
    {
        this.pdfService = pdfService;
        this.whatsAppService = whatsAppService;
        this.logger = logger;
    }

    [HttpPost("GenerateAndSendCheckInForm")]
    public async Task<IActionResult> GenerateAndSendCheckInForm([FromBody] CheckInReservationDTO model)
    {
        try
        {
            // Generate the document
            string uniqueFileName = pdfService.FillCheckInFormAsync(model);
            // Construct the document URL
            string documentUrl = $"{Request.Scheme}://{Request.Host}/output/{uniqueFileName}";
            // Send the document via WhatsApp
            await whatsAppService.SendDocumentAsync(model.Mobile, documentUrl, "Your check-in form");

            return Ok("Document sent via WhatsApp.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in DocumentController while trying to send the check-In Form generated document via WhatsApp.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request. Please try again later.");
        }
    
    }

}
