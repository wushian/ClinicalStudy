using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.DTOs;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.Repositories.MemoryRepositories;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories {
	[TestFixture]
	public class QueryRepositoryTest {
		[Test]
		public void MappingTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			var entity = new Query {
				Id = 1,
				AnswerAuthor = new User {Id = 11},
				AnswerText = "oldAnswer",
				QueryAuthor = new User {Id = 12},
				QueryText = "oldQuestion"
			};
			var updatedEntity = new Query {
				Id = 1,
				AnswerAuthor = new User {Id = 21},
				AnswerText = "newAnswer",
				QueryAuthor = new User {Id = 22},
				QueryText = "newQuestion"
			};
			var repository = new QueryRepository(dataStorage.Object);

			dataStorage.Setup(ds => ds.GetData<Query>()).Returns(new List<Query> {entity});
			//Act
			repository.Edit(updatedEntity);
			//Assert
			//Values are equal
			Assert.That(entity.AnswerAuthor, Is.EqualTo(updatedEntity.AnswerAuthor));
			Assert.That(entity.AnswerText, Is.EqualTo(updatedEntity.AnswerText));

			Assert.That(entity.QueryAuthor, Is.EqualTo(updatedEntity.QueryAuthor));
			Assert.That(entity.QueryText, Is.EqualTo(updatedEntity.QueryText));
		}

		[Test]
		public void GetQueriesForQuestionsTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			var queries = new List<Query>(10);
			for (int i = 0; i < 10; i++) {
				queries.Add(new Query {Id = i, Question = new Question {Id = i + 20}});
			}
			var repository = new QueryRepository(dataStorage.Object);
			dataStorage.Setup(ds => ds.GetData<Query>()).Returns(queries);

			//Act
			var result = repository.GetQueriesForQuestions(new List<int> {2, 23, 29, 31, 24});

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count(), Is.EqualTo(3));
		}

		[Test]
		public void GetOpenQueries() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			var clinic = new Clinic {Caption = "Clinic1"};
			var doctor1 = new User {FirstName = "DoctorFirst1", LastName = "DoctorLast1", Clinic = clinic};
			var doctor2 = new User {FirstName = "DoctorFirst2", LastName = "DoctorLast2", Clinic = clinic};
			var patient1 = new Patient {PatientNumber = 11, Doctor = doctor1};
			var patient2 = new Patient {PatientNumber = 12, Doctor = doctor2};
			var visit1 = new Visit {Caption = "Visit1", Patient = patient1};
			var visit2 = new Visit {Caption = "Visit2", Patient = patient2};
			var form1 = new Form {FormType = FormType.Happiness, Visit = visit1};
			var form2 = new Form {FormType = FormType.Demographics, Visit = visit2};
			var question1 = new Question {Form = form1};
			var question2 = new Question {Form = form2};
			var query1 = new Query {Id = 1, QueryText = "Text1", Question = question1};
			var query2 = new Query {Id = 2, QueryText = "Text2", AnswerText = "Answer1", Question = question2};


			var repository = new QueryRepository(dataStorage.Object);
			dataStorage.Setup(ds => ds.GetData<Query>()).Returns(new List<Query> {query1, query2});

			//Act
			var result = repository.GetOpenQueries();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count(), Is.EqualTo(1));
			var query = result.ToList()[0];
			Assert.That(query.FormType, Is.EqualTo(FormType.Happiness));
			Assert.That(query.ClinicName, Is.EqualTo("Clinic1"));
			Assert.That(query.DoctorName, Is.EqualTo("DoctorLast1"));
			Assert.That(query.QuestionText, Is.EqualTo("Text1"));
			Assert.That(query.PatientNumber, Is.EqualTo(11));
			Assert.That(query.VisitName, Is.EqualTo("Visit1"));
		}

		[TestFixture]
		public class GetQueriesReportData {

			[Test]
			public void CorrectModel() {
				//Arrange
				var dataStorage = new Mock<IDataStorage>();
				dataStorage.Setup(ds => ds.GetData<Query>()).Returns(new List<Query>());
				var repository = new QueryRepository(dataStorage.Object);

				//Act
				var result = repository.GetQueriesReportData();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result is List<QueryReportDto>);
				Assert.That(result.Count(), Is.EqualTo(0));
			}

			[Test]
			public void CorrectModelMapping() {
				//Arrange
				var dataStorage = new Mock<IDataStorage>();
				var repository = new QueryRepository(dataStorage.Object);

				var clinic = new Clinic {Caption = "Clinic"};
				var doctor = new User {FirstName = "DoctorFirst", LastName = "DoctorLast", Clinic = clinic};
				var patient = new Patient {Doctor = doctor};
				var visit = new Visit {Caption = "Visit", Patient = patient};
				var form1 = new Form {FormType = FormType.Happiness, Visit = visit};
				var question1 = new Question {Caption = "QuestionCaption1", Form = form1};
				var query1 = new Query {Id = 1, Question = question1};

				var form2 = new Form {FormType = FormType.Inventory, Visit = visit};
				var question2 = new Question {Caption = "QuestionCaption2", Form = form2};
				var query2 = new Query {Id = 2, Question = question2, AnswerText = "Answer"};

				dataStorage.Setup(ds => ds.GetData<Query>()).Returns(new List<Query> {query1, query2});

				//Act
				var result = repository.GetQueriesReportData();

				//Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Count(), Is.EqualTo(2));
				var dto = result.ToList()[0];
				Assert.That(dto.FormType, Is.EqualTo(FormType.Happiness));
				Assert.That(dto.ClinicName, Is.EqualTo("Clinic"));
				Assert.That(dto.DoctorName, Is.EqualTo("DoctorFirst DoctorLast"));
				Assert.That(dto.QuestionName, Is.EqualTo("QuestionCaption1"));
				Assert.That(dto.IsOpen, Is.True);

				dto = result.ToList()[1];
				Assert.That(dto.FormType, Is.EqualTo(FormType.Inventory));
				Assert.That(dto.ClinicName, Is.EqualTo("Clinic"));
				Assert.That(dto.DoctorName, Is.EqualTo("DoctorFirst DoctorLast"));
				Assert.That(dto.QuestionName, Is.EqualTo("QuestionCaption2"));
				Assert.That(dto.IsOpen, Is.False);
			}
		}
	}
}
