using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Progress.Database;
using Progress.Domain.Model;

namespace Progress.Infrastructure.Database.Repository
{
  public class FinanceRepository : DatabaseRepository<CashReceipt, NzFinanse>
  {
    public FinanceRepository(NavireoDbContext dbContext,
                              IConfigurationProvider automapperConfiguration)
      : base(dbContext, automapperConfiguration, nameof(NzFinanse.NzfId), x => x.NzfId, x => x.Id)
    {
    }

    public CashReceipt? GetCashReceipt(int id)
    {
      var nz = EntitySet.Include(x => x.NzfIdAdresuNavigation).FirstOrDefault(x => x.NzfId == id);
      if (nz != null)
      {
        var result = Mapper.Map<CashReceipt>(nz);
        return result;
      }
      return null;
    }
  }
}
