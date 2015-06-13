using System.Collections.Generic;
using ClinicalStudy.DomainModel;

namespace ClinicalStudy.DemoData.DemoBuilders {
	public class ClinicsBuilder {
		private int clinicIdIncrement = 1;

		public List<Clinic> GetClinics() {
			var list = new List<Clinic>();
			list.Add(CreateClinic("St. Mary Hospital"));
			list.Add(CreateClinic("Florida General Hospital"));
			list.Add(CreateClinic("Johnson Neuropsychiatric Hospital"));
			list.Add(CreateClinic("Orlando Clinic"));
			list.Add(CreateClinic("Redmond Medical Center"));
			list.Add(CreateClinic("Dobson Institute for Rehabilitation"));
			list.Add(CreateClinic("Cofir Clinic"));
			list.Add(CreateClinic("St. Petersburg University Hospital"));
			list.Add(CreateClinic("University of Jenkintown Medical Center"));
			return list;
		}


		private Clinic CreateClinic(string caption) {
			return new Clinic() {
				Id = clinicIdIncrement++,
				Caption = caption
			};
		}
	}
}
