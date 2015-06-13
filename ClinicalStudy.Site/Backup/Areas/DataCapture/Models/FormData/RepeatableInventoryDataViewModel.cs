using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicalStudy.Site.Areas.DataCapture.Models.FormData {
	public class RepeatableInventoryDataViewModel {
		public int Id { get; set; }

		public int InnerId { get; set; }

		[Required(ErrorMessage = "Please specify Date used")]
		public DateTime? DateUsed { get; set; }

		public int DateUsedQuestionId { get; set; }

		[Required(ErrorMessage = "Please specify Quantity used")]
		public decimal QuantityUsed { get; set; }

		public int QuantityUsedQuestionId { get; set; }
	}
}
