namespace RentalCheckIn.Repositories;
public interface ILHostRepository
{
    Task<LHost> GetLHostByEmailAsync(string mailAddress);
    // Refactor the DTO to return LHost tailored to the Authenticator functions after bringing them to the Business logic layer
    Task<HostLoginResponseDTO> GetLoginResponseDataByEmail(string mailAddress);
    Task<LHost> GetLHostByIdAsync(uint id);
    Task AddLHostAsync(LHost lHost);
    Task UpdateLHostAsync(LHost lHost);
    Task<bool> UpdateLHostPartialAsync(LHost lHost, Action<LHost> patchData);
    Task<LHost> GetLHostByEmailVerificationTokenAsync(string token);
    Task<LHost> GetLHostByPasswordResetTokenAsync(string token);
}
