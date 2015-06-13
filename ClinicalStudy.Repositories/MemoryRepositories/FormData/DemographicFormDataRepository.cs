using System.Linq;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.Repositories.MemoryRepositories.FormData {
	public class DemographicFormDataRepository : GenericFormDataRepository<DemographicFormData>,
	                                             IDemographicFormDataRepository {
		public DemographicFormDataRepository(IDataStorage dataStorage) : base(dataStorage) {
		}

		protected internal override void MapEntity(DemographicFormData stored, DemographicFormData updated) {
			UpdateQuestionData(stored.DateOfBirth, updated.DateOfBirth);
			UpdateQuestionData(stored.Race, updated.Race);
			UpdateQuestionData(stored.Sex, updated.Sex);
			UpdateQuestionData(stored.Other, updated.Other);

			base.MapEntity(stored, updated);
		}
	                                             }
}
