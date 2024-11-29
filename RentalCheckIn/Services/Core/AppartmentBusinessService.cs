//using RentalCheckIn.Controllers;

//namespace RentalCheckIn.Services.Core;

//public class ApartmentBusinessService : IApartmentBusinessService
//{
//    private readonly IApartmentRepository ApartmentRepository;
//    private readonly ILogger<ApartmentBusinessService> logger;

//    public ApartmentBusinessService(IApartmentRepository ApartmentRepository, ILogger<ApartmentBusinessService> logger)
//    {
//        this.ApartmentRepository = ApartmentRepository;
//        this.logger = logger;
//    }
//    public async Task<IEnumerable<string>> GetDistinctApartmentNames()
//    {
//        try
//        {
//            return await ApartmentRepository.GetDistinctApartmentNames();
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "An unexpected error occurred in AccountService while trying to login LHost.");
//            return Enumerable.Empty<string>();
//        }
//    }
//}
