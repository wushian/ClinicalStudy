using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.Repositories.MemoryRepositories {
	public class AttachmentRepository : GenericRepository<Attachment>, IAttachmentRepository {
		public AttachmentRepository(IDataStorage dataStorage) : base(dataStorage) {
		}

		protected internal override void MapEntity(Attachment stored, Attachment updated) {
			stored.FileName = updated.FileName;
			stored.FileSize = updated.FileSize;
			stored.MimeType = updated.MimeType;
			stored.StorageFileName = updated.StorageFileName;
		}
	}
}
