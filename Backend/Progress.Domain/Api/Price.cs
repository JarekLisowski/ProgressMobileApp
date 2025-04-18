namespace Progress.Domain.Api;

public class Price
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public decimal? PriceNet { get; set; }
  public decimal? PriceGross { get; set; }
  public string CurrencyName { get; set; } = "PLN";
  public decimal? TaxPercent { get; set; }
  public string TaxName { get; set; } = string.Empty;
  public int? CurrencyId { get; set; }
}
