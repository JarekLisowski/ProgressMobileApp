namespace Progress.Domain.Api;

public class PromoItem
{
  public int Id { get; set; }

  public string Name { get; set; } = null!;

  public int SetId { get; set; }

  public bool Gratis { get; set; }

  public Price Price { get; set; } = new();

  public int Quantity { get; set; }

  public int DiscountPercent { get; set; }

  public decimal? MinimumPrice { get; set; }

  public int DiscountSetId { get; set; }

  public PromoItemProduct[] Products { get; set; } = [];

}
