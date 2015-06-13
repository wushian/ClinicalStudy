using System;
using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.Repositories.MemoryRepositories;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories {
	[TestFixture]
	public class QuestionRepositoryTest {
		[Test]
		public void MappingTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			var entity = new Question {
				Id = 1,
				DataType = QuestionDataType.Integer,
				Value = "2",
				File = null
			};
			var updatedEntity = new Question {
				Id = 1,
				DataType = QuestionDataType.Integer,
				Value = "3",
				File = new Attachment()
			};
			var repository = new QuestionRepository(dataStorage.Object);

			dataStorage.Setup(ds => ds.GetData<Question>()).Returns(new List<Question> {entity});
			//Act
			repository.Edit(updatedEntity);
			//Assert
			//Values are equal
			Assert.That(entity.DataType, Is.EqualTo(updatedEntity.DataType));
			Assert.That(entity.Value, Is.EqualTo(updatedEntity.Value));
			Assert.That(entity.File, Is.EqualTo(updatedEntity.File));
		}
	}
}
