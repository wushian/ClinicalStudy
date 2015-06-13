using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository {
	public class ClinicRepository : GenericRepository<Clinic>, IClinicRepository {
		public ClinicRepository(IClinicalStudyContextFactory contextFactory) : base(contextFactory) {
		}

		public Clinic GetClinicForDoctor(string login) {
			return GetAll().FirstOrDefault();
		}
	}
}
