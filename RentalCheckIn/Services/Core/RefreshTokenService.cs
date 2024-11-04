using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace RentalCheckIn.Services.Core
{
    public class RefreshTokenService
    {

        public RefreshToken GenerateRefreshToken(uint userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = GenerateSecureToken(),
                // Set refresh token lifespan
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                IsRevoked = false,
                HostId = userId
            };

            // Store the token in the database (assuming you're using EF Core)
            //_context.RefreshTokens.Add(refreshToken);
            //_context.SaveChanges();

            return refreshToken;
        }

        private string GenerateSecureToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
