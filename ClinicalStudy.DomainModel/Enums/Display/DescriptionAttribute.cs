using System;

namespace ClinicalStudy.DomainModel.Enums.Display {
	public class DescriptionAttribute : Attribute {
		private string description;
		public string Description { get { return description; } }

		public DescriptionAttribute(string description) {
			this.description = description;
		}
	}
}
