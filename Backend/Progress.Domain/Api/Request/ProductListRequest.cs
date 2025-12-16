namespace Progress.Domain.Api.Request
{
  public class ProductListRequest : ApiListRequest
  {
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public bool? OnlyAvailable { get; set; }
  }
}
