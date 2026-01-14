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

    public Document[] GetDocuments(int dokType, int? customerId, DateTime from, DateTime to)
    {
      var data = EntitySet.AsNoTracking()
        .Where(it => it.DokPlatnikId == customerId && it.DokTyp == dokType && it.DokDataWyst >= from && it.DokDataWyst <= to)
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

    public Document[] GetDocumentsOwnCustomers(int dokType, int userCechaKhId, DateTime fromDate, DateTime toDate)
    {
      var data = (from khCechy in DbContext.KhCechaKhs.AsNoTracking()
                  join dok in DbContext.IfVwDokuments.AsNoTracking() on new { khId = khCechy.CkIdKhnt, cechaId = khCechy.CkIdCecha } equals new { khId = dok.DokPlatnikId ?? 0, cechaId = userCechaKhId }
                  where dok.DokTyp == dokType && dok.DokStatus != 2 && dok.DokDataWyst >= fromDate && dok.DokDataWyst <= toDate
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
