namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LHostController : ControllerBase
{
    private readonly ILHostService lHostService;
    private readonly ILogger<LHostController> logger;

    // Change to use a service don't call the repository directly. 
    public LHostController(ILHostService lHostService, ILogger<LHostController> logger) 
    {
        this.lHostService = lHostService;
        this.logger = logger;
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<LHost>> GetLHostbyEmail(string email)
    {
        try
        {
            var lHost = await lHostService.GetLHostByEmailAsync(email);

            if (lHost == null)
            {
                return NotFound();
            }

            return Ok(lHost);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in LHostController while trying to get LHost by Email.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                              "Error retrieving Data from Database");
        }
    }
    
    [HttpGet("id/{id}")]
    public async Task<ActionResult<LHost>> GetLHostbyId(uint id)
    {
        try
        {
            var lHost = await lHostService.GetLHostByIdAsync(id);

            if (lHost == null)
            {
                return NotFound();
            }

            return Ok(lHost);
            
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in LHostController while trying to get LHost by Id.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                              "Error retrieving Data from Database");
        }
    }
}
