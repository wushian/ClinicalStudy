using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ClinicalStudy.DomainModel.DTOs;
using ClinicalStudy.DomainModel.Enums.Display;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.Analytics.Models.Charts;

namespace ClinicalStudy.Site.Areas.Analytics.Controllers {
	public class ChartsController : AnalyticsBaseController {
		private readonly IFormRepository formRepository;
		private readonly IPatientRepository patientRepository;
		private readonly IQueryRepository queryRepository;

		public ChartsController(IPatientRepository patientRepository, IFormRepository formRepository,
		                        IQueryRepository queryRepository) {
			this.patientRepository = patientRepository;
			this.formRepository = formRepository;
			this.queryRepository = queryRepository;
		}

		public ActionResult PatientsPerVisit() {
			IList<PatientStateDto> data = patientRepository.GetPatientsStateData();
			var query = from stateData in data
			                                           orderby stateData.VisitTypeValue
			                                           group stateData by stateData.VisitType
			                                           into gr
			                                           select new PatientStateViewModel {
			                                           	SortingKey = (int) gr.Key,
			                                           	StudyState = EnumHelper.GetDescription(gr.Key),
			                                           	PatientsNumber = gr.Count()
			                                           };


			var model = new PatientStateList();
			model.AddRange(query.ToList());
			if (Request.IsAjaxRequest())
				return PartialView("_PatientsPerVisit", model);
			return View("PatientsPerVisit", model);
		}

		public ActionResult PatientsPerVisitPerClinic(string studyState, int stateNumber) {
			IList<PatientStateDto> data = patientRepository.GetPatientsStateData();
			var query = from stateData in data
			                                           where EnumHelper.GetDescription(stateData.VisitType) == studyState
			                                           group stateData by stateData.ClinicName
			                                           into gr
			                                           orderby gr.Key
			                                           select new PatientStateViewModel {
			                                           	StudyState = studyState,
			                                           	EntityCaption = gr.Key,
			                                           	PatientsNumber = gr.Count()
			                                           };


			var model = new PatientStateList {
				PatientState = studyState,
				PatientStateColorNumber = stateNumber
			};
			model.AddRange(query.ToList());


			if (Request.IsAjaxRequest())
				return PartialView("_PatientsPerVisitPerClinic", model);
			return View("PatientsPerVisitPerClinic", model);
		}

		public ActionResult PatientsPerVisitPerDoctor(string studyState, string clinicName, int stateNumber) {
			IList<PatientStateDto> data = patientRepository.GetPatientsStateDataForClinic(clinicName);
			var query = from stateData in data
			                                           where EnumHelper.GetDescription(stateData.VisitType) == studyState
			                                           group stateData by stateData.DoctorName
			                                           into gr
			                                           orderby gr.Key
			                                           select new PatientStateViewModel {
			                                           	StudyState = studyState,
			                                           	EntityCaption = gr.Key,
			                                           	PatientsNumber = gr.Count()
			                                           };


			var model = new PatientStateList {
				PatientState = studyState,
				PatientStateColorNumber = stateNumber,
				ClinicName = clinicName,
			};
			model.AddRange(query.ToList());
			if (Request.IsAjaxRequest())
				return PartialView("_PatientsPerVisitPerDoctor", model);
			return View("PatientsPerVisitPerDoctor", model);
		}


		public ActionResult UnfinishedCrfsPerClinic() {
			IList<FormDto> unfinishedCrfs = formRepository.GetUnfinishedCrfs();

			var query = from formData in unfinishedCrfs
			                                            orderby formData.ClinicName , (int) formData.FormType
			                                            group formData by new {formData.ClinicName, formData.FormType}
			                                            into gr
			                                            select new UnfinishedCrfViewModel {
			                                            	EntityName = gr.Key.ClinicName,
			                                            	FormType = EnumHelper.GetDescription(gr.Key.FormType),
			                                            	FormsNumber = gr.Count()
			                                            };

			var model = new UnfinishedCrfsList();

			model.AddRange(query.ToList());


			if (Request.IsAjaxRequest())
				return PartialView("_UnfinishedCrfsPerClinic", model);
			return View(model);
		}


		public ActionResult UnfinishedCrfsPerDoctor(string clinicName) {
			IList<FormDto> unfinishedCrfs = formRepository.GetUnfinishedCrfs();

			var query = from formData in unfinishedCrfs
			                                            where formData.ClinicName == clinicName
			                                            orderby formData.DoctorName , (int) formData.FormType
			                                            group formData by new {formData.DoctorName, formData.FormType}
			                                            into gr
			                                            select new UnfinishedCrfViewModel {
			                                            	EntityName = gr.Key.DoctorName,
			                                            	FormType = EnumHelper.GetDescription(gr.Key.FormType),
			                                            	FormsNumber = gr.Count()
			                                            };

			var model = new UnfinishedCrfsList();
			model.ClinicName = clinicName;
			model.AddRange(query.ToList());

			if (Request.IsAjaxRequest())
				return PartialView("_UnfinishedCrfsPerDoctor", model);
			return View(model);
		}

		public ActionResult OpenQueries() {
			var model = new List<QueryRecordViewModel>();

			var query = from q in queryRepository.GetOpenQueries()
			                                          orderby q.ClinicName , (int) q.FormType
			                                          group q by new {q.ClinicName, q.FormType}
			                                          into gr
			                                          select new QueryRecordViewModel {
			                                          	ClinicName = gr.Key.ClinicName,
			                                          	FormType = EnumHelper.GetDescription(gr.Key.FormType),
			                                          	QueriesNumber = gr.Count()
			                                          };

			model.AddRange(query.ToList());
			if (Request.IsAjaxRequest())
				return PartialView("_OpenQueries", model);
			return View(model);
		}
	}
}
