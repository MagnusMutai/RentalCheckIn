
namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly IReservationService reservationService;
    private readonly ILogger<ReservationController> logger;

    public ReservationController(IReservationService reservationService, ILogger<ReservationController> logger) 
    {
        this.reservationService = reservationService;
        this.logger = logger;
    }

    [HttpGet("all-table-reservations")]
    public async Task<IActionResult> GetAllTableReservations()
    {
        try
        {
            var reservations = await reservationService.GetAllTableReservationsAsync();

            if (reservations == null )
            {
                return NotFound();
            }

            return Ok(reservations);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in ReservationController while trying to get All Reservation Table reservations.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                  "Error retrieving Data from Database");
        }
    }
    
    [HttpGet("settings")]
    public async Task<IActionResult> GetSettingsAsync()
    {
        try
        {
            var settings = await reservationService.GetSettingsAsync();
            if (settings == null )
            {
                return NotFound();
            }

            return Ok(settings);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in ReservationController while trying to get table application settings.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                  "Error retrieving Data from Database");
        }
    }

    [HttpGet("check-in-form-reservation/{reservationId}")]
    public async Task<IActionResult> GetCheckInFormReservationById(uint reservationId)
    {
        try
        {
            var reservations = await reservationService.GetCheckInReservationByIdAsync(reservationId);
            
            if (reservations == null)
            {
                return NotFound();
            }

            return Ok(reservations);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in ReservationController while trying to get Check-In Form reservation by Id.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                  "Error retrieving Data from Database");
        }
    }

    // Patch?
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReservation(uint id, [FromBody] CheckInReservationUpdateDTO checkInModel)
    {
        try
        {
            if (id != checkInModel.Id)
                return BadRequest("Reservation ID mismatch");

            var result = await reservationService.UpdateCheckInReservationPartialAsync(checkInModel);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in ReservationController while trying to partially Update a reservation through check-in form.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                  "Error retrieving Data from Database");
        }
    }
}
