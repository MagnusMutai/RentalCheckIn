namespace RentalCheckIn.Services.UI;
public interface ILHostUIService
{
    Task<LHost> GetLHostByEmail(string mailAddress);
    Task<LHost> GetLHostById(uint lHostId);
}
