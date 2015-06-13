namespace ClinicalStudy.Repositories.EntityFrameworkRepository {
	public interface IClinicalStudyContextFactory {
		ClinicalStudyContext Retrieve();
	}
}
