using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel {
	public class Attachment : BaseEntity {
		public string FileName { get; set; }
		public string MimeType { get; set; }
		public string StorageFileName { get; set; }
		public int FileSize { get; set; }
	}
}
