using System.Web.Mvc;

namespace ClinicalStudy.Site.Areas.DataCapture.HtmlHelpers {
	public static class StydyUrlHelper {
      public static string PatientIconClass(this UrlHelper src, string sex, bool? isSelected) {
            string sexCss = "unknown";
            switch (sex) {
                case "0":
                    sexCss = "male";
                    break;
                case "1":
                    sexCss = "female";
                    break;
            }

            if (isSelected ?? false) 
                sexCss += " selected";
            return sexCss;
        }
	}
}
