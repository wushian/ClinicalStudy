using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.DTOs;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.Repositories.MemoryRepositories {
	public class PatientRepository : GenericRepository<Patient>, IPatientRepository {
		public PatientRepository(IDataStorage dataStorage) : base(dataStorage) {
		}

		protected internal override void MapEntity(Patient stored, Patient updated) {
			stored.IsActive = updated.IsActive;
			stored.EnrollDate = updated.EnrollDate;
			stored.IsEnrolled = updated.IsEnrolled;
			stored.PatientInitials = updated.PatientInitials;
			stored.PatientNumber = updated.PatientNumber;
			stored.Caption = updated.Caption;
			stored.RandomisationDate = updated.RandomisationDate;
			stored.RandomisationNumber = updated.RandomisationNumber;
		}

		public Patient GetPatientByUniqueNumber(int patientNumber) {
			return FindBy(p => p.PatientNumber == patientNumber).FirstOrDefault();
		}

		public int GetMaxPatientNumber() {
			return GetAll().Max(p => p.PatientNumber);
		}

		public IList<PatientStateDto> GetPatientsStateData() {
			return GetPatientsStateData(null);

		}


		public IList<PatientStateDto> GetPatientsStateDataForClinic(string clinicName) {
			return GetPatientsStateData(psd => psd.ClinicName == clinicName);
		}

		public IList<HappinessChangeDto> GetHappinessChangeData() {

			var query = from patient in GetAll()
				let visitB = patient.Visits.FirstOrDefault(x => x.VisitTypeValue == (int) VisitType.Baseline)
				let visit1D = patient.Visits.FirstOrDefault(x => x.VisitTypeValue == (int) VisitType.Day1)
				let visit10D = patient.Visits.FirstOrDefault(x => x.VisitTypeValue == (int) VisitType.Day10)
				let formD = visitB.Forms.FirstOrDefault(x => x.FormTypeValue == (int) FormType.Demographics)
				let formHap1D = visit1D.Forms.FirstOrDefault(x => x.FormTypeValue == (int) FormType.Happiness)
				let formHap10D = visit10D.Forms.FirstOrDefault(x => x.FormTypeValue == (int) FormType.Happiness)
				select new HappinessChangeDto {
					ClinicId = patient.Doctor.Clinic.Id,
					ClinicName = patient.Doctor.Clinic.Caption,
					DoctorId = patient.Doctor.Id,
					DoctorFirstName = patient.Doctor.FirstName,
					DoctorLastName = patient.Doctor.LastName,
					PatientId = patient.Id,
					PatientNumber = patient.PatientNumber,
					DemographicFormId = (formD != null) ? formD.Id : 0,
					HappinessDay1FormId = (formHap1D != null) ? formHap1D.Id : 0,
					HappinessDay10FormId = (formHap10D != null) ? formHap10D.Id : 0
				};

			return query.ToList();

		}

		private IList<PatientStateDto> GetPatientsStateData(Expression<Func<PatientStateDto, bool>> condition) {
			var query = from patient in GetAll()
						let visit = patient.Visits.OrderBy(v => v.VisitDate).LastOrDefault(v => v.VisitDate <= DateTime.Now.Date)
						//where visit.VisitDate < DateTime.Now
						select new PatientStateDto {
							ClinicId = patient.Doctor.Clinic.Id,
							ClinicName = patient.Doctor.Clinic.Caption,
							DoctorId = patient.Doctor.Id,
							DoctorName = patient.Doctor.LastName,
							VisitType = (visit != null) ? visit.VisitType : VisitType.None
						};
			if (condition != null)
				query = query.Where(condition);
			
			return query.ToList();
		}
	}
}
