namespace RentalCheckIn.Repositories;
public interface IApartmentRepository
{
    Task<IEnumerable<string>> GetDistinctApartmentNames();
}
