using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface.FormData;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository.FormData {
	public class InventoryFormDataRepository : GenericFormDataRepository<InventoryFormData>, IInventoryFormDataRepository {
		public InventoryFormDataRepository(IClinicalStudyContextFactory contextFactory) : base(contextFactory) {
		}
	}
}
