using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.DTOs;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.Repositories.Interface;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository {
	public class VisitRepository : GenericRepository<Visit>, IVisitRepository {
		public IList<Visit> GetVisitsForPatient(int patientId) {
			return FindBy(v => v.Patient.Id == patientId).ToList();
		}

		public VisitRepository(IClinicalStudyContextFactory contextFactory) : base(contextFactory) {
		}

		public IList<Visit> GetDailyVisits(string doctorLogin, DateTime date) {
			var today = date.Date;
			return FindBy(v => v.Patient.Doctor.Login == doctorLogin &&
					v.Patient.IsActive && 
					((v.ExpectedVisitDate != null && EntityFunctions.TruncateTime(v.ExpectedVisitDate.Value) == today)
						|| EntityFunctions.TruncateTime(v.VisitDate) == today)).ToList();
		}

	    public IList<Visit> GetAllVisits(string doctorLogin)
	    {
            return FindBy(v => v.Patient.Doctor.Login == doctorLogin && v.Patient.IsActive).ToList();
	    }

	    public Visit GetVisitByPatientNumberAndVisitName(int patientNumber, string visitName) {
			return FindBy(v => v.Patient.PatientNumber == patientNumber && v.Caption == visitName).FirstOrDefault();
		}


		public IList<AeAnalyticsDto> GetAeAnalyticsData() {
			return (from visitAe in GetAll()
				let patient = visitAe.Patient
				let visitB = patient.Visits.FirstOrDefault(x => x.VisitTypeValue == (int) VisitType.Baseline)
				let formD = visitB.Forms.FirstOrDefault(x => x.FormTypeValue == (int) FormType.Demographics)
				let formAe = visitAe.Forms.FirstOrDefault(x => x.FormTypeValue == (int) FormType.AdverseEvent)
				where visitAe.VisitTypeValue == (int) VisitType.AdverseEventVisit
				select new AeAnalyticsDto {
					ClinicName = patient.Doctor.Clinic.Caption,
					DoctorFirstName = patient.Doctor.FirstName,
					DoctorLastName = patient.Doctor.LastName,
					DemographicFormId = (formD != null) ? formD.Id : 0,
					AeFormId = (formAe != null) ? formAe.Id : 0
				}).ToList();
		}
	}
}
