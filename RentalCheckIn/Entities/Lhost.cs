using System;
using System.Collections.Generic;

namespace RentalCheckIn.Entities;

public partial class Lhost
{
    public uint HostId { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string TotpSecret { get; set; }


    public string MailAddress { get; set; } = null!;

    public DateTime? LastLogin { get; set; }

    public byte LoginAttempts { get; set; }

    public DateTime? IsBlockedSince { get; set; }

    public sbyte IsDisabled { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
