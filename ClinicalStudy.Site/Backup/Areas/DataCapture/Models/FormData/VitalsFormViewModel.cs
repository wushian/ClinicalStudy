using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicalStudy.Site.Areas.DataCapture.Models.FormData {
	public class VitalsFormViewModel : BaseFormDataViewModel {
		[Required(ErrorMessage = "Please specify Actual Time")]
		public DateTime? ActualTime { get; set; }

		public int ActualTimeQuestionId { get; set; }

		[Required(ErrorMessage = "Please specify Height")]
		[Range(60, 250, ErrorMessage = "{0} should be between {1} and {2} cm")]
		public decimal Height { get; set; }

		public int HeightQuestionId { get; set; }

		[Required(ErrorMessage = "Please specify Weight")]
		[Range(20, 200, ErrorMessage = "{0} should be between {1} and {2} kg")]
		public decimal Weight { get; set; }

		public int WeightQuestionId { get; set; }

		[Required(ErrorMessage = "Please specify Temperature")]
		[Range(35.4, 42.0, ErrorMessage = "{0} should be between {1} and {2} °C")]
		public decimal Temperature { get; set; }

		public int TemperatureQuestionId { get; set; }

		[Required(ErrorMessage = "Please specify Heart rate")]
		[Range(10, 200, ErrorMessage = "Heart Rate should be between {1} and {2} BPM")]
		public int HeartRate { get; set; }

		public int HeartRateQuestionId { get; set; }

		[Required(ErrorMessage = "Parameter is required")]
		[Range(10, 240, ErrorMessage = "Systolic should be between {1} and {2} mmHg")]
		public string BloodPressureSystolic { get; set; }

		public int BloodPressureSystolicQuestionId { get; set; }

		[Required(ErrorMessage = "Parameter is required")]
		[Range(10, 200, ErrorMessage = "Diastolic should be between {1} and {2} mmHg")]
		public string BloodPressureDiastolic { get; set; }

		public int BloodPressureDiastolicQuestionId { get; set; }
	}
}
