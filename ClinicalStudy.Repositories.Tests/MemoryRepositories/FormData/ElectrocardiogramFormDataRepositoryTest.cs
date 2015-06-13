using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using ClinicalStudy.Repositories.MemoryRepositories.FormData;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories.FormData {
	[TestFixture]
	public class ElectrocardiogramFormDataRepositoryTest {
		[Test]
		public void MappingTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			var entity = new ElectrocardiogramFormData() {
				Id = 1,
				ElectrocardiogramActualTime = new Question {Id = 2, Value = "2"},
				ElectrocardiogramAttachment = new Question {Id = 3, Value = "3"}
			};
			var updatedEntity = new ElectrocardiogramFormData() {
				Id = 1,
				ElectrocardiogramActualTime = new Question {Id = 3, Value = "3"},
				ElectrocardiogramAttachment = new Question {Id = 4, Value = "4"}
			};
			var repository = new ElectrocardiogramFormDataRepository(dataStorage.Object);

			dataStorage.Setup(ds => ds.GetData<ElectrocardiogramFormData>()).Returns(new List<ElectrocardiogramFormData> {entity});
			//Act
			repository.Edit(updatedEntity);
			//Assert
			//Values are equal
			Assert.That(entity.ElectrocardiogramActualTime.Value, Is.EqualTo(updatedEntity.ElectrocardiogramActualTime.Value));
			Assert.That(entity.ElectrocardiogramAttachment.Value, Is.EqualTo(updatedEntity.ElectrocardiogramAttachment.Value));

			//Objects are not equal
			Assert.That(entity.ElectrocardiogramActualTime, Is.Not.EqualTo(updatedEntity.ElectrocardiogramActualTime));
			Assert.That(entity.ElectrocardiogramAttachment, Is.Not.EqualTo(updatedEntity.ElectrocardiogramAttachment));
		}
	}
}
