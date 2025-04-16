using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfxApiPromocjaPozycjaTowar
{
    public int PozycjaId { get; set; }

    public string TwSymbol { get; set; } = null!;

    public virtual IfxApiPromocjaPozycja Pozycja { get; set; } = null!;
}
