using System;
using System.Linq;
using System.Linq.Expressions;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.Repositories.MemoryRepositories {
	/// <summary>
	/// Generic part for every in-memory repository
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class GenericRepository<T> : IRepository<T>
		where T : BaseEntity {
		protected IDataStorage DataStorage { get; set; }

		public GenericRepository(IDataStorage dataStorage) {
			DataStorage = dataStorage;
		}

		public IQueryable<T> GetAll() {
			return DataStorage.GetData<T>().AsQueryable();
		}

		public T GetByKey(int id) {
			return DataStorage.GetData<T>().Where(e => e.Id == id).SingleOrDefault();
		}

		public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate) {
			return DataStorage.GetData<T>().Where(predicate.Compile()).AsQueryable();
		}

		public void Add(T entity) {
			T existing = GetByKey(entity.Id);
			if (existing != null)
				throw new InvalidOperationException("Entity with the same key already exist!");
			//emulate Identity field in SQL Server
			if (entity.Id == 0) {
				var allStored = GetAll();
				if (allStored.Any())
					entity.Id = allStored.Max(e => e.Id) + 1;
				else
					entity.Id = 1;
			}
			DataStorage.GetData<T>().Add(entity);
		}

		public void Delete(T entity) {
			T existing = GetByKey(entity.Id);
			if (existing == null)
				throw new InvalidOperationException("Entity does not exist");
			DataStorage.GetData<T>().Remove(existing);
		}

		public void Edit(T entity) {
			T existing = GetByKey(entity.Id);
			MapEntity(existing, entity);
		}

		public void Save() {
			//Do nothing
		}

		protected internal abstract void MapEntity(T stored, T updated);
		}
}
