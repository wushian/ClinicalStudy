using System;
using System.Collections.Generic;
using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.DTOs;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.Repositories.Interface;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository {
	public class QueryRepository : GenericRepository<Query>, IQueryRepository {
		public QueryRepository(IClinicalStudyContextFactory contextFactory)
			: base(contextFactory) {
		}

		public IEnumerable<Query> GetQueriesForQuestions(IEnumerable<int> questionIds) {
			return FindBy(q => questionIds.Contains(q.Question.Id));
		}

		public IEnumerable<QueryDto> GetOpenQueries() {
			var query = from q in GetAll()
				where q.AnswerText == null
				select new QueryDto() {
					FormType = (FormType) q.Question.Form.FormTypeValue,
					ClinicName = q.Question.Form.Visit.Patient.Doctor.Clinic.Caption,
					DoctorName = q.Question.Form.Visit.Patient.Doctor.LastName,
					QuestionText = q.QueryText,
					PatientNumber = q.Question.Form.Visit.Patient.PatientNumber,
					VisitName = q.Question.Form.Visit.Caption
				};
			return query.ToList();
		}


		public IEnumerable<QueryReportDto> GetQueriesReportData() {
			var query = from q in GetAll()
				select new QueryReportDto {
					ClinicName = q.Question.Form.Visit.Patient.Doctor.Clinic.Caption,
					DoctorFirstName = q.Question.Form.Visit.Patient.Doctor.FirstName,
					DoctorLastName = q.Question.Form.Visit.Patient.Doctor.LastName,
					FormType = (FormType) q.Question.Form.FormTypeValue,
					QuestionName = q.Question.Caption,
					IsOpen = q.AnswerText == null
				};
			return query.ToList();
		}
	}
}
