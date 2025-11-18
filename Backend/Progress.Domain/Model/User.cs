namespace Progress.Domain.Model
{
  /// <summary>
  /// Użytkownik Mobilny
  /// </summary>
  public class User
  {
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Flaga czy jest mobilny
    /// </summary>
    public bool Mobile { get; set; }

    /// <summary>
    /// Symbol
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Login
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Hasło
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Imie
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Nazwisko
    /// </summary>
    public string Surname { get; set; } = string.Empty;

    /// <summary>
    /// Id urządzenia
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// Nazwa magazynu
    /// </summary>
    public string? StoreName { get; set; }

    /// <summary>
    /// Nazwa kasy
    /// </summary>
    public string? Kasa { get; set; }

    public int? KasaId { get; set; }

    /// <summary>
    /// Id przypisanej cechy
    /// </summary>
    public int? CechaId { get; set; }

    /// <summary>
    /// Nazwa cechy
    /// </summary>
    public string CechaNazwa { get; set; } = string.Empty;

    /// <summary>
    /// Domyślny poziom cenowy
    /// </summary>
    public int DefaultPrice { get; set; }

    /// <summary>
    /// Lista dostępnych poziomów cenowych
    /// </summary>
    public List<PriceLevel> PriceLevelList { get; set; } = new List<PriceLevel>();

    /// <summary>
    /// Flaga czy obsługuje płatności odroczone
    /// </summary>
    public bool SpecialPayment { get; set; } //płatności odroczone np kredyt kupiecki

    /// <summary>
    /// Flaga czy może wydłużyć czas spłaty dokumentu
    /// </summary>
    public bool SpecialPaymentExtendDeadline { get; set; }

    /// <summary>
    /// Maksymalna kwota płatności odroczonej
    /// </summary>
    public decimal MaxSpecialPayment { get; set; }

    public decimal MinSpecialPayment { get; set; }

    /// <summary>
    /// Typ sprzedaży hurt/detal/hurt i detal
    /// </summary>
    //public IFoxCommerce.BO.Constants.RodzajSprzedazy TypSprzedazy { get; set; }

    public int PromocjaGrupaId { get; set; }

    public bool DiscountAllowed { get; set; }
    public bool CanExtendPaymentDeadline { get; set; }
    public int DiscountMax { get; set; }
    public int? StoreId { get; set; }
  }
}
