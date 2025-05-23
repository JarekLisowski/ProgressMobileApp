﻿using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class DokVat
{
    public int VtId { get; set; }

    public int VtDokId { get; set; }

    public int VtVatId { get; set; }

    public decimal VtVatProc { get; set; }

    public decimal VtWartNetto { get; set; }

    public decimal VtWartVat { get; set; }

    public decimal VtWartBrutto { get; set; }

    public decimal? VtWartNettoWaluta { get; set; }

    public decimal? VtWartVatWaluta { get; set; }

    public decimal? VtWartBruttoWaluta { get; set; }

    public virtual DokDokument VtDok { get; set; } = null!;

    public virtual SlStawkaVat VtVat { get; set; } = null!;
}
