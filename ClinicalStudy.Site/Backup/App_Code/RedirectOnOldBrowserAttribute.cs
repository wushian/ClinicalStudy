using System.Web.Mvc;
using System.Web.Routing;

namespace ClinicalStudy.Site {

	public class RedirectOnOldBrowserAttribute : ActionFilterAttribute {
		public string Controller { get; set; }
		public string Action { get; set; }


		public int ObsoleteIeVersion = 6;

		public override void OnActionExecuting(ActionExecutingContext filterContext) {



			var request = filterContext.HttpContext.Request;

			if (request.Browser.Browser.Trim().ToUpperInvariant().Equals("IE") && request.Browser.MajorVersion <= ObsoleteIeVersion) {
				filterContext.Result =
					new RedirectToRouteResult(
						new RouteValueDictionary {
							{"controller", Controller},
							{"action", Action}
						});
				return;
			}

			base.OnActionExecuting(filterContext);
		}
	}
}
