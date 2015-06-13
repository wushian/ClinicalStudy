using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using ClinicalStudy.Repositories.MemoryRepositories.FormData;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories.FormData {
	[TestFixture]
	public class DemographicFormDataRepositoryTest {
		[Test]
		public void MappingTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			var entity = new DemographicFormData() {
				Id = 1,
				DateOfBirth = new Question {Id = 2, Value = "2"},
				Other = new Question {Id = 3, Value = "3"},
				Race = new Question {Id = 4, Value = "4"},
				Sex = new Question {Id = 5, Value = "5"}
			};
			var updatedEntity = new DemographicFormData() {
				Id = 1,
				DateOfBirth = new Question {Id = 2, Value = "2"},
				Other = new Question {Id = 3, Value = "3"},
				Race = new Question {Id = 4, Value = "4"},
				Sex = new Question {Id = 5, Value = "5"}
			};
			var repository = new DemographicFormDataRepository(dataStorage.Object);

			dataStorage.Setup(ds => ds.GetData<DemographicFormData>()).Returns(new List<DemographicFormData> {entity});
			//Act
			repository.Edit(updatedEntity);
			//Assert
			//Values are equal
			Assert.That(entity.DateOfBirth.Value, Is.EqualTo(updatedEntity.DateOfBirth.Value));
			Assert.That(entity.Other.Value, Is.EqualTo(updatedEntity.Other.Value));
			Assert.That(entity.Race.Value, Is.EqualTo(updatedEntity.Race.Value));
			Assert.That(entity.Sex.Value, Is.EqualTo(updatedEntity.Sex.Value));

			//Objects are not equal
			Assert.That(entity.DateOfBirth, Is.Not.EqualTo(updatedEntity.DateOfBirth));
			Assert.That(entity.Other, Is.Not.EqualTo(updatedEntity.Other));
			Assert.That(entity.Race, Is.Not.EqualTo(updatedEntity.Race));
			Assert.That(entity.Sex, Is.Not.EqualTo(updatedEntity.Sex));
		}
	}
}
