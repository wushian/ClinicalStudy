using System.Collections.Generic;

namespace ClinicalStudy.Repositories.MemoryRepositories.DataStorage {
	public interface IDataStorage {
		IList<T> GetData<T>();
	}
}
