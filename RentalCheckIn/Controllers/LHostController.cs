namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LHostController : ControllerBase
{
    private readonly IHostRepository hostRepository;

    public LHostController(IHostRepository hostRepository) 
    {
        this.hostRepository = hostRepository;
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<LHost>> GetLHost(string email)
    {
        try
        {
            var lHost = await this.hostRepository.GetLHostByEmailAsync(email);
            if (lHost == null)
            {
                return BadRequest();
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
