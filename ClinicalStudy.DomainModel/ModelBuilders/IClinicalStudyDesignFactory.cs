namespace ClinicalStudy.DomainModel.ModelBuilders {
	public interface IClinicalStudyDesignFactory {
		IClinicalStudyDesign Create();
		void Release(IClinicalStudyDesign clinicalStudyDesign);
	}
}
