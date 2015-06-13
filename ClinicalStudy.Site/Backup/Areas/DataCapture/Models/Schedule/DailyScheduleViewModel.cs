using System;
using System.Collections.Generic;

namespace ClinicalStudy.Site.Areas.DataCapture.Models.Schedule {
	public class DailyScheduleViewModel {
		public IList<ScheduledVisitViewModel> ScheduledVisits { get; set; }
		public DateTime Date { get; set; }
		public string DateDescription { get; set; }
	}
}
