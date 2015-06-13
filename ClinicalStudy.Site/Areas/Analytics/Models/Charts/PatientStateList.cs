using System.Collections.Generic;

namespace ClinicalStudy.Site.Areas.Analytics.Models.Charts {
	public class PatientStateList : List<PatientStateViewModel> {
		public string PatientState { get; set; }
		public int PatientStateColorNumber { get; set; }
		public string ClinicName { get; set; }
	}
}
