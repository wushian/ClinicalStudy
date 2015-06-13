using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository {
	public class AttachmentRepository : GenericRepository<Attachment>, IAttachmentRepository {
		public AttachmentRepository(IClinicalStudyContextFactory contextFactory) : base(contextFactory) {
		}
	}
}
