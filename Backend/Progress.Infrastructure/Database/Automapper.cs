using AutoMapper;
using Progress.Database;
using Progress.Domain.Model;

namespace Progress.Infrastructure.Database
{
	public class AutoMapper : Profile
	{
		public AutoMapper()
		{
			CreateMap<TwTowar, Product>()
				.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.TwId))
				.ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.TwNazwa))
				.ForMember(dst => dst.Code, opt => opt.MapFrom(src => src.TwSymbol))
				.ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.TwOpis))
				.ForMember(dst => dst.Unit, opt => opt.MapFrom(src => src.TwJednMiary))
				.ForMember(dst => dst.Prices, opt => opt.MapFrom((src, dst) => CreatePriceDictionary(src.TwCena)))
				.ForMember(dst => dst.Stock, opt => opt.MapFrom((src, dst) => GetStan(src)))
				;

			CreateMap<TwZdjecieTw, ProductImage>()
				.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.ZdId))
				.ForMember(dst => dst.Primary, opt => opt.MapFrom(src => src.ZdGlowne))
				.ForMember(dst => dst.ProductId, opt => opt.MapFrom(src => src.ZdIdTowar))
				.ForMember(dst => dst.Image, opt => opt.MapFrom(src => src.ZdZdjecie));

			CreateMap<SlCechaTw, ProductCategory>()
				.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.CtwId))
				.ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.CtwNazwa));

			CreateMap<IfxApiPromocjaZestaw, PromoSet>()
				.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Nazwa))
				.ForMember(dst => dst.ValidFrom, opt => opt.MapFrom(src => src.DataOd))
				.ForMember(dst => dst.ValidUntil, opt => opt.MapFrom(src => src.DataDo))
				.ForMember(dst => dst.Image, opt => opt.MapFrom(src => src.Img))
				.ForMember(dst => dst.Items, opt => opt.MapFrom(src => src.IfxApiPromocjaPozycjas));

			CreateMap<IfxApiPromocjaPozycja, PromoItem>()
				.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dst => dst.Quantity, opt => opt.MapFrom(src => src.Ilosc))
        .ForPath(dst => dst.Price.TaxPercent, opt => opt.MapFrom(src => src.StawkaVat))
				.ForPath(dst => dst.Price.PriceNet, opt => opt.MapFrom(src => src.CenaNetto))
				.ForMember(dst => dst.MinimumPrice, opt => opt.MapFrom(src => src.MinCena))
				.ForMember(dst => dst.DiscountPercent, opt => opt.MapFrom(src => src.Rabat))
				.ForMember(dst => dst.Gratis, opt => opt.MapFrom(src => src.Gratis))
				.ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Nazwa))
				.ForMember(dst => dst.DiscountSetId, opt => opt.MapFrom(src => src.ZestawId))
				.ForMember(dst => dst.Products, opt => opt.MapFrom(src => src.IfxApiPromocjaPozycjaTowars))
				;

			CreateMap<IfxApiPromocjaPozycjaTowar, PromoItemProduct>()
				.ForMember(dst => dst.ProductCode, opt => opt.MapFrom(src => src.TwSymbol))
				.ForMember(dst => dst.PromoItemId, opt => opt.MapFrom(src => src.PozycjaId));

		}

		private decimal GetStan(TwTowar towar)
		{
			return towar.TwStans.FirstOrDefault()?.StStan ?? 0;
		}

		private Dictionary<int, Price> CreatePriceDictionary(TwCena? src)
		{
			if (src == null)
				return new Dictionary<int, Price>();

			var priceIndex = 0;
			var result = new Dictionary<int, Price>()
			{
				{ priceIndex++, CreateProductPrice(src, tc => tc.TcCenaNetto0, tc => tc.TcCenaBrutto0) },
				{ priceIndex++, CreateProductPrice(src, tc => tc.TcCenaNetto1, tc => tc.TcCenaBrutto1) },
				{ priceIndex++, CreateProductPrice(src, tc => tc.TcCenaNetto2, tc => tc.TcCenaBrutto2) },
				{ priceIndex++, CreateProductPrice(src, tc => tc.TcCenaNetto3, tc => tc.TcCenaBrutto3) },
				{ priceIndex++, CreateProductPrice(src, tc => tc.TcCenaNetto4, tc => tc.TcCenaBrutto4) },
				{ priceIndex++, CreateProductPrice(src, tc => tc.TcCenaNetto5, tc => tc.TcCenaBrutto5) },
				{ priceIndex++, CreateProductPrice(src, tc => tc.TcCenaNetto6, tc => tc.TcCenaBrutto6) },
				{ priceIndex++, CreateProductPrice(src, tc => tc.TcCenaNetto7, tc => tc.TcCenaBrutto7) },
				{ priceIndex++, CreateProductPrice(src, tc => tc.TcCenaNetto8, tc => tc.TcCenaBrutto8) },
				{ priceIndex++, CreateProductPrice(src, tc => tc.TcCenaNetto9, tc => tc.TcCenaBrutto9) },
				{ priceIndex++, CreateProductPrice(src, tc => tc.TcCenaNetto10, tc => tc.TcCenaBrutto10) },
			};
			return result;
		}

		private Price CreateProductPrice(TwCena src, Func<TwCena, decimal?> funcNetto, Func<TwCena, decimal?> funcBrutto)
		{
			return new Price()
			{
				Id = src.TcId,
				PriceNet = funcNetto(src),
				PriceGross = funcBrutto(src),
			};
		}
	}
}
