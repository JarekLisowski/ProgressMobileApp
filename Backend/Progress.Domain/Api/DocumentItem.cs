namespace Progress.Domain.Api
{
  public class DocumentItem
  {
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal PriceNet { get; set; }
    public decimal PriceGross { get; set; }
    public decimal LineNet { get; set; }
    public decimal LineGross { get; set; }
    public decimal TaxRate { get; set; }
    public decimal DiscountRate { get; set; }
    public int? PromoSetId { get; set; }
    public int? PromoItemId { get; set; }
    public Product? Product { get; set; }

  }
}