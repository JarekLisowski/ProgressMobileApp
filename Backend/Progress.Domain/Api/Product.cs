using Progress.Domain.Model;

namespace Progress.Domain.Api;

public class Product
{
	public int Id { get; set; }
	public string Code { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public decimal Stock { get; set; }
    public ProductPrice Price { get; set; } = new();
    public Dictionary<int, ProductPrice> Prices { get; set; } = new Dictionary<int, ProductPrice>();
	public decimal TaxRate { get; set; }
	public decimal TaxName { get; set; }
	public string CategoryName { get; set; } = string.Empty;
	public int CategoryId { get; set; }
	public string BarCode { get; set; } = string.Empty;

	public int ImagesCount { get; set; } = 0;
}

