namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocalizationController : ControllerBase
{
    private readonly ILocalizationService localizationService;
    private readonly ILogger<LocalizationController> logger;

    public LocalizationController(ILocalizationService localizationService, ILogger<LocalizationController> logger)
    {
        this.localizationService = localizationService;
        this.logger = logger;
    }

    [HttpGet("languages/flags")]
    public async Task<IActionResult> GetAllLanguageFlags()
    {
        try
        {
            var languageFlags = await localizationService.GetAllLanguageFlagsAsync();
            
            if (languageFlags == null)
            {
                return NotFound();
            }

            return Ok(languageFlags);
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "An unexpected error occurred in LocalizationController while trying to get language flags.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                      "Error retrieving Data from Database");
        }
    }

    [HttpPost("apartments/names")]
    public async Task<ActionResult<Dictionary<uint, string>>> GetApartmentNames([FromBody] LocalizationRequest<uint> request)
    {
        try
        {
            var apartmentNames = await localizationService.GetApartmentNamesAsync(request.Data, request.Culture);
         
            if (apartmentNames == null) 
            {
                return NotFound();
            }

            return Ok(apartmentNames);
        }
        catch (Exception ex)       
        {
            logger.LogError(ex, "An unexpected error occurred in LocalizationController while trying to get language-specific apartment names.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                      "Error retrieving Data from Database");
        }
    }

    [HttpPost("statuses/labels")]
    public async Task<ActionResult<Dictionary<uint, string>>> GetStatusLabels([FromBody] LocalizationRequest<uint> request)
    {
        try
        {
            var statusLabels = await localizationService.GetStatusLabelsAsync(request.Data, request.Culture);

            if (statusLabels == null)
            {
                return NotFound();
            }
            
            return Ok(statusLabels);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in LocalizationController while trying to fetch language-specific StatusLabels.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                      "Error retrieving Data from Database");
        }
    }
}
