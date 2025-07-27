using Progress.Domain.Navireo;
using Progress.Navireo.Exceptions;
using Progress.Navireo.Extensions;
using Progress.Navireo.Helpers;
using System.Runtime.InteropServices;

namespace Progress.Navireo.Managers
{
  public class DocumentManager
  {
    Database.NavireoDbContext dbContext;

    Logger Logger;

    Navireo.NavireoApplication navireoApplication;

    InsERT.Navireo? NavireoInstance => navireoApplication.GetNavireo();

    public DocumentManager(Database.NavireoDbContext dbContext, Logger logger, Navireo.NavireoApplication navireoApplication)
    {
      this.dbContext = dbContext;
      Logger = logger;
      this.navireoApplication = navireoApplication;
    }


    /// <summary>
    /// Dodaje, aktualizuje, usuwa dokumenty
    /// </summary>
    /// <param name="document">Dokument do aktualizacji, dodania, usunięcia</param>
    /// <param name="operatorId">Id użytkownika</param>
    /// <returns>Lista par Id-GUID dodanych/zaktualizowanych obiektów</returns>
    public IEnumerable<KeyValuePair<string, int>> UpdateDocument(CommerceDocumentBase document)
    {
      var result = new List<KeyValuePair<string, int>>();
      if (NavireoInstance == null)
      {
        return result;
      }
      InsERT.SuDokumentyManager dokManager = null;
      InsERT.SuDokument suDokument = null;
      InsERT.SuPozycje pozManager = null;

      int operatorId = document.UserId;

      var ifx_user = GetIFxUser(operatorId, dbContext);
      var pd_User = dbContext.PdUzytkowniks.FirstOrDefault(x => x.UzId == operatorId);
      if (pd_User == null)
        throw new Exception("Brak użytkownika");

      try
      {
        Logger.Log(LogType.DocumentUpdate, string.Format("Numer dokumentu: {0}", document.FullNumber), operatorId);
        //document.FullNumber = MakeNumber(document.FullNumber);
        //document.SettlementFullNumber = MakeNumber(document.SettlementFullNumber);
        //Logger.Log(LogType.DocumentUpdate, string.Format("Nowy numer dokumentu: {0}", document.FullNumber), operatorId);
        SetMagazynId(NavireoInstance, (int)pd_User.UzIdMagazynu, operatorId);

        dokManager = NavireoInstance.SuDokumentyManager;

        if (document.IsNew)
        {
          //if (CheckSkipDocumentNumber(document, dbContext))
          //{
          //  Logger.Log(LogType.DocumentUpdate, string.Format("Pominięto zapis dokumentu: {0}", document.FullNumber), operatorId);
          //  result.Add(new KeyValuePair<string, int>(document.GUID, 0));
          //  return result;
          //}
          //int existingId = CheckDocumentExists(document, dbContext);
          //if (existingId > 0)
          //{
          //  Logger.Log(LogType.DocumentUpdate, string.Format("Dokument jest już w Navireo: {0}. Dokument nie został dodany.", document.FullNumber), operatorId);
          //  result.Add(new KeyValuePair<string, int>(document.GUID, existingId));
          //  return result;
          //}
          //CheckAndChangeDocumentNumbers(document, operatorId, dbContext);
          suDokument = CreateDocument(operatorId, document, dokManager);
          SetKasaId(suDokument, (int)pd_User.UzIdKasy);

          //if (document.SettlementNumber != 0 && KPNumerCzyIstnieje(document.SettlementFullNumber, document.SettlementNumber, (int)pd_User.UzIdKasy, null, document.IssueDate.Year, dbContext))
          //  throw new DocumentUpdateException(DocumentUpdateResult.NumberError, string.Format("Nie można zapisać dokumentu ponieważ powtórzony został numer KP: {0}", document.SettlementFullNumber));

        }
        else if (document.IsUpdated)
        {
          var dokDB = dbContext.DokDokuments.FirstOrDefault(x => x.DokId == document.Id);
          if (dokDB == null)
            throw new Exception(string.Format("Brak dokumentu o Id: {0}", document.Id));
          suDokument = WczytajDokument(operatorId, document, dokManager, suDokument);

          //if (document.SettlementNumber != 0 && KPNumerCzyIstnieje(document.SettlementFullNumber, document.SettlementNumber, (int)pd_User.uz_IdKasy, suDokument.Identyfikator, document.IssueDate.Year, dbContext))
          //  throw new DocumentUpdateException(DocumentUpdateResult.NumberError, string.Format("Nie można zapisać dokumentu ponieważ powtórzony został numer KP: {0}", document.SettlementFullNumber));

          int kh_Id = suDokument.KontrahentId;
          if (ifx_user.CechaId != null)
          {
            var kh = dbContext.KhCechaKhs.FirstOrDefault(x => x.CkIdCecha == ifx_user.CechaId && x.CkIdKhnt == kh_Id);
            if (kh == null)
              throw new UserException(string.Format("Brak uprawnień użytkownika do odczytu dokumentu: '{0}' kontrahenta o Id: {1}", dokDB.DokNrPelny, kh_Id));
          }
          result.Add(new KeyValuePair<string, int>(document.GUID, document.Id));
        }

        else if (document.IsDeleted)
        {
          DeleteDocument(document.Id, dokManager);
          return result;
        }
        else
          return result;

        if (document.IsNew)
        {
          //SetDocumentNumber(document, suDokument);
          SetRelatedDocument(document, suDokument);
          if (document.BuyerAddress == null && document.DocumentType != DocumentEnum.Receipt)
            throw new Exception("Brak ustawionego kontrahenta");

          UstawKontrahenta(suDokument, document, NavireoInstance);

        }

        suDokument.DataWystawienia = document.IssueDate;
        try
        {
          suDokument.DataMagazynowa = document.IssueDate;
        }
        catch (Exception)
        { }

        if (!string.IsNullOrEmpty(document.Comment))
          suDokument.Uwagi = document.Comment;

        if (document.PaidCashGross != 0)
          suDokument.PlatnoscGotowkaKwota = document.PaidCashGross;

        //var deliveryManager = new DeliveryManager();
        var deliveryList = GetDeliveryTypeList();

        pozManager = suDokument.Pozycje;
        if (document.DocumentItems != null)
        {
          var itemsResult = UpdateDocumentItems(document.DocumentItems.Where(x => !deliveryList.Any(d => d.Id == x.Product.Id)), suDokument, pozManager);  //oprócz dostaw
          result.AddRange(itemsResult);
        }

        if (ChceckObjectChanged(document.Delivery))
          UpdateDocumentDelivery(document, suDokument, pozManager, deliveryList);

        int khId = suDokument.KontrahentId;

        if (document.Payment != null)
          SetPayment(document, suDokument);

        string uwagi = suDokument.Uwagi ?? "";

        //if (document.ToPerson != null)
        //  suDokument.Odebral = document.ToPerson.Name;
        //          suDokument.Wystawil = pd_User.uz_Imie + " " + pd_User.uz_Nazwisko;

        try
        {
          suDokument.ZapiszSymulacja();
        }
        catch (Exception ex)
        {
          Logger.Log(LogType.DocumentUpdate, string.Format("Nie można zapisać dokumentu: {0}", document.FullNumber), operatorId);
          foreach (InsERT.SuBrak brak in suDokument.PozycjeBrakujace)
          {
            string s = string.Format("Brak towaru: {0}, ilość na dokumencie: {1}, dostępne na magazynie: {2}, brakuje: {3}", brak.TowarSymbol, brak.IloscJm, brak.MagazynStan, brak.Brak);
            Logger.Log(LogType.DocumentUpdate, s, operatorId);
          }
          Logger.Log(LogType.DocumentUpdate, "Zapis bez skutku magazynowego.", operatorId);
          suDokument.StatusDokumentu = InsERT.SubiektDokumentStatusEnum.gtaSubiektDokumentStatusOdlozony;
        }
        suDokument.Zapisz();

        document.Id = suDokument.Identyfikator;
        document.Number = suDokument.Numer;
        document.FullNumber = suDokument.NumerPelny;

        //if (document.DocumentType == DocumentEnum.SalesInvoice || document.DocumentType == DocumentEnum.Receipt)
        //  SetDocumentLoyaltyPoints(suDokument, dbContext);

        suDokument.Zamknij();
        result.Add(new KeyValuePair<string, int>(document.GUID, document.Id));

        if (document.IsDeleted == false)
          SetPersonelId(document.Id, operatorId);

        //if (document.SettlementNumber != 0)
        //  SetKPNumer(document, (int)pd_User.uz_IdKasy, document.IssueDate.Year, dbContext);

        if (document.IsNew)
        {
          SetIFx_ApiDokumentyZapisane(document);
          //if (document.DocumentType != DocumentEnum.StoreOrder)
          //  SetRouteStopDocument(document.FullNumber, operatorId, khId);
        }

        //if (document.IsNew)
        //  SaveMobileGuid(document, dbContext);

        return result;
      }

      finally
      {
        if (dokManager != null)
          Marshal.ReleaseComObject(dokManager);
        dokManager = null;

        if (suDokument != null)
          Marshal.ReleaseComObject(suDokument);
        suDokument = null;

        if (pozManager != null)
          Marshal.ReleaseComObject(pozManager);
        pozManager = null;
      }

    }

    //private int CheckDocumentExists(CommerceDocumentBase document, NavireoEntities dbContext)
    //{
    //  var doc = dbContext.IFx_Dokument.FirstOrDefault(it => it.idok_MobileGUID == document.GUID);
    //  if (doc != null)
    //    return doc.idok_Id;
    //  return 0;
    //}

    /// <summary>
    /// Zapisuje w bazie informację że dodano dokument przez API
    /// </summary>
    /// <param name="document">Dodany dokument</param>
    /// <param name="dbContext">Połączenie z DB</param>
    private void SetIFx_ApiDokumentyZapisane(CommerceDocumentBase document)
    {
      var dok = dbContext.IfxApiDokumentyZapisanes.FirstOrDefault(x => x.Id == document.Id);
      if (dok == null)
      {
        dok = new Database.IfxApiDokumentyZapisane
        {
          Id = document.Id,
          Numer = document.FullNumber,
          Data = DateTime.Now
        };
        dbContext.IfxApiDokumentyZapisanes.Add(dok);
      }
      dbContext.SaveChanges();
    }

    /// <summary>
    /// Dodaje, aktualizuje,usuwa pozycje dokumentu 
    /// </summary>
    /// <param name="documentItems">Pozycje dokumentu</param>
    /// <param name="suDokument">InsERT.SuDokument</param>
    /// <param name="pozManager">Pozycje Manager</param>
    /// <param name="dbContext">DbContext</param>
    /// <returns>Lista par Id-GUID pozycji</returns>
    private IEnumerable<KeyValuePair<string, int>> UpdateDocumentItems(IEnumerable<DocumentItem> documentItems, InsERT.SuDokument suDokument, InsERT.SuPozycje pozManager)
    {
      var result = new List<KeyValuePair<string, int>>();
      if (documentItems == null || !documentItems.Any()) return result;

      foreach (var item in documentItems.Where(x => x.IsNew)) //dodaje nowe pozycje
      {
        InsERT.SuPozycja suPozycja = null;
        try
        {
          if (item.Product != null)
          {
            if (!item.IsNew && !item.IsUpdated && !item.IsDeleted) continue;

            var twTowar = dbContext.TwTowars.FirstOrDefault(x => x.TwId == item.Product.Id);
            if (twTowar != null && item.Product.Id != 0)
            {
              if (twTowar.TwZablokowany)
                throw new DocumentUpdateException(DocumentUpdateResult.ProductError, string.Format("Towar symbolu: '{0}' jest zablokowany", twTowar.TwSymbol));
            }
            else
            {
              twTowar = dbContext.TwTowars.FirstOrDefault(x => x.TwSymbol == item.Product.Code);
              if (twTowar == null)
                throw new DocumentUpdateException(DocumentUpdateResult.ProductError, string.Format("Brak towaru o symbolu: {0}", twTowar.TwSymbol));

              if (twTowar.TwZablokowany)
                throw new DocumentUpdateException(DocumentUpdateResult.ProductError, string.Format("Towar symbolu: '{0}' jest zablokowany", twTowar.TwSymbol));
            }

            if (item.IsNew)
            {
              suPozycja = pozManager.Dodaj(twTowar.TwId);
              result.Add(new KeyValuePair<string, int>(item.GUID, suPozycja.Id));
            }

            if (suDokument.LiczonyOdCenNetto == true)
              suPozycja.CenaNettoPrzedRabatem = item.PriceNet;
            else
            {
              suPozycja.CenaBruttoPrzedRabatem = item.PriceGross;
            }
            if (item.DiscountRate < 100)
              suPozycja.RabatProcent = item.DiscountRate;
            else
            {
              suPozycja.RabatProcent = 0;
              suPozycja.CenaNettoPrzedRabatem = 0;
            }
            if (item.Tax != null && item.Tax.Id > 0)
            {
              suPozycja.VatId = item.Tax.Id;
            }
            suPozycja.IloscJm = item.Amount;
            item.Id = suPozycja.Id;
            SetMappingDiscountItemId(item);
            SetDiscountItemCategory(item, suPozycja);
          }
        }
        finally
        {
          if (suPozycja != null) Marshal.ReleaseComObject(suPozycja);
          suPozycja = null;
        }
      }

      foreach (var oPoz in suDokument.Pozycje) //aktualizacja i usuwanie pozycji
      {
        InsERT.SuPozycja suPozycja = (InsERT.SuPozycja)oPoz;
        try
        {
          int twid = suPozycja.TowarId;
          string symbol = suPozycja.TowarSymbol;
          var item = documentItems.FirstOrDefault(x => x.Product.Id == twid || x.Product.Code == symbol);
          if (item != null)
          {
            if (item.IsDeleted)
            {
              SetMappingDiscountItemId(item);
              suPozycja.Usun();
              continue;
            }

            if (item.IsUpdated)
            {
              if (suDokument.LiczonyOdCenNetto == true)
                suPozycja.CenaNettoPrzedRabatem = item.PriceNet;
              else
                suPozycja.CenaBruttoPrzedRabatem = item.PriceGross;

              suPozycja.IloscJm = item.Amount;
              SetMappingDiscountItemId(item);
              continue;
            }
          }
        }
        finally
        {
          if (suPozycja != null) Marshal.ReleaseComObject(suPozycja);
          suPozycja = null;
        }
      }
      return result;
    }

    /// <summary>
    /// Ustawia kategorię promocyjną dla towarów z zestawów promocyjnych
    /// </summary>
    /// <param name="item"></param>
    /// <param name="suPozycja"></param>
    /// <param name="dbContext"></param>
    private void SetDiscountItemCategory(DocumentItem item, InsERT.SuPozycja suPozycja)
    {
      if (item.DiscountItemId != 0 || item.DiscountFamilyId != 0)
      {
        var kategoriaId = Helpers.Parameters.GetParameterInt("PromocjaKategoriaId", dbContext);
        suPozycja.KategoriaId = kategoriaId;
      }
    }

    /// <summary>
    /// Ustawia mapowanie Id pozycji promocji z pozycją dokumentu
    /// </summary>
    /// <param name="item"></param>
    /// <param name="dbContext"></param>
    private void SetMappingDiscountItemId(DocumentItem item)
    {
      var mappedItem = dbContext.IfxApiDokPozycjaPromocjas.FirstOrDefault(x => x.ObId == item.Id);
      if (item.IsNew || item.IsUpdated)
      {
        if (mappedItem == null)
        {
          mappedItem = new Database.IfxApiDokPozycjaPromocja
          {
            ObId = item.Id
          };
          dbContext.IfxApiDokPozycjaPromocjas.Add(mappedItem);
        }
        mappedItem.PromocjaPozycjaId = item.DiscountItemId;
        mappedItem.PozycjaUrzadzenieId = item.DiscountFamilyId;
      }
      if (item.IsDeleted)
      {
        if (mappedItem != null)
          dbContext.IfxApiDokPozycjaPromocjas.Remove(mappedItem);
      }
      dbContext.SaveChanges();
    }

    ///// <summary>
    ///// Wypełnia zmapowane Id pozycji zestawu promocyjnego
    ///// </summary>
    ///// <param name="item">Pozycja dokumentu</param>
    ///// <param name="dbContext">Połączenie z BD</param>
    ///// <returns>Pozycja dokumentu z wypełnionymi DiscountItemId i DiscountItemMobileId</returns>
    //private static DocumentItem GetMappingDiscountItemId(DocumentItem item, Model.NavireoEntities dbContext)
    //{
    //  var mappedItem = dbContext.IFx_ApiDokPozycjaPromocja.FirstOrDefault(x => x.ob_Id == item.Id);
    //  if (mappedItem != null)
    //  {
    //    item.DiscountItemId = mappedItem.PromocjaPozycjaId;
    //    item.DiscountFamilyId = mappedItem.PozycjaUrzadzenieId;
    //  }
    //  return item;
    //}

    /// <summary>
    /// Sprawdza czy istnieje już dokument KP o podanym numerze, w kasie w danym roku
    /// </summary>
    /// <param name="numerPelny">Pełby bumer KP</param>
    /// <param name="numer">Numer KP do sprawdzenia</param>
    /// <param name="kasaId">Id kasy</param>
    /// <param name="documentId">Id dokumentu</param>
    /// <param name="rok">Rok</param>
    /// <param name="dbContext">DbContext</param>
    /// <returns></returns>
    //private bool KPNumerCzyIstnieje(string numerPelny, int numer, int kasaId, int? documentId, int rok, Model.NavireoEntities dbContext)
    //{
    //  if (numerPelny == null)
    //    return false;
    //  numerPelny = numerPelny.Trim();
    //  //var exist = dbContext.nz__Finanse.Where(x => x.nzf_Typ == 17 && x.nzf_IdKasy == kasaId && x.nzf_Numer == numer && x.nzf_Data.Year == rok);
    //  var exists = dbContext.nz__Finanse.Any(x => x.nzf_Typ == 17 && x.nzf_IdKasy == kasaId && x.nzf_NumerPelny == numerPelny && x.nzf_IdDokumentAuto != documentId);

    //  return exists;

    //  //if (documentId != null)
    //  //    exist = exist.Where(x => x.nzf_IdDokumentAuto != documentId);

    //  //return exist.FirstOrDefault() != null;
    //}



    /// <summary>
    /// Ustawia numer KP 
    /// </summary>
    /// <param name="document">Dokument</param>
    /// <param name="kasaId">Id kasy</param>
    /// <param name="dbContext">Połączenie z bazą danych</param>
    private void SetKPNumer(CommerceDocumentBase document, int kasaId, int rok)
    {
      try
      {
        //if (KPNumerCzyIstnieje(document.SettlementFullNumber, document.SettlementNumber, kasaId, document.Id, rok, dbContext))
        //  throw new Exception(string.Format("KP o numerze: {0} jest już przypisane dla innego dokumentu.", document.SettlementFullNumber));

        var nzf = dbContext.NzFinanses.FirstOrDefault(x => x.NzfTyp == 17 && x.NzfIdKasy == kasaId && x.NzfIdDokumentAuto == document.Id);
        if (nzf != null)
        {
          nzf.NzfNumer = document.SettlementNumber;
          nzf.NzfNumerPelny = document.SettlementFullNumber;
          dbContext.SaveChanges();
        }
      }
      catch (Exception e)
      {
        Logger.Log(LogType.Exception, string.Format("Nie udało się ustawić numeru KP :{0} dla dokumentu:{0}", document.SettlementFullNumber, document.FullNumber), null);
        Logger.Log(LogType.Exception, e);
        throw;
      }
    }

    /// <summary>
    /// Ustawia Id wystawiającego dokument
    /// </summary>
    /// <param name="documentId"></param>
    /// <param name="operatorId"></param>
    private void SetPersonelId(int documentId, int operatorId)
    {
      {
        var document = dbContext.DokDokuments.FirstOrDefault(x => x.DokId == documentId);
        if (document != null)
          document.DokPersonelId = operatorId;
        else
          Logger.Log(LogType.Exception, string.Format("Nie udało się ustawić PersonelId: {0} na dokumencie o Id: {1}", documentId, operatorId), operatorId);

        var nz_finanse = dbContext.NzFinanses.Where(x => x.NzfIdDokumentAuto == documentId);
        foreach (var nzf in nz_finanse)
        {
          nzf.NzfIdWystawil = operatorId;
        }
        dbContext.SaveChanges();
      }
    }

    /// <summary>
    /// Czy obiekt został zmodyfikowany
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private static bool ChceckObjectChanged(BObjectBase obj)
    {
      return obj != null && (obj.IsNew || obj.IsUpdated || obj.IsDeleted);
    }

    /// <summary>
    /// Pobiera użytkownika z bazy i sprawdza czy jest mobilny
    /// </summary>
    /// <param name="id">Id użytkownika</param>
    /// <param name="dbContext">Połącznie z bazą</param>
    /// <returns>Użytkownik</returns>
    private Database.IfxApiUzytkownik GetIFxUser(int id, Database.NavireoDbContext dbContext)
    {
      var ifx_user = dbContext.IfxApiUzytkowniks.FirstOrDefault(x => x.UzId == id);
      if (ifx_user == null)
        throw new UserException(string.Format("Brak użytkownika o Id: {0}", id));
      if (ifx_user.Active == false)
        throw new UserException(string.Format("Użytkownik o Id:{0} nie jest mobilny", id));

      return ifx_user;
    }

    /// <summary>
    /// Ustawia kntrahenta na dokumencie
    /// </summary>
    /// <param name="suDokument">Dokument navireo</param>
    /// <param name="document">Dokument</param>
    /// <param name="navireoInstance">Instancja navireo</param>
    /// <param name="dbContext">Połączenie z bazą</param>
    /// <returns></returns>
    private IEnumerable<KeyValuePair<string, int>> UstawKontrahenta(InsERT.SuDokument suDokument, CommerceDocumentBase document, InsERT.Navireo navireoInstance)
    {
      var result = new List<KeyValuePair<string, int>>();
      int khId = 0;
      var buyer = document.BuyerAddress;
      var deliveryAddress = document.DeliveryAddress;
      if (buyer != null)
      {
        if (buyer.IsNew)
        {
          khId = Helpers.NavireoKontrahent.DodajKontrahenta(navireoInstance, dbContext, buyer, deliveryAddress, null);
          result.Add(new KeyValuePair<string, int>(buyer.GUID, khId));
        }

        if (buyer.Id != 0 || !string.IsNullOrEmpty(buyer.Code))
        {
          var kontrhent = dbContext.KhKontrahents.FirstOrDefault(x => x.KhId == buyer.Id && buyer.Id != 0);
          if (kontrhent == null)
            kontrhent = dbContext.KhKontrahents.FirstOrDefault(x => x.KhSymbol == buyer.Code);

          if (kontrhent == null)
            throw new DocumentUpdateException(DocumentUpdateResult.CustomerError, string.Format("Brak kontrahenta o Id: {0} i symbolu: {1}", buyer.Id, buyer.Code));

          khId = kontrhent.KhId;

          var aktywny = Helpers.NavireoKontrahent.SprawdzAktywnoscKontrahenta(khId, navireoInstance);
          if (aktywny == false)
            throw new DocumentUpdateException(DocumentUpdateResult.CustomerError, string.Format("Kontrahent o symbolu: {0} jest zablokowany", kontrhent.KhSymbol));
        }
      }
      try
      {
        if (khId != 0)
          suDokument.KontrahentId = khId;
      }
      catch (Exception e)
      {
        throw new DocumentUpdateException(DocumentUpdateResult.CustomerError, e.Message, e.InnerException);
      }
      return result;
    }

    /// <summary>
    /// Ustawia Id kasy na dokumencie
    /// </summary>
    /// <param name="suDokument">Dokument Navireo</param>
    /// <param name="kasaId">Id kasy</param>
    private void SetKasaId(InsERT.SuDokument suDokument, int kasaId)
    {
      try
      {
        suDokument.KasaId = kasaId;
      }
      catch (Exception)
      {
        Logger.Log(LogType.Exception, "Wystąpił błąd podczas ustawiania kasy dla dokumentu", 0);
        throw;
      }
    }

    /// <summary>
    /// Ustawia magazyn
    /// </summary>
    /// <param name="navireoInstance">Instancja navireo</param>
    /// <param name="magazynId">Is magazynu</param>
    private void SetMagazynId(InsERT.Navireo navireoInstance, int magazynId, int userId)
    {
      try
      {
        navireoInstance.MagazynId = magazynId;
      }
      catch (Exception)
      {
        Logger.Log(LogType.Exception, "Wystąpił błąd podczas ustawiania magazynu", userId);
        throw;
      }
    }

    /// <summary>
    /// Ustawia nr dokumentu
    /// </summary>
    /// <param name="document">Dokument</param>
    /// <param name="suDokument">Dokument navireo</param>
    //private static void SetDocumentNumber(CommerceDocumentBase document, InsERT.SuDokument suDokument)
    //{
    //  try
    //  {
    //    string rozszerzenieNumeru = GetNumberExtension(document.FullNumber);
    //    if (!string.IsNullOrEmpty(rozszerzenieNumeru))
    //      suDokument.NumerRozszerzenie = rozszerzenieNumeru;
    //    suDokument.Numer = document.Number;
    //  }
    //  catch (Exception)
    //  {
    //    throw new DocumentUpdateException(DocumentUpdateResult.NumberError, string.Format("Powtórzony numer dokumentu: {0}", document.FullNumber));
    //  }
    //}

    /// <summary>
    /// Ustawia dokument powiązany
    /// </summary>
    /// <param name="document">Dokument</param>
    /// <param name="suDokument">Dokument navireo</param>
    /// <param name="dbContext">Połączenie z bazą</param>
    private void SetRelatedDocument(CommerceDocumentBase document, InsERT.SuDokument suDokument)
    {
      if (document.RelatedDocumentId != 0 && (suDokument.GetDocumentEnumType() == DocumentEnum.SalesInvoice || suDokument.GetDocumentEnumType() == DocumentEnum.Receipt))
      {
        var dokPowiazany = dbContext.DokDokuments.FirstOrDefault(x => x.DokId == document.RelatedDocumentId);
        if (dokPowiazany == null)
          throw new Exception(string.Format("Dokument powiązany - Brak dokumentu o podanym Id: {0}", document.RelatedDocumentId));
        try
        {
          suDokument.NaPodstawie(document.RelatedDocumentId);
        }
        catch (Exception e)
        {
          if (e.HResult == -2147024891)
            throw new Exception("Uprawnienie: 'Zamówienia od Klientów - Zrealizuj' nie jest dostępne dla użytkownika");
          else
            throw;
        }
        InsERT.SuPozycja pozycja = null;
        try
        {
          foreach (var oPoz in suDokument.Pozycje)
          {
            pozycja = (InsERT.SuPozycja)oPoz;
            pozycja.Usun();
          }
        }
        finally
        {
          if (pozycja != null) Marshal.ReleaseComObject(pozycja);
          pozycja = null;
        }
        foreach (var item in document.DocumentItems)
        {
          item.IsNew = true;
          item.IsUpdated = false;
          item.IsDeleted = false;
        }
      }

      if (!string.IsNullOrEmpty(document.RelatedDocumentNumber))
        suDokument.NumerOryginalny = document.RelatedDocumentNumber;
    }

    /// <summary>
    /// Wczytuje dokument
    /// </summary>
    /// <param name="operatorId"></param>
    /// <param name="document"></param>
    /// <param name="dokManager"></param>
    /// <param name="suDokument"></param>
    /// <returns></returns>
    private static InsERT.SuDokument WczytajDokument(int operatorId, CommerceDocumentBase document, InsERT.SuDokumentyManager dokManager, InsERT.SuDokument dok)
    {
      dok = dokManager.WczytajDokument(document.Id);
      return dok;
    }

    /// <summary>
    /// Ustawia płatność na dokumencie
    /// </summary>
    /// <param name="document">Dokument</param>
    /// <param name="suDokument">Dokument navireo</param>
    /// <param name="dbContext">Połączenie z bazą</param>
    private void SetPayment(CommerceDocumentBase document, InsERT.SuDokument suDokument)
    {
      if (document.IsUpdated && document.PaymentPaidGross == 0) //najpierw ustawiamy całość jako gotówka
      {
        suDokument.PlatnoscKredytKwota = 0;
        suDokument.PlatnoscKartaKwota = 0;
        suDokument.PlatnoscRatyKwota = 0;
        suDokument.PlatnoscPrzelewKwota = 0;
        suDokument.PlatnoscGotowkaKwota = suDokument.WartoscBrutto;
      }

      //Jeżeli inna płatniż gotówka
      if (document.Payment != null)
      {
        var configuration = dbContext.IfxApiUstawienia.First();

        if (document.PaymentPaidGross > suDokument.WartoscBrutto)
          document.PaymentPaidGross = suDokument.WartoscBrutto;

        if (document.PaymentPaidGross == 0)
          document.PaymentPaidGross = suDokument.WartoscBrutto - document.PaidCashGross;

        if (document.IsUpdated)  //if update to zerujemy platnosci
          suDokument.PlatnoscGotowkaKwota = suDokument.WartoscBrutto;

        var ifx_platnosci = dbContext.IfxApiFormaPlatnoscis.FirstOrDefault(x => x.Id == document.Payment.Id);
        if (ifx_platnosci == null)
          throw new Exception(string.Format("Brak płatności o Id:{0} w tabeli IFx_ApiFormaPlatnosci", document.Payment.Id));

        if (ifx_platnosci.FpId == configuration.PlatnoscPrzelew)
        {
          suDokument.PlatnoscKredytKwota = document.PaymentPaidGross;
          suDokument.Rozliczony = true;
          suDokument.PlatnoscKredytTermin = document.IssueDate;
        }
        else if (ifx_platnosci.FpId == configuration.PlatnoscKredytKupiecki)
        {
          suDokument.PlatnoscKredytKwota = document.PaymentPaidGross;
          suDokument.PlatnoscKredytTermin = document.PaymentDeadline;
        }
        else
        {
          var sl_platnosc = dbContext.SlFormaPlatnoscis.FirstOrDefault(x => x.FpId == ifx_platnosci.FpId);
          if (sl_platnosc == null)
            throw new Exception(string.Format("Brak platności o Id: {0} w tabeli sl_FormaPlatnosci", ifx_platnosci.FpId));

          if (sl_platnosc.FpTyp == 3)
          {
            suDokument.PlatnoscRatyId = sl_platnosc.FpId;
            suDokument.PlatnoscRatyKwota = document.PaymentPaidGross;
          }
          else if (sl_platnosc.FpTyp == 1)
          {
            suDokument.PlatnoscKartaId = sl_platnosc.FpId;
            suDokument.PlatnoscKartaKwota = document.PaymentPaidGross;
          }
        }
      }
    }

    /// <summary>
    /// Ustawia dostawę na dokumencie, jeżeli cena==0 to dodaje w uwagach
    /// </summary>
    /// <param name="document">Dokument</param>
    /// <param name="suDokument">Dokument Navireo</param>
    /// <param name="pozManager">Pozycje manager</param>
    /// <param name="dbContext">Połączenie z bazą</param>
    /// <param name="deliveryList">Dostępne płatności</param>
    private void UpdateDocumentDelivery(CommerceDocumentBase document, InsERT.SuDokument suDokument, InsERT.SuPozycje pozManager, IEnumerable<DeliveryType> deliveryList)
    {
      //string comment = suDokument.Uwagi ?? "";

      if (document.IsNew)
      {
        document.Delivery.IsNew = true;
        document.Delivery.IsUpdated = false;
        document.Delivery.IsDeleted = false;
      }
      if (document.Delivery.Amount <= 0)
        document.Delivery.Amount = 1;

      InsERT.SuPozycja pozycja = null;
      try
      {
        foreach (var oPoz in suDokument.Pozycje) //usuwamy pozycję dostawy z dokumentu
        {
          pozycja = (InsERT.SuPozycja)oPoz;
          var isDelivery = deliveryList.FirstOrDefault(x => x.Id == pozycja.TowarId);
          if (isDelivery != null)
            pozycja.Usun();
        }
      }
      finally
      {
        if (pozycja != null) Marshal.ReleaseComObject(pozycja);
        pozycja = null;
      }

      if (document.Delivery.IsNew || document.Delivery.IsUpdated)
      {
        var dostawa = deliveryList.FirstOrDefault(x => x.Id == document.Delivery.Id);
        if (dostawa == null)
          dostawa = deliveryList.FirstOrDefault(x => x.Code == document.Delivery.Code);
        if (dostawa == null)
          throw new Exception(string.Format("Brak dostawy o Id: {0} i symbolu: '{1}'", document.Delivery.Id, document.Delivery.Code));
        if (dostawa.PriceNet != 0 || dostawa.PriceGross != 0)
        {
          var documentItem = new DocumentItem
          {
            IsNew = true,
            Product = new Product
            {
              Id = dostawa.Id,
              Code = dostawa.Code,
            },
            PriceNet = document.Delivery.PriceNet != 0 ? document.Delivery.PriceNet : dostawa.PriceNet,
            //PriceGross = document.Delivery.PriceGross != 0 ? document.Delivery.PriceGross : dostawa.PriceGross,
            Amount = document.Delivery.Amount
          };
          UpdateDocumentItems(new List<DocumentItem>() { documentItem }, suDokument, pozManager); //zapisujemy dostawę jako pozycję
        }
        else
        {
          string deliveryComment = string.Format("[Dostawa:{0}]", dostawa.Name);
          string comment = suDokument.Uwagi ?? "";
          if (!comment.Contains(deliveryComment))
            suDokument.Uwagi = deliveryComment + suDokument.Uwagi;
        }
      }
    }

    /// <summary>
    /// Usuwa/dezaktywuje dokument w Navireo
    /// </summary>
    /// <param name="documentId">Id dokumentu</param>
    /// <param name="dokManager">Dokument manager</param>
    /// <param name="dbContext">DbContext</param>
    private void DeleteDocument(int documentId, InsERT.SuDokumentyManager dokManager)
    {
      var configuration = dbContext.IfxApiUstawienia.First();
      InsERT.SuDokument dok = null;
      try
      {
        var dokDB = dbContext.DokDokuments.FirstOrDefault(x => x.DokId == documentId);
        if (dokDB == null)
          throw new Exception(string.Format("Brak dokumentu do usunięcia o Id: {0}", documentId));
        dok = dokManager.WczytajDokument(documentId);
        switch (dokDB.DokTyp)
        {
          case 2:
            {
              if (configuration.InvoiceDeleteAsCancel) //zamiast usuwania anulujemy dokument
              {
                dok.Uniewaznij();
                dok.Zamknij();
              }
              else
              {
                if (!dok.MoznaUsunac)
                {
                  dok.Uniewaznij();
                  dok.Zamknij();
                }
                else
                  dok.Usun();
              }
              break;
            }
          case 15:
            {
              if (configuration.OrderDeleteAsCancel)
              {
                dok.Uniewaznij();
                dok.Zamknij();
              }
              else
              {
                if (!dok.MoznaUsunac)
                {
                  dok.Uniewaznij();
                  dok.Zamknij();
                }
                else
                  dok.Usun();
              }
              break;
            }
          case 16:
            {
              if (configuration.OrderDeleteAsCancel)
              {
                dok.Uniewaznij();
                dok.Zamknij();
              }
              else
              {
                if (!dok.MoznaUsunac)
                {
                  dok.Uniewaznij();
                  dok.Zamknij();
                }
                else
                  dok.Usun();
              }
              break; ;
            }
          case 21:
            {
              if (configuration.ReceiptDeleteAsCancel)
              {
                dok.Uniewaznij();
                dok.Zamknij();
              }
              else
              {
                if (!dok.MoznaUsunac)
                {
                  dok.Uniewaznij();
                  dok.Zamknij();
                }
                else
                  dok.Usun();
              }
              break;
            }
        }
      }
      finally
      {
        if (dok != null)
          Marshal.ReleaseComObject(dok);
        dok = null;
      }
    }

    /// <summary>
    /// Sprawdza uprawnienia użytkownika i tworzy dokument Navireo
    /// </summary>
    /// <param name="operatorId">Id użytkownika</param>
    /// <param name="document">Dokument</param>
    /// <param name="dokManager">Navireo dokument manager</param>
    /// <param name="dbContext">DbContext</param>
    /// <returns>InsERT.SuDokument</returns>
    private InsERT.SuDokument CreateDocument(int operatorId, CommerceDocumentBase document, InsERT.SuDokumentyManager dokManager)
    {
      InsERT.SuDokument suDokument = null;
      switch (document.DocumentType)
      {
        case DocumentEnum.CustomerOrder:
          {
            if (UzytkownikPosiadaUprawnienie(Uprawnienia.ZKDodaj, operatorId) == false)
              throw new Exception("Uprawnienie: 'Zamówienia od Klientów - Dodaj' nie jest dostępne dla użytkownika");
            try
            {
              suDokument = dokManager.DodajZK();
            }
            catch (Exception)
            {
              throw new Exception("Uprawnienie: 'Zamówienia od Klientów - Dodaj' nie jest dostępne dla użytkownika głównego");
            }
            break;
          }
        case DocumentEnum.Receipt:
          {
            if (document.BuyerAddress != null && (document.BuyerAddress.IsNew || document.BuyerAddress.Id != 0 || !string.IsNullOrEmpty(document.BuyerAddress.Code)))
            {
              if (UzytkownikPosiadaUprawnienie(Uprawnienia.PAiDodaj, operatorId) == false)
                throw new Exception("Uprawnienie: 'Sprzedaż detaliczna - Dodaj paragon imienny' nie jest dostępne dla użytkownika");

              try
              {
                suDokument = dokManager.DodajPAi();
              }
              catch (Exception)
              {
                throw new Exception("Uprawnienie: 'Sprzedaż detaliczna - Dodaj paragon imienny' nie jest dostępne dla użytkownika głównego");
              }
            }
            else
            {
              if (UzytkownikPosiadaUprawnienie(Uprawnienia.PADodaj, operatorId) == false)
                throw new Exception("Uprawnienie: 'Sprzedaż detaliczna - Dodaj paragon' nie jest dostępne dla użytkownika");

              try
              {
                suDokument = dokManager.DodajPA();
              }
              catch (Exception)
              {
                throw new Exception("Uprawnienie: 'Sprzedaż detaliczna - Dodaj paragon' nie jest dostępne dla użytkownika głównego");
              }
            }

            break;
          }
        case DocumentEnum.SalesInvoice:
          {
            if (UzytkownikPosiadaUprawnienie(Uprawnienia.FSDodaj, operatorId) == false)
              throw new Exception("Uprawnienie: 'Faktury sprzedaży - Dodaj fakturę sprzedaży' nie jest dostępne dla użytkownika");

            try
            {
              suDokument = dokManager.DodajFS();
            }
            catch (Exception)
            {
              throw new Exception("Uprawnienie: 'Faktury sprzedaży - Dodaj fakturę sprzedaży' nie jest dostępne dla użytkownika głównego");
            }
            break;
          }
        case DocumentEnum.StoreOrder:
          {
            if (UzytkownikPosiadaUprawnienie(Uprawnienia.ZMMDodaj, operatorId) == false)
              throw new Exception("Uprawnienie: 'Dokument ZMM - Dodaj' nie jest dostępne dla użytkownika");
            try
            {
              suDokument = dokManager.DodajDokumentDefiniowalny(1);
            }
            catch (Exception)
            {
              throw new Exception("Uprawnienie: 'Dokument ZMM - Dodaj' nie jest dostępne dla użytkownika głównego");
            }
            break;
          }
        default:
          {
            throw new Exception(string.Format("Not implemented type of document :'{0}'", document.DocumentType));
          }
      }
      return suDokument;
    }

    ///// <summary>
    ///// Pobiera dokument z Navireo
    ///// </summary>
    ///// <param name="navireoInstance">Navireo instance</param>
    ///// <param name="documentId">Id dokumentu do pobrania</param>
    ///// <param name="propertySelector">Konf. wypełnianych pól </param>
    ///// <param name="operatorId">Id użytkownika</param>
    ///// <returns></returns>
    //public CommerceDocumentBase GetDocument(InsERT.Navireo navireoInstance, int documentId, DocumentPropertySelector propertySelector, int operatorId)
    //{
    //  if (propertySelector == null)
    //    propertySelector = new DocumentPropertySelector();

    //  CommerceDocumentBase result = new CommerceDocumentBase();
    //  Model.IF_vwDokument dok_BD;
    //  using (var dbConnection = new Model.NavireoEntities())
    //  {
    //    var configuration = dbConnection.IFx_ApiUstawienia.First();
    //    var ifx_user = dbConnection.IFx_ApiUzytkownik.FirstOrDefault(x => x.uz_Id == operatorId && x.Active);
    //    if (ifx_user == null) throw new Exception("Brak uprawnień użytkownika");

    //    dok_BD = dbConnection.IF_vwDokument.FirstOrDefault(x => x.dok_Id == documentId);
    //    if (dok_BD == null) throw new Exception(string.Format("Brak dokumentu o Id: {0}", documentId));

    //    if (ifx_user.cecha_Id != null && dok_BD.dok_PlatnikId != null)
    //    {
    //      var kh = dbConnection.kh_CechaKh.FirstOrDefault(x => x.ck_IdCecha == ifx_user.cecha_Id && x.ck_IdKhnt == dok_BD.dok_PlatnikId);
    //      if (kh == null)
    //        throw new UnauthorizedAccessException(string.Format("Brak uprawnień użytkownika do odczytu dokumentu kontrahenta o Id: {0}", dok_BD.dok_PlatId ?? 0));
    //    }

    //    InsERT.SuDokumentyManager suDokumentyManager = null;
    //    InsERT.SuDokument suDokument = null;
    //    InsERT.KontrahenciManager khManager = null;
    //    try
    //    {
    //      suDokumentyManager = navireoInstance.SuDokumentyManager;
    //      suDokument = suDokumentyManager.WczytajDokument(documentId);

    //      result = new CommerceDocumentBase();
    //      result.Id = suDokument.Identyfikator;
    //      result.Number = suDokument.Numer;
    //      result.FullNumber = suDokument.NumerPelny;
    //      result.IssueDate = dok_BD != null && dok_BD.idok_DataCzasWyst != null ? dok_BD.idok_DataCzasWyst : suDokument.DataWystawienia;
    //      result.PaymentDeadline = dok_BD.dok_PlatTermin;
    //      result.TotalNet = suDokument.WartoscNetto;
    //      result.TotalGross = suDokument.WartoscBrutto;
    //      result.RelatedDocumentId = suDokument.GetDocumentEnumType() == BO.Constants.DocumentEnum.CustomerOrder ? dok_BD.DokPowiazanyId ?? 0 : dok_BD.dok_DoDokId ?? 0;
    //      result.RelatedDocumentNumber = suDokument.GetDocumentEnumType() == BO.Constants.DocumentEnum.CustomerOrder ? dok_BD.DokPowiazanyNumer : dok_BD.dok_DoDokNrPelny;
    //      result.DocumentType = suDokument.GetDocumentEnumType();
    //      result.Comment = suDokument.Uwagi;
    //      result.Currency = new Currency
    //      {
    //        Code = suDokument.WalutaSymbol,
    //        Ratio = suDokument.WalutaKurs
    //      };
    //      result.Completed = dok_BD.dok_Status == 8;
    //      result.PaidCashGross = suDokument.PlatnoscGotowkaKwota;
    //      result.PriceBaseOnNet = suDokument.LiczonyOdCenNetto;
    //      result.Comment = suDokument.Uwagi;

    //      result.SellerAddress = NavireoKontrahent.Sprzedawca(dbConnection);
    //      result.BuyerAddress = Helpers.NavireoKontrahent.GetKontrahentAdrH(dok_BD.dok_PlatnikAdreshId, dbConnection);

    //      result.ToPay = dok_BD.nzf_WartoscDoZaplaty ?? 0;
    //      result.PaymentDeadline = dok_BD.dok_PlatTermin.HasValue ? dok_BD.dok_PlatTermin.Value : dok_BD.DataWystawienia;

    //      result.FromPerson = new User
    //      {
    //        Name = suDokument.Wystawil
    //      };
    //      result.ToPerson = new User
    //      {
    //        Name = suDokument.Odebral
    //      };

    //      FillDocumentUpdateAndDeletePermision(operatorId, result, dok_BD, dbConnection, configuration, suDokument);

    //      result.Payment = GetDocumentPayment(result, dok_BD, dbConnection, configuration);

    //      result.DocumentItems = GetDocumentItems(dbConnection, navireoInstance, propertySelector, result, suDokument);

    //      suDokument.Zamknij();
    //    }
    //    finally
    //    {
    //      if (suDokumentyManager != null)
    //        Marshal.ReleaseComObject(suDokumentyManager);
    //      suDokumentyManager = null;

    //      if (suDokument != null)
    //        Marshal.ReleaseComObject(suDokument);
    //      suDokument = null;
    //      if (khManager != null)
    //        Marshal.ReleaseComObject(khManager);
    //      khManager = null;
    //      khManager = null;
    //    }
    //    return result;
    //  }
    //}

    /// <summary>
    /// Ustawia flagi CanUpdate i CanDelete 
    /// </summary>
    /// <param name="userId">Id użytkownika</param>
    /// <param name="document">Dokument</param>
    /// <param name="dok_BD">IF_vwDokument entity</param>
    /// <param name="dbConnection">DbContext</param>
    /// <param name="configuration">Konfiguracja</param>
    /// <param name="suDokument">SuDokument</param>
    //private static void FillDocumentUpdateAndDeletePermision(int userId, CommerceDocumentBase result, Model.IF_vwDokument dok_BD, Model.NavireoEntities dbConnection, Model.IFx_ApiUstawienia configuration, InsERT.SuDokument suDokument)
    //{
    //  if (suDokument.GetDocumentEnumType() == BO.Constants.DocumentEnum.Receipt)
    //  {
    //    result.CanEdit = suDokument.MoznaEdytowac;
    //    result.CanDelete = suDokument.MoznaUsunac;
    //  }
    //  else if (suDokument.GetDocumentEnumType() == BO.Constants.DocumentEnum.SalesInvoice)
    //  {
    //    result.CanEdit = suDokument.MoznaEdytowac;
    //    result.CanDelete = suDokument.MoznaUsunac;
    //  }
    //  else if (suDokument.GetDocumentEnumType() == BO.Constants.DocumentEnum.CustomerOrder)
    //  {
    //    result.CanEdit = dok_BD.dok_Status != 8 && UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.ZKPopraw, userId, dbConnection);
    //    if (configuration.OrderDeleteAsCancel)
    //    {
    //      result.CanDelete = dok_BD.dok_Status != 8 && UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.DokUniewaznij, userId, dbConnection);
    //    }
    //    else
    //    {
    //      result.CanDelete = dok_BD.dok_Status != 8 && UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.ZKUsun, userId, dbConnection);
    //    }
    //  }
    //  else if (suDokument.GetDocumentEnumType() == BO.Constants.DocumentEnum.StoreOrder)
    //  {
    //    result.CanEdit = dok_BD.dok_Status != 8 && UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.ZMMPopraw, userId, dbConnection);
    //    if (configuration.OrderDeleteAsCancel)
    //    {
    //      result.CanDelete = dok_BD.dok_Status != 8 && UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.DokUniewaznij, userId, dbConnection);
    //    }
    //    else
    //    {
    //      result.CanDelete = dok_BD.dok_Status != 8 && UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.ZMMUsun, userId, dbConnection);
    //    }
    //  }
    //}

    ///// <summary>
    ///// Pobiera pozycje dokumentu
    ///// </summary>
    ///// <param name="dbContext">DbContext</param>
    ///// <param name="navireoInstance">Navireo instance</param>
    ///// <param name="propertySelector">Konf. wypełnianych pól DocumentItem</param>
    ///// <param name="document">Dokument</param>
    ///// <param name="suDokument">SuDokument</param>
    ///// <returns>Lista pozycji dokumentu</returns>
    //private static List<DocumentItem> GetDocumentItems(Model.NavireoEntities dbContext, InsERT.Navireo navireoInstance, DocumentPropertySelector propertySelector, CommerceDocumentBase document, InsERT.SuDokument suDokument)
    //{
    //  var productManager = new ProductManager();
    //  var deliveryManager = new DeliveryManager();
    //  var deliveryList = deliveryManager.GetList();

    //  List<DocumentItem> documentItems = new List<DocumentItem>();

    //  InsERT.SuPozycja pozycja = null;
    //  foreach (var oPoz in suDokument.Pozycje)
    //  {
    //    pozycja = (InsERT.SuPozycja)oPoz;
    //    try
    //    {
    //      var dostawa = deliveryList.FirstOrDefault(x => x.Id == pozycja.TowarId);
    //      if (dostawa != null)
    //      {
    //        document.Delivery = new DeliveryType
    //        {
    //          Id = pozycja.TowarId,
    //          Amount = Convert.ToInt32(pozycja.IloscJm),
    //          Code = pozycja.TowarSymbol,
    //          Name = pozycja.TowarNazwa,
    //          PriceNet = pozycja.CenaNettoPoRabacie,
    //          PriceGross = pozycja.CenaBruttoPoRabacie,
    //          TaxValue = pozycja.VatProcent
    //        };
    //      }
    //      if (propertySelector.FillDocumentItems)
    //      {
    //        var item = new DocumentItem();
    //        item.Id = pozycja.Id;
    //        item.Amount = pozycja.IloscJm;

    //        item.DiscountRate = pozycja.RabatProcent;

    //        item.PriceNet = pozycja.CenaNettoPrzedRabatem;
    //        item.PriceNetAfterDiscount = pozycja.CenaNettoPoRabacie;

    //        item.PriceGross = pozycja.CenaBruttoPrzedRabatem;
    //        item.PriceGrossAfterDiscount = pozycja.CenaBruttoPoRabacie;

    //        item.TotalNet = pozycja.WartoscNettoPrzedRabatem;
    //        item.TotalNetAfterDiscount = pozycja.WartoscNettoPoRabacie;

    //        item.TotalGross = pozycja.WartoscBruttoPrzedRabatem;
    //        item.TotalGrossAfterDiscount = pozycja.WartoscBruttoPoRabacie;

    //        item.TotalTax = pozycja.WartoscBruttoPrzedRabatem - pozycja.WartoscNettoPrzedRabatem;
    //        item.TotalTaxAfterDiscount = pozycja.WartoscVatPoRabacie;
    //        item.Tax = new Tax
    //        {
    //          Id = pozycja.VatId,
    //          Rate = pozycja.VatProcent
    //        };
    //        item.Product = productManager.GetProduct(navireoInstance, pozycja.TowarId, pozycja.TowarSymbol, propertySelector.ProductPropertySelector);
    //        GetMappingDiscountItemId(item, dbContext);
    //        documentItems.Add(item);
    //      }
    //    }
    //    finally
    //    {
    //      if (pozycja != null) Marshal.ReleaseComObject(pozycja);
    //      pozycja = null;
    //    }
    //  }
    //  return documentItems;
    //}

    ///// <summary>
    ///// Pobiera płatność dokumentu
    ///// </summary>
    ///// <param name="document">Dokument</param>
    ///// <param name="dok_BD"></param>
    ///// <param name="dbConnection">IF_vwDokument entity</param>
    ///// <param name="configuration">Konfiguracja Api</param>
    ///// <returns></returns>
    //private static PaymentType GetDocumentPayment(CommerceDocumentBase document, Model.IF_vwDokument dok_BD, Model.NavireoEntities dbConnection, Model.IFx_ApiUstawienia configuration)
    //{
    //  int przelewId = configuration.Platnosc_Przelew;
    //  int kredytKupieckiId = configuration.Platnosc_KredytKupiecki;

    //  Model.IFx_ApiFormaPlatnosci ifx_platnosc = null;
    //  if (dok_BD.dok_KwKarta == 0 && dok_BD.dok_KwGotowka == 0 && dok_BD.dok_KwKredyt == 0 && dok_BD.dok_KwDoZaplaty == 0)  //płatność przelewem
    //  {
    //    document.PaymentPaidGross = dok_BD.dok_KwWartosc ?? 0;
    //    ifx_platnosc = dbConnection.IFx_ApiFormaPlatnosci.FirstOrDefault(x => x.fp_Id == przelewId);
    //  }
    //  else if (dok_BD.dok_KwKredyt != 0)  //płatność - kredyt(lista) 
    //  {
    //    document.PaymentPaidGross = dok_BD.dok_KwKredyt ?? 0;
    //    ifx_platnosc = dbConnection.IFx_ApiFormaPlatnosci.FirstOrDefault(x => x.fp_Id == dok_BD.dok_KredytId);
    //  }
    //  else if (dok_BD.dok_KwKarta != 0) //płatność kartą (lista) 
    //  {
    //    document.PaymentPaidGross = dok_BD.dok_KwKarta ?? 0;
    //    ifx_platnosc = dbConnection.IFx_ApiFormaPlatnosci.FirstOrDefault(x => x.fp_Id == dok_BD.dok_KartaId);
    //  }
    //  else if (dok_BD.dok_KwDoZaplaty != 0) //kredyt kupiecki
    //  {
    //    document.PaymentPaidGross = dok_BD.dok_KwDoZaplaty ?? 0;
    //    ifx_platnosc = dbConnection.IFx_ApiFormaPlatnosci.FirstOrDefault(x => x.fp_Id == kredytKupieckiId);
    //  }

    //  if (ifx_platnosc != null)
    //    return new PaymentType
    //    {
    //      Id = ifx_platnosc.Id,
    //      Name = ifx_platnosc.Nazwa
    //    };
    //  else
    //    return null;
    //}

    ///// <summary>
    ///// Pobiera listę dokumentów
    ///// </summary>
    ///// <param name="listSelector">Selekcja i sortowanie listy dokumentów</param>
    ///// <param name="propertySelector">Konf. wypełnianych pól w CommerceDocumentBase</param>
    ///// <param name="userId">Id użytkownika</param>
    ///// <param name="storeId">Id magazynu</param>
    ///// <returns>Lista dokumentów</returns>
    //public ListAfterPagination<CommerceDocumentBase> GetList(DocumentListSelector listSelector, DocumentPropertySelector propertySelector, int? userId, int storeId)
    //{
    //  if (listSelector == null)
    //    listSelector = new DocumentListSelector();
    //  if (propertySelector == null)
    //    propertySelector = new DocumentPropertySelector();

    //  int typDokumentu = listSelector.DocumentType.TypDokumentu();

    //  using (var dbConnection = new Model.NavireoEntities(180))
    //  {
    //    var apiConfiguration = dbConnection.IFx_ApiUstawienia.First();
    //    bool dokPopraw = false, dokUsun = false;

    //    WczytajUprawnienia(listSelector.DocumentType, userId, apiConfiguration, ref dokPopraw, ref dokUsun, dbConnection);

    //    if (listSelector.DocumentType == BO.Constants.DocumentEnum.StoreOrder)
    //      userId = null; //ZMM pobieramy wszystkie, nie dla konkretnego użytkownika

    //    var ifx_user = dbConnection.IFx_ApiUzytkownik.FirstOrDefault(x => x.uz_Id == userId);
    //    if (userId != null && ifx_user == null)
    //      return new ListAfterPagination<CommerceDocumentBase>();
    //    var pdUzytkownik = dbConnection.PdUzytkowniks.FirstOrDefault(x => x.uz_Id == userId);

    //    IQueryable<Model.IF_vwDokument> query = null;
    //    if (userId == null || ifx_user.cecha_Id == null) //jeżeli brak użytkownika lub brak przypisanej cechy 
    //      query = (from dok in dbConnection.IF_vwDokument
    //               where dok.dok_Status != 2
    //               select dok).AsQueryable();
    //    else
    //    {
    //      query = (from khCechy in dbConnection.kh_CechaKh
    //               join dok in dbConnection.IF_vwDokument on new { khId = khCechy.ck_IdKhnt, cechaId = khCechy.ck_IdCecha } equals new { khId = (int)dok.dok_PlatnikId, cechaId = (int)ifx_user.cecha_Id }
    //               where dok.dok_Status != 2
    //               select dok).AsQueryable();
    //      if (listSelector.DocumentType == BO.Constants.DocumentEnum.Receipt)
    //      {
    //        //do listy dodajemy paragony nieimienne na podstawie osoby wystawiającej dokument
    //        query = query.Union(dbConnection.IF_vwDokument.Where(x => x.dok_Typ == typDokumentu && x.dok_PersonelId == userId && x.dok_PlatnikId == null)).AsQueryable();
    //      }
    //    }

    //    if (typDokumentu == 0)
    //    {
    //      //jeżeli typ dokumentu jest nieokreślony to zwracamy paragony i faktury
    //      int invoice = DocumentEnum.SalesInvoice.TypDokumentu();
    //      int receipt = DocumentEnum.Receipt.TypDokumentu();
    //      query = query.Where(x => x.dok_Typ == invoice || x.dok_Typ == receipt);
    //    }
    //    else
    //    {
    //      //jeżeli typ jest określony to zwracamy tylko dokumenty tego typu
    //      query = query.Where(x => x.dok_Typ == typDokumentu);
    //    }

    //    query = FilterDocumentList(listSelector, query);

    //    query = SetSortingDocumentList(listSelector.DocumentSorting, query);

    //    long totalRecordCount = query.Count();


    //    query = Helpers.QueryHelper<Model.IF_vwDokument>.SetPagination(query, listSelector);

    //    var sellerAddress = NavireoKontrahent.Sprzedawca(dbConnection);
    //    var paymentList = dbConnection.IFx_ApiFormaPlatnosci.ToList();

    //    List<CommerceDocumentBase> result = new List<CommerceDocumentBase>();
    //    foreach (var item in query)
    //    {
    //      bool canEdit = false, canDelete = false;

    //      canEdit = item.dok_Status != 8 && dokPopraw;
    //      canDelete = item.dok_Status != 8 && dokUsun;

    //      var document = new CommerceDocumentBase
    //      {
    //        Id = item.dok_Id,
    //        DocumentType = item.DocumentEnumType(),
    //        CanEdit = canEdit,
    //        CanDelete = canDelete,
    //        Completed = item.dok_Status == 8,
    //        Number = item.dok_Nr ?? 0,
    //        FullNumber = item.dok_NrPelny,
    //        ToPay = item.nzf_WartoscDoZaplaty ?? 0,
    //        IssueDate = (DateTime)item.DataWystawienia,
    //        DeliveryDate = item.dok_DataZakonczenia,
    //        PaymentDeadline = item.dok_PlatTermin,
    //        PaymentPaidGross = item.dok_WartBrutto - item.dok_KwGotowka ?? 0,
    //        PaidCashGross = (item.dok_KwWartosc ?? 0) - (item.nzf_WartoscDoZaplaty ?? 0),
    //        TotalNet = item.dok_WartNetto,
    //        TotalGross = item.dok_WartBrutto,
    //        TotalTax = item.dok_WartVat,
    //        Currency = new Currency
    //        {
    //          Code = item.dok_Waluta,
    //          Ratio = item.dok_WalutaKurs
    //        },
    //        Comment = item.dok_Uwagi,
    //        RelatedDocumentId = listSelector.DocumentType == BO.Constants.DocumentEnum.CustomerOrder ? item.DokPowiazanyId ?? 0 : item.dok_DoDokId ?? 0,
    //        RelatedDocumentNumber = listSelector.DocumentType == BO.Constants.DocumentEnum.CustomerOrder ? item.DokPowiazanyNumer : item.dok_DoDokNrPelny,
    //      };

    //      document.BuyerAddress = Helpers.NavireoKontrahent.GetKontrahentAdrH(item);
    //      document.BuyerAddress.Id = item.dok_PlatnikId ?? 0;
    //      document.SellerAddress = sellerAddress;


    //      if (propertySelector.FillDocumentItems)
    //        document.DocumentItems = GetDocumentItems(item.dok_DoDokId ?? 0, dbConnection);

    //      result.Add(document);
    //    }
    //    return new ListAfterPagination<CommerceDocumentBase>(result, totalRecordCount);
    //  }
    //}

    ///// <summary>
    ///// Wczytuje uprawnienia do edycji/usunięcia dokumentu dla użytkownika
    ///// </summary>
    ///// <param name="type">Typ dokumentu</param>
    ///// <param name="userId">Id użytkownika</param>
    ///// <param name="configuration">Konfiguracja API</param>
    ///// <param name="dokPopraw">Flaga - edycja</param>
    ///// <param name="dokUsun">Flaga - usunięcie</param>
    ///// <param name="dbConnection">DbContext</param>
    //private static void WczytajUprawnienia(BO.Constants.DocumentEnum? type, int? userId, Model.IFx_ApiUstawienia configuration, ref bool dokPopraw, ref bool dokUsun, Model.NavireoEntities dbConnection)
    //{
    //  if (type == BO.Constants.DocumentEnum.SalesInvoice)
    //  {
    //    dokPopraw = UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.FSPopraw, userId ?? 0, dbConnection);
    //    if (configuration.OrderDeleteAsCancel) //zamiast usuwać unieważniamy dokument -- trzeba sprawdzić czy ifx_user ma uprawnienia
    //      dokUsun = UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.DokUniewaznij, userId ?? 0, dbConnection);
    //    else
    //      dokUsun = UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.FSUsun, userId ?? 0, dbConnection);

    //  }
    //  if (type == BO.Constants.DocumentEnum.Receipt)
    //  {
    //    dokPopraw = UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.PAPopraw, userId ?? 0, dbConnection);
    //    if (configuration.OrderDeleteAsCancel) //zamiast usuwać unieważniamy dokument -- trzeba sprawdzić czy ifx_user ma uprawnienia
    //      dokUsun = UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.DokUniewaznij, userId ?? 0, dbConnection);
    //    else
    //      dokUsun = UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.PAUsun, userId ?? 0, dbConnection);
    //  }
    //  if (type == BO.Constants.DocumentEnum.CustomerOrder)
    //  {
    //    dokPopraw = UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.ZKPopraw, userId ?? 0, dbConnection);
    //    if (configuration.OrderDeleteAsCancel) //zamiast usuwać unieważniamy dokument -- trzeba sprawdzić czy ifx_user ma uprawnienia
    //      dokUsun = UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.DokUniewaznij, userId ?? 0, dbConnection);
    //    else
    //      dokUsun = UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.ZKUsun, userId ?? 0, dbConnection);
    //  }
    //  if (type == BO.Constants.DocumentEnum.StoreOrder)
    //  {
    //    dokPopraw = UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.ZMMPopraw, userId ?? 0, dbConnection);
    //    if (configuration.OrderDeleteAsCancel)
    //      dokUsun = UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.DokUniewaznij, userId ?? 0, dbConnection);
    //    else
    //      dokUsun = UzytkownikPosiadaUprawnienie(Constants.Uprawnienia.ZMMUsun, userId ?? 0, dbConnection);
    //  }
    //}

    ///// <summary>
    ///// Pobiera pozycje dokumentu
    ///// </summary>
    ///// <param name="documentId">Id dokumentu</param>
    ///// <param name="dbConnection">DbContext</param>
    ///// <returns>Lista pozycji dokumentu</returns>
    //private static List<DocumentItem> GetDocumentItems(int documentId, Model.NavireoEntities dbConnection)
    //{
    //  List<DocumentItem> documentItems = new List<DocumentItem>();
    //  documentItems = dbConnection.dok_Pozycja.Where(x => x.ob_DokHanId == documentId).Select(x => new DocumentItem
    //  {
    //    Id = x.ob_Id,
    //    Amount = x.ob_Ilosc,
    //    DiscountRate = x.ob_Rabat,
    //    PriceNetAfterDiscount = x.ob_CenaNetto,
    //    PriceGrossAfterDiscount = x.ob_CenaBrutto,
    //    TotalNetAfterDiscount = x.ob_WartNetto,
    //    TotalGrossAfterDiscount = x.ob_WartBrutto,
    //    TotalTaxAfterDiscount = x.ob_WartVat,
    //    Tax = new Tax
    //    {
    //      Id = x.ob_VatId ?? 0,
    //      Rate = x.ob_VatProc,
    //    },
    //    Product = new Product
    //    {
    //      Id = x.tw__Towar.tw_Id,
    //      Code = x.tw__Towar.tw_Symbol,
    //      Description = new Description
    //      {
    //        Name = x.tw__Towar.tw_Nazwa,
    //        DescriptionShort = x.tw__Towar.tw_Opis
    //      }
    //    }
    //  }).ToList();
    //  return documentItems;
    //}

    ///// <summary>
    ///// Pobiera pierwszą lukę w numeracji (numeracja roczna, dla danego roku)
    ///// </summary>
    ///// <param name="docType">Typ dokumentu</param>
    ///// <param name="storeId">Id magazynu</param>
    ///// <returns>Nr jeżeli znaleziono lukę, 0 jeżeli brak</returns>
    //public int GetHoleInNumeration(DocumentEnum docType, int storeId)
    //{
    //  if (docType == DocumentEnum.DocumentSettlement) //ToDo uzupełnianie luk w numeracji dok. kasowych
    //  {
    //    return 0;
    //  }

    //  int typ = docType.TypDokumentu();

    //  using (var dbContext = new Model.NavireoEntities())
    //  {
    //    var uzupelniajLukiNumeracji = dbContext.IFx_ApiUstawienia.First().UzupelniajLukiNumeracji;
    //    if (uzupelniajLukiNumeracji)
    //    {
    //      var holes = dbContext.IFx_DokLukiNumeracji(DateTime.Now.Date, storeId, typ);
    //      var hole = holes.FirstOrDefault();

    //      if (hole != null)
    //        return hole.Nr ?? 0;
    //      else
    //        return 0;
    //    }
    //    else
    //      return 0;
    //  }
    //}

    ///// <summary>
    ///// Ustawia sortowanie listy dokumentów
    ///// </summary>
    ///// <param name="listSelector"></param>
    ///// <param name="query"></param>
    ///// <returns></returns>
    //private static IQueryable<Model.IF_vwDokument> SetSortingDocumentList(DocumentSorting documentSorting, IQueryable<Model.IF_vwDokument> query)
    //{
    //  if (documentSorting == DocumentSorting.DocumentNumberASC) query = query.OrderBy(x => x.dok_NrPelny);
    //  else if (documentSorting == DocumentSorting.DocumentNumberDESC) query = query.OrderByDescending(x => x.dok_NrPelny);
    //  else if (documentSorting == DocumentSorting.IssueDateASC) query = query.OrderBy(x => x.DataWystawienia);
    //  else if (documentSorting == DocumentSorting.IssueDateDESC) query = query.OrderByDescending(x => x.DataWystawienia);
    //  else if (documentSorting == DocumentSorting.TotalPriceASC) query = query.OrderBy(x => x.dok_WartBrutto);
    //  else if (documentSorting == DocumentSorting.TotalPriceDESC) query = query.OrderByDescending(x => x.dok_WartBrutto);
    //  else if (documentSorting == DocumentSorting.ToPayASC) query = query.OrderBy(x => x.nzf_WartoscDoZaplaty);
    //  else if (documentSorting == DocumentSorting.ToPayDESC) query = query.OrderByDescending(x => x.nzf_WartoscDoZaplaty);
    //  else query = query.OrderByDescending(x => x.DataWystawienia);
    //  return query;
    //}

    ///// <summary>
    ///// Filtrowanie listy dokumentów
    ///// </summary>
    ///// <param name="listSelector">Filtry</param>
    ///// <param name="query">IQueryable<Model.IF_vwDokument></param>
    ///// <returns>IQueryable<Model.IF_vwDokument></returns>
    //private static IQueryable<Model.IF_vwDokument> FilterDocumentList(DocumentListSelector listSelector, IQueryable<Model.IF_vwDokument> query)
    //{
    //  if (listSelector.DocumentType == BO.Constants.DocumentEnum.StoreOrder) query = query.Where(x => x.dok_DefiniowalnyId == 1);
    //  if (listSelector.DeliveryDateUp != null) query = query.Where(x => x.dok_DataZakonczenia >= listSelector.DeliveryDateUp);
    //  if (listSelector.DeliveryDateTo != null) query = query.Where(x => x.dok_DataZakonczenia <= listSelector.DeliveryDateTo);
    //  if (listSelector.IssueDateFrom != null) query = query.Where(x => x.dok_DataWyst >= listSelector.IssueDateFrom);
    //  if (listSelector.IssueDateTo != null) query = query.Where(x => x.dok_DataWyst <= listSelector.IssueDateTo);
    //  if (listSelector.BuyerAdressId != null) query = query.Where(x => x.dok_PlatnikId == listSelector.BuyerAdressId);
    //  if (listSelector.TotalNetUp != null) query = query.Where(x => x.dok_WartNetto >= listSelector.TotalNetUp);
    //  if (listSelector.TotalNetTo != null) query = query.Where(x => x.dok_WartNetto <= listSelector.TotalNetUp);
    //  if (listSelector.BuyerId != null) query = query.Where(x => x.dok_PlatnikId == listSelector.BuyerId);
    //  if (listSelector.Paid != null)
    //  {
    //    if (listSelector.Paid == true)
    //      query = query.Where(x => x.nzf_WartoscDoZaplaty == 0); // tylko zapłacone
    //    else
    //      query = query.Where(x => x.nzf_WartoscDoZaplaty != 0 || x.nzf_WartoscDoZaplaty == null); // tylko niezapłacone
    //  }
    //  if (listSelector.Completed != null)
    //  {
    //    if (listSelector.Completed == true)
    //      query = query.Where(x => x.dok_Status == 8);
    //    else
    //      query = query.Where(x => x.dok_Status != 8);
    //  }

    //  if (string.IsNullOrEmpty(listSelector.FullNumber) == false) //powinnno filtrować tylko po numerze
    //    query = query.Where(x => x.dok_NrPelny.Contains(listSelector.FullNumber) || x.adrh_NazwaPelna.Contains(listSelector.FullNumber)
    //        || x.adrh_Miejscowosc.Contains(listSelector.FullNumber) || x.adrh_Ulica.Contains(listSelector.FullNumber));

    //  return query;
    //}

    ///// <summary>
    ///// Sprawdza czy jest zmiana numeracji dokumenu i KP. Jeżeli tak to ustawia Number i FullNumber, usuwa wpis z bazy.
    ///// </summary>
    ///// <param name="document">Dokument</param>
    ///// <param name="userId">Id użytkownika</param>
    ///// <param name="dbContext">DbContext</param>
    ///// <returns>Dokument z poprawionym numerem</returns>
    //private CommerceDocumentBase CheckAndChangeDocumentNumbers(CommerceDocumentBase document, int userId, Model.NavireoEntities dbContext)
    //{
    //  var zmianaNr = dbContext.IFx_ApiZmianaNumeracjiDokumentu.FirstOrDefault(x => x.Numer == document.Number && x.NumerPelny == document.FullNumber);
    //  if (zmianaNr != null)
    //  {
    //    Logger.Log(LogType.Message, string.Format("Zmiana numeracji dokumentu: {0} na {1}", document.FullNumber, zmianaNr.NowyNumerPelny), userId, null, null, null, DateTime.Now);

    //    document.Number = zmianaNr.NowyNumer;
    //    document.FullNumber = zmianaNr.NowyNumerPelny;
    //    //dbContext.IFx_ApiZmianaNumeracjiDokumentu.Remove(zmianaNr);
    //  }

    //  if (document.SettlementNumber != 0 && !string.IsNullOrEmpty(document.SettlementFullNumber))
    //  {
    //    var zmianaKp = dbContext.IFx_ApiZmianaNumeracjiDokumentu.FirstOrDefault(x => x.Numer == document.SettlementNumber && x.NumerPelny == document.SettlementFullNumber);
    //    if (zmianaKp != null)
    //    {
    //      Logger.Log(LogType.Message, string.Format("Zmiana numeracji dokumentu: {0} na {1}", document.SettlementFullNumber, zmianaKp.NowyNumerPelny), userId, null, null, null, DateTime.Now);

    //      document.SettlementNumber = zmianaKp.NowyNumer;
    //      document.SettlementFullNumber = zmianaKp.NowyNumerPelny;
    //      //dbContext.IFx_ApiZmianaNumeracjiDokumentu.Remove(zmianaKp);
    //    }
    //  }

    //  return document;
    //}

    private static string GetNumberExtension(string num)
    {
      string[] parts = num.Split('/');
      if (parts.Count() == 4)
      {
        return parts[1];
      }
      return "";
    }

    //private static string MakeNumber(string num)
    //{
    //  string[] parts = num.Split('/');
    //  if (parts.Count() == 3)
    //  {
    //    parts[0] = parts[0] + "/JL";
    //    num = string.Join("/", parts);
    //  }
    //  return num;
    //}

    public bool UzytkownikPosiadaUprawnienie(Uprawnienia uprawnienie, int userId)
    {
      int uprawnienieId = (int)uprawnienie;
      var uprBD = dbContext.PdUzytkUpraws.FirstOrDefault(x => x.UzupUprawId == uprawnienieId && x.UzupUzytkId == userId);
      return uprBD != null;
    }

    public IEnumerable<DeliveryType> GetDeliveryTypeList()
    {
      {
        var result = new List<DeliveryType>();
        var deliveryList = from delivery in dbContext.IfxApiSposobDostawies
                           join twTowar in dbContext.TwTowars on delivery.TwId equals twTowar.TwId
                           join twCena in dbContext.TwCenas on delivery.TwId equals twCena.TcIdTowar
                           select new { delivery, twCena, twTowar };

        result = deliveryList.Select(it => new DeliveryType
        {
          Id = it.delivery.TwId,
          IsUpdated = it.delivery.Aktywny,
          IsDeleted = !it.delivery.Aktywny,
          Name = it.delivery.Nazwa,
          PriceNet = it.twCena.TcCenaNetto2 ?? 0,
          PriceGross = it.twCena.TcCenaBrutto2 ?? 0,
          Tax = it.twTowar.TwIdVatSpNavigation != null ? new Tax
          {
            Id = it.twTowar.TwIdVatSpNavigation.VatId,
            Rate = it.twTowar.TwIdVatSpNavigation.VatStawka
          } : null
        }).ToList();
        return result;
      }
    }
  }
}
