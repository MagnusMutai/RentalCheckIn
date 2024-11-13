namespace RentalCheckIn.Services.Core;
public interface ITotpService
{
    string GenerateSecret();
    bool VerifyCode(string secret, string code);
}
