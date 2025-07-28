using Progress.Domain.Navireo;
using Progress.Navireo.Helpers;
using System.Runtime.InteropServices;
using IFoxCommerce.BL.Navireo.Extensions;

namespace Progress.Navireo.Managers
{
  public class CustomerManager
  {
    Database.NavireoDbContext dbContext;

    Logger Logger;

    Navireo.NavireoApplication navireoApplication;

    InsERT.Navireo? NavireoInstance => navireoApplication.GetNavireo();

    public CustomerManager(Database.NavireoDbContext dbContext, Logger logger, Navireo.NavireoApplication navireoApplication)
    {
      this.dbContext = dbContext;
      Logger = logger;
      this.navireoApplication = navireoApplication;
    }

    public List<KeyValuePair<string, int>> UpdateCustomer(int operatorId, Business business)
    {
      if (NavireoInstance == null)
        return null;

      List<KeyValuePair<string, int>> resultList = new List<KeyValuePair<string, int>>();
      InsERT.Kontrahent kontrahentNav = null;
      InsERT.PracownikKh pracownikNav = null;
      InsERT.KhCechy khCechy = null;
      InsERT.KontrahenciManager khManager = null;
      try
      {

        khManager = NavireoInstance.KontrahenciManager;

        var ifx_user = dbContext.IfxApiUzytkowniks.FirstOrDefault(x => x.UzId == operatorId && x.Active);
        if (ifx_user == null) throw new Exception("Brak uprawnień użytkownika");

        if (business.IsNew)
        {
          kontrahentNav = khManager.DodajKontrahenta();
          kontrahentNav.OpiekunId = operatorId;
          if (ifx_user != null && ifx_user.CechaId != null)
          {
            khCechy = kontrahentNav.Cechy;
            khCechy.Dodaj(ifx_user.CechaId);
          }
        }
        if (business.IsUpdated)
        {
          if (ifx_user != null && ifx_user.CechaId != null)
          {
            var cKh = dbContext.KhCechaKhs.FirstOrDefault(x => x.CkIdCecha == ifx_user.CechaId && x.CkIdKhnt == business.Id);
            if (cKh == null) throw new Exception(string.Format("Brak uprawnień użytkownika do kontrahenta"));
          }
          kontrahentNav = khManager.WczytajKontrahenta(business.Id);
          if (!kontrahentNav.MoznaEdytowac) throw new Exception(string.Format("Nie można edytować kontrahenta: {0}", kontrahentNav.NazwaPelna));
        }
        if (business.IsDeleted)
        {
          if (ifx_user != null && ifx_user.CechaId != null)
          {
            var cKh = dbContext.KhCechaKhs.FirstOrDefault(x => x.CkIdCecha == ifx_user.CechaId && x.CkIdKhnt == business.Id);
            if (cKh == null) throw new Exception(string.Format("Brak uprawnień użytkownika do kontrahenta"));
          }
          kontrahentNav = khManager.WczytajKontrahenta(business.Id);
          if (kontrahentNav.MoznaUsunac) kontrahentNav.Usun();
          else
          {
            if (kontrahentNav.MoznaEdytowac)
              kontrahentNav.Aktywny = false;
            else
              throw new Exception(string.Format("Nie można usunąć lub dezaktywować kontrahenta o symbolu: {0}", kontrahentNav.Symbol));
          }
          kontrahentNav.Zapisz();
          kontrahentNav.Zamknij();
        }

        var khSymbole = dbContext.KhKontrahents.Where(x => x.KhSymbol == business.HeadquarterAddress.Code);
        if (khSymbole.Count() > 0 && business.IsNew)
          throw new Exception(string.Format("Istnieje już kontrahent o symbolu: {0}. Nie można dodać kolejnego o tym samym symbolu.", business.HeadquarterAddress.Code));

        if (kontrahentNav == null)
          throw new Exception("Błąd Navireo: nie można dodać/edytować użytkownika");

        if (business.IsNew)
        {
          kontrahentNav.Symbol = business.HeadquarterAddress.Code;
          kontrahentNav.Analityka = "";
        }
        if (business.DeliveryAddresses != null)
        {
          var adresDostawy = business.DeliveryAddresses.FirstOrDefault();
          if (adresDostawy != null)
          {
            if (adresDostawy.IsNew || adresDostawy.IsUpdated)
            {
              if (!string.IsNullOrEmpty(adresDostawy.Name)) kontrahentNav.AdrDostNazwa = adresDostawy.Name.LimitString(50);
              if (!string.IsNullOrEmpty(adresDostawy.ZipCode)) kontrahentNav.AdrDostKodPocztowy = adresDostawy.ZipCode.LimitString(8);
              if (!string.IsNullOrEmpty(adresDostawy.City)) kontrahentNav.AdrDostMiejscowosc = adresDostawy.City.LimitString(40);
              if (!string.IsNullOrEmpty(adresDostawy.Street)) kontrahentNav.AdrDostUlica = adresDostawy.Street.LimitString(60);
              if (!string.IsNullOrEmpty(adresDostawy.Number))
              {
                kontrahentNav.AdrDostNrDomu = adresDostawy.Number.LimitString(10);  //ToDo nr domu i lokalu
                kontrahentNav.AdrDostNrLokalu = "";
              }
              if (adresDostawy.Country != null && !string.IsNullOrEmpty(adresDostawy.Country.Code))
                kontrahentNav.AdrDostPanstwo = Helpers.SQL.GetCountryId(dbContext, adresDostawy.Country.Code);


              kontrahentNav.AdresDostawy = true;
            }
            if (adresDostawy.IsDeleted)
            {
              kontrahentNav.AdresDostawy = false;
            }
          }
        }
        if (!string.IsNullOrEmpty(business.HeadquarterAddress.Telephone) && kontrahentNav.Telefony.Liczba > 0)
        {
          InsERT.KhTelefon tel = null;
          try
          {
            foreach (object oTel in kontrahentNav.Telefony)
            {
              tel = (InsERT.KhTelefon)oTel;
              tel.Usun();
              break;
            }
          }
          finally
          {
            if (tel != null)
              Marshal.ReleaseComObject(tel);
          }
        }
        kontrahentNav.Osoba = false;
        if (business.IsNew && string.IsNullOrEmpty(business.HeadquarterAddress.Name)) throw new Exception("Brak nazwy kontrahenta");
        if (!string.IsNullOrEmpty(business.HeadquarterAddress.Name))
        {
          kontrahentNav.Nazwa = business.HeadquarterAddress.Name.LimitString(50);
          kontrahentNav.NazwaPelna = business.HeadquarterAddress.Name.LimitString(250);
        }
        if (!string.IsNullOrEmpty(business.HeadquarterAddress.VatNumber)) kontrahentNav.NIP = business.HeadquarterAddress.VatNumber.LimitString(20);


        if (!string.IsNullOrEmpty(business.HeadquarterAddress.Street)) kontrahentNav.Ulica = business.HeadquarterAddress.Street.LimitString(60);
        if (!string.IsNullOrEmpty(business.HeadquarterAddress.Number))
        {
          kontrahentNav.NrDomu = business.HeadquarterAddress.Number;
          kontrahentNav.NrLokalu = "";
        }
        if (!string.IsNullOrEmpty(business.HeadquarterAddress.City)) kontrahentNav.Miejscowosc = business.HeadquarterAddress.City;
        if (!string.IsNullOrEmpty(business.HeadquarterAddress.ZipCode)) kontrahentNav.KodPocztowy = business.HeadquarterAddress.ZipCode;
        kontrahentNav.Wojewodztwo = null;
        if (!string.IsNullOrEmpty(business.HeadquarterAddress.Telephone)) kontrahentNav.Telefony.Dodaj(business.HeadquarterAddress.Telephone);
        if (!string.IsNullOrEmpty(business.HeadquarterAddress.Email)) kontrahentNav.Email = business.HeadquarterAddress.Email;
        if (!string.IsNullOrEmpty(business.HeadquarterAddress.Website)) kontrahentNav.WWW = business.HeadquarterAddress.Website;
        if (!string.IsNullOrEmpty(business.HeadquarterAddress.Country.Code)) kontrahentNav.Panstwo = Helpers.SQL.GetCountryId(dbContext, business.HeadquarterAddress.Country.Code);
        kontrahentNav.PowielNIPBezUI = true;

        kontrahentNav.Zapisz();
        business.Id = kontrahentNav.Identyfikator;
        resultList.Add(new KeyValuePair<string, int>(business.GUID, business.Id));
        kontrahentNav.Zamknij();
        Marshal.ReleaseComObject(kontrahentNav);
        kontrahentNav = null;

      }
      finally
      {
        if (khManager != null) Marshal.ReleaseComObject(khManager);
        khManager = null;

        if (kontrahentNav != null) Marshal.ReleaseComObject(kontrahentNav);
        kontrahentNav = null;

        if (pracownikNav != null) Marshal.ReleaseComObject(pracownikNav);
        pracownikNav = null;

        if (khCechy != null) Marshal.ReleaseComObject(khCechy);
        khCechy = null;
      }
      return resultList;
    }
  }
}
