using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfxApiPromocjaZestaw
{
    public int Id { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DataOd { get; set; }

    public DateTime? DataDo { get; set; }

    public string Nazwa { get; set; } = null!;

    public DateTime ZmData { get; set; }

    public DateTime? ZmGrupaData { get; set; }

    public string? Zdjecie { get; set; }

    public byte[]? Img { get; set; }

    public byte Typ { get; set; }

    public virtual ICollection<IfxApiPromocjaGrupaZestaw> IfxApiPromocjaGrupaZestaws { get; set; } = new List<IfxApiPromocjaGrupaZestaw>();

    public virtual ICollection<IfxApiPromocjaPozycja> IfxApiPromocjaPozycjas { get; set; } = new List<IfxApiPromocjaPozycja>();
}
