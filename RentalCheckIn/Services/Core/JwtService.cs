namespace RentalCheckIn.Services.Core;
public class JWTService : IJWTService
{
    private readonly IConfiguration configuration;
    private readonly ILogger<JWTService> logger;

    public JWTService(IConfiguration configuration, ILogger<JWTService> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
    }

    public string GenerateToken(LHost lHost)
    {
        try
        {
            var secretKey = configuration["Jwt:SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                return string.Empty;
            }

            var key = Encoding.ASCII.GetBytes(secretKey);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, lHost.HostId.ToString()),
                new Claim(ClaimTypes.Email, lHost.MailAddress),
                new Claim(ClaimTypes.Name, lHost.FirstName),
                new Claim(ClaimTypes.Role, "Host")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
                Subject = new ClaimsIdentity(claims),
                // Token valid for 1 hour
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "An unexpected error occurred while trying to generate a JWT token for a LHost.");
            return string.Empty;
        }
    }
}
