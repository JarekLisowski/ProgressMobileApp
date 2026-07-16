namespace Progress.Domain.Api.Request
{
  public class ProductListRequest : ApiListRequest
  {
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public bool? OnlyAvailable { get; set; }
    public string? SearchText { get; set; }
    public string[]? Codes { get; set; }
  }
}
