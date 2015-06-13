using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicalStudy.Site.Areas.DataCapture.Models.FormData {
	public class AdverseEventFormViewModel : BaseFormDataViewModel {
		public string AdverseExperience { get; set; }

		public int AdverseExperienceQuestionId { get; set; }
		[Required]
		public DateTime? OnsetDate { get; set; }
		
		public int OnsetDateQuestionId { get; set; }
		[Required]
		public DateTime? OnsetTime { get; set; }
		
		public int OnsetTimeQuestionId { get; set; }
		[Required]
		public DateTime? EndDate { get; set; }

		public int EndDateQuestionId { get; set; }
		[Required]
		public DateTime? EndTime { get; set; }

		public int EndTimeQuestionId { get; set; }

		[Required(ErrorMessage = "Please specify Outcome")]
		[Range(1, 3, ErrorMessage = "Please specify Outcome")]
		public int Outcome { get; set; }

		public int OutcomeQuestionId { get; set; }

		[Required(ErrorMessage = "Please specify Intensity")]
		[Range(1, 3, ErrorMessage = "Please specify Intensity")]
		public int Intensity { get; set; }

		public int IntensityQuestionId { get; set; }

		[Required(ErrorMessage = "Please specify Relationship to Investigational Drug")]
		[Range(1, 4, ErrorMessage = "Please specify Relationship to Investigational Drug")]
		public int RelationshipToInvestigationalDrug { get; set; }

		public int RelationshipToInvestigationalDrugQuestionId { get; set; }
	}
}
