using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository {
	public class UserRepository : GenericRepository<User>, IUserRepository {
		public UserRepository(IClinicalStudyContextFactory contextFactory) : base(contextFactory) {
		}

		public User GetUserByLogin(string username) {
			return FindBy(u => u.Login == username).FirstOrDefault();
		}

		public User GetMainDoctor() {
			return FindBy(u => u.Login == "mitchell").FirstOrDefault();
		}
	}
}
