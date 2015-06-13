using ClinicalStudy.DomainModel.FormData;

namespace ClinicalStudy.Repositories.Interface.FormData {
	public interface IFormDataRepository<T> : IRepository<T> where T : BaseFormData {
		T GetFormDataByFormId(int formId);
	}
}
