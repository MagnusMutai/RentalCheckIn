namespace RentalCheckIn.Repositories;
public interface IAppartmentRepository
{
    Task<IEnumerable<string>> GetDistinctAppartmentNames();
}
