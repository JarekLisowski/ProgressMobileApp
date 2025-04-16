using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Progress.BusinessLogic;
using Progress.Domain.Api;
using Progress.Domain.Api.Request;
using Progress.Domain.Api.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Progress.Api.Controllers
{
  [Route("api/product")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		ProductManager _productManager;
		IMapper _mapper;

		public ProductController(ProductManager productManager, IMapper autoMapper) 
		{
			_productManager = productManager;
			_mapper = autoMapper;
		}

		[HttpPost("list")]
		public ProductListResponse GetProductsFromCategory(ProductListRequest request)
		{
			if (request.CategoryId != null)
			{
				var data = _productManager.GetProductsByCategory(request.CategoryId.Value);
				return new ProductListResponse
				{
					Data = _mapper.Map<Product[]>(data),
				};
			}
			return new ProductListResponse();
		}

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
			var product = _productManager.GetProduct(request.ProductId);
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
    }
}
