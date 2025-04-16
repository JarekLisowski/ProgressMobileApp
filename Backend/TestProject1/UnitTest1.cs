using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Progress.Database;
using Progress.Infrastructure.Database;
using Progress.Infrastructure.Database.Repository;
using DbModel = Progress.Database;
using Model = Progress.Domain.Model;

namespace TestProject1
{
	public class Tests
	{
		string connectionString;
		private ServiceProvider _serviceProvider;

		[SetUp]
		public void Setup()
		{
			connectionString = "Server=(local);Database=PH_Progress;Trusted_Connection=True;TrustServerCertificate=True";
			var services = new ServiceCollection();
			services.AddDbContext<NavireoDbContext>(options => options.UseSqlServer(connectionString));
      services.AddAutoMapper(typeof(Progress.Infrastructure.Database.AutoMapper));
			services.RegisterRepositories();
			//services.AddScoped<IServiceA, ServiceA>();
			//services.AddScoped<ClassUnderTest>(); // Zarejestruj równie¿ testowany obiekt
			_serviceProvider = services.BuildServiceProvider();
		}

		[Test]
		public void Test1()
		{
			try
			{
				var twRepo = _serviceProvider.GetRequiredService<IDatabaseRepository<Model.Product, DbModel.TwTowar>>();
				var t1 = twRepo.Select(1);
				var g = twRepo.SelectWhere(it => it.TwIdGrupa == 37);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		[TearDown]
		public void Finish()
		{
			_serviceProvider.Dispose();
		}
	}
}