using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class SlGrupaKh
{
    public int GrkId { get; set; }

    public string GrkNazwa { get; set; } = null!;

    public virtual ICollection<KhKontrahent> KhKontrahents { get; set; } = new List<KhKontrahent>();

    public virtual ICollection<KhPracownik> KhPracowniks { get; set; } = new List<KhPracownik>();
}
