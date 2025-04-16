namespace Progress.Domain.Api
{
	public class ProductCategory
	{
		public long Id { get; set; }
		public string Name { get; set; } = string.Empty;

		public IEnumerable<ProductCategory>? Subcategories { get; set; }

		public long? ParentId { get; set; }

	}
}
