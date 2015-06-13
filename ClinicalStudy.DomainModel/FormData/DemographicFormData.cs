using System.Collections.Generic;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel.FormData {
	public class DemographicFormData : BaseFormData {
		public virtual Question DateOfBirth { get; set; }
		public virtual Question Race { get; set; }
		public virtual Question Sex { get; set; }
		public virtual Question Other { get; set; }
		public override System.Collections.Generic.IEnumerable<Question> AllQuestions {
			get { return new List<Question>{DateOfBirth, Race, Sex, Other}; }
		}
	}
}
