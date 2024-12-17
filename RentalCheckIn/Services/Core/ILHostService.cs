namespace RentalCheckIn.Services.Core;
public interface ILHostService
{
    Task<LHost> GetLHostByEmailAsync(string mailAddress);
    Task<LHost> GetLHostByIdAsync(uint id);
}
