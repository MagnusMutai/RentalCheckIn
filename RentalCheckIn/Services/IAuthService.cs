namespace RentalCheckIn.Services;

public interface IAuthService
{
    Task<Lhost> RegisterAsync(HostSignUpDto hostSignUpDto);
    Task<Lhost> LoginAsync(HostLoginDto hostLoginDto);
}
