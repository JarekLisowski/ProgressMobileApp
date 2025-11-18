namespace Progress.Domain.Api;

public class ProductStock
{
	public int StTowId { get; set; }

	public int StMagId { get; set; }

	public decimal StStan { get; set; }

	public decimal StStanMin { get; set; }

	public decimal StStanRez { get; set; }

	public decimal StStanMax { get; set; }

	public Product StTow { get; set; } = null!;
}
