using Microsoft.EntityFrameworkCore;

namespace RentalCheckIn.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext context;

    public RefreshTokenRepository(AppDbContext context)
    {
        this.context = context;
    }
    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
    {
        return await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveRefreshTokensByHostIdAsync(uint hostId)
    {
        // Fetch all tokens for the host and filter by IsActive in memory
        return context.RefreshTokens
            .Where(rt => rt.HostId == hostId)
            // Switch to client-side evaluation
            .AsEnumerable()  
            // Apply IsActive filter in memory
            .Where(rt => rt.IsActive)  
            .ToList();
    }

    public async Task RevokeAndAddRefreshTokenAsync(IEnumerable<RefreshToken> revokedTokens, RefreshToken newToken)
    {
        // Update all revoked tokens
        foreach (var token in revokedTokens)
        {
            context.Entry(token).State = EntityState.Modified;
        }

        // Add the new refresh token
        await context.RefreshTokens.AddAsync(newToken);

        // Save changes in a single transaction
        await context.SaveChangesAsync();
    }

}
