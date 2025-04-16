namespace Progress.Domain.Model;

public partial class ProductStock
{
	public int StTowId { get; set; }

	public int StMagId { get; set; }

	public decimal StStan { get; set; }

	public decimal StStanMin { get; set; }

	public decimal StStanRez { get; set; }

	public decimal StStanMax { get; set; }

	//public virtual SlMagazyn StMag { get; set; } = null!;

	public Product StTow { get; set; } = null!;
}
