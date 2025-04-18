namespace Progress.Domain.Api;

public class Product
{
  public int Id { get; set; }
  public string Code { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public decimal Stock { get; set; }
  public Price Price { get; set; } = new();
  public Dictionary<int, Price> Prices { get; set; } = new Dictionary<int, Price>();
  public string CategoryName { get; set; } = string.Empty;
  public int CategoryId { get; set; }
  public string BarCode { get; set; } = string.Empty;
  public int ImagesCount { get; set; } = 0;
  public string Unit { get; set; } = "szt.";
  public string ImgUrl { get; set; } = "";
}

