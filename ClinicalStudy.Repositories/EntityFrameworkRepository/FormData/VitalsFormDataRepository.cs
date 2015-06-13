using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface.FormData;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository.FormData {
	public class VitalsFormDataRepository : GenericFormDataRepository<VitalsFormData>, IVitalsFormDataRepository {
		public VitalsFormDataRepository(IClinicalStudyContextFactory contextFactory) : base(contextFactory) {
		}
	}
}
