using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using ClinicalStudy.Repositories.MemoryRepositories.FormData;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories.FormData {
	[TestFixture]
	public class HappinessFormDataRepositoryTest {
		[Test]
		public void MappingTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			var entity = new HappinessFormData() {
				Id = 1,
				HappinessLevel = new Question {Id = 2, Value = "2"}
			};
			var updatedEntity = new HappinessFormData() {
				Id = 1,
				HappinessLevel = new Question {Id = 3, Value = "3"}
			};
			var repository = new HappinessFormDataRepository(dataStorage.Object);

			dataStorage.Setup(ds => ds.GetData<HappinessFormData>()).Returns(new List<HappinessFormData> {entity});
			//Act
			repository.Edit(updatedEntity);
			//Assert
			//Values are equal
			Assert.That(entity.HappinessLevel.Value, Is.EqualTo(updatedEntity.HappinessLevel.Value));

			//Objects are not equal
			Assert.That(entity.HappinessLevel, Is.Not.EqualTo(updatedEntity.HappinessLevel));
		}
	}
}
