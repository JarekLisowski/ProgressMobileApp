namespace Progress.Domain.Api.Request
{
  public class PaymentRequest : ApiRequest
  {
    public int OperatorId { get; set; }
    public Payment? Payment { get; set; }
  }
}
