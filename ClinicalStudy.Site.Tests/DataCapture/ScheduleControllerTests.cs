using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Castle.Components.DictionaryAdapter;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.DataCapture.Controllers;
using ClinicalStudy.Site.Areas.DataCapture.Models.Schedule;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Site.Tests.DataCapture {
	[TestFixture]
	public class ScheduleControllerTests : ControllerTestsBase {
		private ScheduleController controller = null;
		private Mock<IVisitRepository> visitRepository = null;
		[SetUp]
		public void Setup() {

			visitRepository = new Mock<IVisitRepository>();
			controller = new ScheduleController(visitRepository.Object);
		}
		[Test]
		public void IndexViewTest() {
			//Arrange
			EmulateControllerContext(controller, false);
			//Act
			var result = controller.Index(null);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResult);
			var partialResult = result as ViewResult;
			Assert.That(partialResult.Model, Is.Not.Null);
			Assert.That(partialResult.Model is DailyScheduleViewModel);
		}


		[Test]
		public void IndexModel_Dates_Today_Test() {
			//Arrange
			EmulateControllerContext(controller, false);

			//Act
			var result = controller.Index(null);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var partialResult = result as ViewResultBase;
			Assert.That(partialResult.Model, Is.Not.Null);
			var model = partialResult.Model as DailyScheduleViewModel;
			Assert.That(model.Date, Is.EqualTo(DateTime.Now.Date));
			Assert.That(model.DateDescription, Is.EqualTo("Today"));
		}

		[Test]
		public void IndexModelTest() {
			//Arrange
			EmulateControllerContext(controller, false);

			var visit = new Visit() {
				Id = 1,
				Caption = "Baseline",
				ExpectedVisitDate = DateTime.Now.Date,
				OrderNo = 1,
				Forms = new List<Form>(),
				Patient =
					new Patient {Caption = "Subj 027", PatientNumber = 27}
			};
			visitRepository
				.Setup(r => r.GetDailyVisits(CommonEmulatedUserName, DateTime.Now.Date))
				.Returns(new List<Visit> {
					visit
				});
			//Act
			var result = controller.Index(null);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var partialResult = result as ViewResultBase;
			Assert.That(partialResult.Model, Is.Not.Null);
			var model = partialResult.Model as DailyScheduleViewModel;

			visitRepository.Verify(r => r.GetDailyVisits(CommonEmulatedUserName, DateTime.Now.Date), Times.Once());

			Assert.That(model.ScheduledVisits, Is.Not.Null);
			Assert.That(model.ScheduledVisits.Count, Is.EqualTo(1));
			var visitModel = model.ScheduledVisits[0];
			Assert.That(visitModel.VisitCaption, Is.EqualTo(visit.Caption));
			Assert.That(visitModel.PatientInitials, Is.EqualTo(visit.Patient.PatientInitials));
			Assert.That(visitModel.PatientNumber, Is.EqualTo(visit.Patient.PatientNumber));
			Assert.That(visitModel.VisitState, Is.EqualTo(visit.IsFullyCompleted ? "Completed" : "Incomplete"));
		}


		[Test]
		public void IndexModel_Yesterday_Test() {
			//Arrange
			EmulateControllerContext(controller, false);

			var visit = new Visit() {
				Id = 1,
				Caption = "Baseline",
				ExpectedVisitDate = DateTime.Now.Date.AddDays(-1),
				OrderNo = 1,
				Forms = new List<Form>(),
				Patient =
					new Patient {Caption = "Subj 027", PatientNumber = 27}
			};
			visitRepository
				.Setup(r => r.GetDailyVisits(CommonEmulatedUserName, DateTime.Now.Date.AddDays(-1)))
				.Returns(new List<Visit> {
					visit
				});
			//Act
			var result = controller.Index(DateTime.Now.Date.AddDays(-1));

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var partialResult = result as ViewResultBase;
			Assert.That(partialResult.Model, Is.Not.Null);
			var model = partialResult.Model as DailyScheduleViewModel;

			visitRepository.Verify(r => r.GetDailyVisits(CommonEmulatedUserName, DateTime.Now.Date.AddDays(-1)), Times.Once());

			Assert.That(model.ScheduledVisits, Is.Not.Null);
			Assert.That(model.ScheduledVisits.Count, Is.EqualTo(1));
			var visitModel = model.ScheduledVisits[0];
			Assert.That(visitModel.VisitCaption, Is.EqualTo(visit.Caption));
			Assert.That(visitModel.PatientInitials, Is.EqualTo(visit.Patient.PatientInitials));
			Assert.That(visitModel.PatientNumber, Is.EqualTo(visit.Patient.PatientNumber));
			Assert.That(visitModel.VisitState, Is.EqualTo(visit.IsFullyCompleted ? "Completed" : "Incomplete"));
		}

		[Test]
		public void SummaryViewTest() {
			//Arrange
			EmulateControllerContext(controller, false);
			visitRepository.Setup(x => x.GetDailyVisits(CommonEmulatedUserName, DateTime.Today)).Returns(new List<Visit>());

			//Act
			var result = controller.Summary();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var partialResult = result as ViewResultBase;
			Assert.That(partialResult.ViewName, Is.EqualTo("_Summary"));
			Assert.That(partialResult.Model, Is.Not.Null);
		}

		[Test]
		public void SummaryModelTest() {
			//Arrange
			EmulateControllerContext(controller, false);
			visitRepository.Setup(x => x.GetDailyVisits(CommonEmulatedUserName, DateTime.Today)).Returns(new List<Visit>());

			//Act
			var result = controller.Summary();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var partialResult = result as ViewResultBase;
			Assert.That(partialResult.Model, Is.Not.Null);
			Assert.That(partialResult.Model is DailyScheduleViewModel);
		}
	}
}
