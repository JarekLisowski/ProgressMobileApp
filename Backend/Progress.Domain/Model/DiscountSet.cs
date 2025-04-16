namespace Progress.Domain.Model
{
	public class DiscountSet
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public DateTime ValidFrom { get; set; }
		public DateTime ValidUntil { get; set; }
		public byte[]? Image { get; set; }
		public IEnumerable<DiscountItem>? Items { get; set; }
	}
}
