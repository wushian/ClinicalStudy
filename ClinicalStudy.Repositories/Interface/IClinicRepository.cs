using ClinicalStudy.DomainModel;

namespace ClinicalStudy.Repositories.Interface {
	public interface IClinicRepository : IRepository<Clinic> {
		Clinic GetClinicForDoctor(string login);
	}
}
