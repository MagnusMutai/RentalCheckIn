namespace RentalCheckIn.Services;

public interface IAuthService
{
    Task<Lhost> RegisterAsync(HostSignUpDto hostSignUpDto);
    Task<Lhost> AuthenticateAsync(string mailAddress, string password);
    bool VerifyPassword(string password, string passwordHash);
    string HashPassword(string password);
}
