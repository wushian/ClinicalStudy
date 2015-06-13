using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicalStudy.Site.Areas.DataCapture.Models.FormData {
	public class DemographicFormViewModel : BaseFormDataViewModel {
		[Required(ErrorMessage = "Please specify DOB")]
		public DateTime? DateOfBirth { get; set; }

		public int DateOfBirthQuestionId { get; set; }

		public string Other { get; set; }

		public int OtherQuestionId { get; set; }

		[Required(ErrorMessage = "Please specify Race")]
		public int Race { get; set; }

		public int RaceQuestionId { get; set; }

		[Required(ErrorMessage = "Please specify Sex")]
		public int Sex { get; set; }

		public int SexQuestionId { get; set; }
	}
}
