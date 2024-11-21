namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LHostController : ControllerBase
{
    private readonly ILHostRepository lHostRepository;

    public LHostController(ILHostRepository lHostRepository) 
    {
        this.lHostRepository = lHostRepository;
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<LHost>> GetLHostbyEmail(string email)
    {
        try
        {
            var lHost = await this.lHostRepository.GetLHostByEmailAsync(email);
            if (lHost == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(lHost);
            }

        }
        catch (Exception)
        {

            return StatusCode(StatusCodes.Status500InternalServerError,
                              "Error retrieving Data from Database");
        }
    }
    
    [HttpGet("id/{id}")]
    public async Task<ActionResult<LHost>> GetLHostbyId(uint id)
    {
        try
        {
            var lHost = await this.lHostRepository.GetLHostByIdAsync(id);
            if (lHost == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(lHost);
            }

        }
        catch (Exception)
        {

            return StatusCode(StatusCodes.Status500InternalServerError,
                              "Error retrieving Data from Database");
        }
    }
}
