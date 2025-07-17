using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Progress.Database;
using Progress.Domain.Model;
using Progress.Infrastructure.Database.Repository;

namespace Progress.BusinessLogic
{
	public class PromoManager
	{
    IMapper _autoMapper;
    PromoRepository _promoRepository;
    IDatabaseRepository<PromoItem, IfxApiPromocjaPozycja> _promoItemRepository;
    IDatabaseRepository<Product, TwTowar> _dbProduct;
    string _imagesBaseFolder = "";
    NavireoDbContext _dbContext;

    public PromoManager(
      IMapper autoMapper, 
      NavireoDbContext dbContext,
      PromoRepository promoRepository,
      IDatabaseRepository<PromoItem, IfxApiPromocjaPozycja> promoItemRepository,
      IDatabaseRepository<Product, TwTowar> dbProduct,
      IConfiguration configurationProvider
      )
    {
      _autoMapper = autoMapper;
      _promoRepository = promoRepository;
      _promoItemRepository = promoItemRepository;
      _dbProduct = dbProduct;
      _imagesBaseFolder = configurationProvider.GetValue<string>("PromoImagesBaseDir") ?? "";
      _dbContext = dbContext;
    }

    public IEnumerable<PromoSet> GetPromoSetList()
      => _promoRepository.GetPromoSetList();

    public PromoSet? GetPromoSet(int id)
      => _promoRepository.GetPromoSet(id);

    public PromoItem? GetPromoItem(int id)
    {
      var data = _promoItemRepository.SelectEntity(id);
      if (data != null)
      {
        return _autoMapper.Map<PromoItem>(data);
      }
      return null;
    }

    public PromoItem? GetPromoItemDetials(int id)
    {
      var data = _promoItemRepository.EntitySet
        .AsNoTracking()
        .Include(it => it.IfxApiPromocjaPozycjaTowars)
        .FirstOrDefault(it => it.Id == id);

      if (data != null)
      {
        return _autoMapper.Map<PromoItem>(data);
      }
      return null;
    }

    //public IEnumerable<Product> GetPromoItemProducts(int id)
    //{
    //  var data = _promoItemRepository.EntitySet
    //    .AsNoTracking()
    //    .Include(it => it.IfxApiPromocjaPozycjaTowars)
    //    .FirstOrDefault(it => it.Id == id);

    //  if (data != null)
    //  {
    //    var codes = data.IfxApiPromocjaPozycjaTowars.Select(it => it.TwSymbol).ToList();
    //    var qProducts = _dbProduct.SelectWhere(it => codes.Any(itCode => itCode == it.TwSymbol));
    //    var products = qProducts.ToList();
    //    return products;
    //  }
    //  return Array.Empty<Product>();
    //}

    public byte[]? GetPromoImage(int promoId)
    {
      var promoSet = _promoRepository.Select(promoId);
      if (promoSet != null)
      {
        if (promoSet.Img != null)
          return promoSet?.Img;
        try
        {
          var bytes = File.ReadAllBytes($"{_imagesBaseFolder}\\{promoSet.Image}");
          return bytes;
        }
        catch { }
      }
      return null;
    }

    public Product[] GetProductsForPromoItem(int id)
    {
      return _promoRepository.GetProductsForPromoItem(id);
    }
  }
}
