using AutoMapper;
using Progress.Domain.Model;

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

			CreateMap<Model.Addres, Addres>();
			CreateMap<Model.Customer, Customer>();

			CreateMap<Model.User, User>();

			CreateMap<Model.PaymentMethod, PaymentMethod>();

			CreateMap<Model.DeliveryMethod, DeliveryMethod>();
		}
	}
}
