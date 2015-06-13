using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel.DTOs {
	public class QueryReportDto {
		public string ClinicName { get; set; }
		public string DoctorFirstName { get; set; }
		public string DoctorLastName { get; set; }

		public string DoctorName
		{
			get { return string.Format("{0} {1}", DoctorFirstName, DoctorLastName); }
		}
		public FormType FormType { get; set; }
		public string QuestionName { get; set; }
		public bool IsOpen { get; set; }
	}
}
