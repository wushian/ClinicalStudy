using System;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.Mvc;

namespace ClinicalStudy.Site.Areas.DataCapture.ControlHelper {
	public interface IHtmlEditorAdapter {
		HtmlEditorView GetActiveView(string name);

		string GetHtmlView(string name, ASPxHtmlEditorHtmlEditingSettings htmlEditingSettings,
		                   HtmlEditorValidationSettings validationSettings,
		                   EventHandler<HtmlEditorValidationEventArgs> validationDelegate, out bool isValid);
	}

	public class HtmlEditorAdapter : IHtmlEditorAdapter {
		public HtmlEditorView GetActiveView(string name) {
			return HtmlEditorExtension.GetActiveView(name);
		}

		public string GetHtmlView(string name, ASPxHtmlEditorHtmlEditingSettings htmlEditingSettings,
		                          HtmlEditorValidationSettings validationSettings,
		                          EventHandler<HtmlEditorValidationEventArgs> validationDelegate, out bool isValid) {
			return HtmlEditorExtension.GetHtml(name, htmlEditingSettings, validationSettings, validationDelegate, out isValid);
		}
	}
}
