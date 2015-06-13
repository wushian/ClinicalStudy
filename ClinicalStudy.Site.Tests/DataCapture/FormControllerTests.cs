using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.DomainModel.ModelBuilders;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.Interface.FormData;
using ClinicalStudy.Site.Areas.DataCapture.ControlHelper;
using ClinicalStudy.Site.Areas.DataCapture.Controllers;
using ClinicalStudy.Site.Areas.DataCapture.Models;
using ClinicalStudy.Site.Areas.DataCapture.Models.FormData;
using ClinicalStudy.Site.Areas.DataCapture.Models.Shared;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.Mvc;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Site.Tests.DataCapture {
	[TestFixture]
	[SetCulture("en-US")]
	[SetUICulture("en-US")]
	public class FormControllerTests {

		[TestFixture]
		public class GetEditFormActions : ControllerTestsBase {
			private readonly Mock<IFormRepository> fRep = new Mock<IFormRepository>();
			private readonly Mock<IDemographicFormDataRepository> dfdRep = new Mock<IDemographicFormDataRepository>();
			private readonly Mock<IVitalsFormDataRepository> vfdRep = new Mock<IVitalsFormDataRepository>();
			private readonly Mock<IHappinessFormDataRepository> hfdRep = new Mock<IHappinessFormDataRepository>();
			private readonly Mock<IElectrocardiogramFormDataRepository> efdRep = new Mock<IElectrocardiogramFormDataRepository>();
			private readonly Mock<IInventoryFormDataRepository> ifdRep = new Mock<IInventoryFormDataRepository>();
			private readonly Mock<IAdverseEventFormDataRepository> afdRep = new Mock<IAdverseEventFormDataRepository>();
			private readonly Mock<IAttachmentRepository> attachmentRep = new Mock<IAttachmentRepository>();
			private readonly Mock<IQueryRepository> queryRep = new Mock<IQueryRepository>();

			private FormController formController;

			[TestFixtureSetUp]
			public void TestSetup() {
				formController = new FormController(fRep.Object, dfdRep.Object, vfdRep.Object, hfdRep.Object, efdRep.Object,
													ifdRep.Object, afdRep.Object, attachmentRep.Object, queryRep.Object);
			}

			[Test]
			public void GetEditForm_Demographic_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 31,
					Caption = "TestForm32",
					Visit = new Visit {Caption = "TestVisit33", Patient = new Patient {PatientNumber = 34}},
					FormType = FormType.Demographics
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditDemographicForm"));
				Assert.That(viewResultBase.Model is DemographicFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Demographic_CorrectView_Ajax_Test() {
				//Arrange
				EmulateControllerContext(formController, true);
				var form = new Form {
					Id = 32,
					Caption = "TestForm33",
					Visit = new Visit {Caption = "TestVisit34", Patient = new Patient {PatientNumber = 35}},
					FormType = FormType.Demographics
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditDemographicForm"));
				Assert.That(viewResultBase.Model is DemographicFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Demographic_NonExistingFormData_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 103,
					Caption = "TestForm103",
					Visit = new Visit {Caption = "TestVisit103", Patient = new Patient {PatientNumber = 203}},
					FormType = FormType.Demographics
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				dfdRep.Setup(dr => dr.GetFormDataByFormId(form.Id)).Returns((DemographicFormData) null);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));


				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Vitals_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 33,
					Caption = "TestForm34",
					Visit = new Visit {Caption = "TestVisit35", Patient = new Patient {PatientNumber = 36}},
					FormType = FormType.Vitals
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditVitalsForm"));
				Assert.That(viewResultBase.Model is VitalsFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Vitals_CorrectView_Ajax_Test() {
				//Arrange
				EmulateControllerContext(formController, true);
				var form = new Form {
					Id = 34,
					Caption = "TestForm35",
					Visit = new Visit {Caption = "TestVisit36", Patient = new Patient {PatientNumber = 37}},
					FormType = FormType.Vitals
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditVitalsForm"));
				Assert.That(viewResultBase.Model is VitalsFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Vitals_NonExistingFormData_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 104,
					Caption = "TestForm104",
					Visit = new Visit {Caption = "TestVisit104", Patient = new Patient {PatientNumber = 204}},
					FormType = FormType.Vitals
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				vfdRep.Setup(vr => vr.GetFormDataByFormId(form.Id)).Returns((VitalsFormData) null);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));


				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Happiness_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 35,
					Caption = "TestForm36",
					Visit = new Visit {Caption = "TestVisit37", Patient = new Patient {PatientNumber = 38}},
					FormType = FormType.Happiness
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditHappinessForm"));
				Assert.That(viewResultBase.Model is HappinessFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Happiness_CorrectView_Ajax_Test() {
				//Arrange
				EmulateControllerContext(formController, true);
				var form = new Form {
					Id = 36,
					Caption = "TestForm37",
					Visit = new Visit {Caption = "TestVisit38", Patient = new Patient {PatientNumber = 39}},
					FormType = FormType.Happiness
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditHappinessForm"));
				Assert.That(viewResultBase.Model is HappinessFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Happiness_NonExistingFormData_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 105,
					Caption = "TestForm105",
					Visit = new Visit {Caption = "TestVisit105", Patient = new Patient {PatientNumber = 205}},
					FormType = FormType.Happiness
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				hfdRep.Setup(hr => hr.GetFormDataByFormId(form.Id)).Returns((HappinessFormData) null);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));


				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Electrocardiogram_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 37,
					Caption = "TestForm38",
					Visit = new Visit {Caption = "TestVisit39", Patient = new Patient {PatientNumber = 40}},
					FormType = FormType.Electrocardiogram
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditElectrocardiogramForm"));
				Assert.That(viewResultBase.Model is ElectrocardiogramFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Electrocardiogram_CorrectView_Ajax_Test() {
				//Arrange
				EmulateControllerContext(formController, true);
				var form = new Form {
					Id = 38,
					Caption = "TestForm39",
					Visit = new Visit {Caption = "TestVisit40", Patient = new Patient {PatientNumber = 41}},
					FormType = FormType.Electrocardiogram
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditElectrocardiogramForm"));
				Assert.That(viewResultBase.Model is ElectrocardiogramFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Electrocardiogram_NonExistingFormData_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 106,
					Caption = "TestForm106",
					Visit = new Visit {Caption = "TestVisit106", Patient = new Patient {PatientNumber = 206}},
					FormType = FormType.Electrocardiogram
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				efdRep.Setup(er => er.GetFormDataByFormId(form.Id)).Returns((ElectrocardiogramFormData) null);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));


				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Inventory_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 39,
					Caption = "TestForm40",
					Visit = new Visit {Caption = "TestVisit41", Patient = new Patient {PatientNumber = 42}},
					FormType = FormType.Inventory
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditInventoryForm"));
				Assert.That(viewResultBase.Model is InventoryFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Inventory_CorrectView_Ajax_Test() {
				//Arrange
				EmulateControllerContext(formController, true);
				var form = new Form {
					Id = 40,
					Caption = "TestForm41",
					Visit = new Visit {Caption = "TestVisit42", Patient = new Patient {PatientNumber = 43}},
					FormType = FormType.Inventory
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditInventoryForm"));
				Assert.That(viewResultBase.Model is InventoryFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Inventory_NonExistingFormData_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 107,
					Caption = "TestForm107",
					Visit = new Visit {Caption = "TestVisit107", Patient = new Patient {PatientNumber = 207}},
					FormType = FormType.Inventory
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				ifdRep.Setup(ir => ir.GetFormDataByFormId(form.Id)).Returns((InventoryFormData) null);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));


				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_AdverseEvent_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 41,
					Caption = "TestForm42",
					Visit = new Visit {Caption = "TestVisit43", Patient = new Patient {PatientNumber = 44}},
					FormType = FormType.AdverseEvent
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditAdverseEventForm"));
				Assert.That(viewResultBase.Model is AdverseEventFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_AdverseEvent_CorrectView_Ajax_Test() {
				//Arrange
				EmulateControllerContext(formController, true);
				var form = new Form {
					Id = 42,
					Caption = "TestForm43",
					Visit = new Visit {Caption = "TestVisit44", Patient = new Patient {PatientNumber = 45}},
					FormType = FormType.AdverseEvent
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditAdverseEventForm"));
				Assert.That(viewResultBase.Model is AdverseEventFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_AdverseEvent_NonExistingFormData_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 108,
					Caption = "TestForm108",
					Visit = new Visit {Caption = "TestVisit108", Patient = new Patient {PatientNumber = 208}},
					FormType = FormType.AdverseEvent
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				afdRep.Setup(ar => ar.GetFormDataByFormId(form.Id)).Returns((AdverseEventFormData) null);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));


				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_NonExistingForm_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 101,
					Caption = "TestForm101",
					Visit = new Visit {Caption = "TestVisit101", Patient = new Patient {PatientNumber = 201}},
					FormType = FormType.AdverseEvent
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns((Form) null);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));

				VerifyRepositoriesCalls(form, true);
			}

			[Test]
			public void GetEditForm_UndefinedFormType_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 102,
					Caption = "TestForm102",
					Visit = new Visit {Caption = "TestVisit102", Patient = new Patient {PatientNumber = 202}},
					FormType = FormType.None
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);

				//Act
				var viewResult = formController.EditForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));

				VerifyRepositoriesCalls(form, true);
			}

			private void SetupFormDataRepositories(Form form) {
				//Arrange
				dfdRep.Setup(dr => dr.GetFormDataByFormId(form.Id)).Returns(new DemographicFormData {
					Form = form,
					DateOfBirth =
				                                                            	new Question(),
					Other = new Question(),
					Race = new Question(),
					Sex = new Question()
				});
				vfdRep.Setup(vr => vr.GetFormDataByFormId(form.Id)).Returns(new VitalsFormData {
					Form = form,
					ActualTime = new Question(),
					Height = new Question(),
					Weight = new Question(),
					Temperature = new Question(),
					HeartRate = new Question(),
					BloodPressureDiastolic =
				                                                            	new Question(),
					BloodPressureSystolic =
				                                                            	new Question()
				});
				hfdRep.Setup(hr => hr.GetFormDataByFormId(form.Id)).Returns(new HappinessFormData {
					Form = form,
					HappinessLevel = new Question()
				});
				efdRep.Setup(er => er.GetFormDataByFormId(form.Id)).Returns(new ElectrocardiogramFormData {
					Form = form,
					ElectrocardiogramActualTime =
				                                                            	new Question()
				});
				ifdRep.Setup(ir => ir.GetFormDataByFormId(form.Id)).Returns(new InventoryFormData {
					Form = form,
					BatchNumber =
				                                                            	new Question(),
					MedicationUsage = new List<RepeatableInventoryData>(),
					QuantityShipped =
				                                                            	new Question(),
					ReceiptDate =
				                                                            	new Question(),
					ShipDate = new Question()
				});
				afdRep.Setup(ar => ar.GetFormDataByFormId(form.Id)).Returns(new AdverseEventFormData {
					Form = form,
					AdverseExperience =
				                                                            	new Question(),
					EndDate =
				                                                            	new Question(),
					EndTime =
				                                                            	new Question(),
					Intensity =
				                                                            	new Question(),
					OnsetDate =
				                                                            	new Question(),
					OnsetTime =
				                                                            	new Question(),
					Outcome =
				                                                            	new Question(),
					RelationshipToInvestigationalDrug
				                                                            	= new Question()
				});
			}

			private void VerifyRepositoriesCalls(Form form, bool zeroCallsOnFormData = false) {
				fRep.Verify(r => r.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption), Times.Once());
				dfdRep.Verify(r => r.GetFormDataByFormId(form.Id),
				              !zeroCallsOnFormData && form.FormType == FormType.Demographics ? Times.Once() : Times.Never());
				vfdRep.Verify(r => r.GetFormDataByFormId(form.Id),
				              !zeroCallsOnFormData && form.FormType == FormType.Vitals ? Times.Once() : Times.Never());
				hfdRep.Verify(r => r.GetFormDataByFormId(form.Id),
				              !zeroCallsOnFormData && form.FormType == FormType.Happiness ? Times.Once() : Times.Never());
				efdRep.Verify(r => r.GetFormDataByFormId(form.Id),
				              !zeroCallsOnFormData && form.FormType == FormType.Electrocardiogram ? Times.Once() : Times.Never());
				ifdRep.Verify(r => r.GetFormDataByFormId(form.Id),
				              !zeroCallsOnFormData && form.FormType == FormType.Inventory ? Times.Once() : Times.Never());
				afdRep.Verify(r => r.GetFormDataByFormId(form.Id),
				              !zeroCallsOnFormData && form.FormType == FormType.AdverseEvent ? Times.Once() : Times.Never());
			}
		}

		[SetCulture("en-US")]
		[SetUICulture("en-US")]
		public class PostEditFormActions : ControllerTestsBase {
			private Mock<IFormRepository> fRep;
			private Mock<IDemographicFormDataRepository> dfdRep;
			private Mock<IVitalsFormDataRepository> vfdRep;
			private Mock<IHappinessFormDataRepository> hfdRep;
			private Mock<IElectrocardiogramFormDataRepository> efdRep;
			private Mock<IInventoryFormDataRepository> ifdRep;
			private Mock<IAdverseEventFormDataRepository> afdRep;
			private Mock<IAttachmentRepository> attachmentRep;
			private Mock<IQueryRepository> queryRep;
			private Mock<IChangeNoteBuilder> changeNoteBuilder;

			private FormController formController;

			[SetUp]
			public void TestSetup() {
				fRep = new Mock<IFormRepository>();
				dfdRep = new Mock<IDemographicFormDataRepository>();
				vfdRep = new Mock<IVitalsFormDataRepository>();
				hfdRep = new Mock<IHappinessFormDataRepository>();
				efdRep = new Mock<IElectrocardiogramFormDataRepository>();
				ifdRep = new Mock<IInventoryFormDataRepository>();
				afdRep = new Mock<IAdverseEventFormDataRepository>();
				attachmentRep = new Mock<IAttachmentRepository>();
				queryRep = new Mock<IQueryRepository>();
				changeNoteBuilder = new Mock<IChangeNoteBuilder>();
				formController = new FormController(fRep.Object, dfdRep.Object, vfdRep.Object, hfdRep.Object, efdRep.Object,
				                                    ifdRep.Object, afdRep.Object, attachmentRep.Object, queryRep.Object, changeNoteBuilder.Object);
			}

			[Test]
			public void PostEditDemographicFormData_ChangedValues_IncompleteForm_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Incomplete,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};

				var formData = new DemographicFormData {
					Id = 12,
					Form = form,
					DateOfBirth = new Question() { Value = new DateTime(2011, 10, 11).ToString(CultureInfo.InvariantCulture) },
					Other = new Question() { Value = String.Empty },
					Race = new Question() { Value = "4" },
					Sex = new Question() { Value = "2" }
				};
				var model = new DemographicFormViewModel {
					Id = 12,
					FormId = 13,
					DateOfBirth = new DateTime(2012, 1, 24),
					Race = 3,
					Sex = 1,
					Other = "Some text"
				};


				fRep.Setup(r => r.GetByKey(model.FormId)).Returns(form);

				//here we return "formdata before editing" - this data should be overwritten from model
				dfdRep.Setup(r => r.GetByKey(model.Id)).Returns(formData);

				//as checking of saved formdata is quite complicated, we will save the passed object and inspect it later
				DemographicFormData demogFormData = null;
				dfdRep.Setup(r => r.Edit(It.IsAny<DemographicFormData>())).Callback<DemographicFormData>(fd => demogFormData = fd);

				Form demogForm = null;
				fRep.Setup(r => r.Edit(It.IsAny<Form>())).Callback<Form>(f => demogForm = f);

				//Act
				var result = formController.EditDemographicForm(model);

				//Assert
				CheckEditFormDataRedirectRouteValues(result, form);

				dfdRep.Verify(r => r.GetByKey(model.Id), Times.Once());
				dfdRep.Verify(r => r.Edit(It.IsAny<DemographicFormData>()), Times.Once());
				dfdRep.Verify(r => r.Save(), Times.Once());

				fRep.Verify(r => r.GetByKey(model.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Once());
				fRep.Verify(r => r.Save(), Times.Once());

				Assert.That(demogFormData, Is.Not.Null);
				Assert.That(demogFormData.Id, Is.EqualTo(model.Id));
				Assert.That(demogFormData.DateOfBirth.Value,
							Is.EqualTo(new DateTime(2012, 1, 24).ToString(CultureInfo.InvariantCulture)));
				Assert.That(demogFormData.Race.Value, Is.EqualTo("3"));
				Assert.That(demogFormData.Sex.Value, Is.EqualTo("1"));
				Assert.That(demogFormData.Other.Value, Is.EqualTo("Some text"));

				Assert.That(demogForm, Is.Not.Null);
				Assert.That(demogForm.Id, Is.EqualTo(model.FormId));
				Assert.That(demogForm.FormState, Is.EqualTo(FormState.Completed));
			}

			[Test]
			public void PostEditDemographicFormData_ChangedUnconfirmedValues_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Completed,
					Caption = "TestForm",
					Visit = new Visit {Caption = "TestVisit", Patient = new Patient {PatientNumber = 123}}
				};

				var formData = new DemographicFormData {
					Id = 12,
					Form = form,
					DateOfBirth = new Question() { Id = 14, Caption  = "Date of Birth", Value = new DateTime(2011, 10, 11).ToString(CultureInfo.InvariantCulture)},
					Other = new Question() { Id = 15, Caption = "Other", Value = String.Empty },
					Race = new Question() { Id = 16, Caption = "Race", Value = "4" },
					Sex = new Question() { Id = 17, Caption = "Sex", Value = "1" }
				};
				var incomingModel = new DemographicFormViewModel {
					Id = 12,
					FormId = 13,
					DateOfBirth = new DateTime(2012, 1, 24),
					Race = 3,
					Sex = 0,
					Other = "Some text"
				};


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);
				//here we return "formdata before editing" - this data should not be overwritten from model as no any data change note provided
				dfdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);
				
				//Act
				var result = formController.EditDemographicForm(incomingModel);

				//Assert
				dfdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				dfdRep.Verify(r => r.Edit(It.IsAny<DemographicFormData>()), Times.Never());
				dfdRep.Verify(r => r.Save(), Times.Never());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Never());
				fRep.Verify(r => r.Save(), Times.Never());


				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				DemographicFormViewModel model = viewResultBase.Model as DemographicFormViewModel;
				Assert.That(model, Is.Not.Null);

				Assert.That(model.DataChangeReasonRequired);
				Assert.That(model.ChangeInfos, Is.Not.Null);
				Assert.That(model.ChangeInfos.Count, Is.EqualTo(4));


				AssertChangeInfo(formData.DateOfBirth, DateTime.Parse(formData.DateOfBirth.Value).ToShortDateString(), incomingModel.DateOfBirth.Value.ToShortDateString(), model);
				AssertChangeInfo(formData.Race, "American Indian", "Asian", model);
				AssertChangeInfo(formData.Sex, "Female", "Male", model);
				AssertChangeInfo(formData.Other, string.Empty, "Some text", model);
			}


			[Test]
			public void PostEditDemographicFormData_ChangedConfirmedValues_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Completed,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};

				var formData = new DemographicFormData {
					Id = 12,
					Form = form,
					DateOfBirth = new Question() { Id = 14, Caption = "Date of Birth", Value = new DateTime(2011, 10, 11).ToString(CultureInfo.InvariantCulture) },
					Other = new Question() { Id = 15, Caption = "Other", Value = String.Empty },
					Race = new Question() { Id = 16, Caption = "Race", Value = "4" },
					Sex = new Question() { Id = 17, Caption = "Sex", Value = "1" }
				};
				var incomingModel = new DemographicFormViewModel {
					Id = 12,
					FormId = 13,
					DateOfBirth = new DateTime(2012, 1, 24),
					Race = 3,
					Sex = 0,
					Other = "Some text"
				};
				incomingModel.ChangeInfos = new List<ChangeNoteViewModel> {
					new ChangeNoteViewModel{QuestionId = formData.DateOfBirth.Id, ChangeReason = "DateOfBirth Reason"},
					new ChangeNoteViewModel{QuestionId = formData.Race.Id, ChangeReason = "Race Reason"},
					new ChangeNoteViewModel{QuestionId = formData.Sex.Id, ChangeReason = "Sex Reason"},
					new ChangeNoteViewModel{QuestionId = formData.Other.Id, ChangeReason = "Other Reason"},
				};


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);
				//here we return "formdata before editing" - this data should not be overwritten from model as no any data change note provided
				dfdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//as checking of saved formdata is quite complicated, we will save the passed object and inspect it later
				DemographicFormData demogFormData = null;
				dfdRep.Setup(r => r.Edit(It.IsAny<DemographicFormData>())).Callback<DemographicFormData>(fd => demogFormData = fd);
				//Act
				var result = formController.EditDemographicForm(incomingModel);

				//Assert
				CheckEditFormDataRedirectRouteValues(result, form);

				dfdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				dfdRep.Verify(r => r.Edit(It.IsAny<DemographicFormData>()), Times.Once());
				dfdRep.Verify(r => r.Save(), Times.Once());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Once());
				fRep.Verify(r => r.Save(), Times.Once());

				Assert.That(demogFormData, Is.Not.Null);
				Assert.That(demogFormData.Id, Is.EqualTo(incomingModel.Id));

				
				Assert.That(demogFormData.DateOfBirth.Value,
							Is.EqualTo(new DateTime(2012, 1, 24).ToString(CultureInfo.InvariantCulture)));
				Assert.That(demogFormData.Race.Value, Is.EqualTo("3"));
				Assert.That(demogFormData.Sex.Value, Is.EqualTo("0"));
				Assert.That(demogFormData.Other.Value, Is.EqualTo("Some text"));
				changeNoteBuilder.Verify(b => b.CreateChangeNote(It.IsAny<Question>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>()), Times.Exactly(4));
			}

			[Test]
			public void PostEditDemographicFormData_NonExistingFormData_Test() {
				//Arrange
				var form = new Form {
					Id = 13,
					FormState = FormState.Incomplete
				};
				var model = new DemographicFormViewModel {
					Id = 12,
					FormId = 13,
					DateOfBirth = new DateTime(2012, 1, 24),
					Race = 1,
					Sex = 2,
					Other = "Some text"
				};
				fRep.Setup(r => r.GetByKey(13)).Returns(form);

				//here we return "formdata before editing" - this data should be overwritten from model
				dfdRep.Setup(r => r.GetByKey(12)).Returns((DemographicFormData) null);

				//Act
				var result = formController.EditDemographicForm(model);

				//Assert
				CheckErrorInfoViewResult(result);
			}

			[Test]
			public void PostEditDemographicFormData_NonExistingForm_Test() {
				//Arrange
				var model = new DemographicFormViewModel {
					Id = 12,
					FormId = 13,
					DateOfBirth = new DateTime(2012, 1, 24),
					Race = 1,
					Sex = 2,
					Other = "Some text"
				};
				fRep.Setup(r => r.GetByKey(13)).Returns((Form) null);

				//Act
				var result = formController.EditDemographicForm(model);

				//Assert
				CheckErrorInfoViewResult(result);
			}




			[Test]
			public void PostEditVitalsFormData_ChangedValues_IncompleteForm_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Incomplete,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};

				var formData = new VitalsFormData {
					Id = 12,
					Form = form,
					ActualTime = new Question() { Id = 100, Value = new DateTime(2011, 10, 11, 12, 13, 14).ToString(CultureInfo.InvariantCulture) },
					Height = new Question() { Id = 101, Value = "170" },
					Weight = new Question() { Id = 102, Value = "50" },
					HeartRate = new Question() { Id = 103, Value = "20" },
					Temperature = new Question() { Id = 104, Value = "30" },
					BloodPressureSystolic = new Question() { Id = 105, Value = "120" },
					BloodPressureDiastolic = new Question() { Id = 106, Value = "70" }
				};
				var incomingModel = new VitalsFormViewModel {
					Id = 12,
					FormId = 13,
					ActualTime = new DateTime(2012, 10, 11, 15, 16, 17),
					Height = 171,
					Weight = 51,
					HeartRate = 21,
					Temperature = 31,
					BloodPressureSystolic = "121",
					BloodPressureDiastolic = "71"
				};


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);

				//here we return "formdata before editing" - this data should be overwritten from model
				vfdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//as checking of saved formdata is quite complicated, we will save the passed object and inspect it later
				VitalsFormData savedFormData = null;
				vfdRep.Setup(r => r.Edit(It.IsAny<VitalsFormData>())).Callback<VitalsFormData>(fd => savedFormData = fd);

				Form savedForm = null;
				fRep.Setup(r => r.Edit(It.IsAny<Form>())).Callback<Form>(f => savedForm = f);

				//Act
				var result = formController.EditVitalsForm(incomingModel);

				//Assert
				CheckEditFormDataRedirectRouteValues(result, form);

				vfdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				vfdRep.Verify(r => r.Edit(It.IsAny<VitalsFormData>()), Times.Once());
				vfdRep.Verify(r => r.Save(), Times.Once());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Once());
				fRep.Verify(r => r.Save(), Times.Once());

				Assert.That(savedFormData, Is.Not.Null);
				Assert.That(savedFormData.Id, Is.EqualTo(incomingModel.Id));
				Assert.That(savedFormData.ActualTime.Value,
							Is.EqualTo(new DateTime(2012, 10, 11, 15, 16, 17).ToString(CultureInfo.InvariantCulture)));
				Assert.That(savedFormData.Height.Value, Is.EqualTo("171"));
				Assert.That(savedFormData.Weight.Value, Is.EqualTo("51"));
				Assert.That(savedFormData.HeartRate.Value, Is.EqualTo("21"));
				Assert.That(savedFormData.Temperature.Value, Is.EqualTo("31"));
				Assert.That(savedFormData.BloodPressureSystolic.Value, Is.EqualTo("121"));
				Assert.That(savedFormData.BloodPressureDiastolic.Value, Is.EqualTo("71"));

				Assert.That(savedForm, Is.Not.Null);
				Assert.That(savedForm.Id, Is.EqualTo(incomingModel.FormId));
				Assert.That(savedForm.FormState, Is.EqualTo(FormState.Completed));
			}

			[Test]
			public void PostEditVitalsFormData_ChangedUnconfirmedValues_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Completed,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};
				
				var formData = new VitalsFormData {
					Id = 12,
					Form = form,
					ActualTime = new Question() { Id = 100, Value = new DateTime(2011, 10, 11, 12, 13, 14).ToString(CultureInfo.InvariantCulture) },
					Height = new Question() { Id = 101, Value = "170" },
					Weight = new Question() { Id = 102, Value = "50" },
					HeartRate = new Question() { Id = 103, Value = "20" },
					Temperature = new Question() { Id = 104, Value = "30" },
					BloodPressureSystolic = new Question() { Id = 105, Value = "120" },
					BloodPressureDiastolic = new Question() { Id = 106, Value = "70" }
				};
				var incomingModel = new VitalsFormViewModel {
					Id = 12,
					FormId = 13,
					ActualTime = new DateTime(2012, 10, 11, 15, 16, 17),
					Height = 171,
					Weight = 51,
					HeartRate = 21,
					Temperature = 31,
					BloodPressureSystolic = "121",
					BloodPressureDiastolic = "71"
				};


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);
				//here we return "formdata before editing" - this data should not be overwritten from model as no any data change note provided
				vfdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//Act
				var result = formController.EditVitalsForm(incomingModel);

				//Assert
				vfdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				vfdRep.Verify(r => r.Edit(It.IsAny<VitalsFormData>()), Times.Never());
				vfdRep.Verify(r => r.Save(), Times.Never());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Never());
				fRep.Verify(r => r.Save(), Times.Never());


				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				VitalsFormViewModel model = viewResultBase.Model as VitalsFormViewModel;
				Assert.That(model, Is.Not.Null);

				Assert.That(model.DataChangeReasonRequired);
				Assert.That(model.ChangeInfos, Is.Not.Null);
				Assert.That(model.ChangeInfos.Count, Is.EqualTo(7));


				AssertChangeInfo(formData.Height, "170", "171", model);
				AssertChangeInfo(formData.Weight, "50", "51", model);
				AssertChangeInfo(formData.HeartRate, "20", "21", model);
				AssertChangeInfo(formData.Temperature, "30", "31", model);
				AssertChangeInfo(formData.BloodPressureSystolic, "120", "121", model);
				AssertChangeInfo(formData.BloodPressureDiastolic, "70", "71", model);
				AssertChangeInfo(formData.ActualTime, "12:13 PM", "3:16 PM", model);
			}


			[Test]
			public void PostEditVitalsFormData_ChangedConfirmedValues_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Completed,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};

				var formData = new VitalsFormData {
					Id = 12,
					Form = form,
					ActualTime = new Question() { Id = 100, Value = new DateTime(2011, 10, 11, 12, 13, 14).ToString(CultureInfo.InvariantCulture) },
					Height = new Question() { Id = 101, Value = "170" },
					Weight = new Question() { Id = 102, Value = "50" },
					HeartRate = new Question() { Id = 103, Value = "20" },
					Temperature = new Question() { Id = 104, Value = "30" },
					BloodPressureSystolic = new Question() { Id = 105, Value = "120" },
					BloodPressureDiastolic = new Question() { Id = 106, Value = "70" }
				};
				var incomingModel = new VitalsFormViewModel {
					Id = 12,
					FormId = 13,
					ActualTime = new DateTime(2012, 10, 11, 15, 16, 17),
					Height = 171,
					Weight = 51,
					HeartRate = 21,
					Temperature = 31,
					BloodPressureSystolic = "121",
					BloodPressureDiastolic = "71"
				};

				incomingModel.ChangeInfos = new List<ChangeNoteViewModel> {
					new ChangeNoteViewModel{QuestionId = formData.ActualTime.Id, ChangeReason = "ActualTime Reason"},
					new ChangeNoteViewModel{QuestionId = formData.Weight.Id, ChangeReason = "Weight Reason"},
					new ChangeNoteViewModel{QuestionId = formData.Height.Id, ChangeReason = "Height Reason"},
					new ChangeNoteViewModel{QuestionId = formData.HeartRate.Id, ChangeReason = "HeartRate Reason"},
					new ChangeNoteViewModel{QuestionId = formData.Temperature.Id, ChangeReason = "Temperature Reason"},
					new ChangeNoteViewModel{QuestionId = formData.BloodPressureSystolic.Id, ChangeReason = "BloodPressureSystolic Reason"},
					new ChangeNoteViewModel{QuestionId = formData.BloodPressureDiastolic.Id, ChangeReason = "BloodPressureDiastolic Reason"},
				};


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);
				//here we return "formdata before editing" - this data should not be overwritten from model as no any data change note provided
				vfdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//as checking of saved formdata is quite complicated, we will save the passed object and inspect it later
				VitalsFormData savedFormData = null;
				vfdRep.Setup(r => r.Edit(It.IsAny<VitalsFormData>())).Callback<VitalsFormData>(fd => savedFormData = fd);
				//Act
				var result = formController.EditVitalsForm(incomingModel);

				//Assert
				CheckEditFormDataRedirectRouteValues(result, form);

				vfdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				vfdRep.Verify(r => r.Edit(It.IsAny<VitalsFormData>()), Times.Once());
				vfdRep.Verify(r => r.Save(), Times.Once());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Once());
				fRep.Verify(r => r.Save(), Times.Once());

				Assert.That(savedFormData, Is.Not.Null);
				Assert.That(savedFormData.Id, Is.EqualTo(incomingModel.Id));
				Assert.That(savedFormData.ActualTime.Value,
							Is.EqualTo(new DateTime(2012, 10, 11, 15, 16, 17).ToString(CultureInfo.InvariantCulture)));
				Assert.That(savedFormData.Height.Value, Is.EqualTo("171"));
				Assert.That(savedFormData.Weight.Value, Is.EqualTo("51"));
				Assert.That(savedFormData.HeartRate.Value, Is.EqualTo("21"));
				Assert.That(savedFormData.Temperature.Value, Is.EqualTo("31"));
				Assert.That(savedFormData.BloodPressureSystolic.Value, Is.EqualTo("121"));
				Assert.That(savedFormData.BloodPressureDiastolic.Value, Is.EqualTo("71"));

				changeNoteBuilder.Verify(b => b.CreateChangeNote(It.IsAny<Question>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>()), Times.Exactly(7));
			}


			[Test]
			public void PostEditVitalsFormData_NonExistingFormData_Test() {
				//Arrange
				var form = new Form {
					Id = 13,
					FormState = FormState.Incomplete
				};
				var model = new VitalsFormViewModel {
					Id = 12,
					FormId = 13,
					ActualTime = new DateTime(2012, 10, 11, 15, 16, 17),
					Height = 171,
					Weight = 51,
					HeartRate = 21,
					Temperature = 31,
					BloodPressureSystolic = "121",
					BloodPressureDiastolic = "71"
				};
				fRep.Setup(r => r.GetByKey(13)).Returns(form);

				//here we return "formdata before editing" - this data should be overwritten from model
				vfdRep.Setup(r => r.GetByKey(12)).Returns((VitalsFormData) null);

				//Act
				var result = formController.EditVitalsForm(model);

				//Assert
				CheckErrorInfoViewResult(result);
			}

			[Test]
			public void PostEditVitalsFormData_NonExistingForm_Test() {
				//Arrange
				var model = new VitalsFormViewModel {
					Id = 12,
					FormId = 13,
					ActualTime = new DateTime(2012, 10, 11, 15, 16, 17),
					Height = 171,
					Weight = 51,
					HeartRate = 21,
					Temperature = 31,
					BloodPressureSystolic = "121",
					BloodPressureDiastolic = "71"
				};
				fRep.Setup(r => r.GetByKey(13)).Returns((Form) null);

				//Act
				var result = formController.EditVitalsForm(model);

				//Assert
				CheckErrorInfoViewResult(result);
			}





			[Test]
			public void PostEditHappinessFormData_ChangedValues_IncompleteForm_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Incomplete,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};

				var formData = new HappinessFormData {
					Id = 12,
					Form = form,
					HappinessLevel = new Question() { Id = 75, Value = "75" }
				};
				var model = new HappinessFormViewModel {
					Id = 12,
					FormId = 13,
					HappinessLevel = 91
				};


				fRep.Setup(r => r.GetByKey(model.FormId)).Returns(form);

				//here we return "formdata before editing" - this data should be overwritten from model
				hfdRep.Setup(r => r.GetByKey(model.Id)).Returns(formData);

				//as checking of saved formdata is quite complicated, we will save the passed object and inspect it later
				HappinessFormData savedFormData = null;
				hfdRep.Setup(r => r.Edit(It.IsAny<HappinessFormData>())).Callback<HappinessFormData>(fd => savedFormData = fd);

				Form savedForm = null;
				fRep.Setup(r => r.Edit(It.IsAny<Form>())).Callback<Form>(f => savedForm = f);

				//Act
				var result = formController.EditHappinessForm(model);

				//Assert
				CheckEditFormDataRedirectRouteValues(result, form);

				hfdRep.Verify(r => r.GetByKey(model.Id), Times.Once());
				hfdRep.Verify(r => r.Edit(It.IsAny<HappinessFormData>()), Times.Once());
				hfdRep.Verify(r => r.Save(), Times.Once());

				fRep.Verify(r => r.GetByKey(model.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Once());
				fRep.Verify(r => r.Save(), Times.Once());

				Assert.That(savedFormData, Is.Not.Null);
				Assert.That(savedFormData.Id, Is.EqualTo(model.Id));
				Assert.That(savedFormData.HappinessLevel.Value, Is.EqualTo("91"));

				Assert.That(savedForm, Is.Not.Null);
				Assert.That(savedForm.Id, Is.EqualTo(model.FormId));
				Assert.That(savedForm.FormState, Is.EqualTo(FormState.Completed));
			}

			[Test]
			public void PostEditHappinessFormData_ChangedUnconfirmedValues_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Completed,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};

				var formData = new HappinessFormData {
					Id = 12,
					Form = form,
					HappinessLevel = new Question() { Id = 75, Value = "75" }
				};
				var incomingModel = new HappinessFormViewModel {
					Id = 12,
					FormId = 13,
					HappinessLevel = 50
				};


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);

				//here we return "formdata before editing" - this data should be overwritten from model
				hfdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//Act
				var result = formController.EditHappinessForm(incomingModel);

				//Assert
				hfdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				hfdRep.Verify(r => r.Edit(It.IsAny<HappinessFormData>()), Times.Never());
				hfdRep.Verify(r => r.Save(), Times.Never());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Never());
				fRep.Verify(r => r.Save(), Times.Never());


				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				HappinessFormViewModel model = viewResultBase.Model as HappinessFormViewModel;
				Assert.That(model, Is.Not.Null);

				Assert.That(model.DataChangeReasonRequired);
				Assert.That(model.ChangeInfos, Is.Not.Null);
				Assert.That(model.ChangeInfos.Count, Is.EqualTo(1));


				AssertChangeInfo(formData.HappinessLevel, "Pretty Happy", "Happy", model);
			}


			[Test]
			public void PostEditHappinessFormData_ChangedConfirmedValues_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Completed,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};

				var formData = new HappinessFormData {
					Id = 12,
					Form = form,
					HappinessLevel = new Question() { Id = 75, Value = "75" }
				};
				var incomingModel = new HappinessFormViewModel {
					Id = 12,
					FormId = 13,
					HappinessLevel = 50
				};

				incomingModel.ChangeInfos = new List<ChangeNoteViewModel> {
					new ChangeNoteViewModel{QuestionId = formData.HappinessLevel.Id, ChangeReason = "HappinessLevel Reason"}
				};




				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);

				//here we return "formdata before editing" - this data should be overwritten from model
				hfdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//as checking of saved formdata is quite complicated, we will save the passed object and inspect it later
				HappinessFormData savedFormData = null;
				hfdRep.Setup(r => r.Edit(It.IsAny<HappinessFormData>())).Callback<HappinessFormData>(fd => savedFormData = fd);

				Form savedForm = null;
				fRep.Setup(r => r.Edit(It.IsAny<Form>())).Callback<Form>(f => savedForm = f);

				//Act
				var result = formController.EditHappinessForm(incomingModel);

				//Assert
				CheckEditFormDataRedirectRouteValues(result, form);

				hfdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				hfdRep.Verify(r => r.Edit(It.IsAny<HappinessFormData>()), Times.Once());
				hfdRep.Verify(r => r.Save(), Times.Once());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Once());
				fRep.Verify(r => r.Save(), Times.Once());

				Assert.That(savedFormData, Is.Not.Null);
				Assert.That(savedFormData.Id, Is.EqualTo(incomingModel.Id));
				Assert.That(savedFormData.HappinessLevel.Value, Is.EqualTo("50"));

				Assert.That(savedForm, Is.Not.Null);
				Assert.That(savedForm.Id, Is.EqualTo(incomingModel.FormId));
				Assert.That(savedForm.FormState, Is.EqualTo(FormState.Completed));


				changeNoteBuilder.Verify(b => b.CreateChangeNote(It.IsAny<Question>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>()), Times.Exactly(1));
			}

			[Test]
			public void PostEditHappinessFormData_NonExistingFormData_Test() {
				//Arrange
				var form = new Form {
					Id = 13,
					FormState = FormState.Incomplete
				};
				var model = new HappinessFormViewModel {
					Id = 12,
					FormId = 13,
					HappinessLevel = 90
				};
				fRep.Setup(r => r.GetByKey(13)).Returns(form);

				//here we return "formdata before editing" - this data should be overwritten from model
				hfdRep.Setup(r => r.GetByKey(12)).Returns((HappinessFormData) null);

				//Act
				var result = formController.EditHappinessForm(model);

				//Assert
				CheckErrorInfoViewResult(result);
			}

			[Test]
			public void PostEditHappinessFormData_NonExistingForm_Test() {
				//Arrange
				var model = new HappinessFormViewModel {
					Id = 12,
					FormId = 13,
					HappinessLevel = 90
				};
				fRep.Setup(r => r.GetByKey(13)).Returns((Form) null);

				//Act
				var result = formController.EditHappinessForm(model);

				//Assert
				CheckErrorInfoViewResult(result);
			}



			[Test]
			public void PostEditElectrocardiogramFormData_ChangedValues_IncompleteForm_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Incomplete,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};


				var formData = new ElectrocardiogramFormData {
					Id = 12,
					Form = form,
					ElectrocardiogramActualTime =
						new Question() { Id = 101, Value = new DateTime(2011, 10, 11, 12, 13, 14).ToString(CultureInfo.InvariantCulture) },
					ElectrocardiogramAttachment = new Question() { Id = 102 }
				};
				var incomingModel = new ElectrocardiogramFormViewModel {
					Id = 12,
					FormId = 13,
					ElectrocardiogramActualTime = new DateTime(2012, 1, 2, 3, 4, 5),
					AttachmentId = 14
				};


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);
				attachmentRep
					.Setup(r => r.GetByKey(incomingModel.AttachmentId.Value))
					.Returns(new Attachment() {
						Id = 14,
						FileName = "test.pdf",
						FileSize = 1234,
						MimeType = "application/pdf",
						StorageFileName = "abc.pdf"
					});


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);

				//here we return "formdata before editing" - this data should be overwritten from model
				efdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//as checking of saved formdata is quite complicated, we will save the passed object and inspect it later
				ElectrocardiogramFormData savedFormData = null;
				efdRep.Setup(r => r.Edit(It.IsAny<ElectrocardiogramFormData>())).Callback<ElectrocardiogramFormData>(fd => savedFormData = fd);

				Form savedForm = null;
				fRep.Setup(r => r.Edit(It.IsAny<Form>())).Callback<Form>(f => savedForm = f);

				//Act
				var result = formController.EditElectrocardiogramForm(incomingModel);

				//Assert
				CheckEditFormDataRedirectRouteValues(result, form);

				efdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				efdRep.Verify(r => r.Edit(It.IsAny<ElectrocardiogramFormData>()), Times.Once());
				efdRep.Verify(r => r.Save(), Times.Once());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Once());
				fRep.Verify(r => r.Save(), Times.Once());


				Assert.That(savedFormData, Is.Not.Null);
				Assert.That(savedFormData.Id, Is.EqualTo(incomingModel.Id));
				Assert.That(savedFormData.ElectrocardiogramActualTime.Value,
							Is.EqualTo(new DateTime(2012, 1, 2, 3, 4, 5).ToString(CultureInfo.InvariantCulture)));


				Assert.That(savedForm, Is.Not.Null);
				Assert.That(savedForm.Id, Is.EqualTo(incomingModel.FormId));
				Assert.That(savedForm.FormState, Is.EqualTo(FormState.Completed));
			}

			[Test]
			public void PostEditElectrocardiogramFormData_ChangedUnconfirmedValues_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Completed,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};

				var formData = new ElectrocardiogramFormData {
					Id = 12,
					Form = form,
					ElectrocardiogramActualTime =
						new Question() { Id = 101, Value = new DateTime(2011, 10, 11, 12, 13, 14).ToString(CultureInfo.InvariantCulture) },
					ElectrocardiogramAttachment = new Question() { Id = 102 }
				};
				var incomingModel = new ElectrocardiogramFormViewModel {
					Id = 12,
					FormId = 13,
					ElectrocardiogramActualTime = new DateTime(2012, 1, 2, 15, 16, 17),
					AttachmentId = 14
				};


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);
				attachmentRep
					.Setup(r => r.GetByKey(incomingModel.AttachmentId.Value))
					.Returns(new Attachment() {
						Id = 14,
						FileName = "test.pdf",
						FileSize = 1234,
						MimeType = "application/pdf",
						StorageFileName = "abc.pdf"
					});


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);
				//here we return "formdata before editing" - this data should not be overwritten from model as no any data change note provided
				efdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//Act
				var result = formController.EditElectrocardiogramForm(incomingModel);

				//Assert
				efdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				efdRep.Verify(r => r.Edit(It.IsAny<ElectrocardiogramFormData>()), Times.Never());
				efdRep.Verify(r => r.Save(), Times.Never());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Never());
				fRep.Verify(r => r.Save(), Times.Never());


				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				ElectrocardiogramFormViewModel model = viewResultBase.Model as ElectrocardiogramFormViewModel;
				Assert.That(model, Is.Not.Null);

				Assert.That(model.DataChangeReasonRequired);
				Assert.That(model.ChangeInfos, Is.Not.Null);
				Assert.That(model.ChangeInfos.Count, Is.EqualTo(2));


				AssertChangeInfo(formData.ElectrocardiogramActualTime, "12:13 PM", "3:16 PM", model);
				AssertChangeInfo(formData.ElectrocardiogramAttachment, "N/A", "test.pdf", model);
			}


			[Test]
			public void PostEditElectrocardiogramFormData_ChangedConfirmedValues_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Completed,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};

				var formData = new ElectrocardiogramFormData {
					Id = 12,
					Form = form,
					ElectrocardiogramActualTime =
						new Question() { Id = 101, Value = new DateTime(2011, 10, 11, 12, 13, 14).ToString(CultureInfo.InvariantCulture) },
					ElectrocardiogramAttachment = new Question() { Id = 102 }
				};
				var incomingModel = new ElectrocardiogramFormViewModel {
					Id = 12,
					FormId = 13,
					ElectrocardiogramActualTime = new DateTime(2012, 1, 2, 3, 4, 5),
					AttachmentId = 14
				};


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);
				attachmentRep
					.Setup(r => r.GetByKey(incomingModel.AttachmentId.Value))
					.Returns(new Attachment() {
						Id = 14,
						FileName = "test.pdf",
						FileSize = 1234,
						MimeType = "application/pdf",
						StorageFileName = "abc.pdf"
					});

				incomingModel.ChangeInfos = new List<ChangeNoteViewModel> {
					new ChangeNoteViewModel{QuestionId = formData.ElectrocardiogramActualTime.Id, ChangeReason = "ElectrocardiogramActualTime Reason"},
					new ChangeNoteViewModel{QuestionId = formData.ElectrocardiogramAttachment.Id, ChangeReason = "ElectrocardiogramAttachment Reason"}
				};


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);
				//here we return "formdata before editing" - this data should not be overwritten from model as no any data change note provided
				efdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//as checking of saved formdata is quite complicated, we will save the passed object and inspect it later
				ElectrocardiogramFormData savedFormData = null;
				efdRep.Setup(r => r.Edit(It.IsAny<ElectrocardiogramFormData>())).Callback<ElectrocardiogramFormData>(fd => savedFormData = fd);
				//Act
				var result = formController.EditElectrocardiogramForm(incomingModel);

				//Assert
				CheckEditFormDataRedirectRouteValues(result, form);

				efdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				efdRep.Verify(r => r.Edit(It.IsAny<ElectrocardiogramFormData>()), Times.Once());
				efdRep.Verify(r => r.Save(), Times.Once());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Once());
				fRep.Verify(r => r.Save(), Times.Once());


				Assert.That(savedFormData, Is.Not.Null);
				Assert.That(savedFormData.Id, Is.EqualTo(incomingModel.Id));
				Assert.That(savedFormData.ElectrocardiogramActualTime.Value,
							Is.EqualTo(new DateTime(2012, 1, 2, 3, 4, 5).ToString(CultureInfo.InvariantCulture)));


				changeNoteBuilder.Verify(b => b.CreateChangeNote(It.IsAny<Question>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>()), Times.Exactly(2));
			}


			//[Test]
			//public void PostEditElectrocardiogramFormData_ExistingFormData_Test() {
			//    //Arrange
			//    EmulateControllerContext(formController, false);
			//    var form = new Form {
			//        Id = 13,
			//        FormState = FormState.Incomplete,
			//        Caption = "TestForm",
			//        Visit = new Visit {Caption = "TestVisit", Patient = new Patient {PatientNumber = 123}}
			//    };

			//    var formData = new ElectrocardiogramFormData {
			//        Id = 12,
			//        Form = form,
			//        ElectrocardiogramActualTime =
			//            new Question() {Id=101, Value = new DateTime(2011, 10, 11, 12, 13, 14).ToString(CultureInfo.InvariantCulture)},
			//        ElectrocardiogramAttachment = new Question(){Id = 102}
			//    };
			//    var incomingModel = new ElectrocardiogramFormViewModel {
			//        Id = 12,
			//        FormId = 13,
			//        ElectrocardiogramActualTime = new DateTime(2012, 1, 2, 3, 4, 5),
			//        AttachmentId = 14
			//    };


			//    fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);
			//    attachmentRep
			//        .Setup(r => r.GetByKey(incomingModel.AttachmentId.Value))
			//        .Returns(new Attachment() {
			//            Id = 14,
			//            FileName = "test.pdf",
			//            FileSize = 1234,
			//            MimeType = "application/pdf",
			//            StorageFileName = "abc.pdf"
			//        });

			//    //here we return "formdata before editing" - this data should be overwritten from model
			//    efdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

			//    //as checking of saved formdata is quite complicated, we will save the passed object and inspect it later
			//    ElectrocardiogramFormData savedFormData = null;
			//    efdRep.Setup(r => r.Edit(It.IsAny<ElectrocardiogramFormData>())).Callback<ElectrocardiogramFormData>(
			//        fd => savedFormData = fd);

			//    Form savedForm = null;
			//    fRep.Setup(r => r.Edit(It.IsAny<Form>())).Callback<Form>(f => savedForm = f);

			//    //Act
			//    var result = formController.EditElectrocardiogramForm(incomingModel);

			//    //Assert
			//    CheckEditFormDataRedirectRouteValues(result, form);

			//    efdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
			//    efdRep.Verify(r => r.Edit(It.IsAny<ElectrocardiogramFormData>()), Times.Once());
			//    efdRep.Verify(r => r.Save(), Times.Once());

			//    fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
			//    fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Once());
			//    fRep.Verify(r => r.Save(), Times.Once());

			//    attachmentRep.Verify(r => r.GetByKey(incomingModel.AttachmentId.Value), Times.AtLeastOnce());

			//    Assert.That(savedFormData, Is.Not.Null);
			//    Assert.That(savedFormData.Id, Is.EqualTo(incomingModel.Id));
			//    Assert.That(savedFormData.ElectrocardiogramActualTime.Value,
			//                Is.EqualTo(new DateTime(2012, 1, 2, 3, 4, 5).ToString(CultureInfo.InvariantCulture)));

			//    Assert.That(savedForm, Is.Not.Null);
			//    Assert.That(savedForm.Id, Is.EqualTo(incomingModel.FormId));
			//    Assert.That(savedForm.FormState, Is.EqualTo(FormState.Completed));
			//}

			[Test]
			public void PostEditElectrocardiogramFormData_NonExistingFormData_Test() {
				//Arrange
				var form = new Form {
					Id = 13,
					FormState = FormState.Incomplete
				};
				var model = new ElectrocardiogramFormViewModel {
					Id = 12,
					FormId = 13,
					ElectrocardiogramActualTime = new DateTime(2012, 1, 2, 3, 4, 5)
				};
				fRep.Setup(r => r.GetByKey(13)).Returns(form);

				//here we return "formdata before editing" - this data should be overwritten from model
				efdRep.Setup(r => r.GetByKey(12)).Returns((ElectrocardiogramFormData) null);

				//Act
				var result = formController.EditElectrocardiogramForm(model);

				//Assert
				CheckErrorInfoViewResult(result);
			}

			[Test]
			public void PostEditElectrocardiogramFormData_NonExistingForm_Test() {
				//Arrange
				var model = new ElectrocardiogramFormViewModel {
					Id = 12,
					FormId = 13,
					ElectrocardiogramActualTime = new DateTime(2012, 1, 2, 3, 4, 5)
				};
				fRep.Setup(r => r.GetByKey(13)).Returns((Form) null);

				//Act
				var result = formController.EditElectrocardiogramForm(model);

				//Assert
				CheckErrorInfoViewResult(result);
			}

			[Test]
			public void PostEditInventoryFormData_ChangedValues_IncompleteForm_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Incomplete,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};


				var formData = new InventoryFormData {
					Id = 12,
					Form = form,
					ReceiptDate =
						new Question() { Id = 101, Value = new DateTime(2011, 10, 11, 12, 13, 14).ToString(CultureInfo.InvariantCulture) },
					ShipDate = new Question() { Id = 102, Value = new DateTime(2012, 1, 2, 3, 4, 5).ToString(CultureInfo.InvariantCulture) },
					BatchNumber = new Question() { Id = 103, Value = "170" },
					QuantityShipped = new Question() { Id = 104, Value = "50" }
				};
				var incomingModel = new InventoryFormViewModel {
					Id = 12,
					FormId = 13,
					ReceiptDate = new DateTime(2012, 10, 11, 15, 16, 17),
					ShipDate = new DateTime(2011, 4, 5, 6, 7, 8),
					BatchNumber = 171,
					QuantityShipped = 51
				};



				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);

				//here we return "formdata before editing" - this data should be overwritten from model
				ifdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//as checking of saved formdata is quite complicated, we will save the passed object and inspect it later
				InventoryFormData savedFormData = null;
				ifdRep.Setup(r => r.Edit(It.IsAny<InventoryFormData>())).Callback<InventoryFormData>(fd => savedFormData = fd);

				Form savedForm = null;
				fRep.Setup(r => r.Edit(It.IsAny<Form>())).Callback<Form>(f => savedForm = f);

				//Act
				var result = formController.EditInventoryForm(incomingModel);

				//Assert
				CheckEditFormDataRedirectRouteValues(result, form);

				ifdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				ifdRep.Verify(r => r.Edit(It.IsAny<InventoryFormData>()), Times.Once());
				ifdRep.Verify(r => r.Save(), Times.Once());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Once());
				fRep.Verify(r => r.Save(), Times.Once());

				Assert.That(savedFormData, Is.Not.Null);
				Assert.That(savedFormData.Id, Is.EqualTo(incomingModel.Id));
				Assert.That(savedFormData.ReceiptDate.Value,
							Is.EqualTo(new DateTime(2012, 10, 11, 15, 16, 17).ToString(CultureInfo.InvariantCulture)));
				Assert.That(savedFormData.ShipDate.Value,
							Is.EqualTo(new DateTime(2011, 4, 5, 6, 7, 8).ToString(CultureInfo.InvariantCulture)));
				Assert.That(savedFormData.BatchNumber.Value, Is.EqualTo("171"));
				Assert.That(savedFormData.QuantityShipped.Value, Is.EqualTo("51"));

				Assert.That(savedForm, Is.Not.Null);
				Assert.That(savedForm.Id, Is.EqualTo(incomingModel.FormId));
				Assert.That(savedForm.FormState, Is.EqualTo(FormState.Completed));
			}

			[Test]
			public void PostEditInventoryFormData_ChangedUnconfirmedValues_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Completed,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};

				var formData = new InventoryFormData {
					Id = 12,
					Form = form,
					ReceiptDate =
						new Question() { Id = 101, Value = new DateTime(2011, 10, 11, 12, 13, 14).ToString(CultureInfo.InvariantCulture) },
					ShipDate = new Question() { Id = 102, Value = new DateTime(2012, 1, 2, 3, 4, 5).ToString(CultureInfo.InvariantCulture) },
					BatchNumber = new Question() { Id = 103, Value = "170" },
					QuantityShipped = new Question() { Id = 104, Value = "50" }
				};
				var incomingModel = new InventoryFormViewModel {
					Id = 12,
					FormId = 13,
					ReceiptDate = new DateTime(2012, 10, 11, 15, 16, 17),
					ShipDate = new DateTime(2011, 4, 5, 6, 7, 8),
					BatchNumber = 171,
					QuantityShipped = 51
				};


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);
				//here we return "formdata before editing" - this data should not be overwritten from model as no any data change note provided
				ifdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//Act
				var result = formController.EditInventoryForm(incomingModel);

				//Assert
				ifdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				ifdRep.Verify(r => r.Edit(It.IsAny<InventoryFormData>()), Times.Never());
				ifdRep.Verify(r => r.Save(), Times.Never());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Never());
				fRep.Verify(r => r.Save(), Times.Never());


				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				InventoryFormViewModel model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model, Is.Not.Null);

				Assert.That(model.DataChangeReasonRequired);
				Assert.That(model.ChangeInfos, Is.Not.Null);
				Assert.That(model.ChangeInfos.Count, Is.EqualTo(4));

				AssertChangeInfo(formData.ReceiptDate, DateTime.Parse(formData.ReceiptDate.Value).ToShortDateString(), incomingModel.ReceiptDate.Value.ToShortDateString(), model);
				AssertChangeInfo(formData.ShipDate, DateTime.Parse(formData.ShipDate.Value).ToShortDateString(), incomingModel.ShipDate.Value.ToShortDateString(), model);
				AssertChangeInfo(formData.BatchNumber, "170", "171", model);
				AssertChangeInfo(formData.QuantityShipped, "50", "51", model);
			}


			[Test]
			public void PostEditInventoryFormData_ChangedConfirmedValues_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Completed,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};

				var formData = new InventoryFormData {
					Id = 12,
					Form = form,
					ReceiptDate =
						new Question() { Id = 101, Value = new DateTime(2011, 10, 11, 12, 13, 14).ToString(CultureInfo.InvariantCulture) },
					ShipDate = new Question() { Id = 102, Value = new DateTime(2012, 1, 2, 3, 4, 5).ToString(CultureInfo.InvariantCulture) },
					BatchNumber = new Question() { Id = 103, Value = "170" },
					QuantityShipped = new Question() { Id = 104, Value = "50" }
				};
				var incomingModel = new InventoryFormViewModel {
					Id = 12,
					FormId = 13,
					ReceiptDate = new DateTime(2012, 10, 11, 15, 16, 17),
					ShipDate = new DateTime(2011, 4, 5, 6, 7, 8),
					BatchNumber = 171,
					QuantityShipped = 51
				};

				incomingModel.ChangeInfos = new List<ChangeNoteViewModel> {
					new ChangeNoteViewModel{QuestionId = formData.ReceiptDate.Id, ChangeReason = "ReceiptDate Reason"},
					new ChangeNoteViewModel{QuestionId = formData.ShipDate.Id, ChangeReason = "ShipDate Reason"},
					new ChangeNoteViewModel{QuestionId = formData.BatchNumber.Id, ChangeReason = "BatchNumber Reason"},
					new ChangeNoteViewModel{QuestionId = formData.QuantityShipped.Id, ChangeReason = "QuantityShipped Reason"}
				};


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);
				//here we return "formdata before editing" - this data should not be overwritten from model as no any data change note provided
				ifdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//as checking of saved formdata is quite complicated, we will save the passed object and inspect it later
				InventoryFormData savedFormData = null;
				ifdRep.Setup(r => r.Edit(It.IsAny<InventoryFormData>())).Callback<InventoryFormData>(fd => savedFormData = fd);
				//Act
				var result = formController.EditInventoryForm(incomingModel);

				//Assert
				CheckEditFormDataRedirectRouteValues(result, form);

				ifdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				ifdRep.Verify(r => r.Edit(It.IsAny<InventoryFormData>()), Times.Once());
				ifdRep.Verify(r => r.Save(), Times.Once());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Once());
				fRep.Verify(r => r.Save(), Times.Once());

				Assert.That(savedFormData, Is.Not.Null);
				Assert.That(savedFormData.Id, Is.EqualTo(incomingModel.Id));
				Assert.That(savedFormData.ReceiptDate.Value,
							Is.EqualTo(new DateTime(2012, 10, 11, 15, 16, 17).ToString(CultureInfo.InvariantCulture)));
				Assert.That(savedFormData.ShipDate.Value,
							Is.EqualTo(new DateTime(2011, 4, 5, 6, 7, 8).ToString(CultureInfo.InvariantCulture)));
				Assert.That(savedFormData.BatchNumber.Value, Is.EqualTo("171"));
				Assert.That(savedFormData.QuantityShipped.Value, Is.EqualTo("51"));

				changeNoteBuilder.Verify(b => b.CreateChangeNote(It.IsAny<Question>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>()), Times.Exactly(4));
			}

			[Test]
			public void PostEditInventoryFormData_NonExistingFormData_Test() {
				//Arrange
				var form = new Form {
					Id = 13,
					FormState = FormState.Incomplete
				};
				var model = new InventoryFormViewModel {
					Id = 12,
					FormId = 13,
					ReceiptDate = new DateTime(2012, 10, 11, 15, 16, 17),
					ShipDate = new DateTime(2011, 4, 5, 6, 7, 8),
					BatchNumber = 171,
					QuantityShipped = 51
				};
				fRep.Setup(r => r.GetByKey(13)).Returns(form);

				//here we return "formdata before editing" - this data should be overwritten from model
				ifdRep.Setup(r => r.GetByKey(12)).Returns((InventoryFormData) null);

				//Act
				var result = formController.EditInventoryForm(model);

				//Assert
				CheckErrorInfoViewResult(result);
			}

			[Test]
			public void PostEditInventoryFormData_NonExistingForm_Test() {
				//Arrange
				var model = new InventoryFormViewModel {
					Id = 12,
					FormId = 13,
					ReceiptDate = new DateTime(2012, 10, 11, 15, 16, 17),
					ShipDate = new DateTime(2011, 4, 5, 6, 7, 8),
					BatchNumber = 171,
					QuantityShipped = 51
				};
				fRep.Setup(r => r.GetByKey(13)).Returns((Form) null);

				//Act
				var result = formController.EditInventoryForm(model);

				//Assert
				CheckErrorInfoViewResult(result);
			}

			[Test]
			public void PostEditAdverseEventFormData_ChangedValues_IncompleteForm_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Incomplete,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};

				var formData = new AdverseEventFormData {
					Id = 12,
					Form = form,
					OnsetDate = new Question() { Id = 101, Value = new DateTime(2011, 10, 11).ToString(CultureInfo.InvariantCulture) },
					OnsetTime = new Question() { Id = 102, Value = new DateTime(2010, 11, 12, 12, 13, 14).ToString(CultureInfo.InvariantCulture) },
					EndDate = new Question() { Id = 103, Value = new DateTime(2012, 1, 2).ToString(CultureInfo.InvariantCulture) },
					EndTime = new Question() { Id = 104, Value = new DateTime(2011, 9, 3, 3, 4, 5).ToString(CultureInfo.InvariantCulture) },
					AdverseExperience = new Question() { Id = 105, Value = "Broken neck" },
					Intensity = new Question() { Id = 106, Value = "1" },
					Outcome = new Question() { Id = 107, Value = "2" },
					RelationshipToInvestigationalDrug = new Question() { Id = 108, Value = "3" }
				};
				var incomingModel = new AdverseEventFormViewModel {
					Id = 12,
					FormId = 13,
					OnsetDate = new DateTime(2010, 1, 2),
					OnsetTime = new DateTime(2011, 1, 1, 5, 4, 3),
					EndDate = new DateTime(2011, 11, 12),
					EndTime = new DateTime(2012, 1, 1, 14, 15, 16),
					Intensity = 4,
					Outcome = 5,
					RelationshipToInvestigationalDrug = 6
				};

				bool isValid = true;
				Mock<IHtmlEditorAdapter> htmlHelper = new Mock<IHtmlEditorAdapter>();
				htmlHelper.Setup(hh => hh.GetActiveView(It.IsAny<string>())).Returns(null);
				htmlHelper.Setup(hh => hh.GetHtmlView(It.IsAny<string>(),
													  It.IsAny<ASPxHtmlEditorHtmlEditingSettings>(),
													  It.IsAny<HtmlEditorValidationSettings>(),
													  It.IsAny<EventHandler<HtmlEditorValidationEventArgs>>(),
													  out isValid)).Returns("Broken brain");

				formController.HtmlEditorAdapter = htmlHelper.Object;


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);

				//here we return "formdata before editing" - this data should be overwritten from model
				afdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//as checking of saved formdata is quite complicated, we will save the passed object and inspect it later
				AdverseEventFormData savedFormData = null;
				afdRep.Setup(r => r.Edit(It.IsAny<AdverseEventFormData>())).Callback<AdverseEventFormData>(fd => savedFormData = fd);

				Form savedForm = null;
				fRep.Setup(r => r.Edit(It.IsAny<Form>())).Callback<Form>(f => savedForm = f);

				//Act
				var result = formController.EditAdverseEventForm(incomingModel);

				//Assert
				CheckEditFormDataRedirectRouteValues(result, form);

				afdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				afdRep.Verify(r => r.Edit(It.IsAny<AdverseEventFormData>()), Times.Once());
				afdRep.Verify(r => r.Save(), Times.Once());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Once());
				fRep.Verify(r => r.Save(), Times.Once());

				Assert.That(savedFormData, Is.Not.Null);
				Assert.That(savedFormData.Id, Is.EqualTo(incomingModel.Id));
				Assert.That(savedFormData.OnsetDate.Value,
							Is.EqualTo(new DateTime(2010, 1, 2).ToString(CultureInfo.InvariantCulture)));
				Assert.That(savedFormData.OnsetTime.Value,
							Is.EqualTo(new DateTime(2011, 1, 1, 5, 4, 3).ToString(CultureInfo.InvariantCulture)));
				Assert.That(savedFormData.EndDate.Value,
							Is.EqualTo(new DateTime(2011, 11, 12).ToString(CultureInfo.InvariantCulture)));
				Assert.That(savedFormData.EndTime.Value,
							Is.EqualTo(new DateTime(2012, 1, 1, 14, 15, 16).ToString(CultureInfo.InvariantCulture)));
				Assert.That(savedFormData.AdverseExperience.Value, Is.EqualTo("Broken brain"));
				Assert.That(savedFormData.Intensity.Value, Is.EqualTo("4"));
				Assert.That(savedFormData.Outcome.Value, Is.EqualTo("5"));
				Assert.That(savedFormData.RelationshipToInvestigationalDrug.Value, Is.EqualTo("6"));

				Assert.That(savedForm, Is.Not.Null);
				Assert.That(savedForm.Id, Is.EqualTo(incomingModel.FormId));
				Assert.That(savedForm.FormState, Is.EqualTo(FormState.Completed));
			}

			[Test]
			public void PostEditAdverseEventFormData_ChangedUnconfirmedValues_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Completed,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};

				var formData = new AdverseEventFormData {
					Id = 12,
					Form = form,
					OnsetDate = new Question() { Id = 101, Value = new DateTime(2011, 10, 11).ToString(CultureInfo.InvariantCulture) },
					OnsetTime = new Question() { Id = 102, Value = new DateTime(2010, 11, 12, 12, 13, 14).ToString(CultureInfo.InvariantCulture) },
					EndDate = new Question() { Id = 103, Value = new DateTime(2012, 1, 2).ToString(CultureInfo.InvariantCulture) },
					EndTime = new Question() { Id = 104, Value = new DateTime(2011, 9, 3, 3, 4, 5).ToString(CultureInfo.InvariantCulture) },
					AdverseExperience = new Question() { Id = 105, Value = "Broken neck" },
					Intensity = new Question() { Id = 106, Value = "1" },
					Outcome = new Question() { Id = 107, Value = "2" },
					RelationshipToInvestigationalDrug = new Question() { Id = 108, Value = "3" }
				};
				var incomingModel = new AdverseEventFormViewModel {
					Id = 12,
					FormId = 13,
					OnsetDate = new DateTime(2010, 1, 2),
					OnsetTime = new DateTime(2011, 1, 1, 5, 4, 3),
					EndDate = new DateTime(2011, 11, 12),
					EndTime = new DateTime(2012, 1, 1, 14, 15, 16),
					Intensity = 2,
					Outcome = 1,
					RelationshipToInvestigationalDrug = 2
				};

				bool isValid = true;
				Mock<IHtmlEditorAdapter> htmlHelper = new Mock<IHtmlEditorAdapter>();
				htmlHelper.Setup(hh => hh.GetActiveView(It.IsAny<string>())).Returns(null);
				htmlHelper.Setup(hh => hh.GetHtmlView(It.IsAny<string>(),
													  It.IsAny<ASPxHtmlEditorHtmlEditingSettings>(),
													  It.IsAny<HtmlEditorValidationSettings>(),
													  It.IsAny<EventHandler<HtmlEditorValidationEventArgs>>(),
													  out isValid)).Returns("Broken brain");

				formController.HtmlEditorAdapter = htmlHelper.Object;


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);
				//here we return "formdata before editing" - this data should not be overwritten from model as no any data change note provided
				afdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//Act
				var result = formController.EditAdverseEventForm(incomingModel);

				//Assert
				afdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				afdRep.Verify(r => r.Edit(It.IsAny<AdverseEventFormData>()), Times.Never());
				afdRep.Verify(r => r.Save(), Times.Never());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Never());
				fRep.Verify(r => r.Save(), Times.Never());


				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				AdverseEventFormViewModel model = viewResultBase.Model as AdverseEventFormViewModel;
				Assert.That(model, Is.Not.Null);

				Assert.That(model.DataChangeReasonRequired);
				Assert.That(model.ChangeInfos, Is.Not.Null);
				Assert.That(model.ChangeInfos.Count, Is.EqualTo(8));

				AssertChangeInfo(formData.OnsetDate, DateTime.Parse(formData.OnsetDate.Value).ToShortDateString(), incomingModel.OnsetDate.Value.ToShortDateString(), model);
				AssertChangeInfo(formData.OnsetTime, "12:13 PM", "5:04 AM", model);

				AssertChangeInfo(formData.EndDate, DateTime.Parse(formData.EndDate.Value).ToShortDateString(), incomingModel.EndDate.Value.ToShortDateString(), model);
				AssertChangeInfo(formData.EndTime, "3:04 AM", "2:15 PM", model);

				AssertChangeInfo(formData.AdverseExperience, "Broken neck", "Broken brain", model);
				AssertChangeInfo(formData.Intensity, "Mild", "Moderate", model);
				AssertChangeInfo(formData.Outcome, "Ongoing", "Resolved", model);
				AssertChangeInfo(formData.RelationshipToInvestigationalDrug, "Suspected (reasonably possible)", "Unlikely", model);
			}


			[Test]
			public void PostEditAdverseEventFormData_ChangedConfirmedValues_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 13,
					FormState = FormState.Completed,
					Caption = "TestForm",
					Visit = new Visit { Caption = "TestVisit", Patient = new Patient { PatientNumber = 123 } }
				};

				var formData = new AdverseEventFormData {
					Id = 12,
					Form = form,
					OnsetDate = new Question() { Id = 101, Value = new DateTime(2011, 10, 11).ToString(CultureInfo.InvariantCulture) },
					OnsetTime = new Question() { Id = 102, Value = new DateTime(2010, 11, 12, 12, 13, 14).ToString(CultureInfo.InvariantCulture) },
					EndDate = new Question() { Id = 103, Value = new DateTime(2012, 1, 2).ToString(CultureInfo.InvariantCulture) },
					EndTime = new Question() { Id = 104, Value = new DateTime(2011, 9, 3, 3, 4, 5).ToString(CultureInfo.InvariantCulture) },
					AdverseExperience = new Question() { Id = 105, Value = "Broken neck" },
					Intensity = new Question() { Id = 106, Value = "1" },
					Outcome = new Question() { Id = 107, Value = "2" },
					RelationshipToInvestigationalDrug = new Question() { Id = 108, Value = "3" }
				};
				var incomingModel = new AdverseEventFormViewModel {
					Id = 12,
					FormId = 13,
					OnsetDate = new DateTime(2010, 1, 2),
					OnsetTime = new DateTime(2011, 1, 1, 5, 4, 3),
					EndDate = new DateTime(2011, 11, 12),
					EndTime = new DateTime(2012, 1, 1, 14, 15, 16),
					Intensity = 4,
					Outcome = 5,
					RelationshipToInvestigationalDrug = 6
				};

				bool isValid = true;
				Mock<IHtmlEditorAdapter> htmlHelper = new Mock<IHtmlEditorAdapter>();
				htmlHelper.Setup(hh => hh.GetActiveView(It.IsAny<string>())).Returns(null);
				htmlHelper.Setup(hh => hh.GetHtmlView(It.IsAny<string>(),
													  It.IsAny<ASPxHtmlEditorHtmlEditingSettings>(),
													  It.IsAny<HtmlEditorValidationSettings>(),
													  It.IsAny<EventHandler<HtmlEditorValidationEventArgs>>(),
													  out isValid)).Returns("Broken brain");

				formController.HtmlEditorAdapter = htmlHelper.Object;
				
				incomingModel.ChangeInfos = new List<ChangeNoteViewModel> {
					new ChangeNoteViewModel{QuestionId = formData.OnsetDate.Id, ChangeReason = "OnsetDate Reason"},
					new ChangeNoteViewModel{QuestionId = formData.OnsetTime.Id, ChangeReason = "OnsetTime Reason"},
					new ChangeNoteViewModel{QuestionId = formData.EndDate.Id, ChangeReason = "EndDate Reason"},
					new ChangeNoteViewModel{QuestionId = formData.EndTime.Id, ChangeReason = "EndTime Reason"},
					new ChangeNoteViewModel{QuestionId = formData.AdverseExperience.Id, ChangeReason = "AdverseExperience Reason"},
					new ChangeNoteViewModel{QuestionId = formData.Intensity.Id, ChangeReason = "Intensity Reason"},
					new ChangeNoteViewModel{QuestionId = formData.Outcome.Id, ChangeReason = "Outcome Reason"},
					new ChangeNoteViewModel{QuestionId = formData.RelationshipToInvestigationalDrug.Id, ChangeReason = "RelationshipToInvestigationalDrug Reason"},
				};


				fRep.Setup(r => r.GetByKey(incomingModel.FormId)).Returns(form);
				//here we return "formdata before editing" - this data should not be overwritten from model as no any data change note provided
				afdRep.Setup(r => r.GetByKey(incomingModel.Id)).Returns(formData);

				//as checking of saved formdata is quite complicated, we will save the passed object and inspect it later
				AdverseEventFormData savedFormData = null;
				afdRep.Setup(r => r.Edit(It.IsAny<AdverseEventFormData>())).Callback<AdverseEventFormData>(fd => savedFormData = fd);
				//Act
				var result = formController.EditAdverseEventForm(incomingModel);

				//Assert
				CheckEditFormDataRedirectRouteValues(result, form);

				afdRep.Verify(r => r.GetByKey(incomingModel.Id), Times.Once());
				afdRep.Verify(r => r.Edit(It.IsAny<AdverseEventFormData>()), Times.Once());
				afdRep.Verify(r => r.Save(), Times.Once());

				fRep.Verify(r => r.GetByKey(incomingModel.FormId), Times.Once());
				fRep.Verify(r => r.Edit(It.IsAny<Form>()), Times.Once());
				fRep.Verify(r => r.Save(), Times.Once());

				Assert.That(savedFormData, Is.Not.Null);
				Assert.That(savedFormData.Id, Is.EqualTo(incomingModel.Id));
				Assert.That(savedFormData.OnsetDate.Value,
							Is.EqualTo(new DateTime(2010, 1, 2).ToString(CultureInfo.InvariantCulture)));
				Assert.That(savedFormData.OnsetTime.Value,
							Is.EqualTo(new DateTime(2011, 1, 1, 5, 4, 3).ToString(CultureInfo.InvariantCulture)));
				Assert.That(savedFormData.EndDate.Value,
							Is.EqualTo(new DateTime(2011, 11, 12).ToString(CultureInfo.InvariantCulture)));
				Assert.That(savedFormData.EndTime.Value,
							Is.EqualTo(new DateTime(2012, 1, 1, 14, 15, 16).ToString(CultureInfo.InvariantCulture)));
				Assert.That(savedFormData.AdverseExperience.Value, Is.EqualTo("Broken brain"));
				Assert.That(savedFormData.Intensity.Value, Is.EqualTo("4"));
				Assert.That(savedFormData.Outcome.Value, Is.EqualTo("5"));
				Assert.That(savedFormData.RelationshipToInvestigationalDrug.Value, Is.EqualTo("6"));

				changeNoteBuilder.Verify(b => b.CreateChangeNote(It.IsAny<Question>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>()), Times.Exactly(8));
			}

			[Test]
			public void PostEditAdverseEventFormData_NonExistingFormData_Test() {
				//Arrange
				var form = new Form {
					Id = 13,
					FormState = FormState.Incomplete
				};
				var model = new AdverseEventFormViewModel {
					Id = 12,
					FormId = 13,
					OnsetDate = new DateTime(2010, 1, 2),
					OnsetTime = new DateTime(2011, 1, 1, 5, 4, 3),
					EndDate = new DateTime(2011, 11, 12),
					EndTime = new DateTime(2012, 1, 1, 14, 15, 16),
					Intensity = 4,
					Outcome = 5,
					RelationshipToInvestigationalDrug = 6
				};
				fRep.Setup(r => r.GetByKey(13)).Returns(form);

				//here we return "formdata before editing" - this data should be overwritten from model
				afdRep.Setup(r => r.GetByKey(12)).Returns((AdverseEventFormData) null);

				//Act
				var result = formController.EditAdverseEventForm(model);

				//Assert
				CheckErrorInfoViewResult(result);
			}

			[Test]
			public void PostEditAdverseEventFormData_NonExistingForm_Test() {
				//Arrange
				var model = new AdverseEventFormViewModel {
					Id = 12,
					FormId = 13,
					OnsetDate = new DateTime(2010, 1, 2),
					OnsetTime = new DateTime(2011, 1, 1, 5, 4, 3),
					EndDate = new DateTime(2011, 11, 12),
					EndTime = new DateTime(2012, 1, 1, 14, 15, 16),
					Intensity = 4,
					Outcome = 5,
					RelationshipToInvestigationalDrug = 6
				};
				fRep.Setup(r => r.GetByKey(13)).Returns((Form) null);

				//Act
				var result = formController.EditAdverseEventForm(model);

				//Assert
				CheckErrorInfoViewResult(result);
			}

			private void CheckEditFormDataRedirectRouteValues(ActionResult result, Form form) {
				Assert.That(result, Is.Not.Null);
				Assert.That(result is RedirectToRouteResult);

				var viewResultBase = result as RedirectToRouteResult;
				Assert.That(viewResultBase.RouteValues, Is.Not.Null);
				Assert.That(viewResultBase.RouteValues.Count, Is.GreaterThanOrEqualTo(5));

				Assert.That(viewResultBase.RouteValues.Where(x => x.Key == "action").FirstOrDefault(), Is.Not.Null);
				Assert.That(viewResultBase.RouteValues.Where(x => x.Key == "action").FirstOrDefault().Value, Is.EqualTo("ViewForm"));

				Assert.That(viewResultBase.RouteValues.Where(x => x.Key == "controller").FirstOrDefault(), Is.Not.Null);
				Assert.That(viewResultBase.RouteValues.Where(x => x.Key == "controller").FirstOrDefault().Value, Is.EqualTo("Form"));

				Assert.That(viewResultBase.RouteValues.Where(x => x.Key == "patientNumber").FirstOrDefault(), Is.Not.Null);
				Assert.That(viewResultBase.RouteValues.Where(x => x.Key == "patientNumber").FirstOrDefault().Value,
				            Is.EqualTo(form.Visit.Patient.PatientNumber));

				Assert.That(viewResultBase.RouteValues.Where(x => x.Key == "visitName").FirstOrDefault(), Is.Not.Null);
				Assert.That(viewResultBase.RouteValues.Where(x => x.Key == "visitName").FirstOrDefault().Value,
				            Is.EqualTo(form.Visit.Caption));

				Assert.That(viewResultBase.RouteValues.Where(x => x.Key == "formName").FirstOrDefault(), Is.Not.Null);
				Assert.That(viewResultBase.RouteValues.Where(x => x.Key == "formName").FirstOrDefault().Value,
				            Is.EqualTo(form.Caption));
			}

			private void CheckErrorInfoViewResult(ActionResult result) {
				Assert.That(result, Is.Not.Null);
				Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var errorModel = viewResultBase.Model as ErrorViewModel;
				Assert.That(errorModel.Caption, Is.EqualTo("Form is not found"));
			}



			private void AssertChangeInfo(Question question, string oldDisplayValue, string newDisplayValue, BaseFormDataViewModel model) {
				AssertChangeInfo(question.Id, question.Caption, oldDisplayValue, newDisplayValue, model);
			}

			private void AssertChangeInfo(int questionId, string caption, string oldDisplayValue, string newDisplayValue,
										  BaseFormDataViewModel model) {
				ChangeNoteViewModel changeInfo;

				changeInfo = model.ChangeInfos.FirstOrDefault(ci => ci.QuestionId == questionId);
				Assert.That(changeInfo, Is.Not.Null, caption + " change info (" + questionId + ") was not found");
				Assert.That(changeInfo.QuestionName, Is.EqualTo(caption), "Captions does not match");
				Assert.That(changeInfo.NewValue, Is.EqualTo(newDisplayValue), "New display value for " + caption + " does not match");
				Assert.That(changeInfo.OriginalValue, Is.EqualTo(oldDisplayValue), "Old display value for " + caption + " does not match");
				Assert.That(string.IsNullOrEmpty(changeInfo.ChangeReason));
			}

		}

		[TestFixture]
		public class GetViewFormActions : ControllerTestsBase {
			private Mock<IFormRepository> fRep;
			private Mock<IDemographicFormDataRepository> dfdRep;
			private Mock<IVitalsFormDataRepository> vfdRep;
			private Mock<IHappinessFormDataRepository> hfdRep;
			private Mock<IElectrocardiogramFormDataRepository> efdRep;
			private Mock<IInventoryFormDataRepository> ifdRep;
			private Mock<IAdverseEventFormDataRepository> afdRep;
			private Mock<IAttachmentRepository> attachmentRep;
			private Mock<IQueryRepository> queryRep;

			private FormController formController;

			[SetUp]
			public void TestSetup() {
				fRep = new Mock<IFormRepository>();
				dfdRep = new Mock<IDemographicFormDataRepository>();
				vfdRep = new Mock<IVitalsFormDataRepository>();
				hfdRep = new Mock<IHappinessFormDataRepository>();
				efdRep = new Mock<IElectrocardiogramFormDataRepository>();
				ifdRep = new Mock<IInventoryFormDataRepository>();
				afdRep = new Mock<IAdverseEventFormDataRepository>();
				attachmentRep = new Mock<IAttachmentRepository>();
				queryRep = new Mock<IQueryRepository>();

				formController = new FormController(fRep.Object, dfdRep.Object, vfdRep.Object, hfdRep.Object, efdRep.Object,
				                                    ifdRep.Object, afdRep.Object, attachmentRep.Object, queryRep.Object);
			}

			[Test]
			public void GetEditForm_Demographic_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 31,
					Caption = "TestForm32",
					Visit = new Visit {Caption = "TestVisit33", Patient = new Patient {PatientNumber = 34}},
					FormType = FormType.Demographics,
					FormState = FormState.Completed
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewDemographicForm"));
				Assert.That(viewResultBase.Model is DemographicFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Demographic_CorrectView_Ajax_Test() {
				//Arrange
				EmulateControllerContext(formController, true);
				var form = new Form {
					Id = 32,
					Caption = "TestForm33",
					Visit = new Visit {Caption = "TestVisit34", Patient = new Patient {PatientNumber = 35}},
					FormType = FormType.Demographics,
					FormState = FormState.Completed
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewDemographicForm"));
				Assert.That(viewResultBase.Model is DemographicFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Demographic_NonExistingFormData_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 103,
					Caption = "TestForm103",
					Visit = new Visit {Caption = "TestVisit103", Patient = new Patient {PatientNumber = 203}},
					FormType = FormType.Demographics,
					FormState = FormState.Completed
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				dfdRep.Setup(dr => dr.GetFormDataByFormId(form.Id)).Returns((DemographicFormData) null);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Vitals_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 33,
					Caption = "TestForm34",
					Visit = new Visit {Caption = "TestVisit35", Patient = new Patient {PatientNumber = 36}},
					FormType = FormType.Vitals
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewVitalsForm"));
				Assert.That(viewResultBase.Model is VitalsFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Vitals_CorrectView_Ajax_Test() {
				//Arrange
				EmulateControllerContext(formController, true);
				var form = new Form {
					Id = 34,
					Caption = "TestForm35",
					Visit = new Visit {Caption = "TestVisit36", Patient = new Patient {PatientNumber = 37}},
					FormType = FormType.Vitals
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewVitalsForm"));
				Assert.That(viewResultBase.Model is VitalsFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Vitals_NonExistingFormData_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 104,
					Caption = "TestForm104",
					Visit = new Visit {Caption = "TestVisit104", Patient = new Patient {PatientNumber = 204}},
					FormType = FormType.Vitals
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				vfdRep.Setup(vr => vr.GetFormDataByFormId(form.Id)).Returns((VitalsFormData) null);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));


				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Happiness_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 35,
					Caption = "TestForm36",
					Visit = new Visit {Caption = "TestVisit37", Patient = new Patient {PatientNumber = 38}},
					FormType = FormType.Happiness
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewHappinessForm"));
				Assert.That(viewResultBase.Model is HappinessFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Happiness_CorrectView_Ajax_Test() {
				//Arrange
				EmulateControllerContext(formController, true);
				var form = new Form {
					Id = 36,
					Caption = "TestForm37",
					Visit = new Visit {Caption = "TestVisit38", Patient = new Patient {PatientNumber = 39}},
					FormType = FormType.Happiness
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewHappinessForm"));
				Assert.That(viewResultBase.Model is HappinessFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Happiness_NonExistingFormData_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 105,
					Caption = "TestForm105",
					Visit = new Visit {Caption = "TestVisit105", Patient = new Patient {PatientNumber = 205}},
					FormType = FormType.Happiness
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				hfdRep.Setup(hr => hr.GetFormDataByFormId(form.Id)).Returns((HappinessFormData) null);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));


				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Electrocardiogram_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 37,
					Caption = "TestForm38",
					Visit = new Visit {Caption = "TestVisit39", Patient = new Patient {PatientNumber = 40}},
					FormType = FormType.Electrocardiogram
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewElectrocardiogramForm"));
				Assert.That(viewResultBase.Model is ElectrocardiogramFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Electrocardiogram_CorrectView_Ajax_Test() {
				//Arrange
				EmulateControllerContext(formController, true);
				var form = new Form {
					Id = 38,
					Caption = "TestForm39",
					Visit = new Visit {Caption = "TestVisit40", Patient = new Patient {PatientNumber = 41}},
					FormType = FormType.Electrocardiogram
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewElectrocardiogramForm"));
				Assert.That(viewResultBase.Model is ElectrocardiogramFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Electrocardiogram_NonExistingFormData_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 106,
					Caption = "TestForm106",
					Visit = new Visit {Caption = "TestVisit106", Patient = new Patient {PatientNumber = 206}},
					FormType = FormType.Electrocardiogram
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				efdRep.Setup(er => er.GetFormDataByFormId(form.Id)).Returns((ElectrocardiogramFormData) null);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));


				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Inventory_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 39,
					Caption = "TestForm40",
					Visit = new Visit {Caption = "TestVisit41", Patient = new Patient {PatientNumber = 42}},
					FormType = FormType.Inventory
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewInventoryForm"));
				Assert.That(viewResultBase.Model is InventoryFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Inventory_CorrectView_Ajax_Test() {
				//Arrange
				EmulateControllerContext(formController, true);
				var form = new Form {
					Id = 40,
					Caption = "TestForm41",
					Visit = new Visit {Caption = "TestVisit42", Patient = new Patient {PatientNumber = 43}},
					FormType = FormType.Inventory
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewInventoryForm"));
				Assert.That(viewResultBase.Model is InventoryFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_Inventory_NonExistingFormData_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 107,
					Caption = "TestForm107",
					Visit = new Visit {Caption = "TestVisit107", Patient = new Patient {PatientNumber = 207}},
					FormType = FormType.Inventory
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				ifdRep.Setup(ir => ir.GetFormDataByFormId(form.Id)).Returns((InventoryFormData) null);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));


				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_AdverseEvent_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 41,
					Caption = "TestForm42",
					Visit = new Visit {Caption = "TestVisit43", Patient = new Patient {PatientNumber = 44}},
					FormType = FormType.AdverseEvent
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewAdverseEventForm"));
				Assert.That(viewResultBase.Model is AdverseEventFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_AdverseEvent_CorrectView_Ajax_Test() {
				//Arrange
				EmulateControllerContext(formController, true);
				var form = new Form {
					Id = 42,
					Caption = "TestForm43",
					Visit = new Visit {Caption = "TestVisit44", Patient = new Patient {PatientNumber = 45}},
					FormType = FormType.AdverseEvent
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				SetupFormDataRepositories(form);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewAdverseEventForm"));
				Assert.That(viewResultBase.Model is AdverseEventFormViewModel);

				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_AdverseEvent_NonExistingFormData_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 108,
					Caption = "TestForm108",
					Visit = new Visit {Caption = "TestVisit108", Patient = new Patient {PatientNumber = 208}},
					FormType = FormType.AdverseEvent
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);
				afdRep.Setup(ar => ar.GetFormDataByFormId(form.Id)).Returns((AdverseEventFormData) null);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));


				VerifyRepositoriesCalls(form);
			}

			[Test]
			public void GetEditForm_NonExistingForm_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 101,
					Caption = "TestForm101",
					Visit = new Visit {Caption = "TestVisit101", Patient = new Patient {PatientNumber = 201}},
					FormType = FormType.AdverseEvent
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns((Form) null);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));

				VerifyRepositoriesCalls(form, true);
			}

			[Test]
			public void GetEditForm_UndefinedFormType_CorrectView_Test() {
				//Arrange
				EmulateControllerContext(formController, false);
				var form = new Form {
					Id = 102,
					Caption = "TestForm102",
					Visit = new Visit {Caption = "TestVisit102", Patient = new Patient {PatientNumber = 202}},
					FormType = FormType.None
				};
				fRep.Setup(fr => fr.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption)).Returns(form);

				//Act
				var viewResult = formController.ViewForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is ViewResultBase);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("ErrorInfo"));
				Assert.That(viewResultBase.Model is ErrorViewModel);
				var model = viewResultBase.Model as ErrorViewModel;
				Assert.That(model.Caption, Is.EqualTo("Form is not found"));

				VerifyRepositoriesCalls(form, true);
			}

			private void SetupFormDataRepositories(Form form) {
				//Arrange
				dfdRep.Setup(dr => dr.GetFormDataByFormId(form.Id)).Returns(new DemographicFormData {
					Form = form,
					DateOfBirth =
				                                                            	new Question(),
					Other = new Question(),
					Race = new Question(),
					Sex = new Question()
				});
				vfdRep.Setup(vr => vr.GetFormDataByFormId(form.Id)).Returns(new VitalsFormData {
					Form = form,
					ActualTime = new Question(),
					Height = new Question(),
					Weight = new Question(),
					Temperature = new Question(),
					HeartRate = new Question(),
					BloodPressureDiastolic =
				                                                            	new Question(),
					BloodPressureSystolic =
				                                                            	new Question()
				});
				hfdRep.Setup(hr => hr.GetFormDataByFormId(form.Id)).Returns(new HappinessFormData {
					Form = form,
					HappinessLevel = new Question()
				});
				efdRep.Setup(er => er.GetFormDataByFormId(form.Id)).Returns(new ElectrocardiogramFormData {
					Form = form,
					ElectrocardiogramActualTime =
				                                                            	new Question()
				});
				ifdRep.Setup(ir => ir.GetFormDataByFormId(form.Id)).Returns(new InventoryFormData {
					Form = form,
					BatchNumber =
				                                                            	new Question(),
					MedicationUsage =
				                                                            	new List
				                                                            	<RepeatableInventoryData>(),
					QuantityShipped =
				                                                            	new Question(),
					ReceiptDate =
				                                                            	new Question(),
					ShipDate = new Question()
				});
				afdRep.Setup(ar => ar.GetFormDataByFormId(form.Id)).Returns(new AdverseEventFormData {
					Form = form,
					AdverseExperience =
				                                                            	new Question(),
					EndDate =
				                                                            	new Question(),
					EndTime =
				                                                            	new Question(),
					Intensity =
				                                                            	new Question(),
					OnsetDate =
				                                                            	new Question(),
					OnsetTime =
				                                                            	new Question(),
					Outcome =
				                                                            	new Question(),
					RelationshipToInvestigationalDrug
				                                                            	= new Question()
				});
			}

			private void VerifyRepositoriesCalls(Form form, bool zeroCallsOnFormData = false) {
				fRep.Verify(r => r.GetForm(form.Visit.Patient.PatientNumber, form.Visit.Caption, form.Caption), Times.Once());
				dfdRep.Verify(r => r.GetFormDataByFormId(form.Id),
				              !zeroCallsOnFormData && form.FormType == FormType.Demographics ? Times.Once() : Times.Never());
				vfdRep.Verify(r => r.GetFormDataByFormId(form.Id),
				              !zeroCallsOnFormData && form.FormType == FormType.Vitals ? Times.Once() : Times.Never());
				hfdRep.Verify(r => r.GetFormDataByFormId(form.Id),
				              !zeroCallsOnFormData && form.FormType == FormType.Happiness ? Times.Once() : Times.Never());
				efdRep.Verify(r => r.GetFormDataByFormId(form.Id),
				              !zeroCallsOnFormData && form.FormType == FormType.Electrocardiogram ? Times.Once() : Times.Never());
				ifdRep.Verify(r => r.GetFormDataByFormId(form.Id),
				              !zeroCallsOnFormData && form.FormType == FormType.Inventory ? Times.Once() : Times.Never());
				afdRep.Verify(r => r.GetFormDataByFormId(form.Id),
				              !zeroCallsOnFormData && form.FormType == FormType.AdverseEvent ? Times.Once() : Times.Never());
			}
		}

		[TestFixture]
		public class InventoryDataGridEditing : ControllerTestsBase {
			private Mock<IInventoryFormDataRepository> ifdRep;
			private Mock<HttpSessionStateBase> session;
			private FormController formController;

			private InventoryFormData inventoryData;
			private List<RepeatableInventoryDataViewModel> repeatableInventoryViewModel;
			private RepeatableInventoryDataViewModel newRepeatableInventoryViewModel;

			[SetUp]
			public void TestSetup() {
				inventoryData = new InventoryFormData {
					Id = 12,
					Form =
						new Form {
							Id = 17,
							Caption = "TestForm",
							Visit = new Visit {Caption = "TstVisit", Patient = new Patient {PatientNumber = 978}}
						},
					BatchNumber = new Question {Value = "15"},
					QuantityShipped = new Question {Value = "16"},
					ReceiptDate =
						new Question {Value = DateTime.Today.ToString(CultureInfo.InvariantCulture)},
					ShipDate =
						new Question
						{Value = DateTime.Today.AddDays(-1).ToString(CultureInfo.InvariantCulture)},
					MedicationUsage = new List<RepeatableInventoryData> {
						new RepeatableInventoryData {
							Id = 34,
							DateUsed =
								new Question {
									Value =
										DateTime.Today.
											AddDays(-2).ToString
											(CultureInfo.
											 	InvariantCulture)
								},
							QuantityUsed =
								new Question
								{Value = "17"}
						},
						new RepeatableInventoryData {
							Id = 35,
							DateUsed =
								new Question {
									Value =
										DateTime.Today.
											AddDays(-3).ToString
											(CultureInfo.
											 	InvariantCulture)
								},
							QuantityUsed =
								new Question
								{Value = "18"}
						}
					}
				};

				repeatableInventoryViewModel = new List<RepeatableInventoryDataViewModel> {
					new RepeatableInventoryDataViewModel {
						Id = 134,	
						InnerId = 34,
						DateUsed = DateTime.Today.AddDays(-2),
						QuantityUsed = 17
					},
					new RepeatableInventoryDataViewModel {
						Id = 135,
						InnerId = 35,
						DateUsed =DateTime.Today.AddDays(-3),
						QuantityUsed = 18
					}
				};
				
				newRepeatableInventoryViewModel = new RepeatableInventoryDataViewModel {
					Id = 0,
					InnerId = 0,
					DateUsed = DateTime.Today.AddDays(8),
					QuantityUsed = 27
				};

				ifdRep = new Mock<IInventoryFormDataRepository>();
				formController = new FormController(null, null, null, null, null, ifdRep.Object, null, null, null);

				session = new Mock<HttpSessionStateBase>();
			}

			[Test]
			public void InlineViewingRepeatableInventoryData_CorrectView() {
				//Arrange
				ifdRep.Setup(fr => fr.GetFormDataByFormId(111)).Returns(inventoryData);

				//Act
				var viewResult = formController.InlineViewingRepeatableInventoryData(111);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(2));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewRepeatableInventoryData"));

				ifdRep.Verify(r => r.GetFormDataByFormId(111), Times.Once());
			}

			[Test]
			public void InlineViewingRepeatableInventoryData_MissingInventoryFormIdParameter() {
				//Arrange
				ifdRep.Setup(fr => fr.GetFormDataByFormId(112)).Returns(inventoryData);

				//Act
				var viewResult = formController.InlineViewingRepeatableInventoryData((int?) null);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewRepeatableInventoryData"));

				ifdRep.Verify(r => r.GetFormDataByFormId(112), Times.Never());
			}

			[Test]
			public void InlineViewingRepeatableInventoryData_IncorrectInventoryFormIdParameter() {
				//Arrange
				ifdRep.Setup(fr => fr.GetFormDataByFormId(113)).Returns(inventoryData);

				//Act
				var viewResult = formController.InlineViewingRepeatableInventoryData(0);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewRepeatableInventoryData"));

				ifdRep.Verify(r => r.GetFormDataByFormId(113), Times.Never());
				ifdRep.Verify(r => r.GetFormDataByFormId(0), Times.Never());
			}

			[Test]
			public void InlineViewingRepeatableInventoryData_FormDataIsNull() {
				//Arrange
				ifdRep.Setup(fr => fr.GetFormDataByFormId(114)).Returns((InventoryFormData) null);

				//Act
				var viewResult = formController.InlineViewingRepeatableInventoryData(114);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewRepeatableInventoryData"));

				ifdRep.Verify(r => r.GetFormDataByFormId(114), Times.Once());
			}

			[Test]
			public void InlineEditingRepeatableInventoryData_CorrectView() {
				//Arrange
				session.SetupGet(x => x["ird_111"]).Returns(repeatableInventoryViewModel);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingRepeatableInventoryData(111);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(2));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_111"], Times.Once());
			}

			[Test]
			public void InlineEditingRepeatableInventoryData_MissingInventoryFormIdParameter() {
				//Arrange
				session.SetupGet(x => x["ird_112"]).Returns(repeatableInventoryViewModel);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingRepeatableInventoryData((int?) null);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_112"], Times.Never());
				session.Verify(s => s["ird_0"], Times.Once());
			}

			[Test]
			public void InlineEditingRepeatableInventoryData_IncorrectInventoryFormIdParameter() {
				//Arrange
				session.SetupGet(x => x["ird_113"]).Returns(repeatableInventoryViewModel);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingRepeatableInventoryData(0);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_113"], Times.Never());
				session.Verify(s => s["ird_0"], Times.Once());
			}

			[Test]
			public void InlineEditingRepeatableInventoryData_FormDataIsNull() {
				//Arrange
				session.SetupGet(x => x["ird_114"]).Returns(null);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingRepeatableInventoryData(114);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_114"], Times.Once());
			}

			[Test]
			public void InlineEditingAddNewRepeatableInventoryData_CorrectView() {
				//Arrange
				session.SetupGet(x => x["ird_211"]).Returns(repeatableInventoryViewModel);
				EmulateControllerContext(formController, session);
				//Act
				var viewResult = formController.InlineEditingAddNewRepeatableInventoryData(211, newRepeatableInventoryViewModel);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(3));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_211"], Times.Once());
			}

			[Test]
			public void InlineEditingAddNewRepeatableInventoryData_MissingInventoryFormIdParameter() {
				//Arrange
				session.SetupGet(x => x["ird_212"]).Returns(repeatableInventoryViewModel);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingAddNewRepeatableInventoryData((int?) null,
				                                                                           newRepeatableInventoryViewModel);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_212"], Times.Never());
				session.Verify(s => s["ird_0"], Times.Once());
			}

			[Test]
			public void InlineEditingAddNewRepeatableInventoryData_IncorrectInventoryFormIdParameter() {
				//Arrange
				session.SetupGet(x => x["ird_213"]).Returns(repeatableInventoryViewModel);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingAddNewRepeatableInventoryData(0, newRepeatableInventoryViewModel);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_213"], Times.Never());
				session.Verify(s => s["ird_0"], Times.Once());
			}

			[Test]
			public void InlineEditingAddNewRepeatableInventoryData_FormDataIsNull() {
				//Arrange
				session.SetupGet(x => x["ird_214"]).Returns(null);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingAddNewRepeatableInventoryData(214, newRepeatableInventoryViewModel);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_214"], Times.Once());
			}

			[Test]
			public void InlineEditingUpdateRepeatableInventoryData_CorrectView() {
				//Arrange
				session.SetupGet(x => x["ird_311"]).Returns(repeatableInventoryViewModel);
				EmulateControllerContext(formController, session);
				newRepeatableInventoryViewModel.InnerId = 34;

				//Act
				var viewResult = formController.InlineEditingUpdateRepeatableInventoryData(311, newRepeatableInventoryViewModel);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(2));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_311"], Times.Once());
			}

			[Test]
			public void InlineEditingUpdateRepeatableInventoryData_CorrectUpdate() {
				//Arrange
				session.SetupGet(x => x["ird_312"]).Returns(repeatableInventoryViewModel);
				EmulateControllerContext(formController, session);
				newRepeatableInventoryViewModel.InnerId = 34;

				//Act
				var viewResult = formController.InlineEditingUpdateRepeatableInventoryData(312, newRepeatableInventoryViewModel);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(2));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				var updatedData = model.MedicationUsage.Where(x => x.InnerId == 34).FirstOrDefault();
				Assert.That(updatedData, Is.Not.Null);
				Assert.That(updatedData.DateUsed, Is.EqualTo(DateTime.Today.AddDays(8)));
				Assert.That(updatedData.QuantityUsed, Is.EqualTo(27));

				var untouchedData = model.MedicationUsage.Where(x => x.InnerId == 35).FirstOrDefault();
				Assert.That(untouchedData, Is.Not.Null);
				Assert.That(untouchedData.DateUsed, Is.EqualTo(DateTime.Today.AddDays(-3)));
				Assert.That(untouchedData.QuantityUsed, Is.EqualTo(18));

				session.Verify(s => s["ird_312"], Times.Once());
			}

			[Test]
			public void InlineEditingUpdateRepeatableInventoryData_MissingInventoryFormIdParameter() {
				//Arrange
				session.SetupGet(x => x["ird_313"]).Returns(repeatableInventoryViewModel);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingUpdateRepeatableInventoryData((int?) null,
				                                                                           newRepeatableInventoryViewModel);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_313"], Times.Never());
				session.Verify(s => s["ird_0"], Times.Once());
			}

			[Test]
			public void InlineEditingUpdateRepeatableInventoryData_IncorrectInventoryFormIdParameter() {
				//Arrange
				session.SetupGet(x => x["ird_314"]).Returns(repeatableInventoryViewModel);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingUpdateRepeatableInventoryData(0, newRepeatableInventoryViewModel);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_314"], Times.Never());
				session.Verify(s => s["ird_0"], Times.Once());
			}

			[Test]
			public void InlineEditingUpdateRepeatableInventoryData_FormDataIsNull() {
				//Arrange
				session.SetupGet(x => x["ird_315"]).Returns(null);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingUpdateRepeatableInventoryData(315, newRepeatableInventoryViewModel);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_315"], Times.Once());
			}

			[Test]
			public void InlineEditingDeleteRepeatableInventoryData_CorrectView() {
				//Arrange
				session.SetupGet(x => x["ird_411"]).Returns(repeatableInventoryViewModel);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingDeleteRepeatableInventoryData(411, 34);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(1));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_411"], Times.Once());
			}

			[Test]
			public void InlineEditingDeleteRepeatableInventoryData_CorrectDelete() {
				//Arrange
				session.SetupGet(x => x["ird_412"]).Returns(repeatableInventoryViewModel);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingDeleteRepeatableInventoryData(412, 34);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(1));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				var updatedData = model.MedicationUsage.Where(x => x.InnerId == 34).FirstOrDefault();
				Assert.That(updatedData, Is.Null);

				var untouchedData = model.MedicationUsage.Where(x => x.InnerId == 35).FirstOrDefault();
				Assert.That(untouchedData, Is.Not.Null);
				Assert.That(untouchedData.DateUsed, Is.EqualTo(DateTime.Today.AddDays(-3)));
				Assert.That(untouchedData.QuantityUsed, Is.EqualTo(18));

				session.Verify(s => s["ird_412"], Times.Once());
			}

			[Test]
			public void InlineEditingDeleteRepeatableInventoryData_MissingInventoryFormIdParameter() {
				//Arrange
				session.SetupGet(x => x["ird_413"]).Returns(repeatableInventoryViewModel);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingDeleteRepeatableInventoryData((int?) null, 34);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_413"], Times.Never());
				session.Verify(s => s["ird_0"], Times.Once());
			}

			[Test]
			public void InlineEditingDeleteRepeatableInventoryData_IncorrectInventoryFormIdParameter() {
				//Arrange
				session.SetupGet(x => x["ird_414"]).Returns(repeatableInventoryViewModel);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingDeleteRepeatableInventoryData(0, 34);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_414"], Times.Never());
				session.Verify(s => s["ird_0"], Times.Once());
			}

			[Test]
			public void InlineEditingDeleteRepeatableInventoryData_FormDataIsNull() {
				//Arrange
				session.SetupGet(x => x["ird_415"]).Returns(null);
				EmulateControllerContext(formController, session);

				//Act
				var viewResult = formController.InlineEditingDeleteRepeatableInventoryData(415, 34);

				//Assert
				Assert.That(viewResult, Is.Not.Null);
				Assert.That(viewResult is PartialViewResult);
				var viewResultBase = viewResult as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.Model is InventoryFormViewModel);
				var model = viewResultBase.Model as InventoryFormViewModel;
				Assert.That(model.MedicationUsage, Is.Not.Null);
				Assert.That(model.MedicationUsage.Count, Is.EqualTo(0));
				Assert.That(viewResultBase.ViewData["EditError"], Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditRepeatableInventoryData"));

				session.Verify(s => s["ird_415"], Times.Once());
			}
		}
	}
}
