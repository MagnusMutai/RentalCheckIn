namespace RentalCheckIn.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocalizationController : ControllerBase
{
    private readonly ILocalizationService localizationService;

    public LocalizationController(ILocalizationService localizationService)
    {
        this.localizationService = localizationService;
    }

    [HttpPost("apartments/names")]
    public async Task<ActionResult<Dictionary<uint, string>>> GetApartmentNames([FromBody] ApartmentNamesRequest request)
    {
        var apartmentNames = await localizationService.GetApartmentNamesAsync(request.ApartmentIds, request.Culture);
        return Ok(apartmentNames);
    }

    [HttpPost("statuses/labels")]
    public async Task<ActionResult<Dictionary<uint, string>>> GetStatusLabels([FromBody] StatusLabelsRequest request)
    {
        var statusLabels = await localizationService.GetStatusLabelsAsync(request.StatusIds, request.Culture);
        return Ok(statusLabels);
    }

    [HttpGet("reservation/{reservationId}/times")]
    public async Task<ActionResult<(string? CheckInTime, string? CheckOutTime)>> GetReservationTimes(uint reservationId)
    {
        var times = await localizationService.GetReservationTimesAsync(reservationId);
        return Ok(times);
    }
}
