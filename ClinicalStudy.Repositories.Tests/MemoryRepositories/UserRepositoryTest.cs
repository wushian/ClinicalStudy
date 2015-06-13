using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.MemoryRepositories;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories {
	[TestFixture]
	public class UserRepositoryTest {
		[Test]
		public void GetSingleUserByLoginTest() {
			//Arrange
			Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
			UserRepository repository = new UserRepository(dataStorage.Object);
			User user1 = new User() {Login = "login1"};
			dataStorage.Setup(ds => ds.GetData<User>()).Returns(new List<User> {user1});

			//Act
			var result = repository.GetUserByLogin("login1");
			//Assert
			Assert.That(result, Is.EqualTo(user1));
		}

		[Test]
		public void GetCorrectUserByLoginTest() {
			//Arrange
			Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
			UserRepository repository = new UserRepository(dataStorage.Object);
			User user1 = new User() {Login = "login1"};
			User user2 = new User() {Login = "login2"};
			dataStorage.Setup(ds => ds.GetData<User>()).Returns(new List<User> {user1, user2});

			//Act
			var result = repository.GetUserByLogin("login1");
			//Assert
			Assert.That(result, Is.EqualTo(user1));
		}

		[Test]
		public void DontGetUserByWrongLoginTest() {
			//Arrange
			Mock<IDataStorage> dataStorage = new Mock<IDataStorage>();
			UserRepository repository = new UserRepository(dataStorage.Object);
			User user1 = new User() {Login = "login1"};
			dataStorage.Setup(ds => ds.GetData<User>()).Returns(new List<User> {user1});

			//Act
			var result = repository.GetUserByLogin("login2");
			//Assert
			Assert.That(result, Is.Null);
		}
	}
}
