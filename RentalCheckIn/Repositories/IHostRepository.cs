namespace RentalCheckIn.Repositories;
public interface IHostRepository
{
    Task<Lhost> GetByEmailAsync(string mailAddress);
    Task<Lhost> GetByIdAsync(int id);
    Task AddLHostAsync(Lhost lhost);
    Task UpdateLHostAsync(Lhost lhost);
}
