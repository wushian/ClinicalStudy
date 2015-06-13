using System.Collections.Generic;

namespace ClinicalStudy.Site.Areas.Analytics.Models {
	public class ClinicMasterViewModel {
		public List<ClinicMasterInfoDto> Clinics { get; set; }
	}

	public class ClinicMasterInfoDto {
		public int ClinicId { get; set; }
		public string ClinicName { get; set; }
		public int DoctorsCount { get; set; }
	}
}
