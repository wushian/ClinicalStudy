using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Castle.Components.DictionaryAdapter;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.DTOs;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.Enums.Answers;
using ClinicalStudy.DomainModel.Enums.Display;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.Interface.FormData;
using ClinicalStudy.Site.Areas.Analytics.Controllers;
using ClinicalStudy.Site.Areas.Analytics.Models.Analytics;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Site.Tests.Analytics {
	[TestFixture]
	[SetCulture("en-US")]
	[SetUICulture("en-US")]
	public class AnalyticsControllerTest {
		[TestFixture]
		public class HappinessTest : ControllerTestsBase {
			[TestCase(false, "Happiness")]
			[TestCase(true, "_Happiness")]
			public void HappinessAnalytics_View(bool async, string viewName) {
				//Arrange
				Mock<IPatientRepository> pRepository;
				Mock<IDemographicFormDataRepository> dfdRepository;
				Mock<IHappinessFormDataRepository> hfdRepository;

				var dto = CreateHappinessChangeDto(10, 20, 30, 40, 50, 60);

				var dtos = new List<HappinessChangeDto> {dto};
				var races = new Dictionary<int, int> {{dto.PatientId, (int) Race.Asian}};
				var genders = new Dictionary<int, int> {{dto.PatientId, (int) Gender.Female}};
				var happinessChanges = new Dictionary<int, decimal> {{dto.PatientId, 0.2m}};

				AnalyticsController controller;
				ArrangeHappinessTestData(dtos, races, genders, happinessChanges, out controller, out pRepository, out dfdRepository,
				                         out hfdRepository);
				EmulateControllerContext(controller, async);

				//Act
				var result = controller.Happiness();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo(viewName));
			}

			[Test]
			public void HappinessPartial() {
				//Arrange
				Mock<IPatientRepository> pRepository;
				Mock<IDemographicFormDataRepository> dfdRepository;
				Mock<IHappinessFormDataRepository> hfdRepository;

				var dto = CreateHappinessChangeDto(10, 20, 30, 40, 50, 60);

				var dtos = new List<HappinessChangeDto> {dto};
				var races = new Dictionary<int, int> {{dto.PatientId, (int) Race.Asian}};
				var genders = new Dictionary<int, int> {{dto.PatientId, (int) Gender.Female}};
				var happinessChanges = new Dictionary<int, decimal> {{dto.PatientId, 0.2m}};

				AnalyticsController controller;
				ArrangeHappinessTestData(dtos, races, genders, happinessChanges, out controller, out pRepository, out dfdRepository,
				                         out hfdRepository);

				//Act
				var result = controller.HappinessPartial();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is PartialViewResult);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_HappinessPivotGrid"));
			}

			[Test]
			public void HappinessAnalytics_MultipleRecords() {
				//Arrange
				Mock<IPatientRepository> pRepository;
				Mock<IDemographicFormDataRepository> dfdRepository;
				Mock<IHappinessFormDataRepository> hfdRepository;

				var dto1 = CreateHappinessChangeDto(10, 20, 30, 40, 50, 60);
				var dto2 = CreateHappinessChangeDto(10, 20, 31, 41, 51, 61);
				var dto3 = CreateHappinessChangeDto(10, 21, 32, 42, 52, 62);
				var dto4 = CreateHappinessChangeDto(11, 22, 33, 43, 53, 63);

				var dtos = new List<HappinessChangeDto> {dto1, dto2, dto3, dto4};
				var races = new Dictionary<int, int> {
					{dto1.PatientId, (int) Race.Asian},
					{dto2.PatientId, (int) Race.AfricanAmerican},
					{dto3.PatientId, (int) Race.AmericanIndian},
					{dto4.PatientId, (int) Race.White}
				};
				var genders = new Dictionary<int, int> {
					{dto1.PatientId, (int) Gender.Female},
					{dto2.PatientId, (int) Gender.Male},
					{dto3.PatientId, (int) Gender.Male},
					{dto4.PatientId, (int) Gender.Female}
				};
				var happinessChanges = new Dictionary<int, decimal> {
					{dto1.PatientId, 0.1m},
					{dto2.PatientId, 0.2m},
					{dto3.PatientId, 0.3m},
					{dto4.PatientId, 0.4m}
				};

				AnalyticsController controller;
				ArrangeHappinessTestData(dtos, races, genders, happinessChanges, out controller, out pRepository, out dfdRepository,
				                         out hfdRepository);
				EmulateControllerContext(controller, false);

				//Act
				var result = controller.Happiness();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("Happiness"));
				Assert.That(viewResultBase.Model is List<PatientsHappinessChangeViewModel>);
				var model = viewResultBase.Model as List<PatientsHappinessChangeViewModel>;
				Assert.That(model.Count, Is.EqualTo(4));

				AssertHappinessChangeViewModel(model[0], dto1, Race.Asian, Gender.Female, 0.1m, pRepository, dfdRepository,
				                               hfdRepository);
				AssertHappinessChangeViewModel(model[1], dto2, Race.AfricanAmerican, Gender.Male, 0.2m, pRepository, dfdRepository,
				                               hfdRepository);
				AssertHappinessChangeViewModel(model[2], dto3, Race.AmericanIndian, Gender.Male, 0.3m, pRepository, dfdRepository,
				                               hfdRepository);
				AssertHappinessChangeViewModel(model[3], dto4, Race.White, Gender.Female, 0.4m, pRepository, dfdRepository,
				                               hfdRepository);
			}

			private void AssertHappinessChangeViewModel(PatientsHappinessChangeViewModel data, HappinessChangeDto dto, Race race,
			                                            Gender sex, decimal hChange,
			                                            Mock<IPatientRepository> pRepository,
			                                            Mock<IDemographicFormDataRepository> dfdRepository,
			                                            Mock<IHappinessFormDataRepository> hfdRepository) {
				Assert.That(data.ClinicId, Is.EqualTo(dto.ClinicId));
				Assert.That(data.DoctorId, Is.EqualTo(dto.DoctorId));
				Assert.That(data.ClinicName, Is.EqualTo(dto.ClinicName));
				Assert.That(data.DoctorName, Is.EqualTo(dto.DoctorName));
				Assert.That(data.PatientId, Is.EqualTo(dto.PatientId));
				Assert.That(data.PatientNumber, Is.EqualTo(dto.PatientNumber));
				Assert.That(data.Race, Is.EqualTo(EnumHelper.GetDescription(race)));
				Assert.That(data.Gender, Is.EqualTo(EnumHelper.GetDescription(sex)));
				Assert.That(data.HappinessChange, Is.EqualTo(hChange));

				pRepository.Verify(r => r.GetHappinessChangeData(), Times.Once());
				dfdRepository.Verify(r => r.GetFormDataByFormId(dto.DemographicFormId), Times.Once());
				hfdRepository.Verify(r => r.GetFormDataByFormId(dto.HappinessDay1FormId), Times.Once());
				hfdRepository.Verify(r => r.GetFormDataByFormId(dto.HappinessDay10FormId), Times.Once());
			}


			private void ArrangeHappinessTestData(List<HappinessChangeDto> dtos, Dictionary<int, int> races,
			                                      Dictionary<int, int> genders, Dictionary<int, decimal> happinessChanges,
			                                      out AnalyticsController controller, out Mock<IPatientRepository> pRepository,
			                                      out Mock<IDemographicFormDataRepository> dfdRepository,
			                                      out Mock<IHappinessFormDataRepository> hfdRepository) {
				pRepository = new Mock<IPatientRepository>();
				dfdRepository = new Mock<IDemographicFormDataRepository>();
				hfdRepository = new Mock<IHappinessFormDataRepository>();
				controller = new AnalyticsController(pRepository.Object, null, dfdRepository.Object, hfdRepository.Object, null,
				                                     null);

				pRepository.Setup(r => r.GetHappinessChangeData()).Returns(dtos);

				foreach (var dto in dtos) {
					var demog = new DemographicFormData {
						Race = new Question {Value = races[dto.PatientId].ToString()},
						Sex = new Question {Value = genders[dto.PatientId].ToString()}
					};
					dfdRepository.Setup(r => r.GetFormDataByFormId(dto.DemographicFormId)).Returns(demog);

					var hap1 = new HappinessFormData {
						HappinessLevel = new Question {Value = "25"}
					};
					var hap10 = new HappinessFormData {
						HappinessLevel = new Question {Value = (25 + (100*happinessChanges[dto.PatientId])).ToString()}
					};
					hfdRepository.Setup(r => r.GetFormDataByFormId(dto.HappinessDay1FormId)).Returns(hap1);
					hfdRepository.Setup(r => r.GetFormDataByFormId(dto.HappinessDay10FormId)).Returns(hap10);
				}
			}

			private HappinessChangeDto CreateHappinessChangeDto(int clinicId, int doctorId, int patId, int demogFormId,
			                                                    int happinessForm1Id, int happinessForm10Id) {
				return new HappinessChangeDto {
					ClinicId = clinicId,
					ClinicName = String.Format("Clinic{0}", clinicId),
					DoctorId = doctorId,
					DoctorFirstName = String.Format("First{0}", doctorId),
					DoctorLastName = String.Format("Last{0}", doctorId),
					PatientId = patId,
					PatientNumber = 10 + patId,
					DemographicFormId = demogFormId,
					HappinessDay1FormId = happinessForm1Id,
					HappinessDay10FormId = happinessForm10Id
				};
			}
		}

		[TestFixture]
		public class QueriesTest : ControllerTestsBase {
			[TestCase(false, "Queries")]
			[TestCase(true, "_Queries")]
			public void QueriesAnalytics_View(bool async, string viewName) {
				//Arrange
				var qRepository = new Mock<IQueryRepository>();
				var controller = new AnalyticsController(null, null, null, null, null, qRepository.Object);
				qRepository.Setup(r => r.GetQueriesReportData()).Returns(new List<QueryReportDto>());

				EmulateControllerContext(controller, async);

				//Act
				var result = controller.Queries();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo(viewName));
			}

			[Test]
			public void QueriesPartial() {
				//Arrange
				var qRepository = new Mock<IQueryRepository>();
				var controller = new AnalyticsController(null, null, null, null, null, qRepository.Object);
				qRepository.Setup(r => r.GetQueriesReportData()).Returns(new List<QueryReportDto>());

				//Act
				var result = controller.QueriesPartial();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is PartialViewResult);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_QueriesPivotGrid"));
			}

			[Test]
			public void Queries_CorrectViewModel() {
				//Arrange
				var qRepository = new Mock<IQueryRepository>();
				var controller = new AnalyticsController(null, null, null, null, null, qRepository.Object);
				qRepository.Setup(r => r.GetQueriesReportData()).Returns(new List<QueryReportDto>());

				EmulateControllerContext(controller, false);

				//Act
				var result = controller.Queries();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is List<QueriesAnalyticsViewModel>);
				var model = viewResultBase.Model as List<QueriesAnalyticsViewModel>;
				Assert.That(model.Count, Is.EqualTo(0));

				qRepository.Verify(r => r.GetQueriesReportData(), Times.Once());
			}

			[Test]
			public void Queries_CorrectViewModel_Mapping() {
				//Arrange
				var qRepository = new Mock<IQueryRepository>();
				var controller = new AnalyticsController(null, null, null, null, null, qRepository.Object);

				var dto = GetQueryReportDto("Clinic1", "Super", "Doctor", FormType.Happiness, "QuestionCaption", false);

				qRepository.Setup(r => r.GetQueriesReportData()).Returns(new List<QueryReportDto> {dto});

				EmulateControllerContext(controller, false);

				//Act
				var result = controller.Queries();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is List<QueriesAnalyticsViewModel>);
				var model = viewResultBase.Model as List<QueriesAnalyticsViewModel>;
				Assert.That(model.Count, Is.EqualTo(1));

				AssertQueriesAnalyticsViewModel(model[0], dto.ClinicName, dto.DoctorName, dto.FormType, dto.QuestionName, 1, 0);

				qRepository.Verify(r => r.GetQueriesReportData(), Times.Once());
			}

			[Test]
			public void Queries_ComplexTest() {
				//Arrange
				var qRepository = new Mock<IQueryRepository>();
				var controller = new AnalyticsController(null, null, null, null, null, qRepository.Object);

				var dtos = new List<QueryReportDto> {
					GetQueryReportDto("Clinic1", "Super", "Doctor", FormType.Happiness, "QuestionCaption", false),
					GetQueryReportDto("Clinic1", "Super", "Doctor", FormType.Happiness, "QuestionCaption", true),
					GetQueryReportDto("Clinic1", "Super", "Doctor", FormType.Happiness, "QuestionCaption", false),
					GetQueryReportDto("Clinic1", "Super", "Doctor", FormType.Happiness, "QuestionCaption", false),
					GetQueryReportDto("Clinic1", "Super", "Doctor", FormType.Happiness, "QuestionCaption", true),
					GetQueryReportDto("Clinic1", "Super", "Doctor", FormType.Happiness, "QuestionCaption1", false),
					GetQueryReportDto("Clinic1", "Super", "Doctor", FormType.Happiness, "QuestionCaption1", false),
					GetQueryReportDto("Clinic1", "Super", "Doctor", FormType.Inventory, "QuestionCaption", false),
					GetQueryReportDto("Clinic1", "Super", "Doctor", FormType.Demographics, "QuestionCaption", true),
					GetQueryReportDto("Clinic1", "Super1", "Doctor1", FormType.Happiness, "QuestionCaption", false),
					GetQueryReportDto("Clinic1", "Super1", "Doctor1", FormType.Happiness, "QuestionCaption", true),
					GetQueryReportDto("Clinic2", "Super2", "Doctor2", FormType.Electrocardiogram, "QuestionCaption", true),
					GetQueryReportDto("Clinic2", "Super3", "Doctor3", FormType.Vitals, "QuestionCaption", false)
				};

				qRepository.Setup(r => r.GetQueriesReportData()).Returns(new List<QueryReportDto>(dtos));

				EmulateControllerContext(controller, false);

				//Act
				var result = controller.Queries();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is List<QueriesAnalyticsViewModel>);
				var model = viewResultBase.Model as List<QueriesAnalyticsViewModel>;
				Assert.That(model.Count, Is.EqualTo(7));
				AssertQueriesAnalyticsViewModel(model[0], "Clinic1", "Super Doctor", FormType.Happiness, "QuestionCaption", 5, 2);
				AssertQueriesAnalyticsViewModel(model[1], "Clinic1", "Super Doctor", FormType.Happiness, "QuestionCaption1", 2, 0);
				AssertQueriesAnalyticsViewModel(model[2], "Clinic1", "Super Doctor", FormType.Inventory, "QuestionCaption", 1, 0);
				AssertQueriesAnalyticsViewModel(model[3], "Clinic1", "Super Doctor", FormType.Demographics, "QuestionCaption", 1, 1);
				AssertQueriesAnalyticsViewModel(model[4], "Clinic1", "Super1 Doctor1", FormType.Happiness, "QuestionCaption", 2, 1);
				AssertQueriesAnalyticsViewModel(model[5], "Clinic2", "Super2 Doctor2", FormType.Electrocardiogram, "QuestionCaption",
				                                1, 1);
				AssertQueriesAnalyticsViewModel(model[6], "Clinic2", "Super3 Doctor3", FormType.Vitals, "QuestionCaption", 1, 0);

				qRepository.Verify(r => r.GetQueriesReportData(), Times.Once());
			}

			private QueryReportDto GetQueryReportDto(string clinic, string doctorFirst, string doctorLast, FormType form,
			                                         string question, bool isOpen) {
				return new QueryReportDto {
					ClinicName = clinic,
					DoctorFirstName = doctorFirst,
					DoctorLastName = doctorLast,
					FormType = form,
					QuestionName = question,
					IsOpen = isOpen
				};
			}

			private void AssertQueriesAnalyticsViewModel(QueriesAnalyticsViewModel item, string clinic, string doctor,
			                                             FormType form, string question, int queries, int open) {
				Assert.That(item, Is.Not.Null);
				Assert.That(item.Clinic, Is.EqualTo(clinic));
				Assert.That(item.Doctor, Is.EqualTo(doctor));
				Assert.That(item.Crf, Is.EqualTo(EnumHelper.GetDescription(form)));
				Assert.That(item.Question, Is.EqualTo(question));
				Assert.That(item.Queries, Is.EqualTo(queries));
				Assert.That(item.OpenQueries, Is.EqualTo(open));
			}
		}

		[TestFixture]
		public class AdverseEventsTest : ControllerTestsBase {
			[TestCase(false, "AdverseEvents")]
			[TestCase(true, "_AdverseEvents")]
			public void AdverseEventsAnalytics_View(bool async, string viewName) {
				//Arrange
				var vRepository = new Mock<IVisitRepository>();
				var dfdRepository = new Mock<IDemographicFormDataRepository>();
				var aefdRepository = new Mock<IAdverseEventFormDataRepository>();
				var controller = new AnalyticsController(null, vRepository.Object, dfdRepository.Object, null, aefdRepository.Object,
				                                         null);
				vRepository.Setup(r => r.GetAeAnalyticsData()).Returns(new List<AeAnalyticsDto>());

				EmulateControllerContext(controller, async);

				//Act
				var result = controller.AdverseEvents();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo(viewName));
			}

			[Test]
			public void AdverseEventsPartial() {
				//Arrange
				var vRepository = new Mock<IVisitRepository>();
				var dfdRepository = new Mock<IDemographicFormDataRepository>();
				var aefdRepository = new Mock<IAdverseEventFormDataRepository>();
				var controller = new AnalyticsController(null, vRepository.Object, dfdRepository.Object, null, aefdRepository.Object,
				                                         null);
				vRepository.Setup(r => r.GetAeAnalyticsData()).Returns(new List<AeAnalyticsDto>());

				//Act
				var result = controller.AdverseEventsPartial();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is PartialViewResult);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_AdverseEventsPivotGrid"));
			}

			[Test]
			public void AdverseEvents_CorrectViewModel() {
				//Arrange
				var vRepository = new Mock<IVisitRepository>();
				var dfdRepository = new Mock<IDemographicFormDataRepository>();
				var aefdRepository = new Mock<IAdverseEventFormDataRepository>();
				var controller = new AnalyticsController(null, vRepository.Object, dfdRepository.Object, null, aefdRepository.Object,
				                                         null);
				vRepository.Setup(r => r.GetAeAnalyticsData()).Returns(new List<AeAnalyticsDto>());

				EmulateControllerContext(controller, false);

				//Act
				var result = controller.AdverseEvents();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is List<AdverseEventsAnalyticsViewModel>);
				var model = viewResultBase.Model as List<AdverseEventsAnalyticsViewModel>;
				Assert.That(model.Count, Is.EqualTo(0));

				vRepository.Verify(r => r.GetAeAnalyticsData(), Times.Once());
			}

			[Test]
			public void AdverseEvents_CorrectViewModel_Mapping() {
				//Arrange
				var vRepository = new Mock<IVisitRepository>();
				var dfdRepository = new Mock<IDemographicFormDataRepository>();
				var aefdRepository = new Mock<IAdverseEventFormDataRepository>();
				var controller = new AnalyticsController(null, vRepository.Object, dfdRepository.Object, null, aefdRepository.Object,
				                                         null);
				var demogForm =
					CreateTestCrf(FormType.Demographics, 111, (int) Race.Asian, (int) Gender.Female) as DemographicFormData;
				var aeForm =
					CreateTestCrf(FormType.AdverseEvent, 222, (int) AdverseEventIntensity.Moderate,
					              (int) AdverseEventRelanshionship.Probable) as AdverseEventFormData;

				var dto = CreateAeAnalyticsDto("Clinic1", "Super", "Doctor", 111, 222);

				vRepository.Setup(r => r.GetAeAnalyticsData()).Returns(new List<AeAnalyticsDto> {dto});
				dfdRepository.Setup(r => r.GetFormDataByFormId(111)).Returns(demogForm);
				aefdRepository.Setup(r => r.GetFormDataByFormId(222)).Returns(aeForm);

				EmulateControllerContext(controller, false);

				//Act
				var result = controller.AdverseEvents();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is List<AdverseEventsAnalyticsViewModel>);
				var model = viewResultBase.Model as List<AdverseEventsAnalyticsViewModel>;
				Assert.That(model.Count, Is.EqualTo(1));

				AssertAdverseEventsAnalyticsViewModel(model[0], dto.ClinicName, dto.DoctorName, Race.Asian, Gender.Female,
				                                      AdverseEventIntensity.Moderate, AdverseEventRelanshionship.Probable, 1,
				                                      vRepository);
			}

			[Test]
			public void AdverseEvents_ComplexTest() {
				//Arrange
				var vRepository = new Mock<IVisitRepository>();
				var dfdRepository = new Mock<IDemographicFormDataRepository>();
				var aefdRepository = new Mock<IAdverseEventFormDataRepository>();
				var controller = new AnalyticsController(null, vRepository.Object, dfdRepository.Object, null, aefdRepository.Object,
				                                         null);

				var dtos = new List<AeAnalyticsDto> {
					CreateAeAnalyticsDto("Clinic1", "Super", "Doctor", 100, 200),
					CreateAeAnalyticsDto("Clinic1", "Super", "Doctor", 101, 201),
					CreateAeAnalyticsDto("Clinic1", "Super", "Doctor", 102, 202),
					CreateAeAnalyticsDto("Clinic2", "Super1", "Doctor1", 103, 203),
					CreateAeAnalyticsDto("Clinic2", "Super1", "Doctor1", 104, 204),
					CreateAeAnalyticsDto("Clinic2", "Super1", "Doctor1", 105, 205),
					CreateAeAnalyticsDto("Clinic2", "Super2", "Doctor2", 106, 206),
					CreateAeAnalyticsDto("Clinic1", "Super", "Doctor", 107, 207),
					CreateAeAnalyticsDto("Clinic1", "Super4", "Doctor4", 108, 208),
					CreateAeAnalyticsDto("Clinic1", "Super4", "Doctor4", 109, 209),
					CreateAeAnalyticsDto("Clinic3", "Super5", "Doctor5", 110, 210),
					CreateAeAnalyticsDto("Clinic3", "Super6", "Doctor6", 111, 211),
					CreateAeAnalyticsDto("Clinic3", "Super7", "Doctor7", 112, 212)
				};

				vRepository.Setup(r => r.GetAeAnalyticsData()).Returns(new List<AeAnalyticsDto>(dtos));

				var formD1 = CreateTestCrf(FormType.Demographics, 100, (int) Race.Asian, (int) Gender.Male);
				var formD2 = CreateTestCrf(FormType.Demographics, 101, (int) Race.Asian, (int) Gender.Female);
				var formD3 = CreateTestCrf(FormType.Demographics, 102, (int) Race.Asian, (int) Gender.Female);
				var formD4 = CreateTestCrf(FormType.Demographics, 103, (int) Race.Asian, (int) Gender.Male);
				var formD5 = CreateTestCrf(FormType.Demographics, 104, (int) Race.Asian, (int) Gender.Female);
				var formD6 = CreateTestCrf(FormType.Demographics, 105, (int) Race.Asian, (int) Gender.Female);
				var formD7 = CreateTestCrf(FormType.Demographics, 106, (int) Race.HispanicLatino, (int) Gender.Female);
				var formD8 = CreateTestCrf(FormType.Demographics, 107, (int) Race.HispanicLatino, (int) Gender.Male);
				var formD9 = CreateTestCrf(FormType.Demographics, 108, (int) Race.White, (int) Gender.Female);
				var formD10 = CreateTestCrf(FormType.Demographics, 109, (int) Race.White, (int) Gender.Male);
				var formD11 = CreateTestCrf(FormType.Demographics, 110, (int) Race.AfricanAmerican, (int) Gender.Male);
				var formD12 = CreateTestCrf(FormType.Demographics, 111, (int) Race.AfricanAmerican, (int) Gender.Female);
				var formD13 = CreateTestCrf(FormType.Demographics, 112, (int) Race.AmericanIndian, (int) Gender.Male);

				var formAe1 = CreateTestCrf(FormType.AdverseEvent, 200, (int) AdverseEventIntensity.Moderate,
				                            (int) AdverseEventRelanshionship.Suspected);
				var formAe2 = CreateTestCrf(FormType.AdverseEvent, 201, (int) AdverseEventIntensity.Moderate,
				                            (int) AdverseEventRelanshionship.NotRelated);
				var formAe3 = CreateTestCrf(FormType.AdverseEvent, 202, (int) AdverseEventIntensity.Moderate,
				                            (int) AdverseEventRelanshionship.NotRelated);
				var formAe4 = CreateTestCrf(FormType.AdverseEvent, 203, (int) AdverseEventIntensity.Mild,
				                            (int) AdverseEventRelanshionship.Suspected);
				var formAe5 = CreateTestCrf(FormType.AdverseEvent, 204, (int) AdverseEventIntensity.Moderate,
				                            (int) AdverseEventRelanshionship.Unlikely);
				var formAe6 = CreateTestCrf(FormType.AdverseEvent, 205, (int) AdverseEventIntensity.Moderate,
				                            (int) AdverseEventRelanshionship.Unlikely);
				var formAe7 = CreateTestCrf(FormType.AdverseEvent, 206, (int) AdverseEventIntensity.Mild,
				                            (int) AdverseEventRelanshionship.NotRelated);
				var formAe8 = CreateTestCrf(FormType.AdverseEvent, 207, (int) AdverseEventIntensity.Moderate,
				                            (int) AdverseEventRelanshionship.NotRelated);
				var formAe9 = CreateTestCrf(FormType.AdverseEvent, 208, (int) AdverseEventIntensity.Severe,
				                            (int) AdverseEventRelanshionship.Suspected);
				var formAe10 = CreateTestCrf(FormType.AdverseEvent, 209, (int) AdverseEventIntensity.Mild,
				                             (int) AdverseEventRelanshionship.Probable);
				var formAe11 = CreateTestCrf(FormType.AdverseEvent, 210, (int) AdverseEventIntensity.Mild,
				                             (int) AdverseEventRelanshionship.Suspected);
				var formAe12 = CreateTestCrf(FormType.AdverseEvent, 211, (int) AdverseEventIntensity.Severe,
				                             (int) AdverseEventRelanshionship.Probable);
				var formAe13 = CreateTestCrf(FormType.AdverseEvent, 212, (int) AdverseEventIntensity.Moderate,
				                             (int) AdverseEventRelanshionship.NotRelated);

				dfdRepository.Setup(r => r.GetFormDataByFormId(formD1.Id)).Returns(formD1 as DemographicFormData);
				dfdRepository.Setup(r => r.GetFormDataByFormId(formD2.Id)).Returns(formD2 as DemographicFormData);
				dfdRepository.Setup(r => r.GetFormDataByFormId(formD3.Id)).Returns(formD3 as DemographicFormData);
				dfdRepository.Setup(r => r.GetFormDataByFormId(formD4.Id)).Returns(formD4 as DemographicFormData);
				dfdRepository.Setup(r => r.GetFormDataByFormId(formD5.Id)).Returns(formD5 as DemographicFormData);
				dfdRepository.Setup(r => r.GetFormDataByFormId(formD6.Id)).Returns(formD6 as DemographicFormData);
				dfdRepository.Setup(r => r.GetFormDataByFormId(formD7.Id)).Returns(formD7 as DemographicFormData);
				dfdRepository.Setup(r => r.GetFormDataByFormId(formD8.Id)).Returns(formD8 as DemographicFormData);
				dfdRepository.Setup(r => r.GetFormDataByFormId(formD9.Id)).Returns(formD9 as DemographicFormData);
				dfdRepository.Setup(r => r.GetFormDataByFormId(formD10.Id)).Returns(formD10 as DemographicFormData);
				dfdRepository.Setup(r => r.GetFormDataByFormId(formD11.Id)).Returns(formD11 as DemographicFormData);
				dfdRepository.Setup(r => r.GetFormDataByFormId(formD12.Id)).Returns(formD12 as DemographicFormData);
				dfdRepository.Setup(r => r.GetFormDataByFormId(formD13.Id)).Returns(formD13 as DemographicFormData);

				aefdRepository.Setup(r => r.GetFormDataByFormId(formAe1.Id)).Returns(formAe1 as AdverseEventFormData);
				aefdRepository.Setup(r => r.GetFormDataByFormId(formAe2.Id)).Returns(formAe2 as AdverseEventFormData);
				aefdRepository.Setup(r => r.GetFormDataByFormId(formAe3.Id)).Returns(formAe3 as AdverseEventFormData);
				aefdRepository.Setup(r => r.GetFormDataByFormId(formAe4.Id)).Returns(formAe4 as AdverseEventFormData);
				aefdRepository.Setup(r => r.GetFormDataByFormId(formAe5.Id)).Returns(formAe5 as AdverseEventFormData);
				aefdRepository.Setup(r => r.GetFormDataByFormId(formAe6.Id)).Returns(formAe6 as AdverseEventFormData);
				aefdRepository.Setup(r => r.GetFormDataByFormId(formAe7.Id)).Returns(formAe7 as AdverseEventFormData);
				aefdRepository.Setup(r => r.GetFormDataByFormId(formAe8.Id)).Returns(formAe8 as AdverseEventFormData);
				aefdRepository.Setup(r => r.GetFormDataByFormId(formAe9.Id)).Returns(formAe9 as AdverseEventFormData);
				aefdRepository.Setup(r => r.GetFormDataByFormId(formAe10.Id)).Returns(formAe10 as AdverseEventFormData);
				aefdRepository.Setup(r => r.GetFormDataByFormId(formAe11.Id)).Returns(formAe11 as AdverseEventFormData);
				aefdRepository.Setup(r => r.GetFormDataByFormId(formAe12.Id)).Returns(formAe12 as AdverseEventFormData);
				aefdRepository.Setup(r => r.GetFormDataByFormId(formAe13.Id)).Returns(formAe13 as AdverseEventFormData);

				EmulateControllerContext(controller, false);

				//Act
				var result = controller.AdverseEvents();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is List<AdverseEventsAnalyticsViewModel>);
				var model = viewResultBase.Model as List<AdverseEventsAnalyticsViewModel>;
				Assert.That(model.Count, Is.EqualTo(11));

				AssertAdverseEventsAnalyticsViewModel(model[0], "Clinic1", "Super Doctor", Race.Asian, Gender.Male,
				                                      AdverseEventIntensity.Moderate, AdverseEventRelanshionship.Suspected, 1,
				                                      vRepository);
				AssertAdverseEventsAnalyticsViewModel(model[1], "Clinic1", "Super Doctor", Race.Asian, Gender.Female,
				                                      AdverseEventIntensity.Moderate, AdverseEventRelanshionship.NotRelated, 2,
				                                      vRepository);
				AssertAdverseEventsAnalyticsViewModel(model[2], "Clinic2", "Super1 Doctor1", Race.Asian, Gender.Male,
				                                      AdverseEventIntensity.Mild, AdverseEventRelanshionship.Suspected, 1,
				                                      vRepository);
				AssertAdverseEventsAnalyticsViewModel(model[3], "Clinic2", "Super1 Doctor1", Race.Asian, Gender.Female,
				                                      AdverseEventIntensity.Moderate, AdverseEventRelanshionship.Unlikely, 2,
				                                      vRepository);
				AssertAdverseEventsAnalyticsViewModel(model[4], "Clinic2", "Super2 Doctor2", Race.HispanicLatino, Gender.Female,
				                                      AdverseEventIntensity.Mild, AdverseEventRelanshionship.NotRelated, 1,
				                                      vRepository);
				AssertAdverseEventsAnalyticsViewModel(model[5], "Clinic1", "Super Doctor", Race.HispanicLatino, Gender.Male,
				                                      AdverseEventIntensity.Moderate, AdverseEventRelanshionship.NotRelated, 1,
				                                      vRepository);
				AssertAdverseEventsAnalyticsViewModel(model[6], "Clinic1", "Super4 Doctor4", Race.White, Gender.Female,
				                                      AdverseEventIntensity.Severe, AdverseEventRelanshionship.Suspected, 1,
				                                      vRepository);
				AssertAdverseEventsAnalyticsViewModel(model[7], "Clinic1", "Super4 Doctor4", Race.White, Gender.Male,
				                                      AdverseEventIntensity.Mild, AdverseEventRelanshionship.Probable, 1,
				                                      vRepository);
				AssertAdverseEventsAnalyticsViewModel(model[8], "Clinic3", "Super5 Doctor5", Race.AfricanAmerican, Gender.Male,
				                                      AdverseEventIntensity.Mild, AdverseEventRelanshionship.Suspected, 1,
				                                      vRepository);
				AssertAdverseEventsAnalyticsViewModel(model[9], "Clinic3", "Super6 Doctor6", Race.AfricanAmerican, Gender.Female,
				                                      AdverseEventIntensity.Severe, AdverseEventRelanshionship.Probable, 1,
				                                      vRepository);
				AssertAdverseEventsAnalyticsViewModel(model[10], "Clinic3", "Super7 Doctor7", Race.AmericanIndian, Gender.Male,
				                                      AdverseEventIntensity.Moderate, AdverseEventRelanshionship.NotRelated, 1,
				                                      vRepository);

			}

			private void AssertAdverseEventsAnalyticsViewModel(AdverseEventsAnalyticsViewModel data, string clinic, string doctor,
			                                                   Race race, Gender sex,
			                                                   AdverseEventIntensity intensity,
			                                                   AdverseEventRelanshionship relanshionship,
			                                                   int aeCount, Mock<IVisitRepository> vRepository) {
				Assert.That(data.ClinicName, Is.EqualTo(clinic));
				Assert.That(data.DoctorName, Is.EqualTo(doctor));
				Assert.That(data.Race, Is.EqualTo(EnumHelper.GetDescription(race)));
				Assert.That(data.Gender, Is.EqualTo(EnumHelper.GetDescription(sex)));
				Assert.That(data.Intensity, Is.EqualTo(EnumHelper.GetDescription(intensity)));
				Assert.That(data.RelationshipToInvestigationalDrug, Is.EqualTo(EnumHelper.GetDescription(relanshionship)));
				Assert.That(data.AesCount, Is.EqualTo(aeCount));

				vRepository.Verify(r => r.GetAeAnalyticsData(), Times.Once());
			}

			private BaseFormData CreateTestCrf(FormType formType, int formId, int question1, int question2) {
				BaseFormData formData;

				if (formType == FormType.Demographics) {
					formData = new DemographicFormData {
						Id = formId,
						Race = new Question {Value = question1.ToString()},
						Sex = new Question {Value = question2.ToString()}
					};
				}
				else {
					formData = new AdverseEventFormData {
						Id = formId,
						Intensity = new Question {Value = question1.ToString()},
						RelationshipToInvestigationalDrug = new Question {Value = question2.ToString()}
					};
				}

				return formData;
			}

			private AeAnalyticsDto CreateAeAnalyticsDto(string clinic, string doctorFirst, string doctorLast, int demogFormId,
			                                            int aeFormId) {
				return new AeAnalyticsDto {
					ClinicName = clinic,
					DoctorFirstName = doctorFirst,
					DoctorLastName = doctorLast,
					DemographicFormId = demogFormId,
					AeFormId = aeFormId
				};
			}
		}
	}
}
