namespace RentalCheckIn.Services.Core;
public interface ITOTPService
{
    string GenerateSecret();
    bool VerifyCode(string secret, string code);
}
