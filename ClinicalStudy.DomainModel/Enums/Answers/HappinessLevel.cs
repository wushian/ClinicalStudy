using ClinicalStudy.DomainModel.Enums.Display;

namespace ClinicalStudy.DomainModel.Enums.Answers {
	public enum HappinessLevel {
		[Description("Totally Unhappy")]
		TotallyUnhappy = 0,
		[Description("Unhappy")]
		Unhappy = 25,
		[Description("Happy")]
		Happy = 50,
		[Description("Pretty Happy")]
		PrettyHappy = 75,
		[Description("Totally Happy")]
		TotallyHappy = 100
	}
}
