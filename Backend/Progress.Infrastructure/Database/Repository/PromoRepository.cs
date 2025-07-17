using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Progress.Database;
using Progress.Domain.Model;

namespace Progress.Infrastructure.Database.Repository
{
  public class PromoRepository : DatabaseRepository<PromoSet, IfxApiPromocjaZestaw>
  {
    IMapper _mapper;
    DateTime? overrideToday = new DateTime(2025, 4, 1);

    public PromoRepository(NavireoDbContext dbContext, IConfigurationProvider automapperConfiguration, IMapper mapper)
      : base(dbContext, automapperConfiguration, nameof(IfxApiPromocjaZestaw.Id), x => x.Id, x => x.Id)
    {
      _mapper = mapper;
    }

    public IEnumerable<PromoSet> GetPromoSetList()
    {
      var today = overrideToday ?? DateTime.Today;
      var data = EntitySet
        .AsNoTracking()
        .Where(it => it.DataOd <= today && it.DataDo >= today)
        .ToArray();
      
      if (data == null)
        return Array.Empty<PromoSet>();

      var result = _mapper.Map<PromoSet[]>(data);
      return result;
    }

    public PromoSet? GetPromoSet(int id)
    {
      var today = overrideToday ?? DateTime.Today;
      var data = EntitySet
        .AsNoTracking()
        .Include(it => it.IfxApiPromocjaPozycjas)
        .FirstOrDefault(it => it.Id == id && it.DataOd <= today && it.DataDo >= today);      
      if (data == null) 
        return null;
      
      var result = _mapper.Map<PromoSet>(data);
      return result;
    }

    public Product[] GetProductsForPromoItem(int id)
    {
      var data = from promo_poz in DbContext.IfxApiPromocjaPozycjas.AsNoTracking()
                 join promo_tw in DbContext.IfxApiPromocjaPozycjaTowars.AsNoTracking() on promo_poz.Id equals promo_tw.PozycjaId
                 join tw in DbContext.TwTowars.AsNoTracking() on promo_tw.TwSymbol equals tw.TwSymbol
                 join tc in DbContext.TwCenas on tw.TwId equals tc.TcIdTowar
                 join vat in DbContext.SlStawkaVats on tw.TwIdVatSp equals vat.VatId
                 where promo_poz.Id == id
                 orderby promo_poz.Id
                 select new Product
                 {
                   Id = tw.TwId,
                   Code = tw.TwSymbol,
                   Description = tw.TwOpis,
                   Name = tw.TwNazwa,
                   Unit = tw.TwJednMiary,
                   TaxName = vat.VatNazwa,
                   TaxRate = vat.VatStawka,
                   Price = new Price
                   {
                     PriceNet = promo_poz.Gratis ? 0 : promo_poz.CenaNetto > 0 ? promo_poz.CenaNetto : tc.TcCenaNetto1 ?? 0,
                     TaxPercent = vat.VatStawka
                   }
                 };
      var products = data.ToArray();
      foreach(var item in products)
      {
        item.Price.PriceGross = Math.Round((item.Price.PriceNet ?? 0) * (1 + item.TaxRate / 100), 2);
      }
      return products;
    }
  }
}
