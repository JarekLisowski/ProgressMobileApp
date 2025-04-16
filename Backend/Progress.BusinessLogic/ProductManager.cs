using AutoMapper;
using Progress.Database;
using Progress.Domain.Model;
using Progress.Infrastructure.Database.Repository;

namespace Progress.BusinessLogic
{
	public class ProductManager
	{
		IDatabaseRepository<Product, TwTowar> dbProduct;
		IDatabaseRepository<ProductImage, TwZdjecieTw> dbProductImage;
		IDatabaseRepository<ProductCategory, SlCechaTw> dbProductCategoryDictionary;
		IDatabaseRepository<ProductCategory, TwCechaTw> dbProductCategory;
		IDatabaseRepository<ProductCategory, TwStan> dbProductStock;
		ProductRepository dbProductRepository;

		IMapper mapper;
		IConfigurationProvider automapperConfiguration;

		public ProductManager(
			IDatabaseRepository<Product, TwTowar> repoTowary,
			IDatabaseRepository<ProductImage, TwZdjecieTw> repoTowarZdjecie,
			IDatabaseRepository<ProductCategory, TwCechaTw> repoProductCategory2,
			IDatabaseRepository<ProductCategory, SlCechaTw> repoProductCategory,
			ProductRepository productRepository,
			IMapper autoMapper,
			IConfigurationProvider automapperConfigurationProvider
			) 
		{ 
			dbProduct = repoTowary;
			dbProductImage = repoTowarZdjecie;
			dbProductCategory = repoProductCategory2;
			dbProductRepository = productRepository;
			dbProductCategoryDictionary = repoProductCategory;
			mapper = autoMapper;
			automapperConfiguration = automapperConfigurationProvider;
		}

		public IEnumerable<Product> GetProductsByCategory(int id)
		{
			return dbProductRepository.GetProductsByCategory(id);
		}

		public IEnumerable<ProductCategory> GetCategoryList(string? search)
		{
			if (string.IsNullOrWhiteSpace(search))
				return dbProductCategoryDictionary.SelectWhere(it => true).OrderBy(it => it.Name).ToArray();
			return dbProductCategoryDictionary.SelectWhere(it => it.CtwNazwa.StartsWith(search)).OrderBy(it => it.Name).ToArray();
		}

        public ProductCategory? GetCategoryInfo(int id)
        {
			var category = dbProductCategoryDictionary.SelectWhere(it => it.CtwId == id).FirstOrDefault();
			return category;
        }

        public Product? GetProduct(int id)
		{
			return dbProductRepository.GetProduct(id);
		}

		public ProductImage? GetProductImage(int productId, int number = 0)
		{
			if (number == 0)
				return dbProductImage.SelectWhere(it => it.ZdIdTowar == productId && it.ZdGlowne == true).FirstOrDefault();
			
			var zdjecie = dbProductImage.EntitySet
				.Where(it => it.ZdIdTowar == productId)
				.OrderByDescending(it => it.ZdGlowne)
				.ThenBy(it => it.ZdId)
				.Skip(number)
				.Take(1)
				.FirstOrDefault();
			var result = mapper.Map<ProductImage>(zdjecie);
			return result;

		}
	}
}
