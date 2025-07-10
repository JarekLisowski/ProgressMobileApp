using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Progress.Infrastructure.Database.Repository
{
	public interface IDatabaseRepository<T_MODEL, T_ENTITY> where T_ENTITY : class where T_MODEL : class
	{
		DbSet<T_ENTITY> EntitySet { get; }
		public T_MODEL? Select(int id);
		public T_ENTITY? SelectEntity(int id);
		T_ENTITY Add(T_MODEL model);
		T_ENTITY[] Add(T_MODEL[] model);
		void Delete(T_MODEL model);
		public void Delete(int id);
		void DeleteWhere(Expression<Func<T_ENTITY, bool>> expression);
		IEnumerable<T_MODEL> SelectWhere(Expression<Func<T_ENTITY, bool>> expression, bool lateConvert = false);
		void Update(T_MODEL model);
	}
}