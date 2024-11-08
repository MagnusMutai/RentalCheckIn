
namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly IReservationBusinessService reservationService;

    public ReservationController(IReservationBusinessService reservationService) 
    {
        this.reservationService = reservationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllReservations()
    {
        try
        {
            var reservations = await reservationService.GetAllReservationsAsync();
            if (reservations == null )
            {
                return NotFound();
            }
            else
            {
                return Ok(reservations);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                  "Error retrieving Data from Database");
        }
    }

}
