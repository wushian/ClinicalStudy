using DevExpress.XtraReports.UI;

namespace ClinicalStudy.Site.Areas.Analytics.Models.REPORTS {
	public abstract class ReportViewModel {
		public XtraReport Report { get; set; }

		public virtual string ReportName {
			get { return string.Empty; }
		}
	}
}
