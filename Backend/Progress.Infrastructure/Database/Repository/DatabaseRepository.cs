using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Progress.Database;
using System.Linq.Expressions;

namespace Progress.Infrastructure.Database.Repository
{
	public class DatabaseRepository<T_MODEL, T_ENTITY> : DatabaseContext, IDatabaseRepository<T_MODEL, T_ENTITY>
		where T_ENTITY : class
		where T_MODEL : class
	{

		private Func<T_ENTITY, int> _entityKey;
		private Func<T_MODEL, int> _modelKey;

		string _keyName;

		public DbSet<T_ENTITY> EntitySet
		{
			get => DbContext.Set<T_ENTITY>();
		}

		public new NavireoDbContext DbContext 
		{
			get => (NavireoDbContext)base.DbContext;
		}

		public DatabaseRepository(NavireoDbContext dbContext, IConfigurationProvider automapperConfiguration,
			string keyName, Func<T_ENTITY, int> entityKey, Func<T_MODEL, int> modelKey)
				: base(dbContext, automapperConfiguration)
		{
			_modelKey = modelKey;
			_entityKey = entityKey;
			_keyName = keyName;
		}

		protected T_ENTITY ToEntity(T_MODEL model)
				=> Mapper.Map<T_ENTITY>(model);

		protected T_MODEL ToModel(T_ENTITY model)
		=> Mapper.Map<T_MODEL>(model);

		protected int GetKey(T_MODEL model) => _modelKey(model);
		protected int GetKey(T_ENTITY model) => _entityKey(model);

		public T_ENTITY Add(T_MODEL model)
		{
			var entity = ToEntity(model);
			DbContext.Add(entity);
			return entity;
		}

		public T_ENTITY[] Add(T_MODEL[] model)
		{
			var entities = new List<T_ENTITY>();
			foreach (var item in model)
			{
				var entity = ToEntity(item);
				entities.Add(entity);
				DbContext.Add(entity);
			}
			return entities.ToArray();
		}

		protected bool AreEqual(T_ENTITY entity, T_MODEL model)
			=> _entityKey(entity) == _modelKey(model);

		public void Update(T_MODEL model)
		{
			var existing = DbContext.ChangeTracker.Entries<T_ENTITY>().FirstOrDefault(it => AreEqual(it.Entity, model));
			if (existing != null)
			{
				existing.State = EntityState.Detached;
			}
			var entity = ToEntity(model);
			DbContext.Update(entity);
		}

		private Expression<Func<T_ENTITY, bool>> GetKeyConditionExpression(int id)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T_ENTITY));
			var propertyAccessor = Expression.PropertyOrField(parameterExpression, _keyName);
			ConstantExpression valueExpression = Expression.Constant(id);
			var porownanie = Expression.Equal(propertyAccessor, valueExpression);
			var lambdaExpression = Expression.Lambda<Func<T_ENTITY, bool>>(porownanie, false, parameterExpression);
			return lambdaExpression;
		}

		public T_ENTITY? SelectEntity(int id)
		{
			var lambdaExpression = GetKeyConditionExpression(id);
			var entity = DbContext.Set<T_ENTITY>().FirstOrDefault(lambdaExpression);
			return entity;
		}

		public T_MODEL? Select(int id)
		{
			var entity = SelectEntity(id);
			if (entity == null)
				return null;
			return ToModel(entity);
		}

		public IEnumerable<T_MODEL> SelectWhere(Expression<Func<T_ENTITY, bool>> expression)
		{
			return EntitySet.Where(expression)
					.ProjectTo<T_MODEL>(AutomapperConfiguration).ToArray();
		}

		//public void AddOrUpdate(T_MODEL model)
		//{
		//	var existing = Select(model).FirstOrDefault();
		//	if (existing != null)
		//	{
		//		DbContext.Entry(existing).State = EntityState.Detached;
		//		Update(model);
		//	}
		//	else
		//	{
		//		Add(model);
		//	}
		//}

		public void Delete(T_MODEL model)
		{
			Delete(GetKey(model));
		}

		public void Delete(int id)
		{
			var lambdaExpression = GetKeyConditionExpression(id);
			DeleteWhere(lambdaExpression);
		}

		public void DeleteWhere(Expression<Func<T_ENTITY, bool>> expression)
		{
			EntitySet.Where(expression).ExecuteDelete();
		}



	}
}
