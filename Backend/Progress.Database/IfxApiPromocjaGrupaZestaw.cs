using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfxApiPromocjaGrupaZestaw
{
    public int Id { get; set; }

    public int PromocjaGrupaId { get; set; }

    public int PromocjaZestawId { get; set; }

    public virtual IfxApiPromocjaGrupa PromocjaGrupa { get; set; } = null!;

    public virtual IfxApiPromocjaZestaw PromocjaZestaw { get; set; } = null!;
}
