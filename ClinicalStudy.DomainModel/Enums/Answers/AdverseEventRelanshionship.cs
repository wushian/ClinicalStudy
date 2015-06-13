using ClinicalStudy.DomainModel.Enums.Display;

namespace ClinicalStudy.DomainModel.Enums.Answers {
	public enum AdverseEventRelanshionship {
		[Description("Not Related")]
		NotRelated = 1,
		[Description("Unlikely")]
		Unlikely = 2,
		[Description("Suspected (reasonably possible)")]
		Suspected = 3,
		[Description("Probable")]
		Probable = 4
	}
}
