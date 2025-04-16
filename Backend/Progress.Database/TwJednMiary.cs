using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class TwJednMiary
{
    public int JmId { get; set; }

    public int JmIdTowar { get; set; }

    public string JmIdJednMiary { get; set; } = null!;

    public decimal JmPrzelicznik { get; set; }

    public bool JmDlaNaklejek { get; set; }

    public Guid Rowguid { get; set; }

    public virtual TwTowar JmIdTowarNavigation { get; set; } = null!;
}
