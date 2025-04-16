namespace Progress.Domain.Model
{
	public class DiscountItem
	{
		public int Id { get; set; }
		public int DiscountSetId { get; set; }
		public string Name { get; set; }	= string.Empty;
		public bool Gratis { get; set; }
		public decimal PriceNet { get; set; }
		public int Number { get; set; }
		public decimal TaxPercent {  get; set; }
		public decimal DiscountPercent { get; set; }
		public decimal MinimumPrice { get; set; }
		public IEnumerable<DiscountItemProduct>? Products { get; set; }

	}
}
