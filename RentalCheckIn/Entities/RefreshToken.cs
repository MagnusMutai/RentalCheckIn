namespace RentalCheckIn.Entities;
public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime Created { get; set; }
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; }
    // Foreign key to Host table
    public uint HostId { get; set; }  
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => !IsRevoked && !IsExpired;

    // Navigation property to the host
    public LHost LHost{ get; set; }
}
