using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class SlMagazyn
{
    public int MagId { get; set; }

    public string MagSymbol { get; set; } = null!;

    public string MagNazwa { get; set; } = null!;

    public int MagStatus { get; set; }

    public string? MagOpis { get; set; }

    public string? MagAnalityka { get; set; }

    public bool MagGlowny { get; set; }

    public bool MagPos { get; set; }

    public Guid? MagPosident { get; set; }

    public string? MagPosnazwa { get; set; }

    public string? MagPosadres { get; set; }

    public virtual ICollection<PdUzytkownik> PdUzytkowniks { get; set; } = new List<PdUzytkownik>();

    public virtual ICollection<TwStan> TwStans { get; set; } = new List<TwStan>();
}
