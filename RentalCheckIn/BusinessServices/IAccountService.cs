namespace RentalCheckIn.BusinessServices;

public interface IAccountService
{
    Task<Lhost> RegisterAsync(HostSignUpDto hostSignUpDto);
    Task<Lhost> LoginAsync(HostLoginDto hostLoginDto);
    bool VerifyPassword(string password, string passwordHash);
    string HashPassword(string password);
}
