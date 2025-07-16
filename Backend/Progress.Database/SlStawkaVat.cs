using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class SlStawkaVat
{
    public int VatId { get; set; }

    public string VatNazwa { get; set; } = null!;

    public decimal VatStawka { get; set; }

    public string VatSymbol { get; set; } = null!;

    public bool VatCzySystemowa { get; set; }

    public bool VatCzyWidoczna { get; set; }

    public int VatPozycja { get; set; }

    public int VatPozSprzedaz { get; set; }

    public int VatPozZakup { get; set; }

    public int VatPozRr { get; set; }

    public int VatPozDomyslna { get; set; }

    public int VatRodzaj { get; set; }

    public bool VatStawkaZagraniczna { get; set; }

    public bool? VatStawkaZagranicznaPdst { get; set; }

    public int? VatIdPanstwo { get; set; }

    public bool? VatUePanstwo { get; set; }

    public virtual ICollection<DokVat> DokVats { get; set; } = new List<DokVat>();

    public virtual ICollection<TwTowar> TwTowarTwIdVatSpNavigations { get; set; } = new List<TwTowar>();

    public virtual ICollection<TwTowar> TwTowarTwIdVatZakNavigations { get; set; } = new List<TwTowar>();

    public virtual SlPanstwo? VatIdPanstwoNavigation { get; set; }
}
