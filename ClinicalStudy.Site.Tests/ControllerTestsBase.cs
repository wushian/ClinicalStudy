using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace ClinicalStudy.Site.Tests {
	public class ControllerTestsBase {

		public const string CommonEmulatedUserName = "doctorName";

		protected void EmulateControllerContext(ControllerBase controller, bool isAjax) {
			EmulateControllerContext(controller, isAjax, new RouteData(), new Mock<HttpSessionStateBase>());
		}

		protected void EmulateControllerContext(ControllerBase controller, Mock<HttpSessionStateBase> session)
		{
			EmulateControllerContext(controller, false, new RouteData(), session);
		}

		protected void EmulateControllerContext(ControllerBase controller, RouteData routeData) {
			EmulateControllerContext(controller, false, routeData, new Mock<HttpSessionStateBase>());
		}

		protected void EmulateControllerContext(ControllerBase controller, bool isAjax, RouteData routeData, Mock<HttpSessionStateBase> session) {
			var request = new Mock<HttpRequestBase>();
			if (isAjax) {
				//here we emulate standard Ajax header
				request.SetupGet(x => x.Headers).Returns(
					new System.Net.WebHeaderCollection {
						{"X-Requested-With", "XMLHttpRequest"}
					});
			}

			var context = new Mock<HttpContextBase>();
			context.SetupGet(x => x.Session).Returns(session.Object);

			context.SetupGet(x => x.Request).Returns(request.Object);
			context.SetupGet(x => x.User).Returns(new GenericPrincipal(new GenericIdentity(CommonEmulatedUserName), new string[0]));

			controller.ControllerContext = new ControllerContext(context.Object, routeData, controller);
		}
	}
}
