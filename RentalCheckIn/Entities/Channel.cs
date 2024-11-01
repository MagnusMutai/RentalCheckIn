using System;
using System.Collections.Generic;

namespace RentalCheckIn.Entities;

public partial class Channel
{
    public uint ChannelId { get; set; }

    public string ChannelLabel { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
