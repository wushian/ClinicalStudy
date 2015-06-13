using System.Collections.Generic;
using ClinicalStudy.Site.Areas.DataCapture.Models.Query;

namespace ClinicalStudy.Site.Areas.DataCapture.Models.FormData {
	public abstract class BaseFormDataViewModel {
		public int Id { get; set; }
		public int FormId { get; set; }
		public string FormCaption { get; set; }
		public string VisitName { get; set; }
		public int PatientNumber { get; set; }
		public bool IsCompleted { get; set; }
		public bool DataChangeReasonRequired { get; set; }


		public Dictionary<int, QueryShortViewModel> QuestionsWithQueries { get; set; }
		public List<ChangeNoteViewModel> ChangeInfos { get; set; }
	}
}
