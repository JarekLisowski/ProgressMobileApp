using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class SlPanstwo
{
    public int PaId { get; set; }

    public string PaNazwa { get; set; } = null!;

    public string PaKodPanstwaUe { get; set; } = null!;

    public bool PaCzlonekUe { get; set; }

    public string? PaKodPanstwaIso { get; set; }

    public virtual ICollection<AdrEwid> AdrEwids { get; set; } = new List<AdrEwid>();

    public virtual ICollection<DokDokument> DokDokumentDokIdPanstwaKonsumentaNavigations { get; set; } = new List<DokDokument>();

    public virtual ICollection<DokDokument> DokDokumentDokIdPanstwaRozpoczeciaWysylkiNavigations { get; set; } = new List<DokDokument>();

    public virtual ICollection<SlStawkaVat> SlStawkaVats { get; set; } = new List<SlStawkaVat>();
}
