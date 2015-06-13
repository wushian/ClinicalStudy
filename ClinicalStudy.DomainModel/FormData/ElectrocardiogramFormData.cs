using System.Collections.Generic;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel.FormData {
	public class ElectrocardiogramFormData : BaseFormData {
		public virtual Question ElectrocardiogramActualTime { get; set; }
		public virtual Question ElectrocardiogramAttachment { get; set; }

		public override System.Collections.Generic.IEnumerable<Question> AllQuestions {
			get { return new List<Question> {ElectrocardiogramActualTime, ElectrocardiogramAttachment}; }
		}
	}
}
