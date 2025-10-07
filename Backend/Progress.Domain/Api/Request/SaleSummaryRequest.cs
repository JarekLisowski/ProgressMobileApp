namespace Progress.Domain.Api.Request
{
  public class SaleSummaryRequest
  {
    public int OperatorId { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string CurrencyCode { get; set; } = "PLN";
}
}
