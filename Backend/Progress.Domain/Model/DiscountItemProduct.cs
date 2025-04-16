namespace Progress.Domain.Model
{
	public class DiscountItemProduct
	{
		public int ItemId { get; set; }
		public string ProductCode { get; set; } = string.Empty;
		public int? ProductId { get; set; }
	}
}
