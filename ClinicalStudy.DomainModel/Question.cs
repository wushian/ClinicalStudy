using System.Collections.Generic;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel {
	public class Question : BaseEntity {

		public Question() {
			ChangeNotes = new List<ChangeNote>();
		}
		public string Caption { get; set; }
		public string Value { get; set; }

		//this is workaround, related with restriction in EF 4.2
		public QuestionDataType DataType {
			get { return (QuestionDataType) DataTypeValue; }
			set { DataTypeValue = (int) value; }
		}

		public int DataTypeValue { get; set; }

		public virtual Attachment File { get; set; }

		public virtual List<ChangeNote> ChangeNotes { get; set; }

		public virtual Form Form { get; set; }
	}
}
