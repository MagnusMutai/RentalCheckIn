namespace RentalCheckIn.Repositories;
public interface IHostRepository
{
    Task<LHost> GetByEmailAsync(string mailAddress);
    Task<LHost> GetByIdAsync(int id);
    Task AddLHostAsync(LHost lHost);
    Task UpdateLHostAsync(LHost lHost);
    Task<bool> UpdateLHostPartialAsync(LHost lHost, Action<LHost> patchData);
    Task<LHost> GetUserByEmailVerificationTokenAsync(string token);
}
