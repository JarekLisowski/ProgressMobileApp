using Microsoft.Extensions.DependencyInjection;
using DbModel = Progress.Database;
using Model = Progress.Domain.Model;

namespace Progress.Infrastructure.Database.Repository
{
	public static class DataRepositories
	{
		public static IServiceCollection RegisterRepositories(this IServiceCollection services)
		{
			AddRepositories(services);
			return services;
		}

		public static void AddRepositories(IServiceCollection services)
		{
			services.AddScoped<ProductRepository>();
			services.AddScoped<PromoRepository>();
			services.AddScoped<CustomerRepository>();
			services.AddScoped<UserRepository>();
			services.AddScoped(sp => MakeRepository<Model.Product, DbModel.TwTowar>(sp, nameof(DbModel.TwTowar.TwId), x => x.TwId, x => x.Id));
			services.AddScoped(sp => MakeRepository<Model.ProductImage, DbModel.TwZdjecieTw>(sp, nameof(DbModel.TwZdjecieTw.ZdId), x => x.ZdId, x => x.Id));
			services.AddScoped(sp => MakeRepository<Model.ProductCategory, DbModel.SlCechaTw>(sp, nameof(DbModel.SlCechaTw.CtwId), x => x.CtwId, x => x.Id));
			services.AddScoped(sp => MakeRepository<Model.ProductCategory, DbModel.TwCechaTw>(sp, nameof(DbModel.TwCechaTw.ChtIdCecha), x => x.ChtIdCecha, x => x.Id));
			services.AddScoped(sp => MakeRepository<Model.PromoItem, DbModel.IfxApiPromocjaPozycja>(sp, nameof(DbModel.IfxApiPromocjaPozycja.Id), x => x.Id, x => x.Id));
			services.AddScoped(sp => MakeRepository<Model.Customer, DbModel.IfVwKontrahent>(sp, nameof(DbModel.IfVwKontrahent.KhId), x => x.Id, x => x.Id));
			services.AddScoped(sp => MakeRepository<Model.PaymentMethod, DbModel.IfxApiFormaPlatnosci>(sp, nameof(DbModel.IfxApiFormaPlatnosci.Id), x => x.Id, x => x.Id));
			services.AddScoped(sp => MakeRepository<Model.DeliveryMethod, DbModel.IfxApiSposobDostawy>(sp, nameof(DbModel.IfxApiSposobDostawy.Id), x => x.Id, x => x.Id));

    }

    private static IDatabaseRepository<TM, TE> MakeRepository<TM, TE>(IServiceProvider sp, string keyName, Func<TE, int> entityKey, Func<TM, int> modelKey)
			where TE : class where TM : class
		{
			return new DatabaseRepository<TM, TE>(sp.GetRequiredService<DbModel.NavireoDbContext>(), sp.GetRequiredService<global::AutoMapper.IConfigurationProvider>(), keyName, entityKey, modelKey);
		}
	}
}
