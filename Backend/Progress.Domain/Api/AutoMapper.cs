using AutoMapper;
using System.Xml;

namespace Progress.Domain.Api
{
	public class AutoMapper : Profile
	{
		public AutoMapper()
		{
			CreateMap<Model.Product, Product>().ReverseMap();
			CreateMap<Model.ProductImage, ProductImage>();
			CreateMap<Model.ProductCategory, ProductCategory>();
		}
	}
}
