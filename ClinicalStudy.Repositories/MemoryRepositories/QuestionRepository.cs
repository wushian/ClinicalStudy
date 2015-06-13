using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.Repositories.MemoryRepositories {
	public class QuestionRepository : GenericRepository<Question>, IQuestionRepository {
		public QuestionRepository(IDataStorage dataStorage) : base(dataStorage) {
		}

		protected internal override void MapEntity(Question stored, Question updated) {
			stored.Value = updated.Value;
			stored.File = updated.File;
		}
	}
}
