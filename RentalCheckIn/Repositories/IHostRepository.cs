namespace RentalCheckIn.Repositories;
public interface IHostRepository
{
    Task<LHost> GetLHostByEmailAsync(string mailAddress);
    Task<LHost> GetLHostByIdAsync(uint id);
    Task AddLHostAsync(LHost lHost);
    Task UpdateLHostAsync(LHost lHost);
    Task<bool> UpdateLHostPartialAsync(LHost lHost, Action<LHost> patchData);
    Task<LHost> GetLHostByEmailVerificationTokenAsync(string token);
    Task<LHost> GetLHostByPasswordResetTokenAsync(string token);
}
