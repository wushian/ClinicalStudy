using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.DTOs;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.Repositories.Interface;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository {
	public class PatientRepository : GenericRepository<Patient>, IPatientRepository {
		public PatientRepository(IClinicalStudyContextFactory contextFactory) : base(contextFactory) {
		}

		public Patient GetPatientByUniqueNumber(int patientNumber) {
			return GetAll().FirstOrDefault(p => p.PatientNumber == patientNumber);
		}

		public int GetMaxPatientNumber() {
			var allStored = GetAll();
			if (allStored.Any())
				return allStored.Max(p => p.PatientNumber);
			return 0;
		}

		public IList<PatientStateDto> GetPatientsStateData() {
			return GetPatientsStateData(null);
		}

		public IList<PatientStateDto> GetPatientsStateDataForClinic(string clinicName) {
			return GetPatientsStateData(p => p.Doctor.Clinic.Caption == clinicName);
		}

		public IList<HappinessChangeDto> GetHappinessChangeData() {

			var query = from patient in GetAll()
						let visitB = patient.Visits.FirstOrDefault(x => x.VisitTypeValue == (int)VisitType.Baseline)
						let visit1D = patient.Visits.FirstOrDefault(x => x.VisitTypeValue == (int)VisitType.Day1)
						let visit10D = patient.Visits.FirstOrDefault(x => x.VisitTypeValue == (int)VisitType.Day10)
						let formD = visitB.Forms.FirstOrDefault(x => x.FormTypeValue == (int)FormType.Demographics)
						let formHap1D = visit1D.Forms.FirstOrDefault(x => x.FormTypeValue == (int)FormType.Happiness)
						let formHap10D = visit10D.Forms.FirstOrDefault(x => x.FormTypeValue == (int)FormType.Happiness)
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

		private IList<PatientStateDto> GetPatientsStateData(Expression<Func<Patient, bool>> condition) {
			var todaysDate = DateTime.Now.Date;

			var query = from patient in GetAll().Include("Visits")
			            select patient;

			if (condition != null)
				query = query.Where(condition);

			var memoryQuery = from patient in query.ToList()
						let visit = patient.Visits.OrderByDescending(v => v.VisitDate).FirstOrDefault(v => v.VisitDate <= todaysDate)
						select new PatientStateDto {
							ClinicId = patient.Doctor.Clinic.Id,
							ClinicName = patient.Doctor.Clinic.Caption,
							DoctorId = patient.Doctor.Id,
							DoctorName = patient.Doctor.LastName,
							VisitTypeValue = (visit != null) ? visit.VisitTypeValue : (int)VisitType.None
						};


			return memoryQuery.ToList();
		}
	}
}
