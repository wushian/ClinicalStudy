using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using ClinicalStudy.Repositories.MemoryRepositories.FormData;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories.FormData {
	[TestFixture]
	public class GenericFormDataRepositoryTest {
		[Test]
		public void FormDataForForm() {
			//Arrange
			var form = new Form {Id = 15};
			var formData = new FormDataEntity() {Form = form};

			var dataStorage = new Mock<IDataStorage>();
			dataStorage.Setup(ds => ds.GetData<FormDataEntity>()).Returns(new List<FormDataEntity> {formData});
			var repository = new TestFormDataRepository(dataStorage.Object);
			//Act
			var result = repository.GetFormDataByFormId(form.Id);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.EqualTo(formData));
		}

		[Test]
		public void FormDataForManyForms() {
			//Arrange
			var form = new Form {Id = 5};
			var formData1 = new FormDataEntity {Form = form};
			var anotherForm = new Form {Id = 11};
			var formData2 = new FormDataEntity {Form = anotherForm};

			var dataStorage = new Mock<IDataStorage>();
			dataStorage.Setup(ds => ds.GetData<FormDataEntity>()).Returns(new List<FormDataEntity> {formData1, formData2});
			var repository = new TestFormDataRepository(dataStorage.Object);
			//Act
			var result = repository.GetFormDataByFormId(form.Id);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.EqualTo(formData1));
		}

		[Test]
		public void MappingTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			var entity = new FormDataEntity() {Id = 3, Form = new Form {Id = 5}};
			var updatedEntity = new FormDataEntity() {Id = 3, Form = new Form {Id = 6}};
			var repository = new TestFormDataRepository(dataStorage.Object);

			dataStorage.Setup(ds => ds.GetData<FormDataEntity>()).Returns(new List<FormDataEntity> {entity});
			//Act
			repository.Edit(updatedEntity);
			//Assert
			Assert.That(entity.Form.Id, Is.EqualTo(updatedEntity.Form.Id));
		}

		/// <summary>
		/// Utility class that wraps test target
		/// </summary>
		private class TestFormDataRepository : GenericFormDataRepository<FormDataEntity> {
			public TestFormDataRepository(IDataStorage dataStorage)
				: base(dataStorage) {
			}
		}

		/// <summary>
		/// Entity for test which allows us to do not bind our repository test with any real domain model implementation
		/// </summary>
		private class FormDataEntity : BaseFormData {
		}
	}
}
