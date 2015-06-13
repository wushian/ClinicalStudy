namespace ClinicalStudy.Site.Areas.Analytics.Models.Charts {
	public class PatientStateViewModel {
		public int SortingKey { get; set; }
		public string StudyState { get; set; }
		public string EntityCaption { get; set; }
		public int EntityId { get; set; }
		public int PatientsNumber { get; set; }
	}
}
