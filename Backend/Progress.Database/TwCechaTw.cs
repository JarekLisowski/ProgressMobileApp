using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class TwCechaTw
{
    public int ChtId { get; set; }

    public int ChtIdTowar { get; set; }

    public int ChtIdCecha { get; set; }

    public Guid Rowguid { get; set; }

    public virtual SlCechaTw ChtIdCechaNavigation { get; set; } = null!;

    public virtual TwTowar ChtIdTowarNavigation { get; set; } = null!;
}
