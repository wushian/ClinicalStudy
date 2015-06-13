using System.Collections.Generic;
using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository {

	public class ChangeNoteRepository : GenericRepository<ChangeNote>, IChangeNoteRepository {
		public ChangeNoteRepository(IClinicalStudyContextFactory contextFactory) : base(contextFactory) {
		}
	}
}
