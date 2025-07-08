namespace Progress.Domain.Model
{
  public class PaymentMethod
  {
    public int Id { get; set; }

    public int FpId { get; set; }

    public bool Active { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool Deferred { get; set; }
  }
}
