namespace RentalCheckIn.Services.Core;
public interface IRefreshTokenService
{
    Task<RefreshToken?> GenerateRefreshToken(uint lHostId);
    Task<TokenValidateResult> ValidateAndRefreshTokensAsync(string accessToken, string refreshToken);
}
