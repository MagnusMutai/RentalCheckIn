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

    [HttpPost("apartments/names")]
    public async Task<ActionResult<Dictionary<uint, string>>> GetApartmentNames([FromBody] ApartmentNamesRequest request)
    {
        try
        {
            var apartmentNames = await localizationService.GetApartmentNamesAsync(request.ApartmentIds, request.Culture);
         
            if (apartmentNames == null) 
            {
                return NotFound();
            }

            return Ok(apartmentNames);
        }
        catch (Exception ex)       
        {
            logger.LogError(ex, "An unexpected error occurred in LocalizationController while trying to register a user.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                      "Error retrieving Data from Database");
        }
    }

    [HttpPost("statuses/labels")]
    public async Task<ActionResult<Dictionary<uint, string>>> GetStatusLabels([FromBody] StatusLabelsRequest request)
    {
        try
        {
            var statusLabels = await localizationService.GetStatusLabelsAsync(request.StatusIds, request.Culture);

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
