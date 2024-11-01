namespace RentalCheckIn.Services;

public class AuthService : IAuthService
{

    private readonly IHostRepository hostRepository;
    public AuthService(IHostRepository hostRepository)
    {
        this.hostRepository = hostRepository;
    }
    public async Task<Lhost> AuthenticateAsync(string mailAddress, string password)
    {
        var lhost = await hostRepository.GetByEmailAsync(mailAddress);
        if (lhost == null || !VerifyPassword(password, lhost.PasswordHash))
            throw new Exception("Invalid email or password");

        return lhost;
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

        var lhost = new Lhost
        {
            FirstName = hostSignUpDto.FirstName,
            LastName = hostSignUpDto.LastName,
            PasswordHash = HashPassword(hostSignUpDto.Password),
            TotpSecret = totpSecret,
            Username = hostSignUpDto.Email,
            MailAddress = hostSignUpDto.Email,
        };

        await hostRepository.AddLHostAsync(lhost);

        // Return the TOTP secret to the caller to display to the user
        lhost.TotpSecret = totpSecret;
        return lhost;
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
