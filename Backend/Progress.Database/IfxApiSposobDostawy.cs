using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfxApiSposobDostawy
{
    public int Id { get; set; }

    public int TwId { get; set; }

    public bool Aktywny { get; set; }

    public string Nazwa { get; set; } = null!;
}
