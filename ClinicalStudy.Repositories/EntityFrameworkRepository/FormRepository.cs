using System;
using System.Collections.Generic;
using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.DTOs;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.Repositories.Interface;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository {
	public class FormRepository : GenericRepository<Form>, IFormRepository {
		public FormRepository(IClinicalStudyContextFactory contextFactory) : base(contextFactory) {
		}

		public IList<Form> GetVisitForms(int visitId) {
			return FindBy(f => f.Visit.Id == visitId).ToList();
		}

		public Form GetForm(int patientNumber, string visitName, string formName) {
			return FindBy(f => f.Caption == formName
			                   && f.Visit.Caption == visitName
			                   && f.Visit.Patient.PatientNumber == patientNumber).FirstOrDefault();
		}

		IList<FormDto> IFormRepository.GetUnfinishedCrfs() {

			var query = from form in GetAll()
						where form.FormStateValue == (int)FormState.Incomplete
						&& (((form.Visit.VisitDate != null) && (form.Visit.VisitDate <= DateTime.Now))
							|| ((form.Visit.VisitDate == null) && (form.Visit.ExpectedVisitDate != null) && (form.Visit.ExpectedVisitDate <= DateTime.Now)))
						select new FormDto() {
							FormType = (FormType)form.FormTypeValue,
							ClinicName = form.Visit.Patient.Doctor.Clinic.Caption,
							DoctorName = form.Visit.Patient.Doctor.LastName,
							Caption = form.Caption,
							PatientNumber = form.Visit.Patient.PatientNumber,
							VisitName = form.Visit.Caption
						};
			return query.ToList();
		}

	}
}
