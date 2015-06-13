namespace ClinicalStudy.Site.Areas.Analytics.Models.Analytics {
	public class AdverseEventsAnalyticsViewModel {
		public string ClinicName { get; set; }
		public string DoctorName { get; set; }

		public string Race { get; set; }
		public string Gender { get; set; }

		public string Intensity { get; set; }
		public string RelationshipToInvestigationalDrug { get; set; }

		public int AesCount { get; set; }
	}
}
