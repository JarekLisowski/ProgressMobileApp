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
        .Where(it => it.ChtIdCecha == id && it.ChtIdTowarNavigation.TwZablokowany == false && it.ChtIdTowarNavigation.TwSprzedazMobilna == true)
        .OrderBy(it => it.ChtIdTowarNavigation.TwNazwa)
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

    public IEnumerable<Product> GetProductsByGroup(int id, int? categoryId = null, int? stockId = null, int? stockId2 = null, bool onlyAvailable = false)
    {
      var dataDb = DbContext.TwTowars.AsNoTracking()
          .Include(it => it.TwCena)
          .Include(it => it.TwCechaTws.Where(itc => itc.ChtIdCecha == categoryId))
          .Include(it => it.TwStans.Where(it2 => (stockId == null || stockId == it2.StMagId || stockId2 == it2.StMagId)))
        .Where(it => it.TwIdGrupa == id && it.TwZablokowany == false && it.TwSprzedazMobilna == true && it.TwRodzaj == 1)
        .OrderBy(it => it.TwNazwa)
        .ToArray();

      if (categoryId != null)
        dataDb = dataDb.Where(it => it.TwCechaTws.Any()).ToArray();

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
        .FirstOrDefault(it => it.TwId == id && it.TwZablokowany == false && it.TwSprzedazMobilna == true && it.TwRodzaj == 1);
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
        var dbResult = DbContext.TwSearchResult.FromSqlInterpolated($@"
                SELECT s.Rank [Rank], t.tw_Id [TwId], t.tw_Symbol [TwSymbol], t.tw_Nazwa [TwNazwa], t.tw_Zablokowany [TwZablokowany], t.tw_SprzedazMobilna [TwSprzedazMobilna]
                FROM [InsSearch].[Search_tw__Towar] ({searchText}) s
                INNER JOIN tw__Towar t ON s.[Key] = t.tw_Id")
              .Where(it => it.TwZablokowany == false && it.TwSprzedazMobilna == true)
              .OrderByDescending(it => it.Rank).Take(topCount).ToList();
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

    public Product[] SearchProduct(string searchText, int topCount, int storeId1, int storeId2)
    {
      try
      {
        var dbResult = DbContext.TwSearchResult.FromSqlInterpolated($@"
                SELECT s.Rank [Rank], t.tw_Id [TwId], t.tw_Symbol [TwSymbol], t.tw_Nazwa [TwNazwa], t.tw_Zablokowany [TwZablokowany], t.tw_SprzedazMobilna [TwSprzedazMobilna], 
                FROM [InsSearch].[Search_tw__Towar] ({searchText}) s
                INNER JOIN tw__Towar t ON s.[Key] = t.tw_Id
                INNER JOIN tw_Cena tc ON t.tw_Id = tc.tc_IdTowar
                ")
              .Where(it => it.TwZablokowany == false && it.TwSprzedazMobilna == true)
              .OrderByDescending(it => it.Rank).Take(topCount).ToList();
        return dbResult.Select(it => new Product
        {
          Code = it.TwSymbol,
          Name = it.TwNazwa,
          Id = it.TwId,
          ImagesCount = DbContext.TwZdjecieTws.Count(itz => itz.ZdIdTowar == it.TwId),

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

    public ProductCategory[] GetCategoriesInGroup(int groupId, bool stored = true)
    {
      if (!stored)
      {
        var dataDb = DbContext.TwCechaTws.AsNoTracking()
          .Where(it => it.ChtIdTowarNavigation.TwIdGrupa == groupId
              && it.ChtIdTowarNavigation.TwZablokowany == false
              && it.ChtIdTowarNavigation.TwSprzedazMobilna == true
              && it.ChtIdTowarNavigation.TwRodzaj == 1
              && it.ChtIdTowarNavigation.TwIdGrupa == groupId
              )
          .GroupBy(it => it.ChtIdCechaNavigation)
          .Select(it => new { CechaTw = it.Key, Count = it.Count() })
          .ToArray();
        var data = dataDb.Select(it => new ProductCategory
        {
          Id = it.CechaTw.CtwId,
          Name = it.CechaTw.CtwNazwa,
          Count = it.Count
        }).ToArray();
        return data;
      }
      else
      {
        var dataDb = DbContext.IfxGrupaCechy.Where(it => it.GrId == groupId).ToArray();
        var result = dataDb.Select(it => new ProductCategory
        {
          Id = it.CechaId,
          Name = it.Nazwa,
          Count = it.Count
        }).OrderByDescending(it => it.Count).ToArray();
        return result;
      }
    }

    public void AddGroupCategories(int groupId, ProductCategory[] categories)
    {
      DbContext.IfxGrupaCechy.Where(it => it.GrId == groupId).ExecuteDelete();
      var data = categories.Select(it => new IfxGrupaCechy
      { 
        GrId = groupId,
        CechaId = it.Id,
        Nazwa = it.Name,
        Count = it.Count ?? 0
      });
      DbContext.AddRange(data);
      DbContext.SaveChanges();
    }
  }
}
