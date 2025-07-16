using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfxApiDokPozycjaPromocja
{
    public int ObId { get; set; }

    public int PromocjaPozycjaId { get; set; }

    public int PozycjaUrzadzenieId { get; set; }

    public bool? PozycjaPromocyjna { get; set; }
}
