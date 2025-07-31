using Microsoft.EntityFrameworkCore;
using Progress.Domain.Navireo;
using Progress.Domain.Navireo.Api;
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
    public DocumentSaveResponse UpdateDocument(CommerceDocumentBase document)
    {
      var result = new DocumentSaveResponse();
      if (NavireoInstance == null)
      {
        throw new Exception("Nie można uruchomić Navireo");
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
        SetMagazynId(NavireoInstance, (int)pd_User.UzIdMagazynu, operatorId);

        dokManager = NavireoInstance.SuDokumentyManager;

        if (document.IsNew)
        {
          suDokument = CreateDocument(operatorId, document, dokManager);
          SetKasaId(suDokument, (int)pd_User.UzIdKasy);
        }
        else if (document.IsUpdated)
        {
          var dokDB = dbContext.DokDokuments.FirstOrDefault(x => x.DokId == document.Id);
          if (dokDB == null)
            throw new Exception(string.Format("Brak dokumentu o Id: {0}", document.Id));
          suDokument = WczytajDokument(operatorId, document, dokManager, suDokument);

          int kh_Id = suDokument.KontrahentId;
          if (ifx_user.CechaId != null)
          {
            var kh = dbContext.KhCechaKhs.FirstOrDefault(x => x.CkIdCecha == ifx_user.CechaId && x.CkIdKhnt == kh_Id);
            if (kh == null)
              throw new UserException(string.Format("Brak uprawnień użytkownika do odczytu dokumentu: '{0}' kontrahenta o Id: {1}", dokDB.DokNrPelny, kh_Id));
          }
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

        var deliveryList = GetDeliveryTypeList();

        pozManager = suDokument.Pozycje;
        if (document.DocumentItems != null)
        {
          var itemsResult = UpdateDocumentItems(document.DocumentItems.Where(x => !deliveryList.Any(d => d.Id == x.Product.Id)), suDokument, pozManager);  //oprócz dostaw
        }

        if (ChceckObjectChanged(document.Delivery))
          UpdateDocumentDelivery(document, suDokument, pozManager, deliveryList);

        int khId = suDokument.KontrahentId;

        if (document.Payment != null)
          SetPayment(document, suDokument);

        string uwagi = suDokument.Uwagi ?? "";

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
        
        suDokument.Zamknij();
        result.DocumentId = document.Id;
        result.DocumentNumber = document.FullNumber;

        if (document.IsDeleted == false)
          SetPersonelId(document.Id, operatorId);

        if (document.IsNew)
        {
          SetIFx_ApiDokumentyZapisane(document);
        }

        var kp = dbContext.NzFinanses.AsNoTracking().FirstOrDefault(x => x.NzfTyp == 17 && x.NzfIdDokumentAuto == document.Id);
        if (kp != null)
        {
          result.PayDocumentId = kp.NzfId;
          document.LastCashPaymentDocumentId = kp.NzfId;
        }
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
          Tax = it.twTowar.TwIdVatSpNavigation! != null ? new Tax
          {
            Id = it.twTowar.TwIdVatSpNavigation!.VatId,
            Rate = it.twTowar.TwIdVatSpNavigation!.VatStawka
          } : null
        }).ToList();
        return result;
      }
    }
  }
}
