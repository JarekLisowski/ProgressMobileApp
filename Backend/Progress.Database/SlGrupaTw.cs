using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class SlGrupaTw
{
    public int GrtId { get; set; }

    public string GrtNazwa { get; set; } = null!;

    public string? GrtNrAnalityka { get; set; }

    public Guid Rowguid { get; set; }

    public virtual ICollection<TwTowar> TwTowars { get; set; } = new List<TwTowar>();
}
