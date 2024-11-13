namespace RentalCheckIn.Services.Core;
public class JwtService : IJwtService
{
    private readonly IConfiguration configuration;
    public JwtService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public string GenerateToken(LHost lHost)
    {
        var secretKey = configuration["Jwt:SecretKey"];
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
}
