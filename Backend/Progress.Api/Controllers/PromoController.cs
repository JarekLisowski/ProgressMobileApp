using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Progress.BusinessLogic;
using Progress.Domain.Api;
using Progress.Domain.Api.Response;

namespace Progress.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PromoController : ControllerBase
  {
    IMapper _mapper;
    PromoManager _promoManager;

    public PromoController(IMapper autoMapper, PromoManager promoManager)
    {
      _mapper = autoMapper;
      _promoManager = promoManager;
    }

    [HttpPost("list")]
    public PromoSetListResponse List()
    {
      var dbData = _promoManager.GetPromoSetList();
      var data = _mapper.Map<PromoSet[]>(dbData);
      foreach(var item in data)
        item.ImgUrl = $"api/promo/image/{item.Id}";

      return new PromoSetListResponse
      {
        Data = data
      };
    }

    [HttpPost("{id}")]
    public PromoSetResponse Details(int id)
    {
      var dbData = _promoManager.GetPromoSet(id);
      var data = _mapper.Map<PromoSet>(dbData);
      return new PromoSetResponse
      {
        Data = data
      };
    }

    [HttpPost("Item/{id}")]
    public PromoResponse PromoItemDetails(int id)
    {
      var dbData = _promoManager.GetPromoItemDetials(id);
      var data = _mapper.Map<PromoItem>(dbData);
      return new PromoResponse
      {
        Data = data
      };
    }


    [HttpGet("image/{promoId}")]
    public ActionResult GetProductImage(int promoId)
    {
      var image = _promoManager.GetPromoImage(promoId);
      if (image != null )
      {
        return File(image, "image/jpeg");
      }
      return NotFound();
    }

    [HttpPost("productsForPromoItem/{id}")]
    public ProductListResponse GetProductsForPromoItem(int id)
    {
      var data = _promoManager.GetProductsForPromoItem(id);
      return new ProductListResponse
      {
        Data = _mapper.Map<Product[]>(data)
      };
    }
  }
}
