namespace Progress.Domain.Api.Response
{
  public class SearchResponse
  {
    public ProductCategory[]? ProductCategories { get; set; }

    public Product[]? Products { get; set; }
  }
}
