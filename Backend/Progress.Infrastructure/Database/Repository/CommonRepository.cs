using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Progress.Infrastructure.Database.Repository
{
  public class CommonRepository : DatabaseContext
  {
    public CommonRepository(DbContext dbContext, IConfigurationProvider automapperConfiguration)
      : base(dbContext, automapperConfiguration)
    {

    }
  }
}
