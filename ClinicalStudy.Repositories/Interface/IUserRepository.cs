using ClinicalStudy.DomainModel;

namespace ClinicalStudy.Repositories.Interface {
	public interface IUserRepository : IRepository<User> {
		User GetUserByLogin(string username);
		User GetMainDoctor();
	}
}
