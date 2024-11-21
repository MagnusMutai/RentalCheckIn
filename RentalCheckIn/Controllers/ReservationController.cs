
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

    [HttpGet("AllTableReservations")]
    public async Task<IActionResult> GetAllTableReservations()
    {
        try
        {
            var reservations = await reservationService.GetAllTableReservationsAsync();
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
    
    
    [HttpGet("Settings")]
    public async Task<IActionResult> GetSettingsAsync()
    {
        try
        {
            var settings = await reservationService.GetSettingsAsync();
            if (settings == null )
            {
                return NotFound();
            }
            else
            {
                return Ok(settings);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                  "Error retrieving Data from Database");
        }
    }

    [HttpGet("CheckInFormReservation/{reservationId}")]
    public async Task<IActionResult> GetCheckInFormReservationById(uint reservationId)
    {
        try
        {
            var reservations = await reservationService.GetCheckInReservationByIdAsync(reservationId);
            if (reservations == null)
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReservation(uint id, [FromBody] CheckInReservationUpdateDTO checkInModel)
    {
        if (id != checkInModel.Id)
            return BadRequest("Reservation ID mismatch");

        var result = await reservationService.UpdateCheckInReservationAsync(checkInModel);

        if (!result)
            return StatusCode(500, "Failed to update reservation");

        return NoContent();
    }
}
