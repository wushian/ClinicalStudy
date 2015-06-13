using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace ClinicalStudy.Site.Areas.DataCapture.HtmlHelpers {
	public static class AjaxActionHelper {
		public static MvcHtmlString ImageActionLink(this AjaxHelper helper, string imageUrl, string altText, string actionName,
		                                            string controllerName, object routeValues, AjaxOptions ajaxOptions) {
			return ComposeImageActionLink(helper, imageUrl, altText, actionName, controllerName, routeValues, ajaxOptions);
		}

		public static MvcHtmlString ImageActionLink(this AjaxHelper helper, string imageUrl, string altText, string actionName,
		                                            string controllerName, object routeValues, AjaxOptions ajaxOptions,
		                                            object htmlAttributes) {
			return ComposeImageActionLink(helper, imageUrl, altText, actionName, controllerName, routeValues, ajaxOptions,
				htmlAttributes);
		}

		private static MvcHtmlString ComposeImageActionLink(AjaxHelper helper, string imageUrl, string altText,
		                                                    string actionName, string controllerName, object routeValues,
		                                                    AjaxOptions ajaxOptions, object htmlAttributes = null) {
			var builder = new TagBuilder("img");
			builder.MergeAttribute("src", imageUrl);
			builder.MergeAttribute("alt", altText);
			string link =
				helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, ajaxOptions, htmlAttributes).ToHtmlString();
			return new MvcHtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing)));
		}
	}
}
