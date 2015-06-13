using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface.FormData;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository.FormData {
	public class HappinessFormDataRepository : GenericFormDataRepository<HappinessFormData>, IHappinessFormDataRepository {
		public HappinessFormDataRepository(IClinicalStudyContextFactory contextFactory) : base(contextFactory) {
		}
	}
}
