using System.Collections.Generic;
using System.Web.Mvc;
using ClinicalStudy.DomainModel.DTOs;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.Analytics.Controllers;
using ClinicalStudy.Site.Areas.Analytics.Models.Charts;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Site.Tests.Analytics {
	[TestFixture]
	public class ChartsControllerTests : ControllerTestsBase {
		protected Mock<IPatientRepository> PatientRepository;
		protected Mock<IFormRepository> FormRepository;
		protected Mock<IQueryRepository> QueryRepository;
		protected ChartsController Controller;

		protected static readonly string TestHospitalCaption1 = "Test Hospital";
		protected static readonly string AnotherTestHospitalCaption = "Another Test Hospital";

		[SetUp]
		public void Setup() {
			PatientRepository = new Mock<IPatientRepository>();
			FormRepository = new Mock<IFormRepository>();
			QueryRepository = new Mock<IQueryRepository>();
			Controller = new ChartsController(PatientRepository.Object, FormRepository.Object, QueryRepository.Object);
		}

		public class PatientsPerState : ChartsControllerTests {
			[Test]
			public void PatientsPerVisitTest() {
				//Arrange
				EmulateControllerContext(Controller, true);
				IList<PatientStateDto> patientsData = new List<PatientStateDto> {
					new PatientStateDto() {ClinicId = 11, DoctorId = 22, VisitType = VisitType.None},
					new PatientStateDto() {ClinicId = 11, DoctorId = 22, VisitType = VisitType.Baseline},
					new PatientStateDto() {ClinicId = 12, DoctorId = 33, VisitType = VisitType.Day1},
					new PatientStateDto() {ClinicId = 12, DoctorId = 34, VisitType = VisitType.Day1},
					new PatientStateDto() {ClinicId = 12, DoctorId = 34, VisitType = VisitType.Day10}

				};
				PatientRepository.Setup(r => r.GetPatientsStateData()).Returns(patientsData);

				//Act
				var result = Controller.PatientsPerVisit();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is PartialViewResult);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_PatientsPerVisit"));
				Assert.That(viewResultBase.Model is IList<PatientStateViewModel>, "Unexpected model type");
				var model = viewResultBase.Model as IList<PatientStateViewModel>;


				PatientRepository.Verify(r => r.GetPatientsStateData(), Times.Once());
				Assert.That(model.Count, Is.EqualTo(4));
				Assert.That(model[0].StudyState, Is.EqualTo("Enlisted"));
				Assert.That(model[0].PatientsNumber, Is.EqualTo(1));
				Assert.That(model[1].StudyState, Is.EqualTo("Baseline"));
				Assert.That(model[1].PatientsNumber, Is.EqualTo(1));
				Assert.That(model[2].StudyState, Is.EqualTo("1st Day"));
				Assert.That(model[2].PatientsNumber, Is.EqualTo(2));
				Assert.That(model[3].StudyState, Is.EqualTo("Completed"));
				Assert.That(model[3].PatientsNumber, Is.EqualTo(1));
			}


			[Test]
			public void PatientsPerVisitPerClinicTest() {
				//Arrange
				EmulateControllerContext(Controller, true);
				IList<PatientStateDto> patientsData = new List<PatientStateDto> {
					new PatientStateDto() {ClinicId = 11, ClinicName = "Another Hospital", DoctorId = 22, VisitType = VisitType.None},
					new PatientStateDto()
					{ClinicId = 11, ClinicName = "Another Hospital", DoctorId = 22, VisitType = VisitType.Baseline},
					new PatientStateDto() {ClinicId = 12, ClinicName = TestHospitalCaption1, DoctorId = 33, VisitType = VisitType.Day1},
					new PatientStateDto() {ClinicId = 12, ClinicName = TestHospitalCaption1, DoctorId = 34, VisitType = VisitType.Day1},
					new PatientStateDto() {ClinicId = 12, ClinicName = TestHospitalCaption1, DoctorId = 34, VisitType = VisitType.Day10}

				};
				PatientRepository.Setup(r => r.GetPatientsStateData()).Returns(patientsData);
				int colorNumber = 2;

				//Act
				var result = Controller.PatientsPerVisitPerClinic("Baseline", colorNumber);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is PartialViewResult);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_PatientsPerVisitPerClinic"));
				Assert.That(viewResultBase.Model is PatientStateList, "Unexpected model type");
				var model = viewResultBase.Model as PatientStateList;


				PatientRepository.Verify(r => r.GetPatientsStateData(), Times.Once());
				Assert.That(model.Count, Is.EqualTo(1));
				Assert.That(model.PatientState, Is.EqualTo("Baseline"));
				Assert.That(model.PatientStateColorNumber, Is.EqualTo(colorNumber));
				Assert.That(model.ClinicName, Is.Null);
				Assert.That(model[0].StudyState, Is.EqualTo("Baseline"));
				Assert.That(model[0].EntityCaption, Is.EqualTo("Another Hospital"));
				Assert.That(model[0].PatientsNumber, Is.EqualTo(1));
			}


			[Test]
			public void PatientsPerVisitPerDorctorTest() {
				//Arrange
				EmulateControllerContext(Controller, true);
				IList<PatientStateDto> patientsData = new List<PatientStateDto> {
					new PatientStateDto()
					{ClinicId = 12, ClinicName = TestHospitalCaption1, DoctorId = 33, DoctorName = "Brown", VisitType = VisitType.Day1},
					new PatientStateDto()
					{ClinicId = 12, ClinicName = TestHospitalCaption1, DoctorId = 33, DoctorName = "Brown", VisitType = VisitType.Day10},
					new PatientStateDto()
					{ClinicId = 12, ClinicName = TestHospitalCaption1, DoctorId = 34, DoctorName = "Davis", VisitType = VisitType.Day1},
					new PatientStateDto()
					{ClinicId = 12, ClinicName = TestHospitalCaption1, DoctorId = 34, DoctorName = "Davis", VisitType = VisitType.Day1},

				};
				PatientRepository.Setup(r => r.GetPatientsStateDataForClinic(TestHospitalCaption1)).Returns(patientsData);
				int fixedColorNumber = 3;
				//Act
				var result = Controller.PatientsPerVisitPerDoctor("1st Day", TestHospitalCaption1, fixedColorNumber);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is PartialViewResult);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_PatientsPerVisitPerDoctor"));
				Assert.That(viewResultBase.Model is PatientStateList, "Unexpected model type");
				var model = viewResultBase.Model as PatientStateList;


				PatientRepository.Verify(r => r.GetPatientsStateDataForClinic(TestHospitalCaption1), Times.Once());
				Assert.That(model.Count, Is.EqualTo(2));
				Assert.That(model.PatientState, Is.EqualTo("1st Day"));
				Assert.That(model.PatientStateColorNumber, Is.EqualTo(fixedColorNumber));
				Assert.That(model.ClinicName, Is.EqualTo(TestHospitalCaption1));
				Assert.That(model[0].StudyState, Is.EqualTo("1st Day"));
				Assert.That(model[0].EntityCaption, Is.EqualTo("Brown"));
				Assert.That(model[0].PatientsNumber, Is.EqualTo(1));
				Assert.That(model[1].StudyState, Is.EqualTo("1st Day"));
				Assert.That(model[1].EntityCaption, Is.EqualTo("Davis"));
				Assert.That(model[1].PatientsNumber, Is.EqualTo(2));
			}
		}


		public class UnfinishedCrfs : ChartsControllerTests {
			
			[Test]
			public void UnfinishedCrfsTest() {
				//Arrange
				EmulateControllerContext(Controller, true);

				IList<FormDto> formsData = new List<FormDto> {
					new FormDto(){ClinicName = TestHospitalCaption1, DoctorName = "Smith", FormType = FormType.Demographics},
					new FormDto(){ClinicName = TestHospitalCaption1, DoctorName = "Bowman", FormType = FormType.Demographics},
					new FormDto(){ClinicName = TestHospitalCaption1, FormType = FormType.Vitals},
					new FormDto(){ClinicName = AnotherTestHospitalCaption, FormType = FormType.Demographics},

				};
				FormRepository.Setup(r => r.GetUnfinishedCrfs()).Returns(formsData);

				//Act
				var result = Controller.UnfinishedCrfsPerClinic();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is PartialViewResult);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_UnfinishedCrfsPerClinic"));
				Assert.That(viewResultBase.Model is UnfinishedCrfsList, "Unexpected model type");
				var model = viewResultBase.Model as UnfinishedCrfsList;


				FormRepository.Verify(r => r.GetUnfinishedCrfs(), Times.Once());
				Assert.That(model.Count, Is.EqualTo(3));


                Assert.That(model[0].EntityName, Is.EqualTo(AnotherTestHospitalCaption));
                Assert.That(model[0].FormType, Is.EqualTo("Demographics"));
                Assert.That(model[0].FormsNumber, Is.EqualTo(1));

				Assert.That(model[1].EntityName, Is.EqualTo(TestHospitalCaption1));
				Assert.That(model[1].FormType, Is.EqualTo("Demographics"));
				Assert.That(model[1].FormsNumber, Is.EqualTo(2));

				Assert.That(model[2].EntityName, Is.EqualTo(TestHospitalCaption1));
				Assert.That(model[2].FormType, Is.EqualTo("Vitals"));
				Assert.That(model[2].FormsNumber, Is.EqualTo(1));

			}
			[Test]
			public void UnfinishedCrfsPerDoctorTest() {
				//Arrange
				EmulateControllerContext(Controller, true);

				IList<FormDto> formsData = new List<FormDto> {
					new FormDto(){ClinicName = TestHospitalCaption1, DoctorName = "Smith", FormType = FormType.Demographics},
					new FormDto(){ClinicName = TestHospitalCaption1, DoctorName = "Smith", FormType = FormType.Demographics},
					new FormDto(){ClinicName = TestHospitalCaption1, DoctorName = "Bowman", FormType = FormType.Demographics},
					new FormDto(){ClinicName = TestHospitalCaption1, DoctorName = "Smith", FormType = FormType.Vitals},
					new FormDto(){ClinicName = AnotherTestHospitalCaption, DoctorName = "Smith", FormType = FormType.Demographics},

				};
				FormRepository.Setup(r => r.GetUnfinishedCrfs()).Returns(formsData);

				//Act
				var result = Controller.UnfinishedCrfsPerDoctor(TestHospitalCaption1);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is PartialViewResult);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_UnfinishedCrfsPerDoctor"));
				Assert.That(viewResultBase.Model is UnfinishedCrfsList, "Unexpected model type");
				var model = viewResultBase.Model as UnfinishedCrfsList;


				FormRepository.Verify(r => r.GetUnfinishedCrfs(), Times.Once());
				Assert.That(model.Count, Is.EqualTo(3));

                Assert.That(model[0].EntityName, Is.EqualTo("Bowman"));
                Assert.That(model[0].FormType, Is.EqualTo("Demographics"));
                Assert.That(model[0].FormsNumber, Is.EqualTo(1));

				Assert.That(model[1].EntityName, Is.EqualTo("Smith"));
				Assert.That(model[1].FormType, Is.EqualTo("Demographics"));
				Assert.That(model[1].FormsNumber, Is.EqualTo(2));

				Assert.That(model[2].EntityName, Is.EqualTo("Smith"));
				Assert.That(model[2].FormType, Is.EqualTo("Vitals"));
				Assert.That(model[2].FormsNumber, Is.EqualTo(1));
			}
		}



		public class QueriesPerCrf : ChartsControllerTests {

		}
	}
}
