namespace RentalCheckIn.Repositories;

public class HostRepository : IHostRepository
{
    private readonly AppDbContext context;
    public HostRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task AddLHostAsync(Lhost lHost)
    {
        context.Lhosts.Add(lHost);
        await context.SaveChangesAsync();
    }

    public async Task<Lhost> GetByEmailAsync(string mailAddress)
    {
        return await context.Lhosts.FirstOrDefaultAsync(h => h.MailAddress == mailAddress);
    }

    public async Task<Lhost> GetByIdAsync(int id)
    {
        return await context.Lhosts.FindAsync(id);
    }

    public async Task UpdateLHostAsync(Lhost lHost)
    {
        context.Lhosts.Update(lHost);
        await context.SaveChangesAsync();
    }


}
