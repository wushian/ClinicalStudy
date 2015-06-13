using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository {
	public abstract class GenericRepository<T> :
		IRepository<T>
		where T : BaseEntity{

		protected readonly IClinicalStudyContextFactory ContextFactory;

		protected GenericRepository(IClinicalStudyContextFactory contextFactory) {
			ContextFactory = contextFactory;
		}

		public virtual IQueryable<T> GetAll() {
			IQueryable<T> query = ContextFactory.Retrieve().Set<T>();
			return query;
		}

		public T GetByKey(int id) {
			var query = GetAll().FirstOrDefault(e => e.Id == id);
			return query;
		}

		public IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate) {
			IQueryable<T> query = ContextFactory.Retrieve().Set<T>().Where(predicate);
			return query;
		}

		public virtual void Add(T entity) {
			ContextFactory.Retrieve().Set<T>().Add(entity);
		}

		public virtual void Delete(T entity) {
			ContextFactory.Retrieve().Set<T>().Remove(entity);
		}

		public virtual void Edit(T entity) {
			var entry =ContextFactory.Retrieve().Entry(entity);
			if(entry.State != EntityState.Added)
				entry.State = EntityState.Modified;
		}

		public virtual void Save() {
			ContextFactory.Retrieve().SaveChanges();
		}
	}
}
