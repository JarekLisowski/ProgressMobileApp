using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfxApiUzytkownikPoziomyCenowe
{
    public int Id { get; set; }

    public int UzId { get; set; }

    public int CenaId { get; set; }

    public bool Primary { get; set; }

    public virtual IfxApiUzytkownik Uz { get; set; } = null!;
}
