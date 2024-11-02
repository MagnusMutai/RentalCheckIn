using System;
using System.Collections.Generic;

namespace RentalCheckIn.Entities;

public partial class Apartment
{
    public uint ApartmentId { get; set; }

    public string ApartmentName { get; set; } = null!;

    public string? ApartmentType { get; set; }

    public DateTime CreationDate { get; set; }

    public string ImagePath { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
