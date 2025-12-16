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

    public IEnumerable<Product> GetProductsByCategory(int id, int? stockId = null, int? stockId2 = null, bool onlyAvailable = false)
    {
      var dataDb = DbContext.TwCechaTws.AsNoTracking()
        .Include(it => it.ChtIdTowarNavigation)
          .ThenInclude(it => it.TwCena)
        .Include(it => it.ChtIdTowarNavigation)
          .ThenInclude(it => it.TwStans.Where(it2 => (stockId == null || stockId == it2.StMagId || stockId2 == it2.StMagId)))
        .Where(it => it.ChtIdCecha == id && it.ChtIdTowarNavigation.TwZablokowany == false)
        .Select(it => it.ChtIdTowarNavigation)
        .ToArray();

      var productList = new List<Product>();
      foreach (var productDb in dataDb)
      {
        var stock = productDb.TwStans.FirstOrDefault(it => it.StMagId == stockId)?.StStan ?? 0;
        var stockSecondary = productDb.TwStans.FirstOrDefault(it => it.StMagId == stockId2)?.StStan ?? 0;
        if (!onlyAvailable || stock > 0 || stockSecondary > 0)
        {
          var product = Mapper.Map<Product>(productDb);
          product.Stock = stock;
          product.StockSecondary = stockSecondary;
          productList.Add(product);
        }
      }
      return productList;
    }

    public IEnumerable<Product> GetProductsByGroup(int id, int? stockId = null, int? stockId2 = null, bool onlyAvailable = false)
    {
      var dataDb = DbContext.TwTowars.AsNoTracking()
          .Include(it => it.TwCena)
          .Include(it => it.TwStans.Where(it2 => (stockId == null || stockId == it2.StMagId || stockId2 == it2.StMagId)))
        .Where(it => it.TwIdGrupa == id && it.TwZablokowany == false)
        .ToArray();

      var productList = new List<Product>();
      foreach (var productDb in dataDb)
      {
        var stock = productDb.TwStans.FirstOrDefault(it => it.StMagId == stockId)?.StStan ?? 0;
        var stockSecondary = productDb.TwStans.FirstOrDefault(it => it.StMagId == stockId2)?.StStan ?? 0;
        if (!onlyAvailable || stock > 0 || stockSecondary > 0)
        {
          var product = Mapper.Map<Product>(productDb);
          product.Stock = stock;
          product.StockSecondary = stockSecondary;
          productList.Add(product);
        }
      }
      return productList;
    }

    public Product? GetProduct(int id, int priceLevel = 1, int? stockId = null, int? stockId2 = null)
    {
      var productDb = DbContext.TwTowars.AsNoTracking()
        .Include(it => it.TwCena)
        .Include(it => it.TwStans.Where(it2 => (stockId == null || stockId == it2.StMagId || stockId2 == it2.StMagId) ))
        .Include(it => it.TwCechaTws).ThenInclude(it => it.ChtIdCechaNavigation)
        .FirstOrDefault(it => it.TwId == id && it.TwZablokowany == false);
      if (productDb != null)
      {
        var product = Mapper.Map<Product>(productDb);
        if (product != null)
        {
          product.ImagesCount = DbContext.TwZdjecieTws.Count(it => it.ZdIdTowar == id);
          product.Price = product.Prices[priceLevel];
          product.CategoryName = productDb.TwCechaTws.FirstOrDefault()?.ChtIdCechaNavigation.CtwNazwa ?? "";
          product.CategoryId = productDb.TwCechaTws.FirstOrDefault()?.ChtIdCechaNavigation.CtwId ?? 0;
          product.Stock = productDb.TwStans.FirstOrDefault(it => it.StMagId == stockId)?.StStan ?? 0;
          product.StockSecondary = productDb.TwStans.FirstOrDefault(it => it.StMagId == stockId2)?.StStan ?? 0;
        }
        return product;
      }
      return null;
    }

    public Product[] SearchProduct(string searchText, int topCount)
    {
      try
      {
        //var sql = $@"
        //        SELECT TOP {topCount} s.Rank, t.tw_Symbol, t.tw_Nazwa
        //        FROM [InsSearch].[Search_tw__Towar] ('{searchText}') s
        //        INNER JOIN tw__Towar t ON s.[Key] = t.tw_Id
        //        ORDER BY s.Rank DESC";
        //var dbResult = DbContext.TwSearchResult.FromSqlInterpolated(sql)
        var dbResult = DbContext.TwSearchResult.FromSqlInterpolated($@"
                SELECT s.Rank [Rank], t.tw_Id [TwId], t.tw_Symbol [TwSymbol], t.tw_Nazwa [TwNazwa]
                FROM [InsSearch].[Search_tw__Towar] ({searchText}) s
                INNER JOIN tw__Towar t ON s.[Key] = t.tw_Id")
              .OrderByDescending(it => it.Rank).Take(topCount).ToList();
        //return Mapper.Map<Product[]>(dbResult);
        return dbResult.Select(it => new Product
        {
          Code = it.TwSymbol,
          Name = it.TwNazwa,
          Id = it.TwId,
        }).ToArray();
      }
      catch (Exception ex)
      {
      }
      return [];
    }

    public IEnumerable<ProductStock> GetStocks(int stockId, int[] productIds)
    {

      try
      {
        var data = DbContext.TwStans.Where(it => it.StMagId == stockId && productIds.Contains(it.StTowId));
        var result = Mapper.Map<ProductStock[]>(data);
        return result;
      }
      catch (Exception ex)
      {
        return [];
      }
    }
  }
}
