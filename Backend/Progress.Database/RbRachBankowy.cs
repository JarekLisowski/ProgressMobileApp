using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class RbRachBankowy
{
    public int RbId { get; set; }

    public string RbNazwa { get; set; } = null!;

    public int RbTypObiektu { get; set; }

    public string RbNumer { get; set; } = null!;

    public string RbBank { get; set; } = null!;

    public bool RbIban { get; set; }

    public string? RbIdWaluty { get; set; }

    public bool RbPodstawowy { get; set; }

    public string? RbOpis { get; set; }

    public string? RbAnalityka { get; set; }

    public string RbStatus { get; set; } = null!;

    public int? RbIdObiektu { get; set; }

    public bool RbZfss { get; set; }

    public bool RbUs { get; set; }

    public bool RbPodstawowyWirtualny { get; set; }

    public string? RbMnemonik { get; set; }

    public long RbLicznik { get; set; }

    public bool RbWirtualny { get; set; }

    public int RbTypIdentyfikacjiPlatnosci { get; set; }

    public bool RbEbank { get; set; }

    public string? RbDik { get; set; }

    public DateTime? RbDataHist { get; set; }

    public decimal? RbSaldo { get; set; }

    public DateTime? RbDataSaldo { get; set; }

    public int? RbIdUslugi { get; set; }

    public int? RbIdKategorii { get; set; }

    public bool RbIndywidualny { get; set; }

    public int? RbIndywWspolnik { get; set; }

    public bool RbRachunekVat { get; set; }

    public int? RbIdPowiazanyRachunekVat { get; set; }

    public bool RbIndywidualnyPodatkowy { get; set; }

    public int? RbIndywPodatkowyWspolnik { get; set; }

    public string RbNumerZnormalizowany { get; set; } = null!;

    public virtual ICollection<DokDokument> DokDokuments { get; set; } = new List<DokDokument>();

    public virtual ICollection<KhKontrahent> KhKontrahents { get; set; } = new List<KhKontrahent>();

    public virtual ICollection<NzFinanse> NzFinanses { get; set; } = new List<NzFinanse>();

    public virtual SlKategorium? RbIdKategoriiNavigation { get; set; }
}
