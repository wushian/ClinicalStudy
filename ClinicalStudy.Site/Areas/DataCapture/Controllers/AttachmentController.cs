using System;
using System.IO;
using System.Web.Mvc;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;
using DevExpress.Web;
using DevExpress.Web.Mvc;

namespace ClinicalStudy.Site.Areas.DataCapture.Controllers {
	public class AttachmentController : DataCaptureBaseController {
		public const string AttachmentsFilePath = "~/Content/FileStorage/";

		public static readonly UploadControlValidationSettings ElectrocardiogramValidationSettings
			= new UploadControlValidationSettings {
				AllowedFileExtensions = new[] {".jpg", ".jpeg", ".jpe", ".gif", ".png"},
				MaxFileSize = 20971520
			};

		private readonly IAttachmentRepository attachmentRepository;

		public AttachmentController(IAttachmentRepository attachmentRepository) {
			this.attachmentRepository = attachmentRepository;
		}

		public ActionResult CallbacksElectrocardiogramUpload() {
			UploadControlExtension.GetUploadedFiles("ucAttachment", ElectrocardiogramValidationSettings,
				OnElectrocardiogramUploadComplete);
			return null;
		}

		public void OnElectrocardiogramUploadComplete(object sender, FileUploadCompleteEventArgs e) {
			if (e.IsValid) {
				var attachment =
					new Attachment {
						FileName = e.UploadedFile.FileName,
						MimeType = e.UploadedFile.ContentType,
						StorageFileName = Path.ChangeExtension(
							Guid.NewGuid().ToString(),
							Path.GetExtension(e.UploadedFile.FileName)),
						FileSize = (int)e.UploadedFile.ContentLength
					};
				attachmentRepository.Add(attachment);
				SaveFile(e, attachment);
				attachmentRepository.Save();

				e.CallbackData = string.Format(
					"{{ \"id\": {0}, \"name\": \"{1}\", \"link\": \"{2}\" }}",
					attachment.Id.ToString(),
					attachment.FileName,
					Url.Action("GetAttachment", "Attachment", new {id = attachment.Id}));
			}
		}

		private void SaveFile(FileUploadCompleteEventArgs e, Attachment attachment) {
			string filePath = Path.Combine(Server.MapPath(AttachmentsFilePath), attachment.StorageFileName);
			e.UploadedFile.SaveAs(filePath, true);
		}

		public ActionResult GetAttachment(int id) {
			Attachment attachment = attachmentRepository.GetByKey(id);
			if (attachment == null)
				return HttpNotFound("Requested file was not found");

			Stream stream = GetFileStream(attachment);
			return File(stream, attachment.MimeType, attachment.FileName);
		}

		private Stream GetFileStream(Attachment attachment) {
			string filePath = Path.Combine(Server.MapPath(AttachmentsFilePath), attachment.StorageFileName);
			return new FileStream(filePath, FileMode.Open, FileAccess.Read);
		}
	}
}
