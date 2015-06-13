using System.Linq;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface.FormData;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository.FormData {
	public abstract class GenericFormDataRepository<T> : GenericRepository<T>,
	                                                     IFormDataRepository<T>
		where T : BaseFormData {
		protected GenericFormDataRepository(IClinicalStudyContextFactory contextFactory) : base(contextFactory) {
		}

		public T GetFormDataByFormId(int formId) {
			return FindBy(fd => fd.Form.Id == formId).FirstOrDefault();
		}
		}
}
