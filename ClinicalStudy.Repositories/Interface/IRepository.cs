using System;
using System.Linq;
using System.Linq.Expressions;
using ClinicalStudy.DomainModel;

namespace ClinicalStudy.Repositories.Interface {
	public interface IRepository<T> where T : BaseEntity {
		IQueryable<T> GetAll();
		T GetByKey(int id);
		IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
		void Add(T entity);
		void Delete(T entity);
		void Edit(T entity);
		void Save();
	}
}
