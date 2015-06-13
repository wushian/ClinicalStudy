using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.Analytics.Controllers;
using ClinicalStudy.Site.Areas.Analytics.Models;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Site.Tests.Analytics {
	[TestFixture]
	public class AdministrationControllerTest : ControllerTestsBase {
		[Test]
		public void Clinics_MasterGrid() {
			//Arrange
			var cRepository = new Mock<IClinicRepository>();
			var controller = new AdministrationController(cRepository.Object);
			cRepository.Setup(r => r.GetAll()).Returns((new List<Clinic> {
				new Clinic() {Id = 1, Caption = "Clinic1", Doctors = new List<User> {new User {Id = 11}, new User {Id = 12}}},
				new Clinic() {
					Id = 10,
					Caption = "Clinic10",
					Doctors = new List<User> {new User {Id = 101}, new User {Id = 102}, new User {Id = 103}}
				}
			}).AsQueryable());
			EmulateControllerContext(controller, false);

			//Act
			var result = controller.Clinics();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("Clinics"));
			Assert.That(viewResultBase.Model is ClinicMasterViewModel);
			var model = viewResultBase.Model as ClinicMasterViewModel;
			Assert.That(model.Clinics, Is.Not.Null);
			Assert.That(model.Clinics.Count, Is.EqualTo(2));

			Assert.That(model.Clinics[0].ClinicId, Is.EqualTo(1));
			Assert.That(model.Clinics[0].ClinicName, Is.EqualTo("Clinic1"));
			Assert.That(model.Clinics[0].DoctorsCount, Is.EqualTo(2));

			Assert.That(model.Clinics[1].ClinicId, Is.EqualTo(10));
			Assert.That(model.Clinics[1].ClinicName, Is.EqualTo("Clinic10"));
			Assert.That(model.Clinics[1].DoctorsCount, Is.EqualTo(3));

			cRepository.Verify(r => r.GetAll(), Times.Once());
		}

		[Test]
		public void Clinics_MasterGrid_Ajax() {
			//Arrange
			var cRepository = new Mock<IClinicRepository>();
			var controller = new AdministrationController(cRepository.Object);
			cRepository.Setup(r => r.GetAll()).Returns((new List<Clinic> {
				new Clinic() {Id = 1, Caption = "Clinic1", Doctors = new List<User> {new User {Id = 11}, new User {Id = 12}}},
				new Clinic() {
					Id = 10,
					Caption = "Clinic10",
					Doctors = new List<User> {new User {Id = 101}, new User {Id = 102}, new User {Id = 103}}
				}
			}).AsQueryable());
			EmulateControllerContext(controller, true);

			//Act
			var result = controller.Clinics();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_Clinics"));
			Assert.That(viewResultBase.Model is ClinicMasterViewModel);
			var model = viewResultBase.Model as ClinicMasterViewModel;
			Assert.That(model.Clinics, Is.Not.Null);
			Assert.That(model.Clinics.Count, Is.EqualTo(2));

			Assert.That(model.Clinics[0].ClinicId, Is.EqualTo(1));
			Assert.That(model.Clinics[0].ClinicName, Is.EqualTo("Clinic1"));
			Assert.That(model.Clinics[0].DoctorsCount, Is.EqualTo(2));

			Assert.That(model.Clinics[1].ClinicId, Is.EqualTo(10));
			Assert.That(model.Clinics[1].ClinicName, Is.EqualTo("Clinic10"));
			Assert.That(model.Clinics[1].DoctorsCount, Is.EqualTo(3));

			cRepository.Verify(r => r.GetAll(), Times.Once());
		}

		[Test]
		public void Clinics_MasterGrid_Partial()
		{
			//Arrange
			var cRepository = new Mock<IClinicRepository>();
			var controller = new AdministrationController(cRepository.Object);
			cRepository.Setup(r => r.GetAll()).Returns((new List<Clinic> {
				new Clinic() {Id = 1, Caption = "Clinic1", Doctors = new List<User> {new User {Id = 11}, new User {Id = 12}}},
				new Clinic() {
					Id = 10,
					Caption = "Clinic10",
					Doctors = new List<User> {new User {Id = 101}, new User {Id = 102}, new User {Id = 103}}
				}
			}).AsQueryable());
			EmulateControllerContext(controller, true);

			//Act
			var result = controller.ClinicsPartial();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_ClinicsGrid"));
			Assert.That(viewResultBase.Model is ClinicMasterViewModel);
			var model = viewResultBase.Model as ClinicMasterViewModel;
			Assert.That(model.Clinics, Is.Not.Null);
			Assert.That(model.Clinics.Count, Is.EqualTo(2));

			Assert.That(model.Clinics[0].ClinicId, Is.EqualTo(1));
			Assert.That(model.Clinics[0].ClinicName, Is.EqualTo("Clinic1"));
			Assert.That(model.Clinics[0].DoctorsCount, Is.EqualTo(2));

			Assert.That(model.Clinics[1].ClinicId, Is.EqualTo(10));
			Assert.That(model.Clinics[1].ClinicName, Is.EqualTo("Clinic10"));
			Assert.That(model.Clinics[1].DoctorsCount, Is.EqualTo(3));

			cRepository.Verify(r => r.GetAll(), Times.Once());
		}

		[Test]
		public void Clinics_MasterGrid_Empty() {
			//Arrange
			var cRepository = new Mock<IClinicRepository>();
			var controller = new AdministrationController(cRepository.Object);
			cRepository.Setup(r => r.GetAll()).Returns((new List<Clinic>()).AsQueryable());
			EmulateControllerContext(controller, true);
			//Act
			var result = controller.Clinics();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_Clinics"));
			Assert.That(viewResultBase.Model is ClinicMasterViewModel);
			var model = viewResultBase.Model as ClinicMasterViewModel;
			Assert.That(model.Clinics.Count, Is.EqualTo(0));

			cRepository.Verify(r => r.GetAll(), Times.Once());
		}

		[Test]
		public void Doctors_DetailGrid() {
			//Arrange
			var cRepository = new Mock<IClinicRepository>();
			var controller = new AdministrationController(cRepository.Object);
			var clinicId = 111;
			var doctor1 = new User {
				Id = 10,
				FirstName = "First10",
				LastName = "Last10",
				Role = "Doctor",
				Patients = new List<Patient> {new Patient()},
				Login = "login10",
				Photo = new byte[]{1,2,3}
			};
			var doctor2 = new User {
				Id = 20,
				FirstName = "First20",
				LastName = "Last20",
				Role = "Supervisor",
				Patients = new List<Patient> {new Patient(), new Patient()},
				Login = "login20",
				Photo = new byte[]{3,4,5}
			};

			var clinic = new Clinic {Id = clinicId, Caption = "Clinic111", Doctors = new List<User> {doctor1, doctor2}};

			cRepository.Setup(r => r.GetByKey(clinicId)).Returns(clinic);

			//Act
			var result = controller.ClinicDoctors(clinicId);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_ClinicDoctorsGrid"));
			Assert.That(viewResultBase.Model is ClinicDetailsViewModel);
			var model = viewResultBase.Model as ClinicDetailsViewModel;
			Assert.That(model.Doctors, Is.Not.Null);
			Assert.That(model.Doctors.Count, Is.EqualTo(2));

			Assert.That(model.ClinicId, Is.EqualTo(111));

			Assert.That(model.Doctors[0].ClinicId, Is.EqualTo(111));
			Assert.That(model.Doctors[0].DoctorId, Is.EqualTo(10));
			Assert.That(model.Doctors[0].FirstName, Is.EqualTo("First10"));
			Assert.That(model.Doctors[0].LastName, Is.EqualTo("Last10"));
			Assert.That(model.Doctors[0].Role, Is.EqualTo("Doctor"));
			Assert.That(model.Doctors[0].PatientsCount, Is.EqualTo(1));
			Assert.That(model.Doctors[0].Login, Is.EqualTo("login10"));
			Assert.That(model.Doctors[0].Photo, Is.EqualTo(new byte[] { 1, 2, 3 }));

			Assert.That(model.Doctors[1].ClinicId, Is.EqualTo(111));
			Assert.That(model.Doctors[1].DoctorId, Is.EqualTo(20));
			Assert.That(model.Doctors[1].FirstName, Is.EqualTo("First20"));
			Assert.That(model.Doctors[1].LastName, Is.EqualTo("Last20"));
			Assert.That(model.Doctors[1].Role, Is.EqualTo("Supervisor"));
			Assert.That(model.Doctors[1].PatientsCount, Is.EqualTo(2));
			Assert.That(model.Doctors[1].Login, Is.EqualTo("login20"));
			Assert.That(model.Doctors[1].Photo, Is.EqualTo(new byte[] { 3, 4, 5 }));

			cRepository.Verify(r => r.GetByKey(clinicId), Times.Once());
		}

		[Test]
		public void Doctors_DetailGrid_Empty() {
			//Arrange
			var cRepository = new Mock<IClinicRepository>();
			var controller = new AdministrationController(cRepository.Object);
			var clinicId = 222;
			var clinic = new Clinic {Id = clinicId, Caption = "Clinic111", Doctors = new List<User>()};

			cRepository.Setup(r => r.GetByKey(clinicId)).Returns(clinic);

			//Act
			var result = controller.ClinicDoctors(clinicId);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_ClinicDoctorsGrid"));
			Assert.That(viewResultBase.Model is ClinicDetailsViewModel);
			var model = viewResultBase.Model as ClinicDetailsViewModel;

			Assert.That(model.Doctors.Count, Is.EqualTo(0));
			Assert.That(model.ClinicId, Is.EqualTo(clinicId));

			cRepository.Verify(r => r.GetByKey(clinicId), Times.Once());
		}

		[Test]
		public void Doctors_DetailGrid_NonExistingClinic() {
			//Arrange
			var cRepository = new Mock<IClinicRepository>();
			var controller = new AdministrationController(cRepository.Object);
			var clinicId = 333;

			cRepository.Setup(r => r.GetByKey(clinicId)).Returns((Clinic) null);

			//Act
			var result = controller.ClinicDoctors(clinicId);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_ErrorInfo"));
			Assert.That(viewResultBase.Model is ErrorViewModel);
			var model = viewResultBase.Model as ErrorViewModel;
			Assert.That(model.Caption, Is.EqualTo("Clinic is not found"));

			cRepository.Verify(r => r.GetByKey(clinicId), Times.Once());
		}

		[Test]
		public void Doctors_DetailGrid_IncorrectClinicId() {
			//Arrange
			var cRepository = new Mock<IClinicRepository>();
			var controller = new AdministrationController(cRepository.Object);

			cRepository.Setup(r => r.GetByKey(It.IsAny<int>())).Returns(new Clinic());

			//Act
			var result = controller.ClinicDoctors(null);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_ErrorInfo"));
			Assert.That(viewResultBase.Model is ErrorViewModel);
			var model = viewResultBase.Model as ErrorViewModel;
			Assert.That(model.Caption, Is.EqualTo("Clinic is not found"));

			cRepository.Verify(r => r.GetByKey(It.IsAny<int>()), Times.Never());
		}
	}
}
