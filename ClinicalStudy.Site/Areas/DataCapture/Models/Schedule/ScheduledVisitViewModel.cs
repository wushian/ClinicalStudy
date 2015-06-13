namespace ClinicalStudy.Site.Areas.DataCapture.Models.Schedule {
	public class ScheduledVisitViewModel {
		public int VisitId { get; set; }
		public string PatientInitials { get; set; }
		public int PatientNumber { get; set; }
		public string VisitCaption { get; set; }
		public string VisitState { get; set; }
		public string ActualTime { get; set; }
		public bool IsPassed { get; set; }
	}
}
