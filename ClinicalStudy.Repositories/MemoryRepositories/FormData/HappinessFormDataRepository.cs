using System.Linq;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.Repositories.MemoryRepositories.FormData {
	public class HappinessFormDataRepository : GenericFormDataRepository<HappinessFormData>, IHappinessFormDataRepository {
		public HappinessFormDataRepository(IDataStorage dataStorage) : base(dataStorage) {
		}

		protected internal override void MapEntity(HappinessFormData stored, HappinessFormData updated) {
			UpdateQuestionData(stored.HappinessLevel, updated.HappinessLevel);

			base.MapEntity(stored, updated);
		}
	}
}
