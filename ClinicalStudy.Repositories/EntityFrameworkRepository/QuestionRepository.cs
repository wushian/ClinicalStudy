using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository {
	public class QuestionRepository : GenericRepository<Question>, IQuestionRepository {
		public QuestionRepository(IClinicalStudyContextFactory contextFactory) : base(contextFactory) {
		}
	}
}
