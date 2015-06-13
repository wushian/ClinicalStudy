using System;
using System.Collections;
using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.DTOs;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.Repositories.MemoryRepositories;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories {
	/// <summary>
	/// Here we apply another method to structure your unit-tests
	/// We create a single class for a tested class, and a nested class inside that class - for every tested method
	/// </summary>
	[TestFixture]
	public class VisitRepositoryTest {
		[Test]
		public void MappingTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			var entity = new Visit {
				Id = 1,
				Caption = "old",
				OrderNo = 1,
				ExpectedVisitDate = DateTime.Today.AddDays(-1),
				VisitDate = DateTime.Today.AddDays(-2),
				VisitTime = DateTime.Now,
				VisitType = VisitType.AdverseEventVisit,
				Patient = new Patient()
			};
			var updatedEntity = new Visit
			{
				Id = 1,
				Caption = "new",
				OrderNo = 2,
				ExpectedVisitDate = DateTime.Today.AddDays(-2),
				VisitDate = DateTime.Today.AddDays(-3),
				VisitTime = DateTime.Now,
				VisitType = VisitType.Day1,
				Patient = new Patient()
			};
			var repository = new VisitRepository(dataStorage.Object);

			dataStorage.Setup(ds => ds.GetData<Visit>()).Returns(new List<Visit> { entity });
			//Act
			repository.Edit(updatedEntity);
			//Assert
			//Values are equal
			Assert.That(entity.Caption, Is.EqualTo(updatedEntity.Caption));
			Assert.That(entity.ExpectedVisitDate, Is.EqualTo(updatedEntity.ExpectedVisitDate));
			Assert.That(entity.OrderNo, Is.EqualTo(updatedEntity.OrderNo));
			Assert.That(entity.VisitDate, Is.EqualTo(updatedEntity.VisitDate));
			Assert.That(entity.VisitTime, Is.EqualTo(updatedEntity.VisitTime));
			Assert.That(entity.VisitType, Is.EqualTo(updatedEntity.VisitType));
			Assert.That(entity.Patient, Is.EqualTo(updatedEntity.Patient));
		}

		[TestFixture]
		public class PatientVisits {
			[Test]
			public void GetPatientVisit() {
				//Arrange
				string visitName = "TestVisit";
				Patient patient = new Patient() {Id = 5, PatientNumber = 88};
				Visit visit = new Visit() {Patient = patient, Caption = visitName};

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Visit>()).Returns(new List<Visit> {visit});
				VisitRepository repository = new VisitRepository(dataStorage.Object);

				//Act
				Visit result = repository.GetVisitByPatientNumberAndVisitName(patient.PatientNumber, visitName);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result, Is.EqualTo(visit));
			}

			[Test]
			public void SinglePatientVisit() {
				//Arrange
				Patient patient = new Patient() {Id = 5};
				Visit visit1 = new Visit() {Patient = patient};

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Visit>()).Returns(new List<Visit> {visit1});
				VisitRepository repository = new VisitRepository(dataStorage.Object);
				//Act
				IList<Visit> result = repository.GetVisitsForPatient(patient.Id);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(1));
				Assert.That(result[0], Is.EqualTo(visit1));
			}

			[Test]
			public void MultiplePatientsVisits() {
				//Arrange
				Patient patient = new Patient() {Id = 5};
				Visit visit1 = new Visit() {Patient = patient};
				Visit visit2 = new Visit() {Patient = patient};

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Visit>()).Returns(new List<Visit> {visit1, visit2});
				VisitRepository repository = new VisitRepository(dataStorage.Object);
				//Act
				IList<Visit> result = repository.GetVisitsForPatient(patient.Id);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(2));
				Assert.That(result.Contains(visit1));
				Assert.That(result.Contains(visit2));
			}

			[Test]
			public void PatientVisitsForManyPatients() {
				//Arrange
				Patient patient = new Patient() {Id = 5};
				Visit visit1 = new Visit() {Patient = patient};
				Patient anotherPatient = new Patient() {Id = 11};
				Visit visit2 = new Visit() {Patient = anotherPatient};

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Visit>()).Returns(new List<Visit> {visit1, visit2});
				VisitRepository repository = new VisitRepository(dataStorage.Object);
				//Act
				IList<Visit> result = repository.GetVisitsForPatient(patient.Id);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(1));
				Assert.That(result[0], Is.EqualTo(visit1));
			}
		}

		[TestFixture]
		public class TodaysVisits {
			[Test]
			public void SingleExpectedToday() {
				//Arrange
				string login = "user";
				Visit visit1 = new Visit() { ExpectedVisitDate = DateTime.Now.Date, Patient = new Patient() { Doctor = new User() { Login = login }, IsActive = true } };

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Visit>()).Returns(new List<Visit> {visit1});
				VisitRepository repository = new VisitRepository(dataStorage.Object);
				//Act
				IList<Visit> result = repository.GetDailyVisits(login, DateTime.Now.Date);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(1));
				Assert.That(result[0], Is.EqualTo(visit1));
			}

			[Test]
			public void SingleExpectedYesterday() {
				//Arrange
				string login = "user";
				Visit visit1 = new Visit() { ExpectedVisitDate = DateTime.Now.Date.AddDays(-1), Patient = new Patient() { Doctor = new User() { Login = login }, IsActive = true } };

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Visit>()).Returns(new List<Visit> {visit1});
				VisitRepository repository = new VisitRepository(dataStorage.Object);
				//Act
				IList<Visit> result = repository.GetDailyVisits(login, DateTime.Now.Date.AddDays(-1));

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(1));
				Assert.That(result[0], Is.EqualTo(visit1));
			}
			[Test]
			public void SingleExpectedTodayForAnotherDoctor() {
				//Arrange
				string login = "user";
				string anotherLogin = "anotherUser";
				Visit visit1 = new Visit() { ExpectedVisitDate = DateTime.Now.Date, Patient = new Patient() { Doctor = new User() { Login = anotherLogin } } };

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Visit>()).Returns(new List<Visit> { visit1 });
				VisitRepository repository = new VisitRepository(dataStorage.Object);
				//Act
				IList<Visit> result = repository.GetDailyVisits(login, DateTime.Now.Date);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(0));
			}

			[Test]
			public void YesterdaysVisitNotListed() {
				//Arrange
				string login = "user";
				Visit visit1 = new Visit() { ExpectedVisitDate = DateTime.Now.Date.AddDays(-1), Patient = new Patient() { Doctor = new User() { Login = login } } };

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Visit>()).Returns(new List<Visit> {visit1});
				VisitRepository repository = new VisitRepository(dataStorage.Object);
				//Act
				IList<Visit> result = repository.GetDailyVisits(login, DateTime.Now.Date);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(0));
			}


			[Test]
			public void OneExpectedAndOneRealVisit() {
				//Arrange
				string login = "user";
				Visit visit1 = new Visit() { ExpectedVisitDate = DateTime.Now.Date, Patient = new Patient() { Doctor = new User() { Login = login }, IsActive = true} };
				Visit visit2 = new Visit() {
					ExpectedVisitDate = DateTime.Now.Date.AddDays(-1),
					VisitDate = DateTime.Now.Date,
					Patient = new Patient() { Doctor = new User() { Login = login }, IsActive = true }
				};

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Visit>()).Returns(new List<Visit> {visit1, visit2});
				VisitRepository repository = new VisitRepository(dataStorage.Object);
				//Act
				IList<Visit> result = repository.GetDailyVisits(login, DateTime.Now.Date);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(2));
				Assert.That(result.Contains(visit1));
				Assert.That(result.Contains(visit2));
			}
		}

		[TestFixture]
		public class GetAeAnalyticsData
		{

			[Test]
			public void CorrectModel()
			{
				//Arrange
				var dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Visit>()).Returns(new List<Visit>());
				var repository = new VisitRepository(dataStorage.Object);

				//Act
				var result = repository.GetAeAnalyticsData();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is List<AeAnalyticsDto>);
				Assert.That(result.Count, Is.EqualTo(0));
			}

			[Test]
			public void CorrectModelMapping()
			{
				//Arrange
				var dataStorage = new Mock<IDataStorage>();
				var repository = new VisitRepository(dataStorage.Object);

				var clinic = new Clinic { Caption = "Clinic" };
				var doctor1 = new User { FirstName = "DoctorFirst1", LastName = "DoctorLast1", Clinic = clinic };
				var patient1 = new Patient { Doctor = doctor1 };
				var visit1B = new Visit { Caption = "Visit1B", Patient = patient1, VisitType = VisitType.Baseline, Forms = new List<Form>{new Form { Id = 1, FormType = FormType.Demographics}}};
				var visit1Ae1 = new Visit { Caption = "Visit1Ae1", Patient = patient1, VisitType = VisitType.AdverseEventVisit, Forms = new List<Form> { new Form { Id = 2, FormType = FormType.AdverseEvent } } };
				var visit1Ae2 = new Visit { Caption = "Visit1Ae2", Patient = patient1, VisitType = VisitType.AdverseEventVisit, Forms = new List<Form> { new Form { Id = 3, FormType = FormType.AdverseEvent } } };
				patient1.Visits = new List<Visit> {visit1B, visit1Ae1, visit1Ae2};

				var doctor2 = new User { FirstName = "DoctorFirst2", LastName = "DoctorLast2", Clinic = clinic };
				var patient2 = new Patient { Doctor = doctor2 };
				var visit2B = new Visit { Caption = "Visit2B", Patient = patient2, VisitType = VisitType.Baseline, Forms = new List<Form> { new Form { Id = 13, FormType = FormType.Demographics } } };
				var visit2Ae1 = new Visit { Caption = "Visit2Ae1", Patient = patient2, VisitType = VisitType.AdverseEventVisit, Forms = new List<Form> { new Form { Id = 23, FormType = FormType.AdverseEvent } } };
				patient2.Visits = new List<Visit> { visit2B, visit2Ae1 };

				dataStorage.Setup(ds => ds.GetData<Visit>()).Returns(new List<Visit> { visit1Ae1, visit1Ae2, visit1B, visit2B, visit2Ae1 });

				//Act
				var result = repository.GetAeAnalyticsData();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(3));
				var dto = result[0];
				Assert.That(dto.ClinicName, Is.EqualTo("Clinic"));
				Assert.That(dto.DoctorName, Is.EqualTo("DoctorFirst1 DoctorLast1"));
				Assert.That(dto.DemographicFormId, Is.EqualTo(1));
				Assert.That(dto.AeFormId, Is.EqualTo(2));

				dto = result[1];
				Assert.That(dto.ClinicName, Is.EqualTo("Clinic"));
				Assert.That(dto.DoctorName, Is.EqualTo("DoctorFirst1 DoctorLast1"));
				Assert.That(dto.DemographicFormId, Is.EqualTo(1));
				Assert.That(dto.AeFormId, Is.EqualTo(3));

				dto = result[2];
				Assert.That(dto.ClinicName, Is.EqualTo("Clinic"));
				Assert.That(dto.DoctorName, Is.EqualTo("DoctorFirst2 DoctorLast2"));
				Assert.That(dto.DemographicFormId, Is.EqualTo(13));
				Assert.That(dto.AeFormId, Is.EqualTo(23));
			}
		}
	}
}
