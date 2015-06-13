using System.IO;
using System.Web;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.DataCapture.Controllers;
using DevExpress.Web;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Site.Tests.DataCapture {
	[TestFixture]
	public class AttachmentControllerTests {
		[Test]
		[Ignore("For now there is only very restricted support of mocking")]
		public void TestElectrocardiogramUploadValid() {
			//Arrange
			Mock<IAttachmentRepository> attachmentRepository = new Mock<IAttachmentRepository>();
			AttachmentController controller = new AttachmentController(attachmentRepository.Object);
			string fileName = "file.pdf";
			var stream = new MemoryStream(new byte[] {1, 2, 3, 4});
			Mock<HttpPostedFile> httpFile = new Mock<HttpPostedFile>();
			httpFile.SetupGet(f => f.FileName).Returns(fileName);


			var args = new FileUploadCompleteEventArgs(
				string.Empty,
				true,
				string.Empty,
				new UploadedFile(httpFile.Object));
			//Act
			controller.OnElectrocardiogramUploadComplete(null, args);

			//Assert
		}
	}
}
