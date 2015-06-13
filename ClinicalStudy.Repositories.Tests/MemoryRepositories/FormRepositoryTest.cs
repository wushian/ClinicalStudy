using System;
using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.DTOs;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.Repositories.MemoryRepositories;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories {
	[TestFixture]
	public class FormRepositoryTest {


		protected Mock<IDataStorage> DataStorage;
		protected FormRepository Repository;

		[Test]
		public void GetFormTest() {
			//Arrange
			const int patientNumber = 123;
			const string visitName = "TestVisit";
			const string formName = "TestForm";
			var form = new Form() {
				Visit = new Visit {Caption = visitName, Patient = new Patient {PatientNumber = patientNumber}},
				Caption = formName
			};

			DataStorage.Setup(ds => ds.GetData<Form>()).Returns(new List<Form> {form});
			//Act
			var result = Repository.GetForm(patientNumber, visitName, formName);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.EqualTo(form));
		}
		[SetUp]
		public void Setup() {
			DataStorage = new Mock<IDataStorage>();
			Repository = new FormRepository(DataStorage.Object);
		}

		[TestFixture]
		public class VisitForms : FormRepositoryTest {
			[Test]
			public void SingleFormForVisit() {
				//Arrange
				Visit visit = new Visit() {Id = 5};
				Form form1 = new Form() {Visit = visit};
				DataStorage.Setup(ds => ds.GetData<Form>()).Returns(new List<Form> {form1});
				//Act
				IList<Form> result = Repository.GetVisitForms(visit.Id);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(1));
				Assert.That(result[0], Is.EqualTo(form1));
			}

			[Test]
			public void MultipleFormsForVisit() {
				//Arrange
				Visit visit = new Visit() {Id = 5};
				Form form1 = new Form() {Visit = visit};
				Form form2 = new Form() {Visit = visit};

				DataStorage.Setup(ds => ds.GetData<Form>()).Returns(new List<Form> {form1, form2});
				//Act
				IList<Form> result = Repository.GetVisitForms(visit.Id);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(2));
				Assert.That(result[0], Is.EqualTo(form1));
				Assert.That(result[1], Is.EqualTo(form2));
			}

			[Test]
			public void VisitFormsForManyVisits() {
				//Arrange
				Visit visit = new Visit() {Id = 5};
				Form form1 = new Form() {Visit = visit};
				Visit anotherVisit = new Visit() {Id = 11};
				Form form2 = new Form() {Visit = anotherVisit};

				DataStorage.Setup(ds => ds.GetData<Form>()).Returns(new List<Form> {form1, form2});
				//Act
				IList<Form> result = Repository.GetVisitForms(visit.Id);

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(1));
				Assert.That(result[0], Is.EqualTo(form1));
			}
		}

		[TestFixture]
		public class GetUnfinishedForms : FormRepositoryTest {
			private static string TestHospitalCaption = "Test Hospital";
			private static string TestDoctorLastName = "Smith";

			[Test]
			public void NotFilledInRealVisit() {
				//Arrange
				Visit visit = new Visit() {
					Id = 5,
					VisitDate = DateTime.Now.AddDays(-1),
					Patient = GetTestPatient()
				};
				
				Form form1 = new Form() { Visit = visit, FormState = FormState.Incomplete, FormType = FormType.Demographics};

				DataStorage.Setup(ds => ds.GetData<Form>()).Returns(new List<Form> { form1});
				//Act
				IList<FormDto> result = Repository.GetUnfinishedCrfs();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(1));
				Assert.That(result[0].ClinicName, Is.EqualTo(TestHospitalCaption));
				Assert.That(result[0].DoctorName, Is.EqualTo(TestDoctorLastName));
				Assert.That(result[0].FormType, Is.EqualTo(FormType.Demographics));
			}

			[Test]
			public void NotFilledInScheduledVisit() {
				//Arrange
				Visit visit = new Visit() {
					Id = 5,
					VisitDate = null,
					ExpectedVisitDate = DateTime.Now.AddDays(-1),
					Patient = GetTestPatient()
				};

				Form form1 = new Form() { Visit = visit, FormState = FormState.Incomplete, FormType = FormType.Demographics };

				DataStorage.Setup(ds => ds.GetData<Form>()).Returns(new List<Form> { form1 });
				//Act
				IList<FormDto> result = Repository.GetUnfinishedCrfs();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(1));
				Assert.That(result[0].ClinicName, Is.EqualTo(TestHospitalCaption));
				Assert.That(result[0].DoctorName, Is.EqualTo(TestDoctorLastName));
				Assert.That(result[0].FormType, Is.EqualTo(FormType.Demographics));
			}

			[Test]
			public void NotFilledInFutureVisit() {
				//Arrange
				Visit visit = new Visit() {
					Id = 5,
					VisitDate = DateTime.Now.AddDays(+1),
					Patient = GetTestPatient()
				};

				Form form1 = new Form() { Visit = visit, FormState = FormState.Incomplete, FormType = FormType.Demographics };

				DataStorage.Setup(ds => ds.GetData<Form>()).Returns(new List<Form> { form1 });
				//Act
				IList<FormDto> result = Repository.GetUnfinishedCrfs();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count, Is.EqualTo(0));
			}

			private static Patient GetTestPatient() {
				return new Patient() {
					PatientNumber = 25,
					Doctor = new User() {
						Id = 143,
						LastName = TestDoctorLastName,
						Clinic = new Clinic() {
							Id = 3,
							Caption = TestHospitalCaption
						}
					}
				};
			}
		}
	}
}
