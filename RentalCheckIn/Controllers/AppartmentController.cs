
namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppartmentController : ControllerBase
{
    private readonly IAppartmentBusinessService appartmentBusinessService;

    public AppartmentController(IAppartmentBusinessService appartmentBusinessService)
    {
        this.appartmentBusinessService = appartmentBusinessService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDistinctAppartmentNames()
    {
        try
        {
            var appartmentNames = await appartmentBusinessService.GetDistinctAppartmentNames();
            if (appartmentNames == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(appartmentNames);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                  "Error retrieving Data from Database");
        }
    }
}
