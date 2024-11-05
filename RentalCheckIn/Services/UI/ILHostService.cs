namespace RentalCheckIn.Services.UI
{
    public interface ILHostService
    {
        Task<LHost> GetLHostByEmail(string mailAddress);
    }
}
