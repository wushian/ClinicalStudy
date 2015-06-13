using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using ClinicalStudy.Repositories.MemoryRepositories.FormData;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories.FormData {
	[TestFixture]
	public class VitalsFormDataRepositoryTest {
		[Test]
		public void MappingTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			var entity = new VitalsFormData() {
				Id = 1,
				ActualTime = new Question {Id = 2, Value = "2"},
				HeartRate = new Question {Id = 3, Value = "3"},
				Height = new Question {Id = 4, Value = "4"},
				Weight = new Question {Id = 5, Value = "5"},
				Temperature = new Question {Id = 6, Value = "6"},
				BloodPressureDiastolic = new Question {Id = 7, Value = "7"},
				BloodPressureSystolic = new Question {Id = 8, Value = "8"}
			};
			var updatedEntity = new VitalsFormData() {
				Id = 1,
				ActualTime = new Question {Id = 3, Value = "3"},
				HeartRate = new Question {Id = 4, Value = "4"},
				Height = new Question {Id = 5, Value = "5"},
				Weight = new Question {Id = 6, Value = "6"},
				Temperature = new Question {Id = 7, Value = "7"},
				BloodPressureDiastolic = new Question {Id = 8, Value = "8"},
				BloodPressureSystolic = new Question {Id = 9, Value = "9"}
			};
			var repository = new VitalsFormDataRepository(dataStorage.Object);

			dataStorage.Setup(ds => ds.GetData<VitalsFormData>()).Returns(new List<VitalsFormData> {entity});
			//Act
			repository.Edit(updatedEntity);
			//Assert
			//Values are equal
			Assert.That(entity.ActualTime.Value, Is.EqualTo(updatedEntity.ActualTime.Value));
			Assert.That(entity.HeartRate.Value, Is.EqualTo(updatedEntity.HeartRate.Value));
			Assert.That(entity.Height.Value, Is.EqualTo(updatedEntity.Height.Value));
			Assert.That(entity.Weight.Value, Is.EqualTo(updatedEntity.Weight.Value));
			Assert.That(entity.Temperature.Value, Is.EqualTo(updatedEntity.Temperature.Value));
			Assert.That(entity.BloodPressureDiastolic.Value, Is.EqualTo(updatedEntity.BloodPressureDiastolic.Value));
			Assert.That(entity.BloodPressureSystolic.Value, Is.EqualTo(updatedEntity.BloodPressureSystolic.Value));

			//Objects are not equal
			Assert.That(entity.ActualTime, Is.Not.EqualTo(updatedEntity.ActualTime));
			Assert.That(entity.HeartRate, Is.Not.EqualTo(updatedEntity.HeartRate));
			Assert.That(entity.Height, Is.Not.EqualTo(updatedEntity.Height));
			Assert.That(entity.Weight, Is.Not.EqualTo(updatedEntity.Weight));
			Assert.That(entity.Temperature, Is.Not.EqualTo(updatedEntity.Temperature));
			Assert.That(entity.BloodPressureDiastolic, Is.Not.EqualTo(updatedEntity.BloodPressureDiastolic));
			Assert.That(entity.BloodPressureSystolic, Is.Not.EqualTo(updatedEntity.BloodPressureSystolic));
		}
	}
}
