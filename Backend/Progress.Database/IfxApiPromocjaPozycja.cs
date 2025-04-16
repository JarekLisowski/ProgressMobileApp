using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfxApiPromocjaPozycja
{
    public int Id { get; set; }

    public string Nazwa { get; set; } = null!;

    public int ZestawId { get; set; }

    public bool Gratis { get; set; }

    public decimal CenaNetto { get; set; }

    public int Ilosc { get; set; }

    public decimal StawkaVat { get; set; }

    public int Rabat { get; set; }

    public decimal? MinCena { get; set; }

    public virtual ICollection<IfxApiPromocjaPozycjaTowar> IfxApiPromocjaPozycjaTowars { get; set; } = new List<IfxApiPromocjaPozycjaTowar>();

    public virtual IfxApiPromocjaZestaw Zestaw { get; set; } = null!;
}
