using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class SlKategorium
{
    public int KatId { get; set; }

    public string KatNazwa { get; set; } = null!;

    public int KatTyp { get; set; }

    public string KatPodtytul { get; set; } = null!;

    public virtual ICollection<DksKasa> DksKasaKsRkKategoriaKorektyNavigations { get; set; } = new List<DksKasa>();

    public virtual ICollection<DksKasa> DksKasaKsRkKategoriaNavigations { get; set; } = new List<DksKasa>();

    public virtual ICollection<DokDokument> DokDokuments { get; set; } = new List<DokDokument>();

    public virtual ICollection<KhKontrahent> KhKontrahentKhIdEwVatspKategNavigations { get; set; } = new List<KhKontrahent>();

    public virtual ICollection<KhKontrahent> KhKontrahentKhIdEwVatzakKategNavigations { get; set; } = new List<KhKontrahent>();

    public virtual ICollection<KhKontrahent> KhKontrahentKhIdKategoriaKhNavigations { get; set; } = new List<KhKontrahent>();

    public virtual ICollection<KhKontrahent> KhKontrahentKhIdRachKategPrzychodNavigations { get; set; } = new List<KhKontrahent>();

    public virtual ICollection<KhKontrahent> KhKontrahentKhIdRachKategRozchodNavigations { get; set; } = new List<KhKontrahent>();

    public virtual ICollection<NzFinanse> NzFinanses { get; set; } = new List<NzFinanse>();

    public virtual ICollection<RbRachBankowy> RbRachBankowies { get; set; } = new List<RbRachBankowy>();
}
