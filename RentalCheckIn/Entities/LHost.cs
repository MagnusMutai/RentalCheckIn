namespace RentalCheckIn.Entities;

public partial class LHost
{
    public uint HostId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string TotpSecret { get; set; }
    public string MailAddress { get; set; } = null!;
    public string? EmailVerificationToken { get; set; }
    public DateTime EmailVTokenExpiresAt { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }
    public DateTime? LastLogin { get; set; }
    public byte LoginAttempts { get; set; }
    public DateTime? IsBlockedSince { get; set; }
    public sbyte IsDisabled { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public DateTime CreationDate { get; set; }
    public string Selected2FA { get; set; }
    // Add UserHandle for FIDO2
    public byte[]? UserHandle { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public virtual ICollection<LHostCredential> Credentials { get; set; } = new List<LHostCredential>();
}
