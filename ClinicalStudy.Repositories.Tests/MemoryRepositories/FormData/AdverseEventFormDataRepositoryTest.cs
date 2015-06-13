using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using ClinicalStudy.Repositories.MemoryRepositories.FormData;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories.FormData {
	[TestFixture]
	public class AdverseEventFormDataRepositoryTest {
		[Test]
		public void MappingTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			var entity = new AdverseEventFormData() {
				Id = 1,
				AdverseExperience = new Question {Id = 2, Value = "2"},
				EndDate = new Question {Id = 3, Value = "3"},
				EndTime = new Question {Id = 4, Value = "4"},
				OnsetDate = new Question {Id = 5, Value = "5"},
				OnsetTime = new Question {Id = 6, Value = "6"},
				Outcome = new Question {Id = 7, Value = "7"},
				Intensity = new Question {Id = 8, Value = "8"},
				RelationshipToInvestigationalDrug = new Question {Id = 9, Value = "9"}
			};
			var updatedEntity = new AdverseEventFormData() {
				Id = 1,
				AdverseExperience = new Question {Id = 3, Value = "3"},
				EndDate = new Question {Id = 4, Value = "4"},
				EndTime = new Question {Id = 5, Value = "5"},
				OnsetDate = new Question {Id = 6, Value = "6"},
				OnsetTime = new Question {Id = 7, Value = "7"},
				Outcome = new Question {Id = 8, Value = "8"},
				Intensity = new Question {Id = 9, Value = "9"},
				RelationshipToInvestigationalDrug = new Question {Id = 10, Value = "10"}
			};
			var repository = new AdverseEventFormDataRepository(dataStorage.Object);

			dataStorage.Setup(ds => ds.GetData<AdverseEventFormData>()).Returns(new List<AdverseEventFormData> {entity});
			//Act
			repository.Edit(updatedEntity);
			//Assert
			//Values are equal
			Assert.That(entity.AdverseExperience.Value, Is.EqualTo(updatedEntity.AdverseExperience.Value));
			Assert.That(entity.EndDate.Value, Is.EqualTo(updatedEntity.EndDate.Value));
			Assert.That(entity.EndTime.Value, Is.EqualTo(updatedEntity.EndTime.Value));
			Assert.That(entity.OnsetDate.Value, Is.EqualTo(updatedEntity.OnsetDate.Value));
			Assert.That(entity.OnsetTime.Value, Is.EqualTo(updatedEntity.OnsetTime.Value));
			Assert.That(entity.Outcome.Value, Is.EqualTo(updatedEntity.Outcome.Value));
			Assert.That(entity.Intensity.Value, Is.EqualTo(updatedEntity.Intensity.Value));
			Assert.That(entity.RelationshipToInvestigationalDrug.Value,
			            Is.EqualTo(updatedEntity.RelationshipToInvestigationalDrug.Value));

			//Objects are not equal
			Assert.That(entity.AdverseExperience, Is.Not.EqualTo(updatedEntity.AdverseExperience));
			Assert.That(entity.EndDate, Is.Not.EqualTo(updatedEntity.EndDate));
			Assert.That(entity.EndTime, Is.Not.EqualTo(updatedEntity.EndTime));
			Assert.That(entity.OnsetDate, Is.Not.EqualTo(updatedEntity.OnsetDate));
			Assert.That(entity.OnsetTime, Is.Not.EqualTo(updatedEntity.OnsetTime));
			Assert.That(entity.Outcome, Is.Not.EqualTo(updatedEntity.Outcome));
			Assert.That(entity.Intensity, Is.Not.EqualTo(updatedEntity.Intensity));
			Assert.That(entity.RelationshipToInvestigationalDrug, Is.Not.EqualTo(updatedEntity.RelationshipToInvestigationalDrug));
		}
	}
}
