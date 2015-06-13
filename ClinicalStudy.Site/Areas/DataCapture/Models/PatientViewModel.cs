using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicalStudy.Site.Areas.DataCapture.Models {
	public class PatientViewModel {
		public int Id { get; set; }
		public string Caption { get; set; }
		public int PatientNumber { get; set; }
		public bool IsActive { get; set; }

		public bool IsEnrolled { get; set; }

		public DateTime? EnrollDate { get; set; }

		[Required(ErrorMessage = "Please enter Patient Initials")]
		[StringLength(3, MinimumLength = 2, ErrorMessage = "Please specify First and Last Name Initials")]
		public string PatientInitials { get; set; }

		public int? RandomisationNumber { get; set; }

		public DateTime? RandomisationDate { get; set; }


		public bool IsNewPatient {
			get { return Id == 0; }
		}
	}
}
