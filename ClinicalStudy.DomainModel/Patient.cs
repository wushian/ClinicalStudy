using System;
using System.Collections.Generic;

namespace ClinicalStudy.DomainModel {
	public class Patient : BaseEntity {
		public Patient() {
			Visits = new List<Visit>();
		}
		public string Caption { get; set; }
		public string PatientInitials { get; set; }
		public int PatientNumber { get; set; }
		public bool IsActive { get; set; }
		public bool IsEnrolled { get; set; }
		public DateTime? EnrollDate { get; set; }
		public int? RandomisationNumber { get; set; }
		public DateTime? RandomisationDate { get; set; }

		public bool IsCompleted { get; set; }

		public virtual User Doctor { get; set; }
		public virtual IList<Visit> Visits { get; set; }
	}
}
