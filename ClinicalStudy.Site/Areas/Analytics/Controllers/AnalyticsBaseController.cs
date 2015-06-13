using System.Web.Mvc;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.Site.Areas.Analytics.Controllers {
	[Authorize(Roles = ClinicalStudyRoles.Supervisor)]
	public class AnalyticsBaseController : Controller {
	}
}
