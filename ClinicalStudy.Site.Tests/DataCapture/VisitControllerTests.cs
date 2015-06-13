using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.ModelBuilders;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.DataCapture.Controllers;
using ClinicalStudy.Site.Areas.DataCapture.Models;
using ClinicalStudy.Site.Areas.DataCapture.Models.Shared;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Site.Tests.DataCapture {
	[TestFixture]
	public class VisitControllerTests : ControllerTestsBase {
		private Mock<IVisitRepository> repository = null;
		private Mock<IClinicalStudyDesignFactory> clinicalDesignFactory = null;
		private VisitController controller = null;

		[SetUp]
		public void Setup() {
			repository = new Mock<IVisitRepository>();
			clinicalDesignFactory = new Mock<IClinicalStudyDesignFactory>();
			controller = new VisitController(repository.Object, clinicalDesignFactory.Object);
			
		}

		[Test]
		public void GetEditVisit_CorrectView_Test() {
			//Arrange
			EmulateControllerContext(controller, false);
			int patientNumber = 88;
			string visitName = "BaseVisit";
			repository.Setup(r => r.GetVisitByPatientNumberAndVisitName(patientNumber, visitName)).Returns(new Visit() {
				Id = 222,
				Caption = visitName,
				IsCompleted = false
			});

			//Act
			var result = controller.ViewEditVisit(null, patientNumber, visitName);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditVisit"));
			Assert.That(viewResultBase.Model is VisitViewModel);
			var model = viewResultBase.Model as VisitViewModel;
			Assert.That(model.Id, Is.EqualTo(222));
			repository.Verify(r => r.GetVisitByPatientNumberAndVisitName(patientNumber, visitName), Times.Once());
		}

		[Test]
		public void ViewEditVisit_ExplicitViewCompleted_CorrectView_Test() {
			//Arrange
			EmulateControllerContext(controller, false);
			int patientNumber = 88;
			string visitName = "BaseVisit";
			repository.Setup(r => r.GetVisitByPatientNumberAndVisitName(patientNumber, visitName)).Returns(new Visit() {
				Id = 222,
				Caption = visitName,
				IsCompleted = true
			});

			//Act
			var result = controller.ViewEditVisit(null, patientNumber, visitName);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewVisit"));
		}
		[Test]
		public void ViewEditVisit_ImplicitViewCompleted_CorrectView_Test() {
			//Arrange
			EmulateControllerContext(controller, false);
			int patientNumber = 88;
			string visitName = "BaseVisit";
			repository.Setup(r => r.GetVisitByPatientNumberAndVisitName(patientNumber, visitName)).Returns(new Visit() {
				Id = 222,
				Caption = visitName,
				IsCompleted = true
			});

			//Act
			var result = controller.ViewEditVisit(false, patientNumber, visitName);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewVisit"));
		}


		[Test]
		public void ViewEditVisit_ImplicitEditCompleted_CorrectView_Test() {
			//Arrange
			EmulateControllerContext(controller, false);
			int patientNumber = 88;
			string visitName = "BaseVisit";
			repository.Setup(r => r.GetVisitByPatientNumberAndVisitName(patientNumber, visitName)).Returns(new Visit() {
				Id = 222,
				Caption = visitName,
				IsCompleted = true
			});

			//Act
			var result = controller.ViewEditVisit(true, patientNumber, visitName);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditVisit"));
		}

		[Test]
		public void GetEditVisit_CorrectView_Ajax_Test() {
			//Arrange
			EmulateControllerContext(controller, true);
			int patientNumber = 88;
			string visitName = "BaseVisit";
			repository.Setup(r => r.GetVisitByPatientNumberAndVisitName(patientNumber, visitName)).Returns(new Visit() {
				Id = 222,
				Caption = visitName
			});

			//Act
			var result = controller.ViewEditVisit(null, patientNumber, visitName);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditVisit"));
			Assert.That(viewResultBase.Model is VisitViewModel);
			var model = viewResultBase.Model as VisitViewModel;
			Assert.That(model.Id, Is.EqualTo(222));
			repository.Verify(r => r.GetVisitByPatientNumberAndVisitName(patientNumber, visitName), Times.Once());
		}

		[Test]
		public void GetEditVisit_CorrectBinding_Test() {
			//Arrange
			EmulateControllerContext(controller, false);
			int patientNumber = 88;

			var visit = new Visit {
				Id = 222,
				Caption = "TestVisit",
				VisitType = VisitType.Baseline,
				OrderNo = 2,
				ExpectedVisitDate = DateTime.Today.AddDays(4),
				VisitDate = DateTime.Today.AddDays(4),
				VisitTime = DateTime.Now
			};

			repository.Setup(r => r.GetVisitByPatientNumberAndVisitName(patientNumber, visit.Caption)).Returns(visit);

			//Act
			var result = controller.ViewEditVisit(null, patientNumber, visit.Caption);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.Model is VisitViewModel);
			var model = viewResultBase.Model as VisitViewModel;
			Assert.That(model.Id, Is.EqualTo(visit.Id));
			Assert.That(model.Caption, Is.EqualTo(visit.Caption));
			Assert.That(model.VisitType, Is.EqualTo(visit.VisitType));
			Assert.That(model.OrderNo, Is.EqualTo(visit.OrderNo));
			Assert.That(model.ExpectedVisitDate, Is.EqualTo(visit.ExpectedVisitDate));
			Assert.That(model.VisitDate, Is.EqualTo(visit.VisitDate));
			Assert.That(model.VisitTime, Is.EqualTo(visit.VisitTime));


			repository.Verify(r => r.GetVisitByPatientNumberAndVisitName(patientNumber, visit.Caption), Times.Once());
		}

		[Test]
		public void GetEditNonExistingVisitTest() {
			//Arrange
			int patientNumber = 88;
			string visitName = "BaseVisit";
			repository.Setup(r => r.GetVisitByPatientNumberAndVisitName(patientNumber, visitName)).Returns((Visit) null);

			//Act
			var result = controller.ViewEditVisit(null, patientNumber, visitName);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
			Assert.That(viewResultBase.Model is ErrorViewModel);
			var model = viewResultBase.Model as ErrorViewModel;
			Assert.That(model.Caption, Is.EqualTo("Visit is not found"));
			repository.Verify(r => r.GetVisitByPatientNumberAndVisitName(patientNumber, visitName), Times.Once());
		}

		[Test]
		public void PostEditExistingVisitTest() {
			//Arrange
			EmulateControllerContext(controller, false);

			var model = new VisitViewModel {
				Id = 12,
				Caption = "TestVisit",
				VisitDate = new DateTime(2012, 1, 24),
				VisitTime = new DateTime(2012, 1, 24, 10, 10, 10),
				ExpectedVisitDate = new DateTime(2012, 1, 25)
			};

			//here we return "visit before editing" - this data should be overwritten from model
			repository.Setup(r => r.GetByKey(It.IsAny<int>())).Returns(
				new Visit() {
					Id = 12,
					Caption = "TestVisit",
					VisitDate = new DateTime(2012, 1, 04),
					VisitTime = new DateTime(2012, 1, 04, 10, 10, 10),
					ExpectedVisitDate = new DateTime(2012, 1, 05),
					Patient = new Patient {PatientNumber = 12}
				}
				);
			//as checking of saved visit is quite complicated, we will save the passed object and inspect it later
			Visit savedVisit = null;
			repository.Setup(r => r.Edit(It.IsAny<Visit>())).Callback<Visit>(v => savedVisit = v);


			//Act
			var result = controller.EditVisit(model);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is RedirectToRouteResult);

			repository.Verify(r => r.GetByKey(model.Id), Times.Once());
			repository.Verify(r => r.Edit(It.IsAny<Visit>()), Times.Once());
			repository.Verify(r => r.Save(), Times.Once());
			Assert.That(savedVisit, Is.Not.Null);
			Assert.That(savedVisit.Caption, Is.EqualTo("TestVisit"));
			Assert.That(savedVisit.VisitDate, Is.EqualTo(new DateTime(2012, 1, 24)));
			Assert.That(savedVisit.VisitTime, Is.EqualTo(new DateTime(2012, 1, 24, 10, 10, 10)));
			Assert.That(savedVisit.ExpectedVisitDate, Is.EqualTo(new DateTime(2012, 1, 25)));
		}

		[Test]
		public void PostEditNonExistingVisitTest() {
			//Arrange
			var model = new VisitViewModel {
				Id = 12,
				Caption = null,
				VisitDate = new DateTime(2012, 1, 24),
				VisitTime = new DateTime(2012, 1, 24, 10, 10, 10),
				ExpectedVisitDate = new DateTime(2012, 1, 25)
			};

			//here we return "visit before editing" - this data should be overwritten from model
			repository.Setup(r => r.GetByKey(It.IsAny<int>())).Returns((Visit) null);

			//as checking of saved visit is quite complicated, we will save the passed object and inspect it later
			Visit savedVisit = null;
			repository.Setup(r => r.Edit(It.IsAny<Visit>())).Callback<Visit>(v => savedVisit = v);


			//Act
			var result = controller.EditVisit(model);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
			Assert.That(viewResultBase.Model is ErrorViewModel);
			var errorModel = viewResultBase.Model as ErrorViewModel;
			Assert.That(errorModel.Caption, Is.EqualTo("Visit is not found"));
		}

		[Test]
		public void CreateAdverseEventTest() {
			//Arrange
			var studyDesign = new Mock<IClinicalStudyDesign>();
			clinicalDesignFactory.Setup(factory => factory.Create()).Returns(studyDesign.Object);
			EmulateControllerContext(controller, false);

			var visit = new Visit() {
				Id = 16,
				Caption = "Adverse Event 4",
				Patient = new Patient() {PatientNumber = 25}
			};


			studyDesign.Setup(sd => sd.AddAdverseEventVisit(3)).Returns(visit);


			//Act
			var result = controller.CreateAdverseEvent(3);

			//Assert
			studyDesign.Verify(sd => sd.AddAdverseEventVisit(3), Times.Once());
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ContentResult);
			var contentResult = result as ContentResult;
			Assert.That(contentResult.Content, Is.EqualTo(visit.Caption));
		}

		[Test]
		public void CreateAdverseEventErrorTest() {
			//Arrange
			var studyDesign = new Mock<IClinicalStudyDesign>();
			clinicalDesignFactory.Setup(factory => factory.Create()).Returns(studyDesign.Object);
			EmulateControllerContext(controller, false);


			//Act
			var result = controller.CreateAdverseEvent(3);

			//Assert
			studyDesign.Verify(sd => sd.AddAdverseEventVisit(3), Times.Once());
			Assert.That(result, Is.Null);
		}


		[Test]
		public void VisitDataContainerView() {
			//Arrange
			EmulateControllerContext(controller, false);

			repository.Setup(r => r.GetVisitByPatientNumberAndVisitName(It.IsAny<int>(), It.IsAny<string>())).Returns(new Visit());

			//Act
			var result = controller.VisitDataContainer(25, "Baseline", null);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var view = result as ViewResultBase;
			var model = view.Model;
			Assert.That(model, Is.Not.Null);
		}


		[Test]
		public void VisitDataContainerModel() {
			//Arrange
			EmulateControllerContext(controller, false);
			var visit = new Visit {
				Id = 125,
				Caption = "Baseline",
				Forms = new List<Form> {
					new Form {Id = 100, Caption = "Vitals", OrderNo = 1},
					new Form {Id = 200, Caption = "Demographic", OrderNo = 0}
				}
			};
			repository
				.Setup(r => r.GetVisitByPatientNumberAndVisitName(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(visit);

			//Act
			var result = controller.VisitDataContainer(25, "Baseline", null);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var view = result as ViewResultBase;
			var model = view.Model as DataContainerViewModel;
			Assert.That(model, Is.Not.Null);

			Assert.That(model.Id, Is.EqualTo(visit.Id));
			Assert.That(model.PatientNumber, Is.EqualTo(25));
			Assert.That(model.SelectedVisitName, Is.EqualTo(visit.Caption));
			Assert.That(model.Children, Is.Not.Null);
			Assert.That(model.Children.Count, Is.EqualTo(2));
			Assert.That(model.Children[0].Id, Is.EqualTo(200));
			Assert.That(model.Children[0].Caption, Is.EqualTo("Demographic"));

			Assert.That(model.Children[1].Id, Is.EqualTo(100));
			Assert.That(model.Children[1].Caption, Is.EqualTo("Vitals"));
		}
	}
}
