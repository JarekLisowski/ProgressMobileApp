using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfVwDokument
{
    public int DokId { get; set; }

    public int DokTyp { get; set; }

    public int DokPodtyp { get; set; }

    public string DokNrPelny { get; set; } = null!;

    public int? DokDoDokId { get; set; }

    public string DokDoDokNrPelny { get; set; } = null!;

    public DateTime DokDataWyst { get; set; }

    public int? DokPlatnikAdreshId { get; set; }

    public int? DokPlatnikId { get; set; }

    public string DokWystawil { get; set; } = null!;

    public string DokOdebral { get; set; } = null!;

    public decimal DokWartNetto { get; set; }

    public decimal DokWartVat { get; set; }

    public decimal DokWartBrutto { get; set; }

    public string DokUwagi { get; set; } = null!;

    public DateTime? DokPlatTermin { get; set; }

    public string? DokWaluta { get; set; }

    public decimal DokWalutaKurs { get; set; }

    public bool DokJestTylkoDoOdczytu { get; set; }

    public int DokStatusFiskal { get; set; }

    public DateTime? DokDataZakonczenia { get; set; }

    public DateTime? IdokDataCzasWyst { get; set; }

    public int? AdrhId { get; set; }

    public string? AdrhNazwaPelna { get; set; }

    public string? AdrhSymbol { get; set; }

    public string? AdrhNip { get; set; }

    public string? AdrhKod { get; set; }

    public string? AdrhMiejscowosc { get; set; }

    public string? AdrhNrDomu { get; set; }

    public string? AdrhNrLokalu { get; set; }

    public string? AdrhTelefon { get; set; }

    public decimal? NzfWartoscDoZaplaty { get; set; }

    public DateTime? DataWystawienia { get; set; }

    public int? DokNr { get; set; }

    public int? DokMagId { get; set; }

    public string? AdrhUlica { get; set; }

    public DateTime? DokDataOtrzym { get; set; }

    public int? DokCenyPoziom { get; set; }

    public int? DokPersonelId { get; set; }

    public int? AdrhIdPanstwo { get; set; }

    public decimal? DokKwKarta { get; set; }

    public decimal? DokKwGotowka { get; set; }

    public decimal? DokKwDoZaplaty { get; set; }

    public decimal? DokKwKredyt { get; set; }

    public decimal? DokKwWartosc { get; set; }

    public int? DokPlatId { get; set; }

    public int? DokKredytId { get; set; }

    public int? DokKartaId { get; set; }

    public int? DokDefiniowalnyId { get; set; }

    public int? DokPowiazanyId { get; set; }

    public string? DokPowiazanyNumer { get; set; }

    public int DokStatus { get; set; }
}
