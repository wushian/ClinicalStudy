using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel.DTOs {
	public class PatientStateDto {
		public int ClinicId { get; set; }
		public string ClinicName { get; set; }
		public int DoctorId { get; set; }
		public string DoctorName { get; set; }
		public VisitType VisitType { 
			get { return (VisitType) VisitTypeValue; }
			set { VisitTypeValue = (int) value; }
		}
		public int VisitTypeValue { get; set; }
	}
}
