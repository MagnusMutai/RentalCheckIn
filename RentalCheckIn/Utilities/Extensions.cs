namespace RentalCheckIn.Utilities;

public static class Extensions
{
    public static bool IsTokenExpired(string token)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return jwt.ValidTo < DateTime.UtcNow;
    }
}
