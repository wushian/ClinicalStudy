using System;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel {
	public class ChangeNote : BaseEntity {
		public string OriginalValue { get; set; }

		public string NewValue { get; set; }

		public string ChangeReason { get; set; }

		public DateTime ChangeDate { get; set; }

		public virtual Question Question { get; set; }
	}
}
