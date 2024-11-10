namespace RentalCheckIn.Utilities;

public static class Extensions
{
    public static bool IsTokenExpired(string token)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return jwt.ValidTo < DateTime.UtcNow;
    }

    public static bool IsTokenAlmostExpired(string token)
    {
        TimeSpan bufferTime = TimeSpan.FromMinutes(15);

        var jwtHandler = new JwtSecurityTokenHandler();
        if (!jwtHandler.CanReadToken(token))
        {
            Console.WriteLine("Invalid JWT token parsed");
            throw new ArgumentException("Invalid JWT token", nameof(token));
        }

        var jwt = jwtHandler.ReadJwtToken(token);
        var expirationTime = jwt.ValidTo;

        var adjustedExpirationTime = expirationTime - bufferTime;

        return adjustedExpirationTime <= DateTime.UtcNow;
    }

}
