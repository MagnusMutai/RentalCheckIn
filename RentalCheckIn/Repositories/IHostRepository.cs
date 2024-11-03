namespace RentalCheckIn.Repositories;
public interface IHostRepository
{
    Task<Lhost> GetByEmailAsync(string mailAddress);
    Task<Lhost> GetByIdAsync(int id);
    Task AddLHostAsync(Lhost lHost);
    Task UpdateLHostAsync(Lhost lHost);
    Task<bool> UpdateLHostPartialAsync(Lhost lHost, Action<Lhost> patchData);
    Task<Lhost> GetUserByEmailVerificationTokenAsync(string token);
}
