
using RentalCheckIn.Repositories;

namespace RentalCheckIn.Services.Core
{
    public class AppartmentBusinessService : IAppartmentBusinessService
    {
        private readonly IAppartmentRepository appartmentRepository;

        public AppartmentBusinessService(IAppartmentRepository appartmentRepository)
        {
            this.appartmentRepository = appartmentRepository;
        }
        public async Task<IEnumerable<string>> GetDistinctAppartmentNames()
        {
            try
            {
                return await appartmentRepository.GetDistinctAppartmentNames();
            }
            catch (Exception ex)
            {
                // Return an empty list on error
                return Enumerable.Empty<string>();
            }
        }
    }
}
