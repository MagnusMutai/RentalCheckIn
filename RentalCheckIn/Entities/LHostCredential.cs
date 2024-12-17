namespace RentalCheckIn.Entities;

public class LHostCredential
{
    public uint Id { get; set; }
    // Unique identifier for the credential, provided by the WebAuthn API.
    public string CredentialId { get; set; }
    // The public key associated with this credential, used to verify assertions.
    public string PublicKey { get; set; }
    // The signature counter to prevent replay attacks.
    // *****
    public long SignCount { get; set; }
    // (optional).
    public DateTime CreatedAt { get; set; }
    // (optional).
    public string? AuthenticatorType { get; set; }
    public uint HostId { get; set; }

    public virtual LHost Host { get; set; }
}
