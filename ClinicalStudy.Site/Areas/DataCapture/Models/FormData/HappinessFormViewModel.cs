using System.ComponentModel.DataAnnotations;

namespace ClinicalStudy.Site.Areas.DataCapture.Models.FormData {
	public class HappinessFormViewModel : BaseFormDataViewModel {
		[Required(ErrorMessage = "Please specify Happiness feeling")]
		public int HappinessLevel { get; set; }

		public int HappinessLevelQuestionId { get; set; }
	}
}
