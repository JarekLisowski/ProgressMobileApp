using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class SlCechaKh
{
    public int CkhId { get; set; }

    public string CkhNazwa { get; set; } = null!;

    public virtual ICollection<KhCechaKh> KhCechaKhs { get; set; } = new List<KhCechaKh>();
}
