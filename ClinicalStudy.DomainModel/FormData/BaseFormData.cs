using System.Collections.Generic;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel.FormData {
	public abstract class BaseFormData : BaseEntity {
		public virtual Form Form { get; set; }
		public virtual IEnumerable<Question> AllQuestions { get { return null; } }
	}
}
