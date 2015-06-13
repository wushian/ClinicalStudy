using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface.FormData;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository.FormData {
	public class ElectrocardiogramFormDataRepository : GenericFormDataRepository<ElectrocardiogramFormData>,
													   IElectrocardiogramFormDataRepository {
		public ElectrocardiogramFormDataRepository(IClinicalStudyContextFactory contextFactory)
			: base(contextFactory) {
		}
	}
}
