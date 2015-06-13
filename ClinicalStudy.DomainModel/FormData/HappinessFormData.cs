using System.Collections.Generic;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel.FormData {
	public class HappinessFormData : BaseFormData {
		public virtual Question HappinessLevel { get; set; }
		public override System.Collections.Generic.IEnumerable<Question> AllQuestions {
			get {
				return new List<Question>{HappinessLevel};
			}
		}
	}
}
