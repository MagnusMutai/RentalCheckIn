using OtpNet;

namespace RentalCheckIn.Services.Core;

public class TotpService
{
    public string GenerateSecret()
    {
        var secretKey = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(secretKey);
    }

    public bool VerifyCode(string secret, string code)
    {
        var totp = new Totp(Base32Encoding.ToBytes(secret));
        return totp.VerifyTotp(code, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
    }
}
