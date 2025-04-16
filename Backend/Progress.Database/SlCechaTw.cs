using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class SlCechaTw
{
    public int CtwId { get; set; }

    public string CtwNazwa { get; set; } = null!;

    public Guid Rowguid { get; set; }

    public virtual ICollection<TwCechaTw> TwCechaTws { get; set; } = new List<TwCechaTw>();
}
