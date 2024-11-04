using Microsoft.EntityFrameworkCore;

namespace RentalCheckIn.Repositories;

public class HostRepository : IHostRepository
{
    private readonly AppDbContext context;
    public HostRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task AddLHostAsync(LHost lHost)
    {
        context.LHosts.Add(lHost);
        await context.SaveChangesAsync();
    }

    public async Task<LHost> GetByEmailAsync(string mailAddress)
    {
        return await context.LHosts.FirstOrDefaultAsync(h => h.MailAddress == mailAddress);
    }

    public async Task<LHost> GetByIdAsync(int id)
    {
        return await context.LHosts.FindAsync(id);
    }

    public async Task UpdateLHostAsync(LHost lHost)
    {
        context.LHosts.Update(lHost);
        await context.SaveChangesAsync();
    }

    public async Task<LHost> GetUserByEmailVerificationTokenAsync(string token)
    {
        return await context.LHosts
            .FirstOrDefaultAsync(u => u.EmailVerificationToken == token && u.EmailVTokenExpiresAt > DateTime.UtcNow);
    }

    public async Task<bool> UpdateLHostPartialAsync(LHost lHost, Action<LHost> patchData)
    {
        // Attach user if not already tracked
        context.LHosts.Attach(lHost);

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
