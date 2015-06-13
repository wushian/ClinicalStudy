using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.Repositories.MemoryRepositories {
	public class ClinicRepository : GenericRepository<Clinic>, IClinicRepository {
		public ClinicRepository(IDataStorage dataStorage) : base(dataStorage) {
		}

		protected internal override void MapEntity(Clinic stored, Clinic updated) {
			stored.Caption = updated.Caption;
		}

		public Clinic GetClinicForDoctor(string login) {
			//as we support only single clinic, we just return first one
			return GetAll().FirstOrDefault();
		}
	}
}
