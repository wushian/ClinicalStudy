using System;

namespace ClinicalStudy.DomainModel {
	public class Timepoint : BaseEntity {
		public DateTime DateAndTime { get; set; }
		public bool WasCorrectedAfterCreation { get; set; }
	}
}
