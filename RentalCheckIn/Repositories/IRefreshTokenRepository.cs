namespace RentalCheckIn.Repositories;

public interface IRefreshTokenRepository
{
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
    Task<IEnumerable<RefreshToken>> GetActiveRefreshTokensByHostIdAsync(uint hostId);
    Task RevokeAndAddRefreshTokenAsync(IEnumerable<RefreshToken> revokedTokens, RefreshToken newToken);
}
