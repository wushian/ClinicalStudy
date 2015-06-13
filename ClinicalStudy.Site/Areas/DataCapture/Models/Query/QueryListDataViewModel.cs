using System.Collections.Generic;

namespace ClinicalStudy.Site.Areas.DataCapture.Models.Query {
	public class QueryListDataViewModel : List<QueryViewModel> {
		public QueryListDataViewModel() {
		}

		public QueryListDataViewModel(int capacity)
			: base(capacity) {
		}

		public QueryListDataViewModel(IEnumerable<QueryViewModel> collection)
			: base(collection) {
		}

		public int OpenQueryNumber { get; set; }
		public int TotalQueryNumber { get; set; }
	}
}
