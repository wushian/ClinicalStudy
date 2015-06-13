using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.Repositories.MemoryRepositories {
	public class ChangeNoteRepository : GenericRepository<ChangeNote>, IChangeNoteRepository {
		public ChangeNoteRepository(IDataStorage dataStorage)
			: base(dataStorage) {
		}

		protected internal override void MapEntity(ChangeNote stored, ChangeNote updated) {
			stored.OriginalValue = updated.OriginalValue;
			stored.NewValue = updated.NewValue;
			stored.ChangeReason = updated.ChangeReason;
			stored.ChangeDate = updated.ChangeDate;
		}
	}
}
