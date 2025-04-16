using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfxApiFormaPlatnosci
{
    public int Id { get; set; }

    public int FpId { get; set; }

    public bool Aktywna { get; set; }

    public string Nazwa { get; set; } = null!;

    public bool PlatnoscOdroczona { get; set; }
}
