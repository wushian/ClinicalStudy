using System.Collections.Generic;

namespace ClinicalStudy.Site.Areas.Analytics.Models {
	public class ClinicDetailsViewModel {
		public int ClinicId { get; set; }
		public List<ClinicDoctorInfoDto> Doctors { get; set; }
	}

	public class ClinicDoctorInfoDto {
		public int ClinicId { get; set; }
		public int DoctorId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Role { get; set; }
		public int PatientsCount { get; set; }
		public string Login { get; set; }
		public byte[] Photo { get; set; }
	}
}
