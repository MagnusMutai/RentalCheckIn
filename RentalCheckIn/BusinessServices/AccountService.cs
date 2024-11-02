using RentalCheckIn.Services;

namespace RentalCheckIn.BusinessServices;
public class AccountService : IAccountService
{
    private readonly IHostRepository hostRepository;
    public AccountService(IHostRepository hostRepository)
    {
        this.hostRepository = hostRepository;
    }
    public async Task<Lhost> LoginAsync(HostLoginDto hostLoginDto)
    {
        // Call api here
        var lHost = await hostRepository.GetByEmailAsync(hostLoginDto.Email);
        if (lHost == null || !VerifyPassword(hostLoginDto.Password, lHost.PasswordHash))
            throw new Exception("Invalid email or password");

        return lHost;
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public async Task<Lhost> RegisterAsync(HostSignUpDto hostSignUpDto)
    {
        var existingUser = await hostRepository.GetByEmailAsync(hostSignUpDto.Email);
        if (existingUser != null)
            throw new Exception("User already exists");

        var totpService = new TotpService();
        var totpSecret = totpService.GenerateSecret();

        var lHost = new Lhost
        {
            FirstName = hostSignUpDto.FirstName,
            LastName = hostSignUpDto.LastName,
            PasswordHash = HashPassword(hostSignUpDto.Password),
            TotpSecret = totpSecret,
            Username = hostSignUpDto.Email,
            MailAddress = hostSignUpDto.Email,
        };

        // Call the api here
        await hostRepository.AddLHostAsync(lHost);

        // Return the TOTP secret to the caller to display to the user
        lHost.TotpSecret = totpSecret;
        return lHost;
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
