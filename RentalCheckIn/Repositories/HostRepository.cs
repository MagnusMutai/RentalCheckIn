using Microsoft.EntityFrameworkCore;

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

    public async Task<Lhost> GetUserByEmailVerificationTokenAsync(string token)
    {
        return await context.Lhosts
            .FirstOrDefaultAsync(u => u.EmailVerificationToken == token && u.EmailVTokenExpiresAt > DateTime.UtcNow);
    }

    public async Task<bool> UpdateLHostPartialAsync(Lhost lHost, Action<Lhost> patchData)
    {
        // Attach user if not already tracked
        context.Lhosts.Attach(lHost);

        patchData(lHost);

        // Mark only specified properties as modified
        foreach (var property in context.Entry(lHost).Properties)
        {
            if (property.IsModified)
            {
                context.Entry(lHost).Property(property.Metadata.Name).IsModified = true;
            }
        }
        // Save changes
        return await context.SaveChangesAsync() > 0; 
    }

}
