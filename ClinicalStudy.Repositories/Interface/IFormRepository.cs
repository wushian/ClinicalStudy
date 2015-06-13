using System.Collections.Generic;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.DTOs;

namespace ClinicalStudy.Repositories.Interface {
	public interface IFormRepository : IRepository<Form> {
		IList<Form> GetVisitForms(int visitId);

		Form GetForm(int patientNumber, string visitName, string formName);
		IList<FormDto> GetUnfinishedCrfs();
	}
}
