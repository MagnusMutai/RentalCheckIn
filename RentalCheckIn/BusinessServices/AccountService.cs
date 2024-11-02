using RentalCheckIn.Services;
using static RentalCheckIn.Responses.CustomResponses;

namespace RentalCheckIn.BusinessServices;
public class AccountService : IAccountService
{
    private readonly IHostRepository hostRepository;
    public AccountService(IHostRepository hostRepository)
    {
        this.hostRepository = hostRepository;
    }
    public async Task<AuthenticationResult> LoginAsync(HostLoginDto hostLoginDto)
    {
        var lHost = await hostRepository.GetByEmailAsync(hostLoginDto.Email);
        if (lHost == null || !VerifyPassword(hostLoginDto.Password, lHost.PasswordHash))
        {
            return new AuthenticationResult
            {
                Success = false,
                Message = "Invalid email or password, please try again"
            };
        }

        return new AuthenticationResult
        {
            Success = true,
            Host = lHost,
        };
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public async Task<AuthenticationResult> RegisterAsync(HostSignUpDto hostSignUpDto)
    {
        var existingHost = await hostRepository.GetByEmailAsync(hostSignUpDto.Email);
        if (existingHost != null)
        {
            return new AuthenticationResult
            {
                Success = false,
                Message = "User already exists"
            };
        }

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

        await hostRepository.AddLHostAsync(lHost);

        // Return the TOTP secret to the caller to display to the user
        lHost.TotpSecret = totpSecret;
        return new AuthenticationResult
        {
            Success = true,
            Host = lHost
        };
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
