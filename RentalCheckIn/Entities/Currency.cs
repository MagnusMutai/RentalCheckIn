using System;
using System.Collections.Generic;

namespace RentalCheckIn.Entities;

public partial class Currency
{
    public uint CurrencyId { get; set; }
    public string CurrencyLabel { get; set; } = null!;
    public string CurrencySymbol { get; set; } = null!;
    public DateTime CreationDate { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
