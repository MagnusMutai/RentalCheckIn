namespace RentalCheckIn.Entities;
public class RefreshToken
{
    public uint Id { get; set; }
    public string Token { get; set; }
    public DateTime Created { get; set; }
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; }
    // Foreign key to Host table
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => !IsRevoked && !IsExpired;
    public uint HostId { get; set; }
    public virtual LHost LHost { get; set; }
}
