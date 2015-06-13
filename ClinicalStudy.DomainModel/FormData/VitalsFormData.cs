using System.Collections.Generic;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel.FormData {
	public class VitalsFormData : BaseFormData {
		public virtual Question ActualTime { get; set; }
		public virtual Question Height { get; set; }
		public virtual Question Weight { get; set; }
		public virtual Question Temperature { get; set; }
		public virtual Question HeartRate { get; set; }
		public virtual Question BloodPressureSystolic { get; set; }
		public virtual Question BloodPressureDiastolic { get; set; }
		public override IEnumerable<Question> AllQuestions {
			get { return new List<Question> {
				ActualTime,
				Height,
				Weight,
				HeartRate,
				Temperature,
				BloodPressureSystolic,
				BloodPressureDiastolic
			}; }
		}
	}
}
