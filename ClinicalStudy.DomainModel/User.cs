using System.Collections.Generic;

namespace ClinicalStudy.DomainModel {
	public class User : BaseEntity {

		public User() {
			Patients = new List<Patient>();
		}
		public string Login { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Role { get; set; }

		public byte[] Photo { get; set; }
		public bool CanVisitWebSite { get; set; }
		public virtual Clinic Clinic { get; set; }
		public virtual List<Patient> Patients { get; set; }

		public override string ToString() {
			return string.Format("FirstName: {0}, LastName: {1}, Role: {2}", FirstName, LastName, Role);
		}
	}
}
