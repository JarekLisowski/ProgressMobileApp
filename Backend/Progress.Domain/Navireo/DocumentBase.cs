namespace Progress.Domain.Navireo
{
  /// <summary>
  /// Dokument 
  /// </summary>
  public class DocumentBase : BObjectBase
  {
    public string Title { get; set; }

    /// <summary>
    /// Adres kupującego - historyczny, w chwili zapisu dokumentu
    /// </summary>
    public Location BuyerAddress { get; set; }

    /// <summary>
    /// Aktualny adres kupującego
    /// </summary>
    //public Location RefBuyerAddress { get; set; }

    /// <summary>
    /// Adres sprzedawcy - historyczny, w chwili zapisu dokumentu
    /// </summary>
    public Location SellerAddress { get; set; }

    /// <summary>
    /// Aktualny adres sprzedawcy
    /// </summary>
    //public Location RefSellerAddress { get; set; }

    /// <summary>
    /// Id dokumentu powiązanego
    /// </summary>
    public int RelatedDocumentId { get; set; }

    /// <summary>
    /// Numer dokumentu powiązanego
    /// </summary>
    public string RelatedDocumentNumber { get; set; }

    /// <summary>
    /// Kwota zapłacona gotówką
    /// </summary>
    public decimal PaidCashGross { get; set; }

    /// <summary>
    /// Kwota zapłacona za pomocą wybranego sposobu płatności 
    /// </summary>
    public decimal PaymentPaidGross { get; set; }

    /// <summary>
    /// Suma netto
    /// </summary>
    public decimal TotalNet { get; set; }

    /// <summary>
    /// Suma brutto
    /// </summary>
    public decimal TotalGross { get; set; }

    /// <summary>
    /// Suma Vat
    /// </summary>
    public decimal TotalTax { get; set; }

    /// <summary>
    /// Kwota pozostała do zapłaty
    /// </summary>
    public decimal ToPay { get; set; }

    /// <summary>
    /// Data wystawienia dokumentu
    /// </summary>
    public DateTime IssueDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Data dostawy
    /// </summary>
    public DateTime? DeliveryDate { get; set; }

    /// <summary>
    /// Data ważności dokumentu np. oferty sprzedaży
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Termin płatności - il. dni od daty wystawienia
    /// </summary>
    public TimeSpan? PaymentDays
    {
      get
      {
        if (PaymentDeadline != null)
          return PaymentDeadline - IssueDate;
        else
          return TimeSpan.FromDays(0);
      }
    }

    /// <summary>
    /// Termin płatności
    /// </summary>
    public DateTime? PaymentDeadline { get; set; }

    /// <summary>
    /// Uwagi do dokumentu
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Kolejny numer dokumentu
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Numer pełny - zgodnie z formatem numeracji
    /// </summary>
    public string FullNumber { get; set; }

    /// <summary>
    /// Flaga oznaczająca możliwość edycji
    /// </summary>
    public bool CanEdit { get; set; }

    /// <summary>
    /// Flaga oznaczająca możliwość usunięcia dokumentu
    /// </summary>
    public bool CanDelete { get; set; }

    /// <summary>
    /// Waluta
    /// </summary>
    public Currency Currency { get; set; }

    /// <summary>
    /// Sposb liczenia cen, True - wg. cen netto, False - wg. cen brutto
    /// </summary>
    public bool? PriceBaseOnNet { get; set; }

    /// <summary>
    /// Flaga czy dok ściągnięty przez API
    /// </summary>
    public bool Downloaded { get; set; }

    public int UserId { get; set; }
    public string UserName { get; set; }
  }
}
