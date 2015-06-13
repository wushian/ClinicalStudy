using System.Web.Mvc;

namespace ClinicalStudy.Site.Areas.DataCapture {
	public class DataCaptureAreaRegistration : AreaRegistration {
		public override string AreaName {
			get {
				return "DataCapture";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context) {
			context.MapRoute(
				null, // Route name
				"DataCapture/Patients/NewPatient", // URL with parameters
				new { controller = "Patient", action = "PatientDataContainer", patientNumber = -1 } // Parameter defaults
			);

			context.MapRoute(
				null, // Route name
				"DataCapture/Patients/{patientNumber}/{visitName}/{formName}", // URL with parameters
				new { controller = "Patient", action = "PatientDataContainer", visitName = UrlParameter.Optional, formName = UrlParameter.Optional } // Parameter defaults
			);

			context.MapRoute(
				"DataCapture_default",
				"DataCapture/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
