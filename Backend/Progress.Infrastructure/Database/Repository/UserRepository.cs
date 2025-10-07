using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Progress.Database;
using Progress.Domain.Interfaces;
using Progress.Domain.Model;

namespace Progress.Infrastructure.Database.Repository
{
  public class UserRepository : DatabaseRepository<User, IfxApiUzytkownik>, IUserRepository
  {
    public UserRepository(
      NavireoDbContext dbContext,
      IConfigurationProvider automapperConfiguration)
      : base(dbContext, automapperConfiguration, nameof(IfxApiUzytkownik.UzId), x => x.UzId, x => x.Id)
    {
    }

    public User? GetUser(int userId)
    {
      var items = from apiUzytkownik in DbContext.IfxApiUzytkowniks.AsNoTracking().Include(it => it.IfxApiUzytkownikPoziomyCenowes)
                  join pd in DbContext.PdUzytkowniks.AsNoTracking() on apiUzytkownik.UzId equals pd.UzId
                  where pd.UzId == userId
                  select new { apiUzytkownik, pd };

      var apiUzytkData = items.FirstOrDefault();
      if (apiUzytkData != null && apiUzytkData.apiUzytkownik != null)
      {
        var apiUzytkownik = apiUzytkData.apiUzytkownik;
        User user = new User();
        user.Id = apiUzytkData.pd.UzId;
        user.Name = apiUzytkData.pd.UzImie;
        user.Surname = apiUzytkData.pd.UzNazwisko;
        user.Login = user.Name + " " + user.Surname;
        user.Code = apiUzytkData.pd.UzIdentyfikator;
        user.Mobile = apiUzytkownik.Active;
        user.SpecialPayment = apiUzytkownik.PlatnosciOdroczone;
        user.SpecialPaymentExtendDeadline = apiUzytkownik.PlatnosciOdroczoneWydluzenieTerminu;
        user.DeviceId = apiUzytkownik.DeviceId;
        user.StoreName = apiUzytkData.pd.UzIdMagazynuNavigation?.MagNazwa;
        user.Kasa = apiUzytkData.pd.UzIdKasyNavigation?.KsNazwa;
        user.KasaId = apiUzytkData.pd.UzIdKasy;
        user.MaxSpecialPayment = apiUzytkownik.MaxPlatnoscOdroczona.GetValueOrDefault();
        user.MinSpecialPayment = apiUzytkownik.MaxPlatnoscOdroczona.GetValueOrDefault();

        var prices = new List<Price>();
        var parametr = DbContext.TwParametrs.FirstOrDefault();
        if (parametr != null)
        {
          if (!string.IsNullOrEmpty(parametr.TwpNazwaCeny1)) prices.Add(new Price { Id = 1, Name = parametr.TwpNazwaCeny1, Curency = parametr.TwpWalutaCeny1 });
          if (!string.IsNullOrEmpty(parametr.TwpNazwaCeny2)) prices.Add(new Price { Id = 2, Name = parametr.TwpNazwaCeny2, Curency = parametr.TwpWalutaCeny2 });
          if (!string.IsNullOrEmpty(parametr.TwpNazwaCeny3)) prices.Add(new Price { Id = 3, Name = parametr.TwpNazwaCeny3, Curency = parametr.TwpWalutaCeny3 });
          if (!string.IsNullOrEmpty(parametr.TwpNazwaCeny4)) prices.Add(new Price { Id = 4, Name = parametr.TwpNazwaCeny4, Curency = parametr.TwpWalutaCeny4 });
          if (!string.IsNullOrEmpty(parametr.TwpNazwaCeny5)) prices.Add(new Price { Id = 5, Name = parametr.TwpNazwaCeny5, Curency = parametr.TwpWalutaCeny5 });
          if (!string.IsNullOrEmpty(parametr.TwpNazwaCeny6)) prices.Add(new Price { Id = 6, Name = parametr.TwpNazwaCeny6, Curency = parametr.TwpWalutaCeny6 });
          if (!string.IsNullOrEmpty(parametr.TwpNazwaCeny7)) prices.Add(new Price { Id = 7, Name = parametr.TwpNazwaCeny7, Curency = parametr.TwpWalutaCeny7 });
          if (!string.IsNullOrEmpty(parametr.TwpNazwaCeny8)) prices.Add(new Price { Id = 8, Name = parametr.TwpNazwaCeny8, Curency = parametr.TwpWalutaCeny8 });
          if (!string.IsNullOrEmpty(parametr.TwpNazwaCeny9)) prices.Add(new Price { Id = 9, Name = parametr.TwpNazwaCeny9, Curency = parametr.TwpWalutaCeny9 });
          if (!string.IsNullOrEmpty(parametr.TwpNazwaCeny10)) prices.Add(new Price { Id = 10, Name = parametr.TwpNazwaCeny10, Curency = parametr.TwpWalutaCeny10 });
        }

        var cecha = DbContext.SlCechaKhs.AsNoTracking().FirstOrDefault(x => x.CkhId == apiUzytkownik.CechaId);
        user.CechaId = cecha?.CkhId ?? 0;
        user.CechaNazwa = cecha?.CkhNazwa ?? "Wszyscy kontrahenci";
        user.PriceLevelList = (from ifx in apiUzytkownik.IfxApiUzytkownikPoziomyCenowes
                               join pr in prices on ifx.CenaId equals pr.Id
                               select new PriceLevel { Id = ifx.CenaId, Name = pr.Name, Primary = ifx.Primary }).ToList();
        user.DefaultPrice = user.PriceLevelList.FirstOrDefault(it => it.Primary)?.Id ?? 1;
        user.PromocjaGrupaId = apiUzytkownik.PromocjaGrupaId.GetValueOrDefault();
        user.CanExtendPaymentDeadline = apiUzytkownik.PlatnosciOdroczoneWydluzenieTerminu;
        user.SpecialPayment = apiUzytkownik.PlatnosciOdroczone;
        user.MaxSpecialPayment = apiUzytkownik.MaxPlatnoscOdroczona.GetValueOrDefault();
        user.MinSpecialPayment = apiUzytkownik.MaxPlatnoscOdroczona.GetValueOrDefault();
        user.DiscountAllowed = apiUzytkownik.Rabat.GetValueOrDefault() > 0;
        user.DiscountMax = apiUzytkownik.Rabat ?? 0;
        return user;
      }
      return null;
    }

    public User? GetByUsername(string username)
    {
      var dbUser = DbContext.PdUzytkowniks.AsNoTracking().FirstOrDefault(u => (u.UzNazwisko + " " + u.UzImie) == username);
      if (dbUser == null)
        return null;

      var user = GetUser(dbUser.UzId);
      if (user != null)
      {
        user.Password = dbUser.UzHaslo;
        return user;
      }

      return null;
    }

  }
}