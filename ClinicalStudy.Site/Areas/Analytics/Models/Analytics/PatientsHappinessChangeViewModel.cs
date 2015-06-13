namespace ClinicalStudy.Site.Areas.Analytics.Models.Analytics {
	public class PatientsHappinessChangeViewModel {
		public int ClinicId { get; set; }
		public string ClinicName { get; set; }
		public int DoctorId { get; set; }
		public string DoctorName { get; set; }
		public int PatientId { get; set; }
		public int PatientNumber { get; set; }

		public string Race { get; set; }
		public string Gender { get; set; }
		public decimal HappinessChange { get; set; }
	}
}
