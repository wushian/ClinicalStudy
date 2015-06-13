using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.Repositories.MemoryRepositories.FormData {
	public abstract class GenericFormDataRepository<T> : GenericRepository<T>, IFormDataRepository<T>
		where T : BaseFormData {
		protected GenericFormDataRepository(IDataStorage dataStorage)
			: base(dataStorage) {
		}

		protected internal override void MapEntity(T stored, T updated) {
			stored.Form = updated.Form;
		}

		public T GetFormDataByFormId(int formId) {
			return FindBy(fd => fd.Form.Id == formId).FirstOrDefault();
		}

		public void UpdateQuestionData(Question stored, Question updated) {
			stored.Value = updated.Value;
		}
		}
}
