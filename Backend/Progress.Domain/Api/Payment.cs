namespace Progress.Domain.Api
{
  /// <summary>
  /// Rozliczenie dokumentu, KP lub wpłata bankowa
  /// </summary>
  public class Payment
  {
    /// <summary>
    /// Id dokumentu którego rozliczenie dotyczy
    /// </summary>
    public int RelatedDocumentId { get; set; }

    /// <summary>
    /// Numer dokumentu którego rozliczenie dotyczy
    /// </summary>
    public string RelatedDocumentNumber { get; set; } = "";

    /// <summary>
    /// Numer rozliczenia
    /// </summary>
    //public int Number { get; set; }

    /// <summary>
    /// Pełny numer rozliczenia
    /// </summary>
    //public string FullNumber { get; set; } = "";

    /// <summary>
    /// Data 
    /// </summary>
    public DateTime IssueDate { get; set; }

    /// <summary>
    /// Wartość rozliczenia
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Tytuł
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// Sprzedawca
    /// </summary>
    //public Location? Seller { get; set; }

    /// <summary>
    /// Kontrahent
    /// </summary>
    //public Location? Buyer { get; set; }
    public int PayerId { get; set; }

    /// <summary>
    /// Wystawił
    /// </summary>
    public string FromPerson { get; set; } = "";

    /// <summary>
    /// Odebrał
    /// </summary>
    public string ToPerson { get; set; } = "";

    public int PaymentType { get; set; }

  }


}
