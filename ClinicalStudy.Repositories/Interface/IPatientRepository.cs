using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.DTOs;

namespace ClinicalStudy.Repositories.Interface {
	public interface IPatientRepository : IRepository<Patient> {
		Patient GetPatientByUniqueNumber(int patientNumber);
		int GetMaxPatientNumber();
		IList<PatientStateDto> GetPatientsStateData();
		IList<PatientStateDto> GetPatientsStateDataForClinic(string clinicName);

		IList<HappinessChangeDto> GetHappinessChangeData();
	}
}
