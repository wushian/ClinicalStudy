using System;
using System.Collections.Generic;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel {
	public class Visit : BaseEntity {

		public Visit() {
			Forms = new List<Form>();
		}
		public string Caption { get; set; }
		public DateTime? VisitDate { get; set; }
		public DateTime? VisitTime { get; set; }
		public DateTime? ExpectedVisitDate { get; set; }
		public int VisitTypeValue { get; set; }

		public VisitType VisitType {
			get { return (VisitType) VisitTypeValue; }
			set { VisitTypeValue = (int) value; }
		}

		public int OrderNo { get; set; }

		public virtual Patient Patient { get; set; }
		public virtual List<Form> Forms { get; set; }
		public bool IsCompleted { get; set; }


		public bool IsFullyCompleted {
			get { return (Forms != null) && !Forms.Exists(f => f.FormStateValue == (int) FormState.Incomplete); }
		}
	}
}
