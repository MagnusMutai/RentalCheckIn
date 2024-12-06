
using RentalCheckIn.Services.Core;

namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly IPDFService pdfService;
    private readonly ILogger<DocumentController> logger;
    private readonly IEmailService emailService;

    public DocumentController(IPDFService pdfService, ILogger<DocumentController> logger, IEmailService emailService)
    {
        this.pdfService = pdfService;
        this.logger = logger;
        this.emailService = emailService;
    }

    [HttpPost("GenerateAndSendCheckInForm")]
    public async Task<IActionResult> GenerateAndSendCheckInForm([FromBody] OperationRequest request)
    {
        try
        {
            // Generate the PDF in memory
            using var pdfStream = pdfService.FillCheckInFormAsync(request.Model, request.Culture);

            string subject = "Your Check-In Form";
            string body = $"Dear {request.Model.GuestFirstName},<br/><br/>" +
                          "Please find your check-in form attached.<br/><br/>" +
                          "Thank you for choosing Snowy.";

            // Send the PDF as an attachment directly from memory
            await emailService.SendEmailAsync(request.Model.MailAddress, subject, body, pdfStream, "CheckInForm.pdf");

            return Ok("Document sent via Email.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in DocumentController while trying to send the check-In Form generated document via WhatsApp.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request. Please try again later.");
        }
    
    }

}
