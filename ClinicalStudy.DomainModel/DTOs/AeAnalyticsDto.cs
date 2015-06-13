using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel.DTOs {
	public class AeAnalyticsDto
	{
		public string ClinicName { get; set; }
		public string DoctorFirstName { get; set; }
		public string DoctorLastName { get; set; }

		public string DoctorName
		{
			get { return string.Format("{0} {1}", DoctorFirstName, DoctorLastName); }
		}
		public int DemographicFormId { get; set; }
		public int AeFormId { get; set; }
	}
}
