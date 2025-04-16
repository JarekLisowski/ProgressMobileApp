using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Progress.Infrastructure.Database.Repository;

public class DatabaseContext
{
	protected IConfigurationProvider AutomapperConfiguration { get; }

	protected IMapper Mapper { get; }

	protected DbContext DbContext { get; }

	public DatabaseContext(
			DbContext dbContext,
			IConfigurationProvider automapperConfiguration)
	{
		AutomapperConfiguration = automapperConfiguration;
		Mapper = automapperConfiguration.CreateMapper();
		DbContext = dbContext;
	}
}
