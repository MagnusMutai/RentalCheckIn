using System;
using System.Collections.Generic;

namespace RentalCheckIn.Entities;

public partial class Setting
{
    public uint SettingId { get; set; }

    public uint RowsPerPage { get; set; }

    public int MaxLoginAttempts { get; set; }

    public DateTime CreationDate { get; set; }
}
