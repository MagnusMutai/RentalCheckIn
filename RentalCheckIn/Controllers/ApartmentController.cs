
//namespace RentalCheckIn.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class ApartmentController : ControllerBase
//{
//    private readonly IApartmentBusinessService ApartmentBusinessService;
//    private readonly ILogger<ApartmentController> logger;

//    public ApartmentController(IApartmentBusinessService ApartmentBusinessService, ILogger<ApartmentController> logger)
//    {
//        this.ApartmentBusinessService = ApartmentBusinessService;
//        this.logger = logger;
//    }

//    [HttpGet]
//    public async Task<IActionResult> GetDistinctApartmentNames()
//    {
//        try
//        {
//            var apartmentNames = await ApartmentBusinessService.GetDistinctApartmentNames();
//            if (apartmentNames == null)
//            {
//                return NotFound();
//            }
//            else
//            {
//                return Ok(apartmentNames);
//            }
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "An unexpected error occurred in ApartmentController while trying to fetch distinct apartment names from the db");
//            return StatusCode(StatusCodes.Status500InternalServerError,
//                  "Error retrieving Data from Database");
//        }
//    }
//}
