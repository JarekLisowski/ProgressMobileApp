using IFoxCommerce.BL.Navireo.Extensions;
using Progress.Domain.Navireo;
using System.Runtime.InteropServices;

namespace Progress.Navireo.Helpers
{
    public static class NavireoKontrahent
    {
        static internal int DodajKontrahenta(InsERT.Navireo navireoInstance, Database.NavireoDbContext dbContext, Location kontrahent, Location DaneDostawy, int? cechaId = null)
        {
            InsERT.KontrahenciManager khManager = null;
            InsERT.Kontrahent kontrahentNav = null;
            InsERT.PracownikKh pracownikNav = null;
            InsERT.KhCechy khChechy = null;
            try
            {
                {
                    khManager = navireoInstance.KontrahenciManager;
                    kontrahentNav = khManager.DodajKontrahenta();
                    kontrahentNav.OpiekunId = navireoInstance.OperatorId;

                    kontrahentNav.Symbol = kontrahent.Code;

                    if (DaneDostawy != null)
                    {
                        kontrahentNav.AdrDostNazwa = DaneDostawy.Name.LimitString(50);
                        kontrahentNav.AdrDostKodPocztowy = DaneDostawy.ZipCode.LimitString(8);
                        kontrahentNav.AdrDostMiejscowosc = DaneDostawy.City.LimitString(40);
                        kontrahentNav.AdrDostUlica = DaneDostawy.Street.LimitString(60) + " " + DaneDostawy.Number.LimitString(10);
                        kontrahentNav.AdrDostNrDomu = DaneDostawy.Number.LimitString(10);

                        kontrahentNav.AdrDostPanstwo = Helpers.SQL.GetCountryId(dbContext, DaneDostawy.Country.Code);
                        kontrahentNav.AdresDostawy = true;

                    }
                    if (cechaId != null)
                    {
                        khChechy = kontrahentNav.Cechy;
                        khChechy.Dodaj(cechaId);
                    }
                    kontrahentNav.Telefony.Dodaj(kontrahent.Telephone);
                    kontrahentNav.Email = kontrahent.Email;
                    kontrahentNav.Ulica = kontrahent.Street = " " + kontrahent.Number;

                    kontrahentNav.Miejscowosc = kontrahent.City;
                    kontrahentNav.KodPocztowy = kontrahent.ZipCode;
                    kontrahentNav.Wojewodztwo = null;
                    kontrahentNav.Panstwo = Helpers.SQL.GetCountryId(dbContext, kontrahent.Country.Code);


                    kontrahentNav.Osoba = false;
                    kontrahentNav.Nazwa = kontrahent.Name.LimitString(50);
                    kontrahentNav.NazwaPelna = kontrahent.Name.LimitString(250);
                    kontrahentNav.NIP = kontrahent.VatNumber.LimitString(20);

                    kontrahentNav.PowielNIPBezUI = true;

                    kontrahentNav.Zapisz();
                    kontrahent.Id = kontrahentNav.Identyfikator;
                    kontrahentNav.Zamknij();
                    Marshal.ReleaseComObject(kontrahentNav);
                    kontrahentNav = null;

                    //if (Pracownik != null)
                    //{
                    //    kontrahentNav = khManager.WczytajKontrahenta(kontrahent.Id);

                    //    pracownikNav = kontrahentNav.PracownicyCRM.Dodaj("");

                    //    pracownikNav.Imie = Pracownik.Name;
                    //    pracownikNav.Nazwisko = Pracownik.Surname;
                    //    if (!string.IsNullOrWhiteSpace(Pracownik.Telephone))
                    //        pracownikNav.Telefony.Dodaj(Pracownik.Telephone);
                    //    if (!string.IsNullOrWhiteSpace(Pracownik.Email))
                    //        pracownikNav.Email = Pracownik.Email;
                    //    pracownikNav.Zapisz();
                    //    pracownikNav.Zamknij();
                    //    Marshal.ReleaseComObject(pracownikNav);
                    //    pracownikNav = null;
                    //}
                    kontrahentNav.Zamknij();
                }
            }
            finally
            {
                if (khManager != null)
                {
                    Marshal.ReleaseComObject(khManager);
                    khManager = null;
                }
                if (kontrahentNav != null)
                {
                    Marshal.ReleaseComObject(kontrahentNav);
                    kontrahentNav = null;
                }
                if (pracownikNav != null)
                {
                    Marshal.ReleaseComObject(pracownikNav);
                    pracownikNav = null;
                }
                if (khChechy != null)
                {
                    Marshal.ReleaseComObject(khChechy);
                    khChechy = null;
                }
            }
            //if (jednorazowy)
            //    Helpers.SQL.UstawJakoJednorazowy(kontrahent.Id, navireoInstance.Baza.PolaczenieAdoNet);
            return kontrahent.Id;
        }

        static internal int PobierzIdPoSymbolu(InsERT.Navireo navireoInstance, string Symbol)
        {
            InsERT.KontrahenciManager khManager = null;
            InsERT.Kontrahent kontrahentNav = null;
            int identyfikator;

            try
            {
                khManager = navireoInstance.KontrahenciManager;
                bool istnieje = khManager.Istnieje(Symbol);
                if (istnieje)
                {
                    kontrahentNav = khManager.WczytajKontrahenta(Symbol);
                    identyfikator = kontrahentNav.Identyfikator;
                }
                else
                    return 0;
            }
            finally
            {
                if (khManager != null)
                {
                    Marshal.ReleaseComObject(khManager);
                    khManager = null;
                }
                if (kontrahentNav != null)
                {
                    Marshal.ReleaseComObject(kontrahentNav);
                    kontrahentNav = null;
                }
            }
            return identyfikator;
        }

        static internal bool SprawdzAktywnoscKontrahenta(int khId, InsERT.Navireo navireoInstance)
        {
            InsERT.KontrahenciManager khManager = null;
            InsERT.Kontrahent kontrahentNav = null;
            try
            {
                khManager = navireoInstance.KontrahenciManager;
                kontrahentNav = khManager.Wczytaj(khId);
                return kontrahentNav.Aktywny;
            }
            finally
            {
                if (khManager != null)
                {
                    Marshal.ReleaseComObject(khManager);
                    khManager = null;
                }
                if (kontrahentNav != null)
                {
                    Marshal.ReleaseComObject(kontrahentNav);
                    kontrahentNav = null;
                }
            }
        }

        //static internal Location GetKontrahentAdrH(int? adrhId, Model.NavireoEntities dataContext)
        //{
        //    if (adrhId == null) return null;
        //    Location result = new Location();
        //    var adresh = dataContext.adr_Historia.FirstOrDefault(x => x.adrh_Id == adrhId);
        //    if (adresh != null)
        //    {
        //        result.Id = adresh.adr__Ewid.adr_IdObiektu;
        //        result.Name = !string.IsNullOrEmpty(adresh.adrh_NazwaPelna) ? adresh.adrh_NazwaPelna : adresh.adrh_Nazwa;
        //        result.Code = adresh.adrh_Symbol;
        //        result.VatNumber = adresh.adrh_NIP;
        //        result.ZipCode = adresh.adrh_Kod;
        //        result.City = adresh.adrh_Miejscowosc;
        //        result.Street = adresh.adrh_Ulica + " " + adresh.adrh_NrDomu;
        //        if (!string.IsNullOrEmpty(adresh.adrh_NrLokalu))
        //            result.Street += "/" + adresh.adrh_NrLokalu;
        //        result.Telephone = adresh.adrh_Telefon;
        //        result.Country = new Country
        //        {
        //            Id = adresh.adrh_IdPanstwo ?? 0
        //        };
        //        return result;
        //    }
        //    else return null;
        //}

        //static internal Location GetKontrahentAdr(int adrhId, Model.NavireoEntities dataContext)
        //{
        //    Location result = new Location();
        //    var adresh = dataContext.adr__Ewid.FirstOrDefault(x => x.adr_IdObiektu == adrhId && x.adr_TypAdresu == 1);
        //    if (adresh != null)
        //    {
        //        result.Id = adresh.adr_IdObiektu;
        //        result.Name = adresh.adr_NazwaPelna;
        //        result.Code = adresh.adr_Symbol;
        //        result.VatNumber = adresh.adr_NIP;
        //        result.ZipCode = adresh.adr_Kod;
        //        result.City = adresh.adr_Miejscowosc;
        //        result.Street = adresh.adr_Ulica + " " + adresh.adr_NrDomu;
        //        if (!string.IsNullOrEmpty(adresh.adr_NrLokalu))
        //            result.Street += "/" + adresh.adr_NrLokalu;
        //        result.Telephone = adresh.adr_Telefon;
        //        result.Country = new Country
        //        {
        //            Id = adresh.adr_IdPanstwo ?? 0
        //        };
        //        return result;
        //    }
        //    else return null;
        //}

        ///// <summary>
        ///// Zwraca adres kontrahenta z dokumentu
        ///// </summary>
        ///// <param name="suDokument">Dokument</param>
        ///// <returns>Adres kontrahenta w chwili wystawiania dokumentu</returns>
        //static internal Location GetKontrahentAdrH(Model.IF_vwDokument dok)
        //{
        //    if (dok.dok_PlatnikId == null)
        //        return new Location(); //dla paragonów nieimiennych
        //    Location result = new Location();
        //    result.Name = dok.adrh_NazwaPelna;
        //    result.Code = dok.adrh_Symbol;
        //    result.VatNumber = dok.adrh_NIP;
        //    result.Street = dok.adrh_Ulica + " " + dok.adrh_NrDomu;
        //    if (!string.IsNullOrEmpty(dok.adrh_NrLokalu))
        //        result.Street += "/" + dok.adrh_NrLokalu;
        //    result.ZipCode = dok.adrh_Kod;
        //    result.City = dok.adrh_Miejscowosc;
        //    result.Telephone = dok.adrh_Telefon;
        //    result.Country = dok.adrh_IdPanstwo != null ? new Country
        //    {
        //        Id = dok.adrh_IdPanstwo ?? 0
        //    } : null;
        //    return result;
        //}

        //static internal Location Sprzedawca(Model.NavireoEntities dbConnection)
        //{
        //    var adrEwid = (from adr in dbConnection.adr__Ewid
        //                   where adr.adr_TypAdresu == 8
        //                   select adr).FirstOrDefault();
        //    if (adrEwid != null)
        //    {
        //        var rachunek = dbConnection.rb__RachBankowy.FirstOrDefault(it => it.rb_IdObiektu == adrEwid.adr_IdObiektu && it.rb_TypObiektu == 0 && it.rb_Podstawowy);
        //        string rachunekNumer = rachunek != null ? rachunek.rb_Numer : "";

        //        Location adres = new Location
        //        {
        //            Id = adrEwid.adr_IdObiektu,
        //            Name = !string.IsNullOrEmpty(adrEwid.adr_NazwaPelna) ? adrEwid.adr_NazwaPelna : adrEwid.adr_Nazwa,
        //            VatNumber = adrEwid.adr_NIP,
        //            Code = adrEwid.adr_Symbol,
        //            ZipCode = adrEwid.adr_Kod,
        //            City = adrEwid.adr_Miejscowosc,
        //            Street = adrEwid.adr_Ulica + " " + adrEwid.adr_NrDomu,
        //            Telephone = adrEwid.adr_Telefon,
        //            BankAccountNumber = rachunekNumer
        //        };
        //        if (!string.IsNullOrEmpty(adrEwid.adr_NrLokalu))
        //            adres.Street += "/" + adrEwid.adr_NrLokalu;
        //        if (adrEwid.sl_Panstwo != null)
        //        {
        //            adres.Country = new Country
        //            {
        //                Id = adrEwid.sl_Panstwo.pa_Id,
        //                Name = adrEwid.sl_Panstwo.pa_Nazwa,
        //                Code = adrEwid.sl_Panstwo.pa_KodPanstwaUE
        //            };
        //        }
        //        return adres;
        //    }
        //    else
        //        throw new Exception("Brak sprzedawcy w bazie danych");
        //}

    }
}
