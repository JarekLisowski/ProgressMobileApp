namespace Progress.Domain.Api.Request
{
  public class ProductStocksRequest : ApiRequest
  {
    public int[] ProductIds { get; set; } = [];
  }
}
