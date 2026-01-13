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
        var data = _productManager.GetProductsByCategory(request.CategoryId.Value, user.StoreId, 1, request.OnlyAvailable ?? false);
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

    [HttpPost("search-products")]
    public ProductListResponse SearchProducts(ProductListRequest request)
    {
      var user = GetUser();
      if (request.SearchText != null && user != null)
      {
        //var data = _productManager.SearchProduct2(request.SearchText, user.StoreId ?? 0, 1, 1000, request.OnlyAvailable ?? false);
        //foreach (var item in data)
        //{
        //  item.SetupUserPrices(user);
        //}
        //return new ProductListResponse
        //{
        //  Data = _mapper.Map<Product[]>(data),
        //};
      }
      return new ProductListResponse();
    }

    [HttpPost("listByBrand")]
    public ProductListResponse GetProductsFromBrand(ProductListRequest request)
    {
      var user = GetUser();
      if (request.BrandId != null && user != null)
      {
        var data = _productManager.GetProductsByGroup(request.BrandId.Value, request.CategoryId, user.StoreId, 1, request.OnlyAvailable ?? false);
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
    public ProductResponse Get(ProductRequest request)
    {
      var user = GetUser();
      if (user == null)
        return new ProductResponse
        {
          IsError = true,
          Message = "User not logged in"
        };
      var priceLevel = user.DefaultPrice;
      var stockId = user.StoreId;
      var stockId2 = 1;

      var product = _productManager.GetProduct(request.ProductId, priceLevel, stockId, stockId2);
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



    [HttpPost("stocks")]
    public ProductsStockResponse GetStocks(ProductStocksRequest request)
    {
      var user = GetUser();
      if (user != null && user.StoreId != null)
      {
        var data = _productManager.GetStocks(user.StoreId.Value, request.ProductIds);
        return new ProductsStockResponse
        {
          Data = _mapper.Map<ProductStock[]>(data)
        };
      }
      return new ProductsStockResponse
      {
        IsError = true,
        Message = "No user"
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

    [HttpPost("brand/list")]
    public ProductCategoryListResponse GetBrandList(string? search = null)
    {

      var data = _productManager.GetGroupsList(search);
      return new ProductCategoryListResponse
      {
        Data = _mapper.Map<ProductCategory[]>(data)
      };
    }

    [HttpPost("brand/info/{id}")]
    public ProductCategoryInfoResponse GetGroupInfo(int id)
    {
      var data = _productManager.GetGroupInfo(id);
      return new ProductCategoryInfoResponse
      {
        Data = _mapper.Map<ProductCategory>(data)
      };
    }

    [HttpPost("brand/{id}/categories")]
    public ProductCategoryListResponse GetBrandCategories(int id)
    {
      var data = _productManager.GetGroupCategories(id);
      return new ProductCategoryListResponse
      {
        Data = _mapper.Map<ProductCategory[]>(data)
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
      result.Brands = _mapper.Map<ProductCategory[]>(_productManager.GetGroupsList(searchtext));
      return result;
    }

    [AllowAnonymous]
    [HttpGet("CreateGroupCategories")]
    public string CreateGroupCategories()
    {
      _productManager.BuildGroupCategories();
      return "OK";
    }


  }
}
