using System.Linq;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.Repositories.MemoryRepositories.FormData {
	public class VitalsFormDataRepository : GenericFormDataRepository<VitalsFormData>, IVitalsFormDataRepository {
		public VitalsFormDataRepository(IDataStorage dataStorage)
			: base(dataStorage) {
		}

		protected internal override void MapEntity(VitalsFormData stored, VitalsFormData updated) {
			UpdateQuestionData(stored.ActualTime, updated.ActualTime);
			UpdateQuestionData(stored.Height, updated.Height);
			UpdateQuestionData(stored.Weight, updated.Weight);
			UpdateQuestionData(stored.Temperature, updated.Temperature);
			UpdateQuestionData(stored.HeartRate, updated.HeartRate);
			UpdateQuestionData(stored.BloodPressureSystolic, updated.BloodPressureSystolic);
			UpdateQuestionData(stored.BloodPressureDiastolic, updated.BloodPressureDiastolic);

			base.MapEntity(stored, updated);
		}
	}
}
