using System.ComponentModel.DataAnnotations;

namespace ClinicalStudy.Site.Areas.DataCapture.Models.Query {
	public class QueryViewModel {
		public int QueryId { get; set; }

		public bool IsClosed { get; set; }
		public string PatientCaption { get; set; }
		public int PatientNumber { set; get; }
		public string VisitCaption { get; set; }
		public string FormCaption { get; set; }
		public string Question { get; set; }


		public string QueryText { get; set; }
		public string QueryAuthor { get; set; }
		public byte[] QueryAuthorImage { get; set; }

		[Required]
		[StringLength(1024)]
		public string Answer { get; set; }

		public string AnswerAuthor { get; set; }
		public byte[] AnswerAuthorImage { get; set; }
	}
}
