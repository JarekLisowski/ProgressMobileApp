namespace Progress.Domain.Model
{
	public class ProductCategory
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;

		public IEnumerable<ProductCategory>? Subcategories { get; set; }

		public long? ParentId { get; set; }

		public IEnumerable<Product>? Products { get; set; }

		public int? Count { get; set; }
	}
}
