
namespace RentalCheckIn.Repositories;

public class ApartmentRepository : IApartmentRepository
{
    private readonly AppDbContext context;

    public ApartmentRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<string>> GetDistinctApartmentNames()
    {
        return await context.Apartments
            .Select(r => r.ApartmentName)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync();
    }
}
