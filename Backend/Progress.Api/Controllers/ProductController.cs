using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.BusinessLogic;
using Progress.Domain.Api;
using Progress.Domain.Api.Request;
using Progress.Domain.Api.Response;
using Progress.Domain.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Progress.Api.Controllers
{
  [Authorize]
  [Route("api/product")]
  [ApiController]
  public class ProductController : ApiControllerBase
  {
    ProductManager _productManager;
    IMapper _mapper;

    public ProductController(ProductManager productManager, IMapper autoMapper, IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
      _productManager = productManager;
      _mapper = autoMapper;
    }

    [HttpPost("list")]
    public ProductListResponse GetProductsFromCategory(ProductListRequest request)
    {
      var user = GetUser();
      if (request.CategoryId != null && user != null)
      {
        var data = _productManager.GetProductsByCategory(request.CategoryId.Value);
        foreach (var item in data)
        {
          item.SetupUserPrices(user);
        }
        return new ProductListResponse
        {
          Data = _mapper.Map<Product[]>(data),
        };
      }
      return new ProductListResponse();
    }

    [AllowAnonymous]
    [HttpGet("image/{productId}")]
    public ActionResult GetProductImage(int productId, int? number = 0)
    {
      var image = _productManager.GetProductImage(productId, number ?? 0);
      if (image != null && image.Image != null)
      {
        return File(image.Image, "image/jpeg");
      }
      return NotFound();
    }

    [HttpPost("details")]
    public ProductResponse? Get(ProductRequest request)
    {
      var priceLevel = GetUser().DefaultPrice;
      var stockId = 1;

      var product = _productManager.GetProduct(request.ProductId, priceLevel, stockId);
      if (product != null)
        return new ProductResponse
        {
          Data = _mapper.Map<Product>(product)
        };
      return new ProductResponse
      {
        IsError = true,
        Message = $"No product Id: {request.ProductId}"
      };
    }

    [HttpPost("category/list")]
    public ProductCategoryListResponse GetCategoryList(string? search = null)
    {
      
      var data = _productManager.GetCategoryList(search);
      return new ProductCategoryListResponse
      {
        Data = _mapper.Map<ProductCategory[]>(data)
      };
    }

    [HttpPost("category/info/{id}")]
    public ProductCategoryInfoResponse GetCategoryInfo(int id)
    {
      var data = _productManager.GetCategoryInfo(id);
      return new ProductCategoryInfoResponse
      {
        Data = _mapper.Map<ProductCategory>(data)
      };
    }

    [HttpGet("search")]
    public SearchResponse Search(string searchtext)
    {
      var result = new SearchResponse();

      result.ProductCategories = _mapper.Map<ProductCategory[]>(_productManager.GetCategoryList(searchtext));
      if (searchtext.Length >= 3)
      {
        var items = _productManager.SearchProduct(searchtext, 15);
        result.Products = items.Select(it => new Product
        {
          Id = it.Id,
          Name = it.Name,
          Stock = it.Stock,
          Code = it.Code,
          CategoryName = it.CategoryName,
        }).ToArray();
      }

      return result;
    }
  }
}
