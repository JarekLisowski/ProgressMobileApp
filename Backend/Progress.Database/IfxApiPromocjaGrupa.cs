using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfxApiPromocjaGrupa
{
    public int Id { get; set; }

    public string Nazwa { get; set; } = null!;

    public virtual ICollection<IfxApiPromocjaGrupaZestaw> IfxApiPromocjaGrupaZestaws { get; set; } = new List<IfxApiPromocjaGrupaZestaw>();
}
