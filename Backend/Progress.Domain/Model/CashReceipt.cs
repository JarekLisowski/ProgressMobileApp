namespace Progress.Domain.Model
{
  public class CashReceipt
  {
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime Date {  get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public Customer? Customer { get; set; }
    public Customer? Seller { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int UserId { get; set; }
  }
}
