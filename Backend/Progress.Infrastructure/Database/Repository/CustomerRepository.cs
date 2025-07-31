using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Progress.Database;
using Progress.Domain.Model;
using Progress.Domain.Navireo;

namespace Progress.Infrastructure.Database.Repository
{
  public class CustomerRepository : DatabaseRepository<Customer, KhKontrahent>
  {
    public CustomerRepository(NavireoDbContext dbContext, IConfigurationProvider automapperConfiguration)
      : base(dbContext, automapperConfiguration, nameof(KhKontrahent.KhId), x => x.KhId, x => x.Id)
    {

    }

    public Customer[] GetCustomers(string pattern, int? cechaId, int? limit = null)
    {
      if (cechaId != null)
      {
        var query = from kh in DbContext.IfVwKontrahents
                    join cecha in DbContext.KhCechaKhs on kh.KhId equals cecha.CkIdKhnt
                    where (cecha.CkIdCecha == cechaId) && (pattern == "" || kh.AdrNazwaPelna!.Contains(pattern) || kh.AdrNip!.StartsWith(pattern))
                    select kh;
        if (limit != null)
          query = query.OrderBy(it => it.AdrNazwa).Take(limit.Value);
        var data = query.ToList();
        return Mapper.Map<Customer[]>(data);
      }
      else
      {
        var query = from kh in DbContext.IfVwKontrahents
                    where (pattern == "" || kh.AdrNazwaPelna!.Contains(pattern) || kh.AdrNip!.StartsWith(pattern))
                    select kh;
        if (limit != null)
          query = query.OrderBy(it => it.AdrNazwa).Take(limit.Value);
        var data = query.ToList();
        return Mapper.Map<Customer[]>(data);
      }
    }

    public Customer GetOwnCompany()
    {
      var adrEwid = DbContext.AdrEwids.AsNoTracking().
                    Include(x => x.AdrIdPanstwoNavigation)
                    .Where(x => x.AdrTypAdresu == 8)
                    .FirstOrDefault();
      if (adrEwid != null)
      {
        var rachunek = DbContext.RbRachBankowies.FirstOrDefault(it => it.RbIdObiektu == adrEwid.AdrIdObiektu && it.RbTypObiektu == 0 && it.RbPodstawowy);
        string rachunekNumer = rachunek != null ? rachunek.RbNumer : "";
        var result = Mapper.Map<Customer>(adrEwid);
        result.BankAccount = rachunekNumer;
        return result;
      }
      return new Customer();
    }
  }
}
