using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfVwKontrahent
{
    public int Id { get; set; }

    public int? KhId { get; set; }

    public string? KhSymbol { get; set; }

    public string? KhRegon { get; set; }

    public string? KhEmail { get; set; }

    public int? KhIdOpiekun { get; set; }

    public string? AdrNazwa { get; set; }

    public string? AdrNazwaPelna { get; set; }

    public string? AdrNip { get; set; }

    public string? AdrTelefon { get; set; }

    public string? AdrUlica { get; set; }

    public string? AdrNrDomu { get; set; }

    public string? AdrNrLokalu { get; set; }

    public string? AdrKod { get; set; }

    public string? AdrMiejscowosc { get; set; }

    public int? AdrPaId { get; set; }

    public string? AdrPaNazwa { get; set; }

    public string? AdrPaKod { get; set; }

    public string? DostNazwa { get; set; }

    public string? DostSymbol { get; set; }

    public string? DostNip { get; set; }

    public string? DostTelefon { get; set; }

    public string? DostUlica { get; set; }

    public string? DostNrDomu { get; set; }

    public string? DostNrLokalu { get; set; }

    public string? DostKod { get; set; }

    public string? DostMiejscowosc { get; set; }

    public int? DostPaId { get; set; }

    public string? DostPaNazwa { get; set; }

    public string? DostPaKod { get; set; }

    public bool? KhZablokowany { get; set; }

    public DateTime? ZmData { get; set; }

    public bool? KhJednorazowy { get; set; }

    public string? KhPracownik { get; set; }

    public bool? KhPlatOdroczone { get; set; }

    public int? FpTermin { get; set; }

    public string? KhWww { get; set; }

    public int? KhCena { get; set; }

    public int? Naleznosc { get; set; }

    public int? IloscDokNierozliczonych { get; set; }

    public int? TerminPlatnosci { get; set; }

    public DateTime? ZmCecha { get; set; }

    public int? KhMaxDokKred { get; set; }
}
