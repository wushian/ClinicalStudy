using System.Collections.Generic;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel.FormData {
	public class InventoryFormData : BaseFormData {
		public InventoryFormData() {
			MedicationUsage = new List<RepeatableInventoryData>();
		}

		public virtual Question QuantityShipped { get; set; }
		public virtual Question BatchNumber { get; set; }
		public virtual Question ReceiptDate { get; set; }
		public virtual Question ShipDate { get; set; }

		public virtual List<RepeatableInventoryData> MedicationUsage { get; set; }


		public override IEnumerable<Question> AllQuestions {
			get { return new List<Question> {QuantityShipped, BatchNumber, ReceiptDate, ShipDate}; }
		}

	}
}
