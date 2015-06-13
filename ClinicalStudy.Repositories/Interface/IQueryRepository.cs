using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.DTOs;

namespace ClinicalStudy.Repositories.Interface {
	public interface IQueryRepository : IRepository<Query> {
		IEnumerable<Query> GetQueriesForQuestions(IEnumerable<int> questionIds);
		IEnumerable<QueryDto> GetOpenQueries();
		IEnumerable<QueryReportDto> GetQueriesReportData();
	}
}
