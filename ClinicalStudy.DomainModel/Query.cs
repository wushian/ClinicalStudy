using System;

namespace ClinicalStudy.DomainModel {
	public class Query : BaseEntity {
		public virtual Question Question { get; set; }

		public virtual User QueryAuthor { get; set; }
		public string QueryText { get; set; }
		public DateTime QueryTime { set; get; }

		public virtual User AnswerAuthor { get; set; }
		public string AnswerText { get; set; }
		public DateTime? AnswerTime { get; set; }
	}
}
