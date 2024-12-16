namespace RentalCheckIn.Utilities;

// For the sake of creating mocks we make them non-static so it be used in dependency injection.
public static class JWTUtils
{
    public static bool IsTokenExpired(string token)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return jwt.ValidTo < DateTime.UtcNow;
    }

    public static bool IsTokenAlmostExpired(string token, TimeSpan bufferTime)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        if (!jwtHandler.CanReadToken(token))
        {
            throw new ArgumentException("Invalid JWT token", nameof(token));
        }

        var jwt = jwtHandler.ReadJwtToken(token);
        var expirationTime = jwt.ValidTo;

        var adjustedExpirationTime = expirationTime - bufferTime;

        return adjustedExpirationTime <= DateTime.UtcNow;
    }

}
