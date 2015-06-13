using System.Collections.Generic;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel.FormData {
	public class AdverseEventFormData : BaseFormData {
		public virtual Question AdverseExperience { get; set; }
		public virtual Question OnsetDate { get; set; }
		public virtual Question OnsetTime { get; set; }
		public virtual Question EndDate { get; set; }
		public virtual Question EndTime { get; set; }
		public virtual Question Outcome { get; set; }
		public virtual Question Intensity { get; set; }
		public virtual Question RelationshipToInvestigationalDrug { get; set; }

		public override System.Collections.Generic.IEnumerable<Question> AllQuestions {
			get {
				return new List<Question> {
					AdverseExperience,
					OnsetDate,
					OnsetTime,
					EndDate,
					EndTime,
					Outcome,
					Intensity,
					RelationshipToInvestigationalDrug
				};
			}
		}
	}
}
