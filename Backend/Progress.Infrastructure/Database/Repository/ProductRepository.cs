using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Progress.Database;
using Progress.Domain.Model;

namespace Progress.Infrastructure.Database.Repository
{
	public class ProductRepository : DatabaseRepository<Product, TwTowar>
	{
		public ProductRepository(NavireoDbContext dbContext, IConfigurationProvider automapperConfiguration)
			: base(dbContext, automapperConfiguration, nameof(TwTowar.TwId), x => x.TwId, x => x.Id)
		{
		}

		public IEnumerable<Product> GetProductsByCategory(int id, int priceLevel = 1)
		{
			var dataDb = DbContext.TwCechaTws.AsNoTracking()
				.Include(it => it.ChtIdTowarNavigation)
				.ThenInclude(it => it.TwCena)
				.Where(it => it.ChtIdCecha == id)
				.Select(it => it.ChtIdTowarNavigation)
				.ToArray();
			var productList = Mapper.Map<Product[]>(dataDb);
			foreach (var product in productList)
			{
                product.Price = product.Prices[priceLevel];

            }
            return productList;
		}

		public Product? GetProduct(int id, int priceLevel = 1)
		{
			var productDb = DbContext.TwTowars.AsNoTracking()
				.Include(it => it.TwCena)
				.Include(it => it.TwStans)
				.Include(it => it.TwCechaTws).ThenInclude(it => it.ChtIdCechaNavigation)
				.FirstOrDefault(it => it.TwId == id);
			if (productDb != null)
			{
				var product = Mapper.Map<Product>(productDb);
				if (product != null)
				{
					product.ImagesCount = DbContext.TwZdjecieTws.Count(it => it.ZdIdTowar == id);
					product.Price = product.Prices[priceLevel];
					product.CategoryName = productDb.TwCechaTws.FirstOrDefault()?.ChtIdCechaNavigation.CtwNazwa ?? "";
					product.CategoryId = productDb.TwCechaTws.FirstOrDefault()?.ChtIdCechaNavigation.CtwId ?? 0;
				}
				return product;
			} 
			return null;
		}
	}
}
