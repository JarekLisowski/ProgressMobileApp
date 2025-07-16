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

    public IEnumerable<Product> GetProductsByCategory(int id)
    {
      var dataDb = DbContext.TwCechaTws.AsNoTracking()
        .Include(it => it.ChtIdTowarNavigation)
        .ThenInclude(it => it.TwCena)
        .Where(it => it.ChtIdCecha == id)
        .Select(it => it.ChtIdTowarNavigation)
        .ToArray();
      var productList = Mapper.Map<Product[]>(dataDb);
      return productList;
    }

    public Product? GetProduct(int id, int priceLevel = 1, int? stockId = null)
    {
      var productDb = DbContext.TwTowars.AsNoTracking()
        .Include(it => it.TwCena)
        .Include(it => it.TwStans.Where(it2 => stockId == null || stockId == it2.StMagId))
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
          product.Stock = productDb.TwStans.FirstOrDefault()?.StStan ?? 0;
        }
        return product;
      }
      return null;
    }

    public Product[] SearchProduct(string searchText, int topCount)
    {
      try
      {
        var sql = $@"
                SELECT TOP {topCount} s.Rank, t.tw_Symbol, t.tw_Nazwa
                FROM [InsSearch].[Search_tw__Towar] ('{searchText}') s
                INNER JOIN tw__Towar t ON s.[Key] = t.tw_Id
                ORDER BY s.Rank DESC";
        //var dbResult = DbContext.TwSearchResult.FromSqlInterpolated(sql)
        var dbResult = DbContext.TwSearchResult.FromSqlInterpolated($@"
                SELECT s.Rank [Rank], t.tw_Id [TwId], t.tw_Symbol [TwSymbol], t.tw_Nazwa [TwNazwa]
                FROM [InsSearch].[Search_tw__Towar] ({searchText}) s
                INNER JOIN tw__Towar t ON s.[Key] = t.tw_Id")
                //ORDER BY s.Rank DESC")
              .OrderBy(it => it.Rank).Take(topCount).ToList();
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
  }
}
