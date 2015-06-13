using System.Collections.Generic;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel.FormData {
	public class RepeatableInventoryData : BaseEntity {
		public virtual Question DateUsed { get; set; }
		public virtual Question QuantityUsed { get; set; }
	}
}
