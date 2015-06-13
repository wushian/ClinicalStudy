using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.DomainModel.ModelBuilders;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.Interface.FormData;
using ClinicalStudy.Site.Areas.DataCapture.Controllers;
using ClinicalStudy.Site.Areas.DataCapture.Models;
using ClinicalStudy.Site.Areas.DataCapture.Models.Patient;
using ClinicalStudy.Site.Areas.DataCapture.Models.Shared;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Site.Tests.DataCapture {
	[TestFixture]
	[SetCulture("en-US")]
	public class PatientControllerTests : ControllerTestsBase {
		private Mock<IPatientRepository> repository = null;
		private Mock<IUserRepository> userRepository = null;
		private Mock<IDemographicFormDataRepository> demographicRepository = null;
		private Mock<IClinicalStudyDesignFactory> clinicalStudyDesignFactory = null;

		private PatientController controller = null;

		[SetUp]
		public void Setup() {
			userRepository = new Mock<IUserRepository>();
			repository = new Mock<IPatientRepository>();
			demographicRepository = new Mock<IDemographicFormDataRepository>();
			clinicalStudyDesignFactory = new Mock<IClinicalStudyDesignFactory>();

			controller = new PatientController(userRepository.Object, repository.Object, demographicRepository.Object, clinicalStudyDesignFactory.Object);
		}

		[Test]
		public void HeaderGroupTest() {
			//Arrange
			repository.Setup(r => r.GetPatientByUniqueNumber(25)).Returns(new Patient {
				Caption = "Subj 25",
				PatientInitials = "AK",
				Doctor = new User() {Clinic = new Clinic() {Caption = "Test Clinic"}},
				PatientNumber = 25
			});
			//act 
			var result = controller.PatientHeaderGroup(25, "Summary");

			//assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var partialResult = result as PartialViewResult;
			Assert.That(partialResult.Model, Is.Not.Null);
			Assert.That(partialResult.Model is HeaderGroupViewModel, "Wrong datatype: " + partialResult.Model.GetType().Name);
			var model = partialResult.Model as HeaderGroupViewModel;
			Assert.That(model.PatientNumber, Is.EqualTo(25));
			Assert.That(model.PatientCaption, Is.EqualTo("Subj 25"));
			Assert.That(model.PatientInitials, Is.EqualTo("AK"));
			Assert.That(model.ClinicCaption, Is.EqualTo("Test Clinic"));
			Assert.That(model.VisitDate, Is.EqualTo(null));
		}

		[Test]
		public void CreatePatientTest() {
			//Arrange
			base.EmulateControllerContext(controller, false);
			repository.Setup(r => r.GetMaxPatientNumber()).Returns(27);


			//Act
			var result = controller.CreatePatient();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var partialResult = result as ViewResultBase;
			Assert.That(partialResult.Model, Is.Not.Null);
			Assert.That(partialResult.Model is PatientViewModel);
			var model = partialResult.Model as PatientViewModel;
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.IsActive, Is.True);
		}

		[Test]
		public void CreatePatient_Ajax_Test() {
			//Arrange
			base.EmulateControllerContext(controller, true);
			repository.Setup(r => r.GetMaxPatientNumber()).Returns(27);


			//Act
			var result = controller.CreatePatient();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var partialResult = result as ViewResultBase;
			Assert.That(partialResult.Model, Is.Not.Null);
			Assert.That(partialResult.Model is PatientViewModel);
			var model = partialResult.Model as PatientViewModel;
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.IsActive, Is.True);
		}

		[Test]
		public void CreatePatientTestSetCorrectUniquePatientNumber() {
			//Arrange
			EmulateControllerContext(controller, false);
			repository.Setup(r => r.GetMaxPatientNumber()).Returns(27);


			//Act
			var result = controller.CreatePatient();

			//Assert
			Assert.That(result, Is.Not.Null);
			var partialResult = result as ViewResultBase;
			var model = partialResult.Model as PatientViewModel;
			Assert.That(model.Id, Is.EqualTo(0));
			Assert.That(model.PatientNumber, Is.EqualTo(28));
			repository.Verify(r => r.GetMaxPatientNumber(), Times.Once());
		}


		[Test]
		public void ShowPatient_Incomplete_CorrectView_Ajax_Test() {
			//Arrange
			EmulateControllerContext(controller, true);
			int patientNumber = 88;
			repository.Setup(r => r.GetPatientByUniqueNumber(patientNumber)).Returns(new Patient() {
				Id = 117,
				PatientNumber = patientNumber,
				IsCompleted = false
			});

			//Act
			var result = controller.ShowPatient(patientNumber);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditPatient"));
		}

		[Test]
		public void ShowPatient_Complete_CorrectView_Ajax_Test() {
			//Arrange
			EmulateControllerContext(controller, true);
			int patientNumber = 88;
			repository.Setup(r => r.GetPatientByUniqueNumber(patientNumber)).Returns(new Patient() {
				Id = 117,
				PatientNumber = patientNumber,
				IsCompleted = true
			});

			//Act
			var result = controller.ShowPatient(patientNumber);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_ViewPatient"));
		}
		[Test]
		public void EditPatient_CorrectView_Ajax_Test() {
			//Arrange
			EmulateControllerContext(controller, true);
			int patientNumber = 88;
			repository.Setup(r => r.GetPatientByUniqueNumber(patientNumber)).Returns(new Patient() {
				Id = 117,
				PatientNumber = patientNumber,
				IsCompleted = true
			});

			//Act
			var result = controller.EditPatient(patientNumber);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_EditPatient"));
		}

		[Test]
		public void GetEditPatient_CorrectBinding_Test() {
			//Arrange
			EmulateControllerContext(controller, false);
			int patientNumber = 88;
			var patient = new Patient() {
				Id = 117,
				PatientNumber = patientNumber,
				IsActive = true,
				IsEnrolled = true,
				EnrollDate = new DateTime(2012, 1, 25),
				Caption = "A088",
				PatientInitials = "AK",
				RandomisationDate = new DateTime(2012, 1, 27),
				RandomisationNumber = 92
			};
			repository.Setup(r => r.GetPatientByUniqueNumber(patientNumber)).Returns(patient);

			//Act
			var result = controller.ShowPatient(patientNumber);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.Model is PatientViewModel);
			var model = viewResultBase.Model as PatientViewModel;
			Assert.That(model.Id, Is.EqualTo(117));
			Assert.That(model.Caption, Is.EqualTo(patient.Caption));
			Assert.That(model.PatientNumber, Is.EqualTo(patient.PatientNumber));
			Assert.That(model.IsActive, Is.EqualTo(patient.IsActive));
			Assert.That(model.IsEnrolled, Is.EqualTo(patient.IsEnrolled));
			Assert.That(model.EnrollDate, Is.EqualTo(patient.EnrollDate));
			Assert.That(model.PatientInitials, Is.EqualTo(patient.PatientInitials));
			Assert.That(model.RandomisationNumber, Is.EqualTo(patient.RandomisationNumber));
			Assert.That(model.RandomisationDate, Is.EqualTo(patient.RandomisationDate));
			repository.Verify(r => r.GetPatientByUniqueNumber(patientNumber), Times.Once());
		}

		[Test]
		public void PostCreatePatientTest() {
			//Arrange
			var studyDesign = new Mock<IClinicalStudyDesign>();
			EmulateControllerContext(controller, false);
			clinicalStudyDesignFactory.Setup(factory => factory.Create()).Returns(studyDesign.Object);


			PatientViewModel model = new PatientViewModel {
				Id = 0,
				Caption = null,
				IsActive = true,
				PatientNumber = 44,
				IsEnrolled = true,
				EnrollDate = new DateTime(2012, 1, 24),
				PatientInitials = "AK",
				RandomisationDate = new DateTime(2012, 1, 25),
				RandomisationNumber = 88
			};

			//as checking of saved patient is quite complicated, we will save the passed object and inspect it later
			Patient savedPatient = new Patient() {
				Caption = "Subj A044",
				PatientNumber = 44,
				Doctor = new User() {Id = 15}
			};
			studyDesign.Setup(sd => sd.CreatePatientForDoctor("doctorName")).Returns(savedPatient);

			//Act
			var result = controller.EditPatient(model);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is RedirectToRouteResult);

			repository.Verify(r => r.Save(), Times.Once());
			Assert.That(savedPatient, Is.Not.Null);
			Assert.That(savedPatient.Caption, Is.EqualTo("Subj A044"));
			Assert.That(savedPatient.IsActive, Is.True);
			Assert.That(savedPatient.PatientNumber, Is.EqualTo(44));
			Assert.That(savedPatient.IsEnrolled, Is.True);
			Assert.That(savedPatient.EnrollDate, Is.EqualTo(new DateTime(2012, 1, 24)));
			Assert.That(savedPatient.PatientInitials, Is.EqualTo("AK"));
			Assert.That(savedPatient.RandomisationDate, Is.EqualTo(new DateTime(2012, 1, 25)));
			Assert.That(savedPatient.RandomisationNumber, Is.EqualTo(88));
			Assert.That(savedPatient.Doctor, Is.Not.Null);
			Assert.That(savedPatient.Doctor.Id, Is.EqualTo(15));
			studyDesign.Verify(sd => sd.CreatePatientForDoctor("doctorName"), Times.Once());
		}

		[Test]
		public void PostEditExistingPatientTest() {
			//Arrange
			EmulateControllerContext(controller, false);
			PatientViewModel model = new PatientViewModel {
				Id = 12,
				Caption = null,
				IsActive = true,
				PatientNumber = 44,
				IsEnrolled = true,
				EnrollDate = new DateTime(2012, 1, 24),
				PatientInitials = "AK",
				RandomisationDate = new DateTime(2012, 1, 25),
				RandomisationNumber = 88
			};

			//here we return "patient before editing" - this data should be overwritten from model
			repository.Setup(r => r.GetByKey(It.IsAny<int>())).Returns(
				new Patient() {
					Id = 12,
					Caption = "A044",
					IsActive = false,
					PatientNumber = 44,
					IsEnrolled = false,
					EnrollDate = new DateTime(2012, 1, 04),
					PatientInitials = "PG",
					RandomisationDate = new DateTime(2012, 1, 05),
					RandomisationNumber = 77
				}
				);
			repository.Setup(r => r.GetMaxPatientNumber()).Returns(43);
			//as checking of saved patient is quite complicated, we will save the passed object and inspect it later
			Patient savedPatient = null;
			repository.Setup(r => r.Edit(It.IsAny<Patient>())).Callback<Patient>(p => savedPatient = p);


			//Act
			var result = controller.EditPatient(model);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is RedirectToRouteResult);

			repository.Verify(r => r.GetByKey(model.Id), Times.Once());
			repository.Verify(r => r.Add(It.IsAny<Patient>()), Times.Never());
			repository.Verify(r => r.Edit(It.IsAny<Patient>()), Times.Once());
			repository.Verify(r => r.Save(), Times.Once());
			Assert.That(savedPatient, Is.Not.Null);
			Assert.That(savedPatient.Caption, Is.EqualTo("A044"));
			Assert.That(savedPatient.IsActive, Is.True);
			Assert.That(savedPatient.PatientNumber, Is.EqualTo(44));
			Assert.That(savedPatient.IsEnrolled, Is.True);
			Assert.That(savedPatient.EnrollDate, Is.EqualTo(new DateTime(2012, 1, 24)));
			Assert.That(savedPatient.PatientInitials, Is.EqualTo("AK"));
			Assert.That(savedPatient.RandomisationDate, Is.EqualTo(new DateTime(2012, 1, 25)));
			Assert.That(savedPatient.RandomisationNumber, Is.EqualTo(88));
		}


		[Test]
		public void PatientGridView() {
			//Arrange
			EmulateControllerContext(controller, false);

			var doctor = new User { Id = 15, Login = CommonEmulatedUserName };
			userRepository.Setup(r => r.GetUserByLogin(CommonEmulatedUserName)).Returns(doctor);
			//Act
			var result = controller.PatientGrid();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var view = result as ViewResultBase;

			Assert.That(view.Model, Is.Not.Null);
			Assert.That(view.Model is PatientsListViewModel);
			var model = view.Model as PatientsListViewModel;

			Assert.That(model.CertainPatientIsActive, Is.True);
			Assert.That(model.CertainPatientPageIndex, Is.EqualTo(-1));
			Assert.That(model.CertainPatientRequested, Is.False);

			Assert.That(model.PatientsList, Is.Not.Null);
			Assert.That(model.PatientsList.Count, Is.EqualTo(0));
		}


		[Test]
		public void PatientGridModelSinglePatient() {
			//Arrange
			EmulateControllerContext(controller, false);
			var doctor = new User { Id = 15, Login = CommonEmulatedUserName };
			userRepository.Setup(r => r.GetUserByLogin(CommonEmulatedUserName)).Returns(doctor);

			var importantForm = new Form() {
				Id = 14,
				FormType = FormType.Demographics
			};


			var wrongForm1 = new Form() {
				Id = 25,
				FormType = FormType.Vitals
			};
			var wrongForm2 = new Form() {
				Id = 25,
				FormType = FormType.Vitals
			};


			var demographicData =
				new DemographicFormData {
					Sex = new Question() {Value = "1"},
					DateOfBirth = new Question() {Value = DateTime.Now.AddYears(-25).ToString(CultureInfo.InvariantCulture)},
					Form = importantForm
				};

			var patient =
				new Patient {
					Id = 4,
					Caption = "A025",
					PatientNumber = 25,
					IsActive = true,
					Doctor = doctor,
					Visits = new List<Visit> {
						new Visit() {
							VisitTypeValue = (int) VisitType.Baseline,
							Forms = new List<Form> {
								importantForm,
								wrongForm1
							}
						},
						new Visit() {
							Forms = new List<Form>() {
								wrongForm2
							}
						}
					}
				};

			repository.Setup(r => r.GetAll()).Returns((new List<Patient> {patient}).AsQueryable());
			demographicRepository.Setup(r => r.GetAll()).Returns((new List<DemographicFormData> {demographicData}).AsQueryable());
			//Act
			var result = controller.PatientGrid();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var view = result as ViewResultBase;

			Assert.That(view.Model, Is.Not.Null);
			Assert.That(view.Model is PatientsListViewModel);
			var model = view.Model as PatientsListViewModel;

			Assert.That(model.CertainPatientIsActive, Is.True);
			Assert.That(model.CertainPatientPageIndex, Is.EqualTo(-1));
			Assert.That(model.CertainPatientRequested, Is.False);

			Assert.That(model.PatientsList, Is.Not.Null);
			Assert.That(model.PatientsList.Count, Is.EqualTo(1));
			var patientModel = model.PatientsList[0];
			
			Assert.That(patientModel.Caption, Is.EqualTo(patient.Caption));
			Assert.That(patientModel.Id, Is.EqualTo(patient.Id));
			Assert.That(patientModel.Gender, Is.EqualTo("Female"));
			Assert.That(patientModel.Age, Is.EqualTo("Age 25"));
			Assert.That(patientModel.PatientNumber, Is.EqualTo(patient.PatientNumber));
			Assert.That(patientModel.IsSelected, Is.False);
		}

		[Test]
		public void PatientGridModelSinglePatientOfAnotherDoctor() {
			//Arrange
			EmulateControllerContext(controller, false);
			var doctor = new User { Id = 15, Login = CommonEmulatedUserName };
			var anotherDoctor = new User { Id = 21, Login = CommonEmulatedUserName + "Another" };
			userRepository.Setup(r => r.GetUserByLogin(CommonEmulatedUserName)).Returns(doctor);

			var importantForm = new Form() {
				Id = 14,
				FormType = FormType.Demographics
			};


			var wrongForm1 = new Form() {
				Id = 25,
				FormType = FormType.Vitals
			};
			var wrongForm2 = new Form() {
				Id = 25,
				FormType = FormType.Vitals
			};


			var demographicData =
				new DemographicFormData {
					Sex = new Question() { Value = "1" },
					DateOfBirth = new Question() { Value = DateTime.Now.AddYears(-25).ToString(CultureInfo.InvariantCulture) },
					Form = importantForm
				};

			var patient =
				new Patient {
					Id = 4,
					Caption = "A025",
					PatientNumber = 25,
					IsActive = true,
					Doctor = anotherDoctor,
					Visits = new List<Visit> {
						new Visit() {
							VisitTypeValue = (int) VisitType.Baseline,
							Forms = new List<Form> {
								importantForm,
								wrongForm1
							}
						},
						new Visit() {
							Forms = new List<Form>() {
								wrongForm2
							}
						}
					}
				};

			repository.Setup(r => r.GetAll()).Returns((new List<Patient> { patient }).AsQueryable());
			demographicRepository.Setup(r => r.GetAll()).Returns((new List<DemographicFormData> { demographicData }).AsQueryable());
			//Act
			var result = controller.PatientGrid();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var view = result as ViewResultBase;

			Assert.That(view.Model, Is.Not.Null);
			Assert.That(view.Model is PatientsListViewModel);
			var model = view.Model as PatientsListViewModel;

			Assert.That(model.CertainPatientIsActive, Is.True);
			Assert.That(model.CertainPatientPageIndex, Is.EqualTo(-1));
			Assert.That(model.CertainPatientRequested, Is.False);

			Assert.That(model.PatientsList, Is.Not.Null);
			Assert.That(model.PatientsList.Count, Is.EqualTo(0));
		}


		[Test]
		public void PatientGridModelSingleSelectedPatient() {
			//Arrange
			var routeData = new RouteData();
			routeData.Values.Add("patientNumber", "25");
			EmulateControllerContext(controller, routeData);
			var doctor = new User { Id = 15, Login = CommonEmulatedUserName };
			userRepository.Setup(r => r.GetUserByLogin(CommonEmulatedUserName)).Returns(doctor);
			
			var importantForm = new Form() {
				Id = 14,
				FormType = FormType.Demographics
			};


			var wrongForm1 = new Form() {
				Id = 25,
				FormType = FormType.Vitals
			};
			var wrongForm2 = new Form() {
				Id = 25,
				FormType = FormType.Vitals
			};


			var demographicData =
				new DemographicFormData {
					Sex = new Question() {Value = "1"},
					DateOfBirth = new Question() {Value = DateTime.Now.AddYears(-25).ToString(CultureInfo.InvariantCulture)},
					Form = importantForm
				};

			var patient =
				new Patient {
					Id = 777,
					Caption = "A025",
					PatientNumber = 25,
					IsActive = true,
					Doctor = doctor,
					Visits = new List<Visit> {
						new Visit() {
							VisitTypeValue = (int) VisitType.Baseline,
							Forms = new List<Form> {
								importantForm,
								wrongForm1
							}
						},
						new Visit() {
							Forms = new List<Form>() {
								wrongForm2
							}
						}
					}
				};

			repository.Setup(r => r.GetAll()).Returns((new List<Patient> {patient}).AsQueryable());
			demographicRepository.Setup(r => r.GetAll()).Returns((new List<DemographicFormData> {demographicData}).AsQueryable());
			//Act
			var result = controller.PatientGrid();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var view = result as ViewResultBase;

			Assert.That(view.Model, Is.Not.Null);
			Assert.That(view.Model is PatientsListViewModel);
			var model = view.Model as PatientsListViewModel;

			Assert.That(model.CertainPatientIsActive, Is.True);
			Assert.That(model.CertainPatientPageIndex, Is.EqualTo(-1));
			Assert.That(model.CertainPatientRequested, Is.False);

			Assert.That(model.PatientsList, Is.Not.Null);
			Assert.That(model.PatientsList.Count, Is.EqualTo(1));
			var patientModel = model.PatientsList[0];

			Assert.That(patientModel.Caption, Is.EqualTo(patient.Caption));
			Assert.That(patientModel.Id, Is.EqualTo(patient.Id));
			Assert.That(patientModel.Gender, Is.EqualTo("Female"));
			Assert.That(patientModel.Age, Is.EqualTo("Age 25"));
			Assert.That(patientModel.PatientNumber, Is.EqualTo(patient.PatientNumber));
			Assert.That(patientModel.IsSelected, Is.True);
		}


		[Test]
		public void PatientGridModelSingleInactivePatient() {
			//Arrange
			EmulateControllerContext(controller, false);

;
			var doctor = new User {Id = 15, Login = CommonEmulatedUserName};
			userRepository.Setup(r => r.GetUserByLogin(CommonEmulatedUserName)).Returns(doctor);

			var importantForm = new Form() {
				Id = 14,
				FormType = FormType.Demographics
			};


			var wrongForm1 = new Form() {
				Id = 25,
				FormType = FormType.Vitals
			};
			var wrongForm2 = new Form() {
				Id = 25,
				FormType = FormType.Vitals
			};


			var demographicData =
				new DemographicFormData {
					Sex = new Question() {Value = "1"},
					DateOfBirth = new Question() {Value = DateTime.Now.AddYears(-25).ToString(CultureInfo.InvariantCulture)},
					Form = importantForm
				};

			var patient =
				new Patient {
					Id = 4,
					Caption = "A025",
					PatientNumber = 25,
					IsActive = false,
					Doctor =doctor,
					Visits = new List<Visit> {
						new Visit() {
							VisitTypeValue = (int) VisitType.Baseline,
							Forms = new List<Form> {
								importantForm,
								wrongForm1
							}
						},
						new Visit() {
							Forms = new List<Form>() {
								wrongForm2
							}
						}
					}
				};

			repository.Setup(r => r.GetAll()).Returns((new List<Patient> {patient}).AsQueryable());
			demographicRepository.Setup(r => r.GetAll()).Returns((new List<DemographicFormData> {demographicData}).AsQueryable());
			//Act
			var result = controller.PatientGrid(null, "inactive");

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var view = result as ViewResultBase;
			Assert.That(view.Model, Is.Not.Null);
			Assert.That(view.Model is PatientsListViewModel);
			var model = view.Model as PatientsListViewModel;

			Assert.That(model.CertainPatientIsActive, Is.False);
			Assert.That(model.CertainPatientPageIndex, Is.EqualTo(-1));
			Assert.That(model.CertainPatientRequested, Is.False);

			Assert.That(model.PatientsList, Is.Not.Null);
			Assert.That(model.PatientsList.Count, Is.EqualTo(1));
			var patientModel = model.PatientsList[0];
			Assert.That(patientModel.Caption, Is.EqualTo(patient.Caption));
			Assert.That(patientModel.Id, Is.EqualTo(patient.Id));
			Assert.That(patientModel.Gender, Is.EqualTo("Female"));
			Assert.That(patientModel.Age, Is.EqualTo("Age 25"));
			Assert.That(patientModel.PatientNumber, Is.EqualTo(patient.PatientNumber));
			Assert.That(patientModel.IsSelected, Is.False);
		}

		[Test]
		public void PatientDataContainerView() {
			//Arrange
			EmulateControllerContext(controller, false);

			repository.Setup(r => r.GetByKey(25)).Returns(new Patient());

			//Act
			var result = controller.PatientDataContainer(25, null, null);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResult);
			var view = result as ViewResultBase;
			var model = view.Model;
			Assert.That(model, Is.Not.Null);
		}

		[Test]
		public void PatientDataContainer_AjaxView() {
			//Arrange
			EmulateControllerContext(controller, true);

			repository.Setup(r => r.GetPatientByUniqueNumber(25)).Returns(new Patient() { Doctor = new User() { Login = CommonEmulatedUserName} });

			//Act
			var result = controller.PatientDataContainer(25, null, null);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var view = result as ViewResultBase;
			var model = view.Model;
			Assert.That(model, Is.Not.Null);
		}

		[Test]
		public void PatientDataContainerModel() {
			//Arrange
			EmulateControllerContext(controller, false);
			var patient = new Patient {
				Id = 11,
				Doctor = new User() { Login = CommonEmulatedUserName},
				Caption = "Subj A025",
				PatientInitials = "AK",
				PatientNumber = 25,
				Visits = new List<Visit> {
					new Visit {Id = 100, Caption = "Vitals", OrderNo = 1},
					new Visit {Id = 200, Caption = "Baseline", OrderNo = 0}
				}
			};
			repository
				.Setup(r => r.GetPatientByUniqueNumber(25))
				.Returns(patient);

			//Act
			var result = controller.PatientDataContainer(25, null, null);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var view = result as ViewResultBase;
			var model = view.Model as DataContainerViewModel;
			Assert.That(model, Is.Not.Null);

			Assert.That(model.Id, Is.EqualTo(patient.Id));
			Assert.That(model.PatientCaption, Is.EqualTo(patient.Caption));
			Assert.That(model.PatientNumber, Is.EqualTo(patient.PatientNumber));
			Assert.That(model.PatientInitials, Is.EqualTo(patient.PatientInitials));
			Assert.That(model.Children, Is.Not.Null);
			Assert.That(model.Children.Count, Is.EqualTo(2));
			Assert.That(model.Children[0].Id, Is.EqualTo(200));
			Assert.That(model.Children[0].Caption, Is.EqualTo("Baseline"));

			Assert.That(model.Children[1].Id, Is.EqualTo(100));
			Assert.That(model.Children[1].Caption, Is.EqualTo("Vitals"));
		}

		[Test]
		public void PatientDataContainerModelPassedSelectedValues() {
			//Arrange
			EmulateControllerContext(controller, false);
			var patient = new Patient {
				Id = 25, 
				Doctor = new User() { Login = CommonEmulatedUserName},
				Caption = "Subj A025",
				PatientInitials = "AK",
				Visits = new List<Visit> {
					new Visit {Id = 100, Caption = "1st Day", OrderNo = 1},
					new Visit {Id = 200, Caption = "Baseline", OrderNo = 0}
				}
			};
			repository
				.Setup(r => r.GetPatientByUniqueNumber(25))
				.Returns(patient);

			//Act
			var result = controller.PatientDataContainer(25, "baseline", "vitals");

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var view = result as ViewResultBase;
			var model = view.Model as DataContainerViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.SelectedVisitName, Is.EqualTo("baseline"));
			Assert.That(model.SelectedFormName, Is.EqualTo("vitals"));
		}

	}
}
