using System;
using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.MemoryRepositories;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories {
	[TestFixture]
	public class ChangeNoteRepositoryTest {
		[Test]
		public void MappingTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			var entity = new ChangeNote {
				Id = 1,
				ChangeDate = DateTime.Today.AddDays(-10),
				ChangeReason = "Calculation error",
				NewValue = "2",
				OriginalValue = "3",
				Question = new Question {Id = 4, Value = "2"}
			};
			var updatedEntity = new ChangeNote() {
				Id = 1,
				ChangeDate = DateTime.Today.AddDays(1),
				ChangeReason = "Typo",
				NewValue = "7",
				OriginalValue = "2",
				Question = new Question {Id = 4, Value = "7"}
			};
			var repository = new ChangeNoteRepository(dataStorage.Object);

			dataStorage.Setup(ds => ds.GetData<ChangeNote>()).Returns(new List<ChangeNote> {entity});
			//Act
			repository.Edit(updatedEntity);
			//Assert
			//Values are equal
			Assert.That(entity.ChangeDate, Is.EqualTo(updatedEntity.ChangeDate));
			Assert.That(entity.ChangeReason, Is.EqualTo(updatedEntity.ChangeReason));
			Assert.That(entity.OriginalValue, Is.EqualTo(updatedEntity.OriginalValue));
			Assert.That(entity.NewValue, Is.EqualTo(updatedEntity.NewValue));
		}
	}
}
