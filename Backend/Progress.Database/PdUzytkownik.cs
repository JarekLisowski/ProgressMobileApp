using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class PdUzytkownik
{
    public int UzId { get; set; }

    public string UzIdentyfikator { get; set; } = null!;

    public string UzNazwisko { get; set; } = null!;

    public string UzImie { get; set; } = null!;

    public string UzLogin { get; set; } = null!;

    public string? UzHaslo { get; set; }

    public bool UzStatus { get; set; }

    public int? UzIdKasy { get; set; }

    public bool UzBlokadaKasy { get; set; }

    public int? UzIdMagazynu { get; set; }

    public int UzProgram { get; set; }

    public bool UzPracaZdalna { get; set; }

    public int? UzIdPracownika { get; set; }

    public int? UzIdKompozycji { get; set; }

    public int? UzIdGrupy { get; set; }

    public string UzEmail { get; set; } = null!;

    public string UzGaduGadu { get; set; } = null!;

    public string UzSkype { get; set; } = null!;

    public int UzRodzajInfoOwierszachListy { get; set; }

    public bool? UzUruchomCentralke { get; set; }

    public string? UzPrefiksOsobisty { get; set; }

    public int? UzLimitStanowisk { get; set; }

    public int UzStatusPrzypomnieniaZmianyVat { get; set; }

    public DateTime? UzDataPonownegoPrzypomnienia { get; set; }

    public int UzKlientEmail { get; set; }

    public string? UzDomena { get; set; }

    public int? UzAlarmyInterwal { get; set; }

    public bool UzLokalizacjaPokazuj { get; set; }

    public string? UzLokalizacja { get; set; }

    public int? UzOstatnieKontoEmail { get; set; }

    public DateTime? UzDataHasla { get; set; }

    public bool UzZmianaHasla { get; set; }

    public int? UzPodnoszenieUprawnienUserId { get; set; }

    public bool UzShowTutorialSms { get; set; }

    public virtual ICollection<KhKontrahent> KhKontrahents { get; set; } = new List<KhKontrahent>();

    public virtual ICollection<KhPracownik> KhPracowniks { get; set; } = new List<KhPracownik>();

    public virtual ICollection<NzFinanse> NzFinanses { get; set; } = new List<NzFinanse>();

    public virtual DksKasa? UzIdKasyNavigation { get; set; }

    public virtual SlMagazyn? UzIdMagazynuNavigation { get; set; }
}
