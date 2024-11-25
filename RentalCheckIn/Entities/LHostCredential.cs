namespace RentalCheckIn.Entities;

public class LHostCredential
{
    public int Id { get; set; }
    // Unique identifier for the credential, provided by the WebAuthn API.
    public string CredentialId { get; set; }
    // The public key associated with this credential, used to verify assertions.
    public string PublicKey { get; set; }
    // The signature counter to prevent replay attacks.
    public long SignCount { get; set; }
    // Metadata or additional properties for the credential (optional).
    public DateTime CreatedAt { get; set; }
    // Additional metadata such as authenticator type (optional).
    public string? AuthenticatorType { get; set; }
    // Foreign key to associate the credential with the host.
    public uint HostId { get; set; }

    public virtual LHost Host { get; set; }
}
