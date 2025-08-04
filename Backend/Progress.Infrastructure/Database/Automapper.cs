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

			CreateMap<TwTowarShort, TwTowar>();

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

			CreateMap<IfVwKontrahent, Customer>()
				.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.KhId))
				.ForMember(dst => dst.AdrCity, opt => opt.MapFrom(src => src.AdrMiejscowosc))
				.ForMember(dst => dst.AdrCountry, opt => opt.MapFrom(src => src.AdrPaNazwa))
				.ForMember(dst => dst.AdrCountryCode, opt => opt.MapFrom(src => src.AdrPaKod))
				.ForMember(dst => dst.AdrCountryId, opt => opt.MapFrom(src => src.AdrPaId))
				.ForMember(dst => dst.AdrName, opt => opt.MapFrom(src => src.AdrNazwa))
				.ForMember(dst => dst.AdrNameFull, opt => opt.MapFrom(src => src.AdrNazwaPelna))
				.ForMember(dst => dst.AdrNip, opt => opt.MapFrom(src => src.AdrNip))
				.ForMember(dst => dst.AdrNumber, opt => opt.MapFrom(src => src.AdrNrLokalu))
				.ForMember(dst => dst.AdrStreet, opt => opt.MapFrom(src => src.AdrUlica))
				.ForMember(dst => dst.AdrStreetNo, opt => opt.MapFrom(src => src.AdrNrDomu))
				.ForMember(dst => dst.AdrTel, opt => opt.MapFrom(src => src.AdrTelefon))
				.ForMember(dst => dst.AdrZipCode, opt => opt.MapFrom(src => src.AdrKod))
				.ForMember(dst => dst.Active, opt => opt.MapFrom(src => src.KhZablokowany == false))
				.ForMember(dst => dst.ChangeDate, opt => opt.MapFrom(src => src.ZmData))
				.ForMember(dst => dst.Code, opt => opt.MapFrom(src => src.KhSymbol))
				.ForMember(dst => dst.CustEmploee, opt => opt.MapFrom(src => src.KhPracownik))
				.ForMember(dst => dst.DelivCity, opt => opt.MapFrom(src => src.DostMiejscowosc))
				.ForMember(dst => dst.DelivCode, opt => opt.MapFrom(src => src.DostSymbol))
				.ForMember(dst => dst.DelivCountry, opt => opt.MapFrom(src => src.DostPaNazwa))
				.ForMember(dst => dst.DelivCountryCode, opt => opt.MapFrom(src => src.DostPaKod))
				.ForMember(dst => dst.DelivCountryId, opt => opt.MapFrom(src => src.DostPaId))
				.ForMember(dst => dst.DelivName, opt => opt.MapFrom(src => src.DostNazwa))
				.ForMember(dst => dst.DelivNip, opt => opt.MapFrom(src => src.DostNip))
				.ForMember(dst => dst.DelivNumber, opt => opt.MapFrom(src => src.DostNrLokalu))
				.ForMember(dst => dst.DelivStreet, opt => opt.MapFrom(src => src.DostUlica))
				.ForMember(dst => dst.DelivStreetNo, opt => opt.MapFrom(src => src.DostNrDomu))
				.ForMember(dst => dst.DelivTel, opt => opt.MapFrom(src => src.DostTelefon))
				.ForMember(dst => dst.DelivZipCode, opt => opt.MapFrom(src => src.DostKod))
				.ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.KhEmail))
				.ForMember(dst => dst.IdOpiekun, opt => opt.MapFrom(src => src.KhIdOpiekun))
				.ForMember(dst => dst.OneTime, opt => opt.MapFrom(src => src.KhJednorazowy))
				.ForMember(dst => dst.PayDays, opt => opt.MapFrom(src => src.FpTermin))
				.ForMember(dst => dst.PriceId, opt => opt.MapFrom(src => src.KhCena))
				.ForMember(dst => dst.Regon, opt => opt.MapFrom(src => src.KhRegon))
				.ForMember(dst => dst.www, opt => opt.MapFrom(src => src.KhWww))
				.ForMember(dst => dst.DeferredPayment, opt => opt.MapFrom((kontrahent, dst) => (kontrahent.KhPlatOdroczone != null && kontrahent.KhPlatOdroczone == true) && (((kontrahent.KhMaxDokKred ?? 0) > (kontrahent.IloscDokNierozliczonych ?? 0)) || kontrahent.KhMaxDokKred == null || kontrahent.KhMaxDokKred == 0) ? true : false))
        .ForMember(dst => dst.SpecialPayment, opt => opt.MapFrom((kontrahent, dst) => (kontrahent.KhPlatOdroczone != null && kontrahent.KhPlatOdroczone == true) && (((kontrahent.KhMaxDokKred ?? 0) > (kontrahent.IloscDokNierozliczonych ?? 0)) || kontrahent.KhMaxDokKred == null || kontrahent.KhMaxDokKred == 0) ? true : false))
        .ForMember(dst => dst.PaymentDeadline, opt => opt.MapFrom((kontrahent, dst) =>  kontrahent.KhPlatOdroczone != null && kontrahent.KhPlatOdroczone == true ? (kontrahent.FpTermin != null ? (int)kontrahent.FpTermin : -1) : 0))
				;

			CreateMap<AdrEwid, Customer>()
				.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.AdrIdObiektu))
				.ForMember(dst => dst.AdrCity, opt => opt.MapFrom(src => src.AdrMiejscowosc))
				.ForMember(dst => dst.AdrCountry, opt => opt.MapFrom((src, dst) => src.AdrIdPanstwoNavigation?.PaNazwa ?? ""))
				.ForMember(dst => dst.AdrCountryCode, opt => opt.MapFrom((src, dst) => src.AdrIdPanstwoNavigation?.PaKodPanstwaUe ?? ""))
				.ForMember(dst => dst.AdrCountryId, opt => opt.MapFrom(src => src.AdrIdPanstwo))
				.ForMember(dst => dst.AdrName, opt => opt.MapFrom(src => src.AdrNazwa))
				.ForMember(dst => dst.AdrNameFull, opt => opt.MapFrom(src => src.AdrNazwaPelna))
				.ForMember(dst => dst.AdrNip, opt => opt.MapFrom(src => src.AdrNip))
				.ForMember(dst => dst.AdrNumber, opt => opt.MapFrom(src => src.AdrNrLokalu))
				.ForMember(dst => dst.AdrStreet, opt => opt.MapFrom(src => src.AdrUlica))
				.ForMember(dst => dst.AdrStreetNo, opt => opt.MapFrom(src => src.AdrNrDomu))
				.ForMember(dst => dst.AdrTel, opt => opt.MapFrom(src => src.AdrTelefon))
				.ForMember(dst => dst.AdrZipCode, opt => opt.MapFrom(src => src.AdrKod));


      CreateMap<IfxApiFormaPlatnosci, PaymentMethod>()
				.ForMember(dst => dst.Deferred, opt => opt.MapFrom(src => src.PlatnoscOdroczona))
				.ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Nazwa));

			CreateMap<IfxApiSposobDostawy, DeliveryMethod>()
				.ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Nazwa))
				.ForMember(dst => dst.Active, opt => opt.MapFrom(src => src.Aktywny));

			CreateMap<IfVwDokument, Document>()
				.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.DokId))
				.ForMember(dst => dst.IssueDate, opt => opt.MapFrom(src => src.DokDataWyst))
				.ForMember(dst => dst.CashPayment, opt => opt.MapFrom(src => src.DokKwGotowka))
				.ForMember(dst => dst.Comment, opt => opt.MapFrom(src => src.DokUwagi))
				.ForMember(dst => dst.CustomerId, opt => opt.MapFrom(src => src.DokPlatnikId))
				.ForMember(dst => dst.DocumentType, opt => opt.MapFrom(src => src.DokTyp))
				.ForMember(dst => dst.PaymentDueDays, opt => opt.MapFrom((src, dst) => src.DokPlatTermin != null ? (src.DokPlatTermin - src.DokDataWyst).Value.Days : 0))
				.ForMember(dst => dst.PaymentToBeSettled, opt => opt.MapFrom(src => src.NzfWartoscDoZaplaty))
				.ForMember(dst => dst.SecondPaymentAmount, opt => opt.MapFrom((src, dst) => (src.DokKwKarta ?? 0) > 0 ? src.DokKwKarta : src.DokKwKredyt))
				.ForMember(dst => dst.SecondPaymentMethod, opt => opt.MapFrom(src => 0))
				.ForMember(dst => dst.TotalNet, opt => opt.MapFrom(src => src.DokWartNetto))
				.ForMember(dst => dst.TotalGross, opt => opt.MapFrom(src => src.DokWartBrutto))
				.ForMember(dst => dst.UserName, opt => opt.MapFrom(src => src.DokWystawil))
				.ForMember(dst => dst.Number, opt => opt.MapFrom(src => src.DokNrPelny))
				.ForMember(dst => dst.Customer, opt => opt.MapFrom((src, dst) => CreateCustomer(src)));

			CreateMap<DokPozycja, DocumentItem>()
				.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.ObId))
				.ForMember(dst => dst.DiscountRate, opt => opt.MapFrom(src => src.ObRabat))
				.ForMember(dst => dst.PriceGross, opt => opt.MapFrom(src => src.ObCenaBrutto))
				.ForMember(dst => dst.PriceNet, opt => opt.MapFrom(src => src.ObCenaNetto))
				.ForMember(dst => dst.ProductId, opt => opt.MapFrom(src => src.ObTowId))
				.ForMember(dst => dst.Quantity, opt => opt.MapFrom(src => src.ObIloscMag))
				.ForMember(dst => dst.TaxRate, opt => opt.MapFrom(src => src.ObVatProc))
				.ForMember(dst => dst.Product, opt => opt.MapFrom(src => src.ObTow))
				.ForMember(dst => dst.LineNet, opt => opt.MapFrom(src => src.ObWartNetto))
				.ForMember(dst => dst.LineGross, opt => opt.MapFrom(src => src.ObWartBrutto));

			CreateMap<NzFinanse, CashReceipt>()
				.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.NzfId))
				.ForMember(dst => dst.Number, opt => opt.MapFrom(src => src.NzfNumerPelny))
				.ForMember(dst => dst.Amount, opt => opt.MapFrom(src => src.NzfWartosc))
				.ForMember(dst => dst.Date, opt => opt.MapFrom(src => src.NzfData))
				.ForMember(dst => dst.UserId, opt => opt.MapFrom(src => src.NzfIdWystawil))
				.ForMember(dst => dst.UserName, opt => opt.MapFrom(src => src.NzfWystawil))
				.ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.NzfTytulem))
				.ForMember(dst => dst.Customer, opt => opt.MapFrom(src => src.NzfIdAdresuNavigation));
		}

    private Customer CreateCustomer(IfVwDokument src)
    {
			var customer = new Customer
			{
				AdrCity = src.AdrhMiejscowosc,
				AdrCountryId = src.AdrhIdPanstwo,
				AdrName = src.AdrhNazwaPelna,
				AdrNameFull = src.AdrhNazwaPelna,
				AdrNip = src.AdrhNip,
				AdrNumber = src.AdrhNrLokalu,
				AdrStreet = src.AdrhUlica,
				AdrStreetNo = src.AdrhNrDomu,
				AdrTel = src.AdrhTelefon,
				AdrZipCode = src.AdrhKod,
				Code = src.AdrhSymbol,
				Id = src.DokPlatnikId ?? 0
			};
			return customer;
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
