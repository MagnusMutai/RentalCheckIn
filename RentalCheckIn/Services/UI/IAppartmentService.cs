namespace RentalCheckIn.Services.UI
{
    public interface IAppartmentService
    {
        Task<IEnumerable<string>> GetDistinctAppartmentNames();
    }
}
