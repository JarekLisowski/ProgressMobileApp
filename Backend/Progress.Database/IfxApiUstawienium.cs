using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfxApiUstawienium
{
    public int Id { get; set; }

    public int TokenLength { get; set; }

    public decimal TokenValidMinutes { get; set; }

    public decimal NavireoIdleMinutes { get; set; }

    public int ImageWidth { get; set; }

    public int ImageHeight { get; set; }

    public string BusinessCodeNumerationFormat { get; set; } = null!;

    public string PrzesylkaSymbol { get; set; } = null!;

    public bool SklepInternetowy { get; set; }

    public bool SprzedazMobilna { get; set; }

    public bool SerwisAukcyjny { get; set; }

    public int PlatnoscKredytKupiecki { get; set; }

    public int PlatnoscPrzelew { get; set; }

    public bool OrderDeleteAsCancel { get; set; }

    public bool InvoiceDeleteAsCancel { get; set; }

    public bool ReceiptDeleteAsCancel { get; set; }

    public int DomyslnyTerminPlatnosci { get; set; }

    public int NavUzytkownik { get; set; }

    public string NavHaslo { get; set; } = null!;

    public string KatalogPaczek { get; set; } = null!;

    public string QuartzNetTriggerName { get; set; } = null!;

    public DateTime UstawieniaData { get; set; }

    public int AvailableMajorVersion { get; set; }

    public int AvailableMinorVersion { get; set; }

    public int AvailableBuildVersion { get; set; }

    public int RequiredMajorVersion { get; set; }

    public int RequiredMinorVersion { get; set; }

    public int RequiredBuildVersion { get; set; }

    public bool UzupelniajLukiNumeracji { get; set; }
}
