namespace RentalCheckIn.Services.Core;
public class TOTPService : ITOTPService
{
    private readonly ILogger<TOTPService> logger;

    public TOTPService(ILogger<TOTPService> logger)
    {
        this.logger = logger;
    }
    public string GenerateSecret()
    {
        try
        {
            var secretKey = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(secretKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in TOTPService while trying to generate a secret key for TOTP.");
            return string.Empty;
        }
    }

    public bool VerifyCode(string secret, string code)
    {
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(secret))
            return false;

        try
        {
            var totp = new Totp(Base32Encoding.ToBytes(secret));
            return totp.VerifyTotp(code, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in TOTPService while trying to verify TOTP code.");
            return false;
        }
    }
}
