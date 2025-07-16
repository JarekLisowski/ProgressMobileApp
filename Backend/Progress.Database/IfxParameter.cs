using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfxParameter
{
    public string ParName { get; set; } = null!;

    public int? ParValueInt { get; set; }

    public string? ParValueString { get; set; }

    public decimal? ParValueDecimal { get; set; }
}
