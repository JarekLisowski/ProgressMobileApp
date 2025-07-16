using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfxApiDokumentyZapisane
{
    public int Id { get; set; }

    public DateTime Data { get; set; }

    public string Numer { get; set; } = null!;
}
