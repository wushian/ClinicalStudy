using ClinicalStudy.DomainModel.Enums.Display;

namespace ClinicalStudy.DomainModel.Enums.Answers {
	public enum Race {
		[Description("White")]
		White = 0,
		[Description("Black or African American")]
		AfricanAmerican = 1,
		[Description("Hispanic/Latino")]
		HispanicLatino = 2,
		[Description("Asian")]
		Asian = 3,
		[Description("American Indian")]
		AmericanIndian = 4,
		[Description("Other")]
		Other = 5
	}
}
