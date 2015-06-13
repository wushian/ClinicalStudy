using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface.FormData;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository.FormData {
	public class DemographicFormDataRepository : GenericFormDataRepository<DemographicFormData>,
												 IDemographicFormDataRepository {
		public DemographicFormDataRepository(IClinicalStudyContextFactory contextFactory)
			: base(contextFactory) {
		}
	}
}
