using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.Enums.Answers;

namespace ClinicalStudy.DomainModel.DTOs {
	public class HappinessChangeDto
	{
		public int ClinicId { get; set; }
		public string ClinicName { get; set; }
		public int DoctorId { get; set; }
		public string DoctorFirstName { get; set; }
		public string DoctorLastName { get; set; }

		public string DoctorName {
			get { return string.Format("{0} {1}", DoctorFirstName, DoctorLastName); }
		}

		public int PatientId { get; set; }
		public int PatientNumber { get; set; }

		public int DemographicFormId { get; set; }
		public int HappinessDay1FormId { get; set; }
		public int HappinessDay10FormId { get; set; }
	}
}
