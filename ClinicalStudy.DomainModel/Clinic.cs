using System.Collections.Generic;

namespace ClinicalStudy.DomainModel {
	public class Clinic : BaseEntity {
		public Clinic() {
			Doctors = new List<User>();
		}
		public string Caption { get; set; }
		public virtual List<User> Doctors { get; set; }

		public override string ToString() {
			return Caption;
		}
	}
}
