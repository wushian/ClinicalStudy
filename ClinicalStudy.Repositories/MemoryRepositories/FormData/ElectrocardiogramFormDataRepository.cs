using System.Linq;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.Repositories.MemoryRepositories.FormData {
	public class ElectrocardiogramFormDataRepository : GenericFormDataRepository<ElectrocardiogramFormData>,
	                                                   IElectrocardiogramFormDataRepository {
		public ElectrocardiogramFormDataRepository(IDataStorage dataStorage) : base(dataStorage) {
		}

		protected internal override void MapEntity(ElectrocardiogramFormData stored, ElectrocardiogramFormData updated) {
			UpdateQuestionData(stored.ElectrocardiogramActualTime, updated.ElectrocardiogramActualTime);
			UpdateQuestionData(stored.ElectrocardiogramAttachment, updated.ElectrocardiogramAttachment);

			base.MapEntity(stored, updated);
		}
	                                                   }
}
