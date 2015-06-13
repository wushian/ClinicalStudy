using System.Collections.Generic;

namespace ClinicalStudy.Site.Areas.DataCapture.Models.Shared {
	public class DataContainerViewModel {
		public int Id { get; set; }
		public int PatientNumber { get; set; }
		public string SelectedVisitName { get; set; }
		public string SelectedFormName { get; set; }
		public List<ConteinerChildViewModel> Children { get; set; }

		public bool IsNew {
			get { return Id == 0; }
		}

		public string PatientCaption { get; set; }

		public string PatientInitials { get; set; }

		public string PageTitle {
			get {
				string title = null;
				if (!string.IsNullOrEmpty(PatientCaption)) {
					title = "Patient " + PatientCaption;
				}
				else {
					title = "New Patient";
				}
				return title;
			}
		}
	}
}
