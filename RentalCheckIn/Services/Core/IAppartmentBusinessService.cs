namespace RentalCheckIn.Services.Core;

public interface IAppartmentBusinessService
{
    Task<IEnumerable<string>> GetDistinctAppartmentNames();
}
