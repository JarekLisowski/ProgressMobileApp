using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Progress.Database;
using Progress.Domain.Model;

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

    public Document[] GetDocuments(int dokType, int? customerId)
    {
      if (customerId != null)
      {
        var data = EntitySet.AsNoTracking()
          .Where(it => it.DokPlatnikId == customerId && it.DokTyp == (int)dokType)
          .OrderByDescending(it => it.DokId)
          .ToArray();
        if (data != null)
        {
          var result = Mapper.Map<Document[]>(data);
          return result;
        }
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
