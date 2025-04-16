using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfxApiUzytkownik
{
    public int UzId { get; set; }

    public bool Active { get; set; }

    public int? CechaId { get; set; }

    public string? DeviceId { get; set; }

    public DateTime ZmData { get; set; }

    public bool PlatnosciOdroczone { get; set; }

    public bool PlatnosciOdroczoneWydluzenieTerminu { get; set; }

    public bool Padodaj { get; set; }

    public bool Fsdodaj { get; set; }

    public bool Zkdodaj { get; set; }

    public bool Zmmdodaj { get; set; }

    public byte? Typ { get; set; }

    public int? PromocjaGrupaId { get; set; }

    public DateTime? PromocjaGrupaIdZmiana { get; set; }

    public int? Rabat { get; set; }

    public decimal? MaxPlatnoscOdroczona { get; set; }

    public virtual ICollection<IfxApiUzytkownikPoziomyCenowe> IfxApiUzytkownikPoziomyCenowes { get; set; } = new List<IfxApiUzytkownikPoziomyCenowe>();
}
