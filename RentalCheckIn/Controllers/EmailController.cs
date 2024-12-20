using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;

namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly IEmailService emailBusinessService;
    private readonly IStringLocalizer<Resource> localizer;
    private readonly ILogger logger;

    public EmailController(IEmailService emailBusinessService, IStringLocalizer<Resource> localizer, ILogger<EmailController> logger)
    {
        this.emailBusinessService = emailBusinessService;
        this.localizer = localizer;
        this.logger = logger;
    }

    [HttpPost("email-with-attachment")]
    public async Task<IActionResult> SendEmailWithAttachment(SendEmailRequest sendEmailRequest)
    {
        try
        {
            if (sendEmailRequest is null)
            {
                return BadRequest(new OperationResult
                {
                    IsSuccess = false,
                    // Could be changed to another more meaningful error.
                    Message = localizer["UnexpectedErrorOccurred"]
                });
            }
            // Convert the byteArray(generated check-in form) into a memory stream.
            var pdfStream = new MemoryStream(sendEmailRequest?.PDFByteArray);
            pdfStream.Position = 0;
            // OperationResult response
            var emailResponse = await emailBusinessService.SendEmailAsync(sendEmailRequest.Email, sendEmailRequest.Subject, sendEmailRequest.Body, pdfStream, "CheckInForm.pdf");
            
            if (emailResponse is null)
            {
                return BadRequest(new OperationResult 
                { 
                    IsSuccess = false,
                    Message = localizer["UnexpectedErrorOccurred"]
                });
            }

            return Ok(emailResponse);
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "An unexpected error in EmailController while trying to send an email with an attachment.");
            return StatusCode(StatusCodes.Status500InternalServerError, new OperationResult
            {
                IsSuccess = false,
                Message = localizer["UnexpectedErrorOccurred"]
            });
        }
    }
}
