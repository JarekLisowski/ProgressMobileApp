using AutoMapper;
using System.Xml;

namespace Progress.Domain.Api
{
	public class AutoMapper : Profile
	{
		public AutoMapper()
		{
			CreateMap<Model.Product, Product>();
			CreateMap<Model.ProductImage, ProductImage>();
			CreateMap<Model.ProductCategory, ProductCategory>();
			CreateMap<Model.Price, Price>();

			CreateMap<Model.PromoSet, PromoSet>();
			CreateMap<Model.PromoItem, PromoItem>();
		}
	}
}
