using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Progress.Database;
using Progress.Domain.Model;

namespace Progress.Infrastructure.Database.Repository
{
  public class PromoRepository : DatabaseRepository<PromoSet, IfxApiPromocjaZestaw>
  {
    IMapper _mapper;
    DateTime? overrideToday = new DateTime(2025, 4, 1);

    public PromoRepository(NavireoDbContext dbContext, IConfigurationProvider automapperConfiguration, IMapper mapper)
      : base(dbContext, automapperConfiguration, nameof(IfxApiPromocjaZestaw.Id), x => x.Id, x => x.Id)
    {
      _mapper = mapper;
    }

    public IEnumerable<PromoSet> GetPromoSetList()
    {
      var today = overrideToday ?? DateTime.Today;
      var data = EntitySet
        .AsNoTracking()
        .Where(it => it.DataOd <= today && it.DataDo >= today)
        .ToArray();
      
      if (data == null)
        return Array.Empty<PromoSet>();

      var result = _mapper.Map<PromoSet[]>(data);
      return result;
    }

    public PromoSet? GetPromoSet(int id)
    {
      var today = overrideToday ?? DateTime.Today;
      var data = EntitySet
        .AsNoTracking()
        .Include(it => it.IfxApiPromocjaPozycjas)
        .FirstOrDefault(it => it.Id == id && it.DataOd <= today && it.DataDo >= today);      
      if (data == null) 
        return null;
      
      var result = _mapper.Map<PromoSet>(data);
      return result;
    }

  }
}
