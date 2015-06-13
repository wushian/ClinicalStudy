using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel {
	public class Form : BaseEntity {
		public string Caption { get; set; }


		//this is workaround, related with restriction in EF 4.2
		public FormState FormState {
			get { return (FormState) FormStateValue; }
			set { FormStateValue = (int) value; }
		}

		public int FormStateValue { get; set; }

		public FormType FormType {
			get { return (FormType) FormTypeValue; }
			set { FormTypeValue = (int) value; }
		}

		public int FormTypeValue { get; set; }


		public int OrderNo { get; set; }

		public virtual Visit Visit { get; set; }
	}
}
