using System;
using System.ComponentModel.DataAnnotations;
using DevExpress.Web;

namespace ClinicalStudy.Site.Areas.DataCapture.Models.FormData {
	public class ElectrocardiogramFormViewModel : BaseFormDataViewModel {
		[Required(ErrorMessage = "Please specify Actual Time")]
		public DateTime? ElectrocardiogramActualTime { get; set; }

		public int ActualTimeQuestionId { get; set; }

		[Required(ErrorMessage = "Please attach an electrocardiogram data file")]
		public int? AttachmentId { get; set; }

		public int ElectrocardiogramAttachmentQuestionId { get; set; }

		public string AttachmentName { get; set; }

		public UploadControlValidationSettings ElectrocardiogramValidationSettings { get; set; }
	}
}
