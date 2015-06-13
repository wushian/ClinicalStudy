using System.Web.Mvc;

namespace ClinicalStudy.Site.Areas.Analytics.Controllers {
	public class DashboardController : AnalyticsBaseController {
		//
		// GET: /Analytics/Dashboard/

		public ActionResult Index() {
			return View();
		}
	}
}
