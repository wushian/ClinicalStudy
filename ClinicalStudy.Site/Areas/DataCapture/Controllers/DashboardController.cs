using System.Web.Mvc;

namespace ClinicalStudy.Site.Areas.DataCapture.Controllers {
	public class DashboardController : DataCaptureBaseController {

		public ActionResult Index() {
			ViewData["Dashboard"] = true;
			if (Request.IsAjaxRequest())
				return PartialView("_Index");
			return View();
		}
	}
}
