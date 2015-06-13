using System.Collections.Generic;

namespace ClinicalStudy.Site.Areas.DataCapture.Models.Patient {
	public class PatientsListViewModel {
		public List<PatientLinkViewModel> PatientsList { get; set; }
		public bool CertainPatientRequested { get; set; }
		public int CertainPatientPageIndex { get; set; }
		public bool CertainPatientIsActive { get; set; }
	}
}
