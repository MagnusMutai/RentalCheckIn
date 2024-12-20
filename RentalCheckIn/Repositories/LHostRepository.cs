﻿namespace RentalCheckIn.Repositories;

public class LHostRepository : ILHostRepository
{
    private readonly AppDbContext context;
    public LHostRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task AddLHostAsync(LHost lHost)
    {
        context.LHosts.Add(lHost);
        await context.SaveChangesAsync();
    }

    public async Task<LHost> GetLHostByEmailAsync(string mailAddress)
    {
        return await context.LHosts.FirstOrDefaultAsync(h => h.MailAddress == mailAddress);
    }
    // Refactor the DTO to return LHost tailored to the Authenticator functions after bringing them to the Business logic layer
    public async Task<HostLoginResponseDTO> GetLoginResponseDataByEmail(string mailAddress)
    {
        return await context.LHosts
            .Where(h => h.MailAddress == mailAddress)
            .Select(h => new HostLoginResponseDTO
            {
                HostId = h.HostId,
                AuthenticatorId = h.AuthenticatorId,
                MailAddress = h.MailAddress
            }).FirstOrDefaultAsync();
    }

    public async Task<LHost> GetLHostByIdAsync(uint id)
    {
        return await context.LHosts.FindAsync(id);
    }

    public async Task UpdateLHostAsync(LHost lHost)
    {
        context.LHosts.Update(lHost);
        await context.SaveChangesAsync();
    }

    public async Task<LHost> GetLHostByEmailVerificationTokenAsync(string token)
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

    public async Task<LHost> GetLHostByPasswordResetTokenAsync(string token)
    {
        return await context.LHosts
          .FirstOrDefaultAsync(h => h.PasswordResetToken == token && h.ResetTokenExpires > DateTime.Now);
    }

}
