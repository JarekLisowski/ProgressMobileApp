namespace Progress.Domain.Api
{
  public class DeliveryMethod
  {
    public int Id { get; set; }
    public int TwId { get; set; }
    public bool Active { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceNet { get; set; }
    public decimal PriceGross { get; set; }
  }
}
