using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Progress.Database;
using Progress.Domain.Navireo;
using Progress.Navireo.Extensions;
using Progress.Navireo.Helpers;
using System.Data;
using System.Runtime.InteropServices;

namespace Progress.Navireo.Managers
{
  public class FinanceManager
  {

    Database.NavireoDbContext dbContext;

    Logger Logger;

    Navireo.NavireoApplication navireoApplication;

    InsERT.Navireo? NavireoInstance => navireoApplication.GetNavireo();

    public FinanceManager(Database.NavireoDbContext dbContext, Logger logger, Navireo.NavireoApplication navireoApplication)
    {
      this.dbContext = dbContext;
      Logger = logger;
      this.navireoApplication = navireoApplication;
    }
    /// <summary>
    /// Pobiera podsumowanie sprzedaży dla użytkownika w podanym zakresie dat
    /// </summary>
    /// <param name="operatorId">Is użytkownika</param>
    /// <param name="dateFrom">Data od</param>
    /// <param name="dateTo">Data do</param>
    /// <param name="currencyCode">Waluta</param>
    /// <returns>Podsumowanie sprzedaży</returns>
    public SaleSummary GetSaleSummary(int operatorId, DateTime dateFrom, DateTime dateTo, string currencyCode)
    {
      SaleSummary summary = new SaleSummary();
      var pd_Uzytkownik = dbContext.PdUzytkowniks.FirstOrDefault(x => x.UzId == operatorId);
      if (pd_Uzytkownik == null)
        throw new Exception(string.Format("Brak użytkownika o Id: {0}", operatorId));
      if (pd_Uzytkownik.UzIdKasy == null)
        throw new Exception(string.Format("Uzytkownik: {0} {1} nie ma przypisanej kasy", pd_Uzytkownik.UzImie, pd_Uzytkownik.UzNazwisko));
      int kasaId = (int)pd_Uzytkownik.UzIdKasy;

      summary.CurrentCash = GetCurrentCashAmount(kasaId);

      //Podsumowanie każdego z typu dokumentu
      var documentSummaryGroupedByType = dbContext.DokDokuments.Where(x => x.DokPersonelId == operatorId && x.DokDataWyst >= dateFrom && x.DokDataWyst <= dateTo)
              .GroupBy(x => x.DokTyp)
              .Select(it => new
              {
                Typ = it.Key,
                DocumentSummary = new DocumentSummary
                {
                  Count = it.Count(),
                  TotalNet = it.Sum(y => y.DokWartNetto),
                  TotalGross = it.Sum(y => y.DokWartBrutto),
                  TotalCashGross = it.Sum(y => y.DokKwGotowka ?? 0)
                }
              }).ToList();

      int invoice = DocumentEnum.SalesInvoice.TypDokumentu();
      int receipt = DocumentEnum.Receipt.TypDokumentu();
      summary.SaleNet = documentSummaryGroupedByType.Where(x => x.Typ == invoice || x.Typ == receipt).Sum(x => x.DocumentSummary.TotalNet);
      summary.SaleGross = documentSummaryGroupedByType.Where(x => x.Typ == invoice || x.Typ == receipt).Sum(x => x.DocumentSummary.TotalGross);
      summary.SaleCashGross = documentSummaryGroupedByType.Where(x => x.Typ == invoice || x.Typ == receipt).Sum(x => x.DocumentSummary.TotalCashGross);

      summary.ReceivedCash = SumaPrzyjetejGotowki(operatorId, dateFrom, dateTo);

      //podsumowanie każdego z dokumentów
      foreach (var docTyp in Constants.Document.DokumentTypList)
      {
        var docSummary = documentSummaryGroupedByType.FirstOrDefault(x => x.Typ == docTyp);
        if (docSummary != null)
        {
          docSummary.DocumentSummary.Type = docTyp.GetDocumentEnumType();
          summary.DocumentSummary.Add(docSummary.DocumentSummary);
        }
        else
        {
          summary.DocumentSummary.Add(new DocumentSummary
          {
            Type = docTyp.GetDocumentEnumType()
          });
        }
      }

      return summary;
    }

    /// <summary>
    /// Pobiera aktualny stan kasy
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="kasaId"></param>
    /// <returns></returns>
    private decimal GetCurrentCashAmount(int kasaId)
    {
      decimal cash = 0;
      var connection = dbContext.Database.GetDbConnection();

      using (var command = connection.CreateCommand())
      {
        // Set the command type to StoredProcedure
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "spFinanse_Kasa_PodajStan";

        // Add input parameters
        command.Parameters.Add(new SqlParameter("@IdKasy", SqlDbType.Int) { Value = kasaId });
        command.Parameters.Add(new SqlParameter("@NaDzien", SqlDbType.DateTime) { Value = DateTime.Today.AddDays(1) });
        command.Parameters.Add(new SqlParameter("@Zakres", SqlDbType.Int) { Value = 1 });
        command.Parameters.Add(new SqlParameter("@Waluta", SqlDbType.Char, 3) { Value = "PLN" });

        // Add the output parameter
        var outputParam = new SqlParameter("@Saldo", SqlDbType.Decimal)
        {
          Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outputParam);

        try
        {
          // Execute the stored procedure. Since it doesn't return rows, ExecuteNonQuery is appropriate.
          command.ExecuteNonQuery();

          // Retrieve the output parameter value
          if (outputParam.Value != DBNull.Value)
          {
            cash = (decimal)outputParam.Value;
          }
        }
        finally
        {
        }
      }

      return cash;
    }

    /// <summary>
    /// Pobiera sumę przyjętej gotówki w podanym zakresie dat
    /// </summary>
    /// <param name="operatorId">Id użytkownika</param>
    /// <param name="dateFrom">Data od</param>
    /// <param name="dateTo">Data do</param>
    /// <param name="dbContext">Połączenie z bazą danych</param>
    /// <returns>Suma przyjętej gotówki</returns>
    private decimal SumaPrzyjetejGotowki(int operatorId, DateTime dateFrom, DateTime dateTo)
    {
      var nzf = dbContext.NzFinanses.Where(x => x.NzfIdWystawil == operatorId && x.NzfTyp == 17 && x.NzfData >= dateFrom && x.NzfData < dateTo);

      return nzf.ToList().Sum(x => x.NzfWartosc);
    }


    /// <summary>
    /// Dodaje rozliczenie dla dokumentu
    /// </summary>
    /// <param name="operatorId">Id użytkownika</param>
    /// <param name="navireoInstance">Instancja Navireo</param>
    /// <param name="settlement">Rozliczenie</param>
    /// <param name="przelew">Czy przelew</param>
    /// <returns></returns>
    public bool SettleDocument(int operatorId, DocumentSettlement settlement, bool przelew = false)
    {
      if (settlement.Value == 0)
        throw new Exception("Brak podanej kwoty TotalGross dokumentu");

      //var result = new List<KeyValuePair<string, int>>();
      InsERT.FinDokumenty naleznosci = null;
      InsERT.FinManager finManager = null;
      try
      {
        int? karta = null;
        if (settlement.PaymentMethod > 0)
        {
          var ifxfp = dbContext.IfxApiFormaPlatnoscis.FirstOrDefault(it => it.Id == settlement.PaymentMethod);
          if (ifxfp != null)
          {
            var fp = dbContext.SlFormaPlatnoscis.FirstOrDefault(it => it.FpId == ifxfp.FpId);
            if (fp != null && fp.FpTerminalPlatniczy && fp.FpAktywna)
            {
              karta = fp.FpId;
            }
          }
        }
        if (settlement.RelatedDocumentId > 0)
        {
          var doc = dbContext.DokDokuments.FirstOrDefault(x => x.DokId == settlement.RelatedDocumentId);
          if (doc == null)
            throw new Exception(string.Format("Brak dokumentu o Id: {0}", settlement.RelatedDocumentId));

          settlement.RelatedDocumentNumber = doc.DokNrPelny;
          settlement.IssueDate = DateTime.Now;

          var pd_Uzytkownik = dbContext.PdUzytkowniks.FirstOrDefault(x => x.UzId == operatorId);
          if (pd_Uzytkownik == null)
            throw new Exception(string.Format("Brak użytkownika o Id: {0}", operatorId));


          finManager = NavireoInstance.FinManager;
          string filtr = string.Format("nzf_Typ=39 and nzf_PodTyp = 1 and nzf_Wartosc>0  and nzf_IdDokumentAuto = {0}", settlement.RelatedDocumentId);
          naleznosci = finManager.OtworzKolekcje(filtr, "nzf_Data");
          if (naleznosci.Liczba == 0)
            throw new Exception(string.Format("Brak należności dla dokumentu o Id: {0}", settlement.RelatedDocumentId));

          foreach (object _oNal in naleznosci)
          {
            InsERT.FinDokument nal = null;
            try
            {
              nal = (InsERT.FinDokument)_oNal;
              if (nal.WartoscBiezaca < settlement.Value)
                throw new Exception(string.Format("Wartość rozliczenia: {1} jest wyższa od kwoty należności: {0}", nal.WartoscBiezaca, settlement.Value));
              int wplataId = RozliczNaleznosc(finManager, nal, settlement.Number, settlement.Value, settlement.IssueDate, (int)pd_Uzytkownik.UzIdKasy, przelew, karta);
              SetWystawil(wplataId, pd_Uzytkownik);
              //result.Add(new KeyValuePair<string, int>(settlement.GUID, wplataId));

            }
            finally
            {
              if (nal != null) Marshal.ReleaseComObject(nal);
              nal = null;
            }
          }
          return true;
        }
        else
        {
          // Wpłata KP
          InsERT.FinDokument kp = null;
          try
          {
            var pd_Uzytkownik = dbContext.PdUzytkowniks.FirstOrDefault(x => x.UzId == operatorId);
            if (pd_Uzytkownik == null)
              throw new Exception(string.Format("Brak użytkownika o Id: {0}", operatorId));
            finManager = NavireoInstance.FinManager;
            kp = finManager.DodajDokumentKasowy(InsERT.DokFinTypEnum.gtaDokFinTypKP, (int)pd_Uzytkownik.UzIdKasy);
            kp.Tytulem = settlement.Title;
            kp.WartoscPoczatkowaWaluta = settlement.Value;
            kp.ObiektPowiazanyWstaw(InsERT.DokFinObiektTypEnum.gtaDokFinObiektKontrahent, settlement.Buyer.Id);
            kp.Data = DateTime.Now;
            kp.Zapisz();
            int wplataId = kp.Identyfikator;
            SetWystawil(wplataId, pd_Uzytkownik);
            return true;
          }
          finally
          {
            if (kp != null)
              Marshal.ReleaseComObject(kp);
            kp = null;
          }

        }
      }
      finally
      {
        if (naleznosci != null) Marshal.ReleaseComObject(naleznosci);
        naleznosci = null;
        if (finManager != null) Marshal.ReleaseComObject(finManager);
        finManager = null;
      }
    }

    /// <summary>
    /// Sprawdza czy istnieje dokument KP o podanym numerze w kasie
    /// </summary>
    /// <param name="numer">Numer KP</param>
    /// <param name="idKasy">Id kasy</param>
    /// <param name="dbContext">Połączenie z bazą</param>
    /// <returns>Istnieje / Nie istnieje</returns>
    //private bool KPNumerCzyIstnieje(string numerPelny, int numer, int idKasy, Model.NavireoEntities dbContext)
    //{
    //  //var kp = dbContext.nz__Finanse.FirstOrDefault(x => x.nzf_Typ == 17 && x.nzf_Podtyp == null && x.nzf_Numer == numer && x.nzf_IdKasy == idKasy && x.nzf_Data.Year == DateTime.Now.Year);            
    //  //return kp != null;
    //  var exists = dbContext.nz__Finanse.Any(x => x.nzf_Typ == 17 && x.nzf_IdKasy == idKasy && x.nzf_NumerPelny == numerPelny);
    //  return exists;
    //}

    ///// <summary>
    ///// Ustawie numer KP
    ///// </summary>
    ///// <param name="id"></param>
    ///// <param name="numer"></param>
    ///// <param name="dbContext"></param>
    //private void KPUstawNumer(int id, int numer, int userId, Model.NavireoEntities dbContext)
    //{
    //  var kp = dbContext.nz__Finanse.FirstOrDefault(x => x.nzf_Id == id && x.nzf_Typ == 17 && x.nzf_Podtyp == null);
    //  if (kp == null)
    //    throw new Exception(string.Format("Nie udało się ustawić numeru: {0} dla KP o Id: {1}", numer, id));
    //  var commonManager = new CommonManager();
    //  var documentNumberFormat = commonManager.GetDocumentsNumerationFormat(userId).FirstOrDefault(x => x.Key == DocumentEnum.DocumentSettlement);
    //  if (documentNumberFormat != null)
    //  {
    //    var numerPelny = documentNumberFormat.Value.Replace("{numer}", Convert.ToString(numer));
    //    numerPelny = numerPelny.Replace("{rok}", Convert.ToString(DateTime.Now.Year));
    //    kp.nzf_Numer = numer;
    //    kp.nzf_NumerPelny = numerPelny;
    //    dbContext.SaveChanges();
    //  }
    //  else
    //    throw new Exception("Brak formatu numeracji dla dokumentów KP");

    //}

    /// <summary>
    /// Rozlicza dokument - dodaje wpłatę
    /// </summary>
    /// <returns></returns>
    private static int RozliczNaleznosc(InsERT.FinManager finManager, InsERT.FinDokument nal, int numer, decimal kwotaSplaty, DateTime data, int kasaId, bool przelew = true, int? fpKartaId = null)
    {
      InsERT.FinDokument wplata = null;
      InsERT.FinRozliczenie rozliczenie = null;
      InsERT.FinCesja finCesja = null;
      try
      {
        if (fpKartaId != null)
        {
          finCesja = nal.Rozliczenia.UtworzCesje();
          finCesja.KartaId = fpKartaId.Value;
          finCesja.DataRozrachunku = DateTime.Today;
          finCesja.KwotaSplaty = kwotaSplaty;
          nal.Rozliczenia.RozliczKarta(finCesja);
          finCesja.ZapiszCesje();
          nal.Zapisz();
          return 0;
        }
        else
        {
          if (przelew)
            wplata = finManager.DodajOperacjeBankowa(InsERT.DokFinTypEnum.gtaDokFinTypBP, 1);
          else
            wplata = finManager.DodajDokumentKasowy(InsERT.DokFinTypEnum.gtaDokFinTypKP, kasaId);
          wplata.ObiektPowiazanyWstaw(InsERT.DokFinObiektTypEnum.gtaDokFinObiektKontrahent, nal.ObiektPowiazanyId);
          wplata.WartoscPoczatkowaWaluta = kwotaSplaty; 
          wplata.Data = data;
          rozliczenie = wplata.Rozliczenia.Rozlicz(nal, wplata.WartoscPoczatkowaWaluta);
          wplata.Tytulem = wplata.GenerujTytul();
          wplata.Zapisz();
          return wplata.Identyfikator;
        }
      }
      finally
      {
        if (wplata != null) Marshal.ReleaseComObject(wplata);
        wplata = null;
        if (rozliczenie != null) Marshal.ReleaseComObject(rozliczenie);
        rozliczenie = null;
      }
    }

    /// <summary>
    /// Zwraca dokumenty nierozliczone
    /// </summary>
    /// <param name="listSelector"></param>
    /// <param name="operatorId"></param>
    /// <returns></returns>
    //public ListAfterPagination<CommerceDocumentBase> GetCommerceDocumentList(FinanceDocumentListSelector listSelector, int userId)
    //{
    //  if (listSelector == null)
    //    listSelector = new FinanceDocumentListSelector();

    //  using (var dbConnection = new Model.NavireoEntities())
    //  {
    //    var ifx_user = dbConnection.IFx_ApiUzytkownik.FirstOrDefault(x => x.uz_Id == userId);
    //    if (ifx_user == null)
    //      return new ListAfterPagination<CommerceDocumentBase>();

    //    IQueryable<Model.IF_vwDokument> query = null;
    //    if (ifx_user.cecha_Id == null)
    //      query = (from dok in dbConnection.IF_vwDokument
    //               where dok.dok_Status != 2
    //               select dok).AsQueryable();
    //    else
    //    {
    //      query = (from khCechy in dbConnection.kh_CechaKh
    //               join dok in dbConnection.IF_vwDokument on new { khId = khCechy.ck_IdKhnt, cechaId = khCechy.ck_IdCecha }
    //               equals new { khId = (int)dok.dok_PlatnikId, cechaId = (int)ifx_user.cecha_Id }
    //               where dok.dok_Status != 2
    //               select dok).AsQueryable();
    //    }

    //    query = FilterDocumentList(listSelector, query);

    //    query = query.Distinct();

    //    query = SetDocumentSortingList(listSelector.Sorting, query);

    //    long totalRecordCount = query.Count();


    //    query = Helpers.QueryHelper<Model.IF_vwDokument>.SetPagination(query, listSelector);

    //    var sellerAddress = NavireoKontrahent.Sprzedawca(dbConnection);

    //    List<CommerceDocumentBase> result = new List<CommerceDocumentBase>();
    //    foreach (var item in query)
    //    {
    //      var customerAddress = Helpers.NavireoKontrahent.GetKontrahentAdrH(item);
    //      var document = new CommerceDocumentBase
    //      {
    //        Id = item.dok_Id,
    //        DocumentType = item.DocumentEnumType(),
    //        Completed = item.dok_Status == 8,
    //        Number = item.dok_Nr ?? 0,
    //        FullNumber = item.dok_NrPelny,
    //        ToPay = item.nzf_WartoscDoZaplaty ?? 0,
    //        IssueDate = (DateTime)item.DataWystawienia,
    //        DeliveryDate = item.dok_DataZakonczenia,
    //        PaymentDeadline = item.dok_PlatTermin,
    //        TotalNet = item.dok_WartNetto,
    //        TotalGross = item.dok_WartBrutto,
    //        TotalTax = item.dok_WartVat,
    //        PaidCashGross = (item.dok_KwWartosc ?? 0) - (item.nzf_WartoscDoZaplaty ?? 0),
    //        Currency = new Currency
    //        {
    //          Code = item.dok_Waluta,
    //          Ratio = item.dok_WalutaKurs
    //        },
    //        Comment = item.dok_Uwagi,
    //        RelatedDocumentId = item.dok_Typ == 16 ? item.DokPowiazanyId ?? 0 : item.dok_DoDokId ?? 0,
    //        RelatedDocumentNumber = item.dok_Typ == 16 ? item.DokPowiazanyNumer : item.dok_DoDokNrPelny,
    //      };

    //      document.BuyerAddress = customerAddress;
    //      if (customerAddress != null)
    //      {
    //        document.BuyerAddress.Id = item.dok_PlatnikId ?? 0;
    //      }
    //      document.SellerAddress = sellerAddress;

    //      if (document.TotalNet < 0)
    //        continue;

    //      if (document.DocumentType == DocumentEnum.SalesInvoiceCorrection)
    //        document.DocumentType = DocumentEnum.SalesInvoice;

    //      result.Add(document);
    //    }
    //    return new ListAfterPagination<CommerceDocumentBase>(result, totalRecordCount);
    //  }
    //}

    /// <summary>
    /// Filtruje listę dokumentów
    /// </summary>
    /// <param name="listSelector">Parametr określający filtrowanie listy</param>
    /// <param name="item">Query</param>
    /// <returns>Query</returns>
    //private static IQueryable<Model.IF_vwDokument> FilterDocumentList(FinanceDocumentListSelector listSelector, IQueryable<Model.IF_vwDokument> query)
    //{
    //  int invoice = DocumentEnum.SalesInvoice.TypDokumentu();
    //  int receipt = DocumentEnum.Receipt.TypDokumentu();
    //  int invoiceCorrection = DocumentEnum.SalesInvoiceCorrection.TypDokumentu();
    //  int salesInvoiceCorrection = DocumentEnum.PurchaseInvoiceCorrection.TypDokumentu();
    //  query = query.Where(x => x.dok_Typ == invoice || x.dok_Typ == receipt || x.dok_Typ == invoiceCorrection || x.dok_Typ == salesInvoiceCorrection);

    //  query = query.Where(x => x.dok_WartBrutto >= 0);

    //  if (listSelector.BuyerId != null)
    //    query = query.Where(x => x.dok_PlatnikId == listSelector.BuyerId);

    //  if (!string.IsNullOrEmpty(listSelector.BuyerCode))
    //    query = query.Where(x => x.adrh_Symbol == listSelector.BuyerCode);

    //  if (listSelector.Settled != null)
    //  {
    //    if (listSelector.Settled == false)
    //      query = query.Where(x => x.nzf_WartoscDoZaplaty > 0);
    //    else
    //      query = query.Where(x => x.nzf_WartoscDoZaplaty == null || x.nzf_WartoscDoZaplaty == 0);
    //  }
    //  return query;
    //}

    /// <summary>
    /// Ustawia sortowanie listy dokumentów
    /// </summary>
    /// <param name="sorting">Sortowanie</param>
    /// <param name="item">Query</param>
    /// <returns>Query</returns>
    //private static IQueryable<Model.IF_vwDokument> SetDocumentSortingList(DocumentSorting sorting, IQueryable<Model.IF_vwDokument> query)
    //{
    //  switch (sorting)
    //  {
    //    default:
    //      {
    //        return query = query.OrderByDescending(x => x.DataWystawienia);
    //      }
    //    case DocumentSorting.DocumentNumberASC:
    //      {
    //        return query = query.OrderBy(x => x.dok_NrPelny);
    //      }
    //    case DocumentSorting.DocumentNumberDESC:
    //      {
    //        return query = query.OrderBy(x => x.dok_NrPelny);
    //      }
    //    case DocumentSorting.IssueDateASC:
    //      {
    //        return query = query.OrderBy(x => x.DataWystawienia);
    //      }
    //    case DocumentSorting.IssueDateDESC:
    //      {
    //        return query = query.OrderBy(x => x.DataWystawienia);
    //      }
    //    case DocumentSorting.TotalPriceASC:
    //      {
    //        return query = query.OrderBy(x => x.dok_WartBrutto);
    //      }
    //    case DocumentSorting.TotalPriceDESC:
    //      {
    //        return query = query.OrderBy(x => x.dok_WartBrutto);
    //      }
    //    case DocumentSorting.ToPayASC:
    //      {
    //        return query = query.OrderBy(x => x.nzf_WartoscDoZaplaty);
    //      }
    //    case DocumentSorting.ToPayDESC:
    //      {
    //        return query = query.OrderBy(x => x.nzf_WartoscDoZaplaty);
    //      }
    //  }
    //}

    /// <summary>
    /// Zwraca rozliczenia dla dokumentu
    /// </summary>
    /// <param name="documentId">Id dokumentu dla którego zwrócić rozliczenia</param>
    /// <returns>Rozliczenia</returns>
    //public List<DocumentSettlement> GetSettlementList(int documentId)
    //{
    //  var result = new List<DocumentSettlement>();

    //  using (var dbContext = new Model.NavireoEntities())
    //  {
    //    var dok = dbContext.dok__Dokument.FirstOrDefault(x => x.dok_Id == documentId);
    //    if (dok == null)
    //      return result;

    //    var nzf = dbContext.nz__Finanse.FirstOrDefault(x => x.nzf_IdDokumentAuto == documentId && x.nzf_Typ == 39 && x.nzf_Podtyp == 1);
    //    if (nzf == null)
    //      return result;

    //    var seller = Helpers.NavireoKontrahent.Sprzedawca(dbContext);
    //    var buyer = Helpers.NavireoKontrahent.GetKontrahentAdr(dok.dok_PlatnikId ?? 0, dbContext);

    //    var rozliczenia = dbContext.vwFinanseRozSplatyRozliczenia.Where(x => x.nzf_Typ == 17 && (x.nzs_IdDlugu == nzf.nzf_Id || x.nzs_IdSplaty == nzf.nzf_Id) && x.nzf_Id != nzf.nzf_Id).ToList();

    //    return rozliczenia.OrderByDescending(x => x.nzf_Data).ThenBy(x => x.nzf_Id).Select(it => new DocumentSettlement
    //    {
    //      Id = it.nzs_Id,
    //      RelatedDocumentId = dok.dok_Id,
    //      RelatedDocumentNumber = dok.dok_NrPelny,
    //      FullNumber = it.nzf_NumerPelny,
    //      IssueDate = it.nzf_Data,
    //      Value = it.nzs_WartoscWaluta,
    //      Title = it.nzf_Tytulem,
    //      FromPerson = it.nzf_Wystawil,
    //      ToPerson = it.nzf_Wystawil, //pewnie do poprawy
    //      Buyer = buyer,
    //      Seller = seller
    //    }).ToList();
    //  }
    //}

    //public DocumentSettlement GetSettlement(int dokKasId)
    //{
    //  using (var dbContext = new Model.NavireoEntities())
    //  {
    //    var dok = dbContext.vwFinanseDokumentKasowy.Where(it => it.dks_Id == dokKasId).FirstOrDefault();
    //    if (dok != null)
    //    {
    //      var seller = Helpers.NavireoKontrahent.Sprzedawca(dbContext);
    //      var buyer = Helpers.NavireoKontrahent.GetKontrahentAdr(dok.dks_IdObiektu ?? 0, dbContext);
    //      return new DocumentSettlement
    //      {
    //        Id = dok.dks_Id,
    //        FullNumber = dok.dks_NumerPelny,
    //        IssueDate = dok.dks_Data,
    //        Value = dok.dks_Wartosc,
    //        Title = dok.dks_Tytulem,
    //        FromPerson = dok.dks_Wystawil,
    //        Buyer = buyer,
    //        Seller = seller
    //      };
    //    }
    //    return null;
    //  }
    //}

    /// <summary>
    /// Ustawia osobę wystawiającą wpłatę
    /// </summary>
    /// <param name="wplataId">Id wpłaty</param>
    /// <param name="pd_Uzytkownik">Model.pd_Uzytkownik</param>
    /// <param name="dbContext">Połączenie z bazą danych</param>
    private void SetWystawil(int wplataId, PdUzytkownik pd_Uzytkownik)
    {
      var wplacil = pd_Uzytkownik.UzImie + " " + pd_Uzytkownik.UzNazwisko;
      var wplata = dbContext.NzFinanses.FirstOrDefault(x => x.NzfId == wplataId);
      if (wplata == null)
        Logger.Log(LogType.Exception, string.Format("Nie udało się ustawić wystawiającego: {0} dla wpłaty o Id: {1}", wplacil, wplata), pd_Uzytkownik.UzId);
      else
      {
        wplata.NzfIdWystawil = pd_Uzytkownik.UzId;
        wplata.NzfWystawil = wplacil;
        dbContext.SaveChanges();
      }
    }
  }
}
