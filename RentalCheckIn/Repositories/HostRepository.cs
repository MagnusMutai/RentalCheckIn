namespace RentalCheckIn.Repositories;

public class HostRepository : IHostRepository
{
    private readonly AppDbContext _context;
    public HostRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddLHostAsync(Lhost lhost)
    {
        _context.Lhosts.Add(lhost);
        await _context.SaveChangesAsync();
    }

    public async Task<Lhost> GetByEmailAsync(string mailAddress)
    {
        return await _context.Lhosts.FirstOrDefaultAsync(h => h.MailAddress == mailAddress);
    }

    public async Task<Lhost> GetByIdAsync(int id)
    {
        return await _context.Lhosts.FindAsync(id);
    }

    public async Task UpdateLHostAsync(Lhost lhost)
    {
        _context.Lhosts.Update(lhost);
        await _context.SaveChangesAsync();
    }
}
