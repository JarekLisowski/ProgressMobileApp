using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Progress.Database;
using Progress.Domain.Model;
using System.Data.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Progress.Infrastructure.Database.Repository
{
  public class DocumentRepository : DatabaseRepository<Document, IfVwDokument>
  {
    private IDatabaseRepository<DocumentItem, DokPozycja> _dokItemRepository;

    public DocumentRepository(NavireoDbContext dbContext,
                              IConfigurationProvider automapperConfiguration,
                              IDatabaseRepository<DocumentItem, DokPozycja> dokItemRepository)
      : base(dbContext, automapperConfiguration, nameof(IfVwDokument.DokId), x => x.DokId, x => x.Id ?? 0)
    {
      _dokItemRepository = dokItemRepository;
    }

    public Document[] GetDocuments(int dokType, int? customerId, DateTime from, DateTime to, int? statusZK)
    {
      bool wszystkie;
      List<int> statusy;
      GetZKStatusList(statusZK, out wszystkie, out statusy);
      var data = EntitySet.AsNoTracking()
        .Where(it => it.DokPlatnikId == customerId && it.DokTyp == dokType && it.DokDataWyst >= from && it.DokDataWyst <= to && (wszystkie || statusy.Contains(it.StatusReal)))
        .OrderByDescending(it => it.DokDataWyst)
        .ThenByDescending(it => it.DokId)
        .ToArray();
      if (data != null)
      {
        var result = Mapper.Map<Document[]>(data);
        return result;
      }
      return [];
    }

    //statusZK: null - wszystkie, 1 - niezrealizowane, 2 - zrealizowane
    public Document[] GetDocumentsOwnCustomers(int dokType, int userCechaKhId, DateTime fromDate, DateTime toDate, int? statusZK)
    {
      bool wszystkie;
      List<int> statusy;
      GetZKStatusList(statusZK, out wszystkie, out statusy);

      var data = (from khCechy in DbContext.KhCechaKhs.AsNoTracking()
                  join dok in DbContext.IfVwDokuments.AsNoTracking() on new { khId = khCechy.CkIdKhnt, cechaId = khCechy.CkIdCecha } equals new { khId = dok.DokPlatnikId ?? 0, cechaId = userCechaKhId }
                  where dok.DokTyp == dokType && dok.DokStatus != 2 && dok.DokDataWyst >= fromDate && dok.DokDataWyst <= toDate && (wszystkie || statusy.Contains(dok.StatusReal))
                  select dok)
                    .OrderByDescending(it => it.DokDataWyst)
                    .ThenByDescending(it => it.DokId)
                    .ToArray();
      if (data != null)
      {
        var result = Mapper.Map<Document[]>(data);
        return result;
      }
      return [];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="statusZK">null - wszystkie, 1 - niezrealizowane, 2 - zrealizowane</param>
    /// <param name="wszystkie"></param>
    /// <param name="statusy"></param>
    private static void GetZKStatusList(int? statusZK, out bool wszystkie, out List<int> statusy)
    {
      wszystkie = false;
      statusy = new List<int>();
      if (statusZK == null || statusZK == 0)
        wszystkie = true;
      else if (statusZK == 1)
        statusy = [0, 1, 3, 4, 5]; //niezrealizowane
      else if (statusZK == 2)
        statusy = [2]; //zrealizowane
    }

    public Document[] GetDocuments(int dokType, int definiowalnyId, int userId)
    {
      var data = EntitySet.AsNoTracking()
        .Where(it => it.DokPersonelId == userId && it.DokTyp == dokType && it.DokDefiniowalnyId == definiowalnyId)
        .OrderByDescending(it => it.DokId)
        .ToArray();
      if (data != null)
      {
        var result = Mapper.Map<Document[]>(data);
        return result;
      }
      return [];
    }

    public Document? GetDocument(int id)
    {
      var document = Select(id);
      if (document != null)
      {
        var items = _dokItemRepository.EntitySet.Include(it => it.ObTow).Where(it => it.ObDokHanId == id).ToArray();
        document.Items = Mapper.Map<DocumentItem[]>(items);
        return document;
      }
      return null;
    }
  }
}
