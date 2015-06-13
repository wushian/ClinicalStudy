using System.Collections.Generic;
using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.MemoryRepositories;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories {
	[TestFixture]
	public class GenericRepositoryTest {
		[Test]
		public void AddTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			TestRepository repository = new TestRepository(dataStorage.Object);
			List<Entity> list = new List<Entity>();
			dataStorage.Setup(ds => ds.GetData<Entity>()).Returns(list);

			var entity = new Entity() {Id = 3};
			//Act
			repository.Add(entity);
			//Assert
			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(list[0].Id, Is.EqualTo(3));
		}

		[Test]
		public void DeleteTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			TestRepository repository = new TestRepository(dataStorage.Object);
			var entity = new Entity() {Id = 3};
			List<Entity> list = new List<Entity>() {entity};
			dataStorage.Setup(ds => ds.GetData<Entity>()).Returns(list);
			//Act
			repository.Delete(entity);
			//Assert
			Assert.That(list.Count, Is.EqualTo(0));
		}

		[Test]
		public void GetByKeyTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			TestRepository repository = new TestRepository(dataStorage.Object);
			var entity = new Entity() {Id = 3};
			List<Entity> list = new List<Entity>() {entity};
			dataStorage.Setup(ds => ds.GetData<Entity>()).Returns(list);
			//Act
			var result = repository.GetByKey(3);
			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(3));
		}

		[Test]
		public void GetSingleNotExistingTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			TestRepository repository = new TestRepository(dataStorage.Object);
			var entity = new Entity() {Id = 3};
			List<Entity> list = new List<Entity>() {entity};
			dataStorage.Setup(ds => ds.GetData<Entity>()).Returns(list);
			//Act
			var result = repository.GetByKey(4);
			//Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public void GetAllTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			TestRepository repository = new TestRepository(dataStorage.Object);
			var entity1 = new Entity() {Id = 3};
			var entity2 = new Entity() {Id = 4};
			List<Entity> list = new List<Entity>() {entity1, entity2};
			dataStorage.Setup(ds => ds.GetData<Entity>()).Returns(list);
			//Act
			var result = repository.GetAll();
			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count(), Is.EqualTo(2));
			Assert.That(result.Count(e => e.Id == 3), Is.EqualTo(1));
			Assert.That(result.Count(e => e.Id == 4), Is.EqualTo(1));
		}

		[Test]
		public void FindByTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			TestRepository repository = new TestRepository(dataStorage.Object);
			var entity1 = new Entity() {Id = 3};
			var entity2 = new Entity() {Id = 4};
			List<Entity> list = new List<Entity>() {entity1, entity2};
			dataStorage.Setup(ds => ds.GetData<Entity>()).Returns(list);
			//Act
			var result = repository.FindBy(e => e.Id == 3);
			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count(), Is.EqualTo(1));
			Assert.That(result.Count(e => e.Id == 3), Is.EqualTo(1));
		}

		[Test]
		public void EditTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			TestRepository repository = new TestRepository(dataStorage.Object);
			var entity1 = new Entity() {Id = 3};
			List<Entity> list = new List<Entity>() {entity1};
			dataStorage.Setup(ds => ds.GetData<Entity>()).Returns(list);
			//Act
			repository.Edit(entity1);
			//Assert
			Assert.That(repository.MappingWasCalled, Is.True);
		}

		/// <summary>
		/// Utility class that wraps test target
		/// </summary>
		private class TestRepository : GenericRepository<Entity> {
			public TestRepository(IDataStorage dataStorage)
				: base(dataStorage) {
			}

			protected override void MapEntity(Entity stored, Entity updated) {
				MappingWasCalled = true;
			}

			public bool MappingWasCalled = false;
		}

		/// <summary>
		/// Entity for test which allows us to do not bind our repository test with any real domain model implementation
		/// </summary>
		private class Entity : BaseEntity {
		}
	}
}
