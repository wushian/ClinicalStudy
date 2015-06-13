using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.Repositories.MemoryRepositories {
	public class UserRepository : GenericRepository<User>, IUserRepository {
		public UserRepository(IDataStorage dataStorage) : base(dataStorage) {
		}

		protected internal override void MapEntity(User stored, User updated) {
			stored.FirstName = updated.FirstName;
			stored.LastName = updated.LastName;
			stored.Login = updated.Login;
			stored.Role = updated.Role;
		}

		public User GetUserByLogin(string username) {
			return FindBy(u => u.Login == username).FirstOrDefault();
		}

		public User GetMainDoctor() {
			return FindBy(u => u.Login == "mitchell").FirstOrDefault();
		}
	}
}
