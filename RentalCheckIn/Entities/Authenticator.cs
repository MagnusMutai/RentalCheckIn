namespace RentalCheckIn.Entities;
public class Authenticator
{
    public uint AuthenticatorId { get; set; }
    public string Name { get; set; } = null!;
    public virtual ICollection<LHost> LHosts { get; set; } = new List<LHost>();
}
