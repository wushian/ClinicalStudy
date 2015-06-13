using System.Collections.Generic;

namespace ClinicalStudy.Site.Areas.Analytics.Models.Charts {
	public class UnfinishedCrfsList : List<UnfinishedCrfViewModel> {
		public string ClinicName { get; set; }
	}
}
