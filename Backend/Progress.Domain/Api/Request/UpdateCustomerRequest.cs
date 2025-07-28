namespace Progress.Domain.Api.Request
{
  public class UpdateCustomerRequest : ApiRequest
  {
    public int OperatorId { get; set; }
    public Customer Customer { get; set; }
  }
}
