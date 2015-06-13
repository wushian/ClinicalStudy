using System.Linq;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.Repositories.MemoryRepositories.FormData {
	public class AdverseEventFormDataRepository : GenericFormDataRepository<AdverseEventFormData>,
	                                              IAdverseEventFormDataRepository {
		public AdverseEventFormDataRepository(IDataStorage dataStorage) : base(dataStorage) {
		}

		protected internal override void MapEntity(AdverseEventFormData stored, AdverseEventFormData updated) {
			UpdateQuestionData(stored.AdverseExperience, updated.AdverseExperience);
			UpdateQuestionData(stored.OnsetDate, updated.OnsetDate);
			UpdateQuestionData(stored.OnsetTime, updated.OnsetTime);
			UpdateQuestionData(stored.EndDate, updated.EndDate);
			UpdateQuestionData(stored.EndTime, updated.EndTime);
			UpdateQuestionData(stored.Outcome, updated.Outcome);
			UpdateQuestionData(stored.Intensity, updated.Intensity);
			UpdateQuestionData(stored.RelationshipToInvestigationalDrug, updated.RelationshipToInvestigationalDrug);

			base.MapEntity(stored, updated);
		}
	                                              }
}
