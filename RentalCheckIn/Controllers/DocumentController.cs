
namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly IPDFService pdfService;
    private readonly IWhatsAppService whatsAppService;

    public DocumentController(IPDFService pdfService, IWhatsAppService whatsAppService)
    {
        this.pdfService = pdfService;
        this.whatsAppService = whatsAppService;
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
            return BadRequest(ex.Message);
        }
    }

}
