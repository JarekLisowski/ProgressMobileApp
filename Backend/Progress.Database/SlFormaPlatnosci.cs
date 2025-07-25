﻿using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class SlFormaPlatnosci
{
    public int FpId { get; set; }

    public string FpNazwa { get; set; } = null!;

    public int FpTermin { get; set; }

    public int FpTyp { get; set; }

    public int? FpRachBankId { get; set; }

    public int? FpCentId { get; set; }

    public int? FpInstKredytId { get; set; }

    public bool? FpGlowna { get; set; }

    public bool FpAktywna { get; set; }

    public bool FpTerminalPlatniczy { get; set; }

    public int? FpFormaPlatnosciWysylajJako { get; set; }

    public string? FpOpisPlatnosciInna { get; set; }

    public virtual ICollection<KhKontrahent> KhKontrahents { get; set; } = new List<KhKontrahent>();
}
