namespace Progress.Domain.Model
{
	public class ProductPrice
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public decimal? PriceNet { get; set; }
		public decimal? PriceGross { get; set; }
		public string CurrencyName { get; set; } = "PLN";
		public int? CurrencyId { get; set; }
	}
}
