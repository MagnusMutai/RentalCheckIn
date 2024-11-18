namespace RentalCheckIn.Entities;

public class LHostCredential
{
    public int Id { get; set; }

    /// <summary>
    /// Unique identifier for the credential, provided by the WebAuthn API.
    /// </summary>
    public string CredentialId { get; set; }

    /// <summary>
    /// The public key associated with this credential, used to verify assertions.
    /// </summary>
    public string PublicKey { get; set; }

    /// <summary>
    /// The signature counter to prevent replay attacks.
    /// </summary>
    public long SignCount { get; set; }

    /// <summary>
    /// Metadata or additional properties for the credential (optional).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Additional metadata such as authenticator type (optional).
    /// </summary>
    public string? AuthenticatorType { get; set; }

    /// <summary>
    /// Foreign key to associate the credential with the host.
    /// </summary>
    public uint HostId { get; set; }

    public virtual LHost Host { get; set; }
}
