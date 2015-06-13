using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClinicalStudy.Site.Areas.DataCapture.Models.FormData {
	public class InventoryFormViewModel : BaseFormDataViewModel {
		[Required(ErrorMessage = "Please specify Quantity Shipped")]
		public decimal QuantityShipped { get; set; }

		public int QuantityShippedQuestionId { get; set; }

		[Required(ErrorMessage = "Please specify Batch Number")]
		public int BatchNumber { get; set; }

		public int BatchNumberQuestionId { get; set; }

        [Required]
		public DateTime? ReceiptDate { get; set; }

		public int ReceiptDateQuestionId { get; set; }

        [Required]
		public DateTime? ShipDate { get; set; }

		public int ShipDateQuestionId { get; set; }

		public List<RepeatableInventoryDataViewModel> MedicationUsage { get; set; }
	}
}
