
using Microsoft.EntityFrameworkCore;

namespace RentalCheckIn.Repositories
{
    public class AppartmentRepository : IAppartmentRepository
    {
        private readonly AppDbContext context;

        public AppartmentRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<string>> GetDistinctAppartmentNames()
        {
            return await context.Apartments
                .Select(r => r.ApartmentName)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync();
        }
    }
}
