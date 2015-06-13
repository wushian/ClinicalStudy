using System;
using System.Collections.Generic;
using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.DTOs;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.Enums.Answers;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.MemoryRepositories;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories {
	[TestFixture]
	public class PatientRepositoryTest {
		[TestFixture]
		public class GetByUniqueNumber {
			[Test]
			public void CorrectSinglePatient() {
				//Arrange
				Patient patient = new Patient() {Id = 5, PatientNumber = 25};

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Patient>()).Returns(new List<Patient> {patient});
				PatientRepository repository = new PatientRepository(dataStorage.Object);
				//Act
				Patient result = repository.GetPatientByUniqueNumber(25);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Id, Is.EqualTo(patient.Id));
			}

			[Test]
			public void CorrectMultiplePatients() {
				//Arrange
				Patient patient = new Patient() {Id = 5, PatientNumber = 25};
				Patient patient2 = new Patient() {Id = 6, PatientNumber = 27};

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Patient>()).Returns(new List<Patient> {patient, patient2});
				PatientRepository repository = new PatientRepository(dataStorage.Object);
				//Act
				Patient result = repository.GetPatientByUniqueNumber(25);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Id, Is.EqualTo(patient.Id));
			}

			[Test]
			public void NoResultOnNonExistingPatient() {
				//Arrange
				Patient patient = new Patient() {Id = 5, PatientNumber = 25};

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Patient>()).Returns(new List<Patient> {patient});
				PatientRepository repository = new PatientRepository(dataStorage.Object);
				//Act
				Patient result = repository.GetPatientByUniqueNumber(27);

				//Assert
				Assert.That(result, Is.Null);
			}
		}

		[TestFixture]
		public class GetPatientsStateData {
			private int doctorId = 53;
			private string doctorName = "Smith";
			private int clinicId = 88;
			private string clinicName = "St. Mary Hospital";

			[Test]
			public void SinglePatientWithSinglePastVisit() {
				//Arrange
				var visits = new List<Visit> {
					new Visit() {VisitType = VisitType.Day1, VisitDate = DateTime.Today.AddDays(-1)}
				};
				var patient = CreateTestPatient(visits);

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Patient>()).Returns(new List<Patient> {patient});
				PatientRepository repository = new PatientRepository(dataStorage.Object);
				//Act
				var result = repository.GetPatientsStateData();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(1));
				var data = result[0];
				Assert.That(data.ClinicId, Is.EqualTo(clinicId));
				Assert.That(data.DoctorId, Is.EqualTo(doctorId));
				Assert.That(data.ClinicName, Is.EqualTo(clinicName));
				Assert.That(data.DoctorName, Is.EqualTo(doctorName));
				Assert.That(data.VisitType, Is.EqualTo(VisitType.Day1));
			}

			[Test]
			public void SinglePatientWithSingleFutureVisit() {
				//Arrange
				var visits = new List<Visit> {
					new Visit() {VisitType = VisitType.Baseline, VisitDate = DateTime.Today.AddDays(+1)}
				};
				var patient = CreateTestPatient(visits);

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Patient>()).Returns(new List<Patient> { patient });
				PatientRepository repository = new PatientRepository(dataStorage.Object);
				//Act
				var result = repository.GetPatientsStateData();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(1));
				var data = result[0];
				Assert.That(data.ClinicId, Is.EqualTo(clinicId));
				Assert.That(data.DoctorId, Is.EqualTo(doctorId));
				Assert.That(data.ClinicName, Is.EqualTo(clinicName));
				Assert.That(data.DoctorName, Is.EqualTo(doctorName));
				Assert.That(data.VisitType, Is.EqualTo(VisitType.None));
			}
			[Test]
			public void SinglePatientWithTwoPastVisit() {
				//Arrange
				var visits = new List<Visit> {
					new Visit() {VisitType = VisitType.Baseline, VisitDate = DateTime.Today.AddDays(-2)},
					new Visit() {VisitType = VisitType.Day1, VisitDate = DateTime.Today.AddDays(-1)}
				};
				var patient = CreateTestPatient(visits);

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Patient>()).Returns(new List<Patient> { patient });
				PatientRepository repository = new PatientRepository(dataStorage.Object);
				//Act
				var result = repository.GetPatientsStateData();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(1));
				var data = result[0];
				Assert.That(data.ClinicId, Is.EqualTo(clinicId));
				Assert.That(data.DoctorId, Is.EqualTo(doctorId));
				Assert.That(data.ClinicName, Is.EqualTo(clinicName));
				Assert.That(data.DoctorName, Is.EqualTo(doctorName));
				Assert.That(data.VisitType, Is.EqualTo(VisitType.Day1));
			}
			[Test]
			public void SinglePatientWithOnePastAndOneFutureVisit() {
				//Arrange
				var visits = new List<Visit> {
					new Visit() {VisitType = VisitType.Baseline, VisitDate = DateTime.Today.AddDays(-2)},
					new Visit() {VisitType = VisitType.Day1, VisitDate = DateTime.Today.AddDays(+1)}
				};
				var patient = CreateTestPatient(visits);

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Patient>()).Returns(new List<Patient> { patient });
				PatientRepository repository = new PatientRepository(dataStorage.Object);
				//Act
				var result = repository.GetPatientsStateData();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(1));
				var data = result[0];
				Assert.That(data.ClinicId, Is.EqualTo(clinicId));
				Assert.That(data.DoctorId, Is.EqualTo(doctorId));
				Assert.That(data.ClinicName, Is.EqualTo(clinicName));
				Assert.That(data.DoctorName, Is.EqualTo(doctorName));
				Assert.That(data.VisitType, Is.EqualTo(VisitType.Baseline));
			}

			[Test]
			public void TwoPatientsWithTwoPastVisit() {
				//Arrange
				var visits1 = new List<Visit> {
					new Visit() {VisitType = VisitType.Baseline, VisitDate = DateTime.Today.AddDays(-2)},
					new Visit() {VisitType = VisitType.Day1, VisitDate = DateTime.Today.AddDays(-1)}
				};
				var visits2 = new List<Visit> {
					new Visit() {VisitType = VisitType.Baseline, VisitDate = DateTime.Today.AddDays(-2)}
				};
				var patient1 = CreateTestPatient(visits1);
				var patient2 = CreateTestPatient(visits2);

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Patient>()).Returns(new List<Patient> { patient1, patient2 });
				PatientRepository repository = new PatientRepository(dataStorage.Object);
				//Act
				var result = repository.GetPatientsStateData();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(2));
				Action<PatientStateDto> check = delegate(PatientStateDto data) {
					Assert.That(data.ClinicId, Is.EqualTo(clinicId));
					Assert.That(data.DoctorId, Is.EqualTo(doctorId));
					Assert.That(data.ClinicName, Is.EqualTo(clinicName));
					Assert.That(data.DoctorName, Is.EqualTo(doctorName));
				};

				check(result[0]);
				check(result[1]);

				Assert.That(result.SingleOrDefault(d => d.VisitType == VisitType.Day1), Is.Not.Null);
				Assert.That(result.SingleOrDefault(d => d.VisitType == VisitType.Baseline), Is.Not.Null);
			}

			private Patient CreateTestPatient(List<Visit> visits) {
				Patient patient = new Patient() {
					Id = 5,
					PatientNumber = 25,
					Doctor = new User() {
						Id = doctorId,
						FirstName = "John",
						LastName = doctorName,
						Role = ClinicalStudyRoles.Doctor,
						Clinic = new Clinic { Id = clinicId, Caption = clinicName }
					},
					Visits = visits
				};
				return patient;
			}
		}

		[TestFixture]
		public class GetHappinessChangeData
		{
			[Test]
			public void OnePatient() {
				//Arrange
				int clinicId = 10, doctorId = 20, patId = 30;
				var patient = CreatePatient(clinicId, doctorId, patId);

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Patient>()).Returns(new List<Patient> { patient });
				PatientRepository repository = new PatientRepository(dataStorage.Object);
				//Act
				var result = repository.GetHappinessChangeData();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(1));
				AssertHappinessChangeData(result[0], clinicId, doctorId, patId);
			}

			[Test]
			public void TwoPatients_SameDoctor()
			{
				//Arrange
				int clinicId = 11, doctorId = 21, pat1Id = 31, pat2Id = 41;
				var patient1 = CreatePatient(clinicId, doctorId, pat1Id);
				var patient2 = CreatePatient(clinicId, doctorId, pat2Id);

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Patient>()).Returns(new List<Patient> { patient1, patient2 });
				PatientRepository repository = new PatientRepository(dataStorage.Object);
				//Act
				var result = repository.GetHappinessChangeData();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(2));
				AssertHappinessChangeData(result[0], clinicId, doctorId, pat1Id);
				AssertHappinessChangeData(result[1], clinicId, doctorId, pat2Id);
			}

			[Test]
			public void TwoPatients_TwoDoctors_SameClinic()
			{
				//Arrange
				int clinicId = 12, doctor1Id = 22, doctor2Id = 32, pat1Id = 42, pat2Id = 52;
				var patient1 = CreatePatient(clinicId, doctor1Id, pat1Id);
				var patient2 = CreatePatient(clinicId, doctor2Id, pat2Id);

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Patient>()).Returns(new List<Patient> { patient1, patient2 });
				PatientRepository repository = new PatientRepository(dataStorage.Object);
				//Act
				var result = repository.GetHappinessChangeData();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(2));
				AssertHappinessChangeData(result[0], clinicId, doctor1Id, pat1Id);
				AssertHappinessChangeData(result[1], clinicId, doctor2Id, pat2Id);
			}

			[Test]
			public void TwoPatients_TwoDoctors_TwoClinics()
			{
				//Arrange
				int clinic1Id = 13, clinic2Id = 23, doctor1Id = 33, doctor2Id = 43, pat1Id = 53, pat2Id = 63;
				var patient1 = CreatePatient(clinic1Id, doctor1Id, pat1Id);
				var patient2 = CreatePatient(clinic2Id, doctor2Id, pat2Id);

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Patient>()).Returns(new List<Patient> { patient1, patient2 });
				PatientRepository repository = new PatientRepository(dataStorage.Object);
				//Act
				var result = repository.GetHappinessChangeData();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(2));
				AssertHappinessChangeData(result[0], clinic1Id, doctor1Id, pat1Id);
				AssertHappinessChangeData(result[1], clinic2Id, doctor2Id, pat2Id);
			}

			[Test]
			public void GetHappinessChangeData_Complex()
			{
				//Arrange
				int clinic1Id = 14, clinic2Id = 24, doctor1Id = 34, doctor2Id = 44, doctor3Id = 54, pat1Id = 64, pat2Id = 74, pat3Id = 84, pat4Id = 94;
				var patient1 = CreatePatient(clinic1Id, doctor1Id, pat1Id);
				var patient2 = CreatePatient(clinic1Id, doctor1Id, pat2Id);
				var patient3 = CreatePatient(clinic2Id, doctor2Id, pat3Id);
				var patient4 = CreatePatient(clinic2Id, doctor3Id, pat4Id);

				Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Patient>()).Returns(new List<Patient> { patient1, patient2, patient3, patient4 });
				PatientRepository repository = new PatientRepository(dataStorage.Object);
				//Act
				var result = repository.GetHappinessChangeData();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(4));
				AssertHappinessChangeData(result[0], clinic1Id, doctor1Id, pat1Id);
				AssertHappinessChangeData(result[1], clinic1Id, doctor1Id, pat2Id);
				AssertHappinessChangeData(result[2], clinic2Id, doctor2Id, pat3Id);
				AssertHappinessChangeData(result[3], clinic2Id, doctor3Id, pat4Id);
			}

			private void AssertHappinessChangeData(HappinessChangeDto data, int clinicId, int doctorId, int patId) {
				Assert.That(data.ClinicId, Is.EqualTo(clinicId));
				Assert.That(data.DoctorId, Is.EqualTo(doctorId));
				Assert.That(data.ClinicName, Is.EqualTo(String.Format("Clinic{0}", clinicId)));
				Assert.That(data.DoctorName, Is.EqualTo(String.Format("First{0} Last{0}", doctorId)));
				Assert.That(data.PatientId, Is.EqualTo(patId));
				Assert.That(data.PatientNumber, Is.EqualTo(10 + patId));
				Assert.That(data.DemographicFormId, Is.EqualTo(201 + patId));
				Assert.That(data.HappinessDay1FormId, Is.EqualTo(202 + patId));
				Assert.That(data.HappinessDay10FormId, Is.EqualTo(203 + patId));
			}

			private Patient CreatePatient(int clinicId, int doctorId, int patId) {
				var clinic = new Clinic {Id = clinicId, Caption = String.Format("Clinic{0}", clinicId)};
				var doctor = new User {
					Id = doctorId,
					Clinic = clinic,
					Role = ClinicalStudyRoles.Doctor,
					FirstName = String.Format("First{0}", doctorId),
					LastName = String.Format("Last{0}", doctorId)
				};
				var pat = new Patient {Id = patId, PatientNumber = 10 + patId, Doctor = doctor};

				var demog = new Form { FormType = FormType.Demographics, Id = 201 + patId};

				var hap1 = new Form {FormType = FormType.Happiness, Id = 202 + patId};
				var hap2 = new Form { FormType = FormType.Happiness, Id = 203 + patId };

				var visitB = new Visit
				             {Id = 101 + patId, Caption = String.Format("Visit{0}", 101 + patId), VisitType = VisitType.Baseline, Forms = new List<Form>{demog}};
				var visit1 = new Visit { Id = 102 + patId, Caption = String.Format("Visit{0}", 102 + patId), VisitType = VisitType.Day1, Forms = new List<Form>{hap1}};
				var visit10 = new Visit { Id = 103 + patId, Caption = String.Format("Visit{0}", 103 + patId), VisitType = VisitType.Day10, Forms = new List<Form> { hap2 } };

				pat.Visits = new List<Visit>{visitB, visit1, visit10};

				return pat;
			}
		} 
	}
}
