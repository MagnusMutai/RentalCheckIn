using System;
using System.Collections.Generic;

namespace RentalCheckIn.Entities;

public partial class Language
{
    public uint LanguageId { get; set; }

    public string LanguageCode { get; set; } = null!;

    public string LanguageName { get; set; } = null!;

    public string Culture { get; set; } = null!;

    public bool IsEnabled { get; set; }

    public string? Svg { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Quest> Quests { get; set; } = new List<Quest>();
}
