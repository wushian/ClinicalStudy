using System.Web.Mvc;

namespace ClinicalStudy.Site.Areas.DataCapture.Models {
	public class ChangeNoteViewModel {
		public int QuestionId { get; set; }

		public int Id { get; set; }
		[AllowHtml]
		public string OriginalValue { get; set; }
		[AllowHtml]
		public string NewValue { get; set; }

		public string ChangeReason { get; set; }

		public string QuestionName { get; set; }
	}
}
