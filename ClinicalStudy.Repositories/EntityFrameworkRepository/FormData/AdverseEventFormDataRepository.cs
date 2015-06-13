using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface.FormData;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository.FormData {
	public class AdverseEventFormDataRepository : GenericFormDataRepository<AdverseEventFormData>,
												  IAdverseEventFormDataRepository {
		public AdverseEventFormDataRepository(IClinicalStudyContextFactory contextFactory) : base(contextFactory) {
		}
												  }
}
