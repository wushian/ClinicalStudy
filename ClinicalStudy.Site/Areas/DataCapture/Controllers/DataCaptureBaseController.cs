using System.Web.Mvc;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.Site.Areas.DataCapture.Controllers {
	/// <summary>
	/// Single base controller for each controller in the area
	/// It allow us to have single security point
	/// </summary>
	[Authorize(Roles = ClinicalStudyRoles.Doctor)]
	public abstract class DataCaptureBaseController : Controller {
	}
}
