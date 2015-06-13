using System;
using System.ComponentModel.DataAnnotations;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.Site.Areas.DataCapture.Models {
	public class VisitViewModel {
		public int Id { get; set; }
		public string Caption { get; set; }
		public DateTime? VisitDate { get; set; }
		public DateTime? VisitTime { get; set; }

		[Required]
		public DateTime? ExpectedVisitDate { get; set; }

		public VisitType VisitType { get; set; }
		public int OrderNo { get; set; }

		public bool NoExpectedVisitDate { get; set; }
	}
}
