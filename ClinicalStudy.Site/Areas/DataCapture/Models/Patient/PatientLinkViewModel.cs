using System;
using System.Globalization;

namespace ClinicalStudy.Site.Areas.DataCapture.Models.Patient {
	public class PatientLinkViewModel {
		public int Id { get; set; }
		public string Caption { get; set; }
		public int PatientNumber { get; set; }


		public string DateOfBirth { get; set; }

		public string Sex { get; set; }

		public bool IsSelected { get; set; }

		public string Gender {
			get {
				if (Sex == null)
					return "Unknown";

				return Sex == "0" ? "Male" : "Female";
			}
		}

		public string Age {
			get {
				if (DateOfBirth == null)
					return "Unknown";
				DateTime date = DateTime.Parse(DateOfBirth, CultureInfo.InvariantCulture);
				return string.Format("Age {0}", Math.Truncate(DateTime.Now.Subtract(date).TotalDays/365));
			}
		}
	}
}
