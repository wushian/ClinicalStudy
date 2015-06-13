using ClinicalStudy.DomainModel.Enums.Display;

namespace ClinicalStudy.DomainModel.Enums {
	public enum VisitType {
		[Description("Enlisted")]
		None = 0,
		[Description("Baseline")]
		Baseline,
		[Description("1st Day")]
		Day1,
		[Description("Completed")]
		Day10,
        [Description("Adverse Event Visit")]
		AdverseEventVisit
	}
}
