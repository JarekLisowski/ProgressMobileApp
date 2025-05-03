using System.ComponentModel.DataAnnotations.Schema;

namespace Progress.Database
{

  public class TwTowarShort
  {
    [Column("Tw_Id")]
    public int TwId { get; set; }
    [Column("Tw_Symbol")]
    public string TwSymbol { get; set; } = null!;
    [Column("Tw_Nazwa")]
    public string TwNazwa { get; set; } = null!;
    [Column("Tw_Opis")]
    public string TwOpis { get; set; } = null!;
    [Column("Tw_IdVatSp")]
    public int? TwIdVatSp { get; set; }
  }
}
