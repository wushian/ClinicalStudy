using System;
using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.DTOs;

namespace ClinicalStudy.Repositories.Interface {
	public interface IVisitRepository : IRepository<Visit> {
		IList<Visit> GetVisitsForPatient(int patientId);
		IList<Visit> GetDailyVisits(string doctorLogin, DateTime date);
        IList<Visit> GetAllVisits(string doctorLogin);
		Visit GetVisitByPatientNumberAndVisitName(int patientNumber, string visitName);
		IList<AeAnalyticsDto> GetAeAnalyticsData();
	}
}
