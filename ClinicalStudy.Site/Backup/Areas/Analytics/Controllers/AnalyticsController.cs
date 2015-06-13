using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ClinicalStudy.DomainModel.DTOs;
using ClinicalStudy.DomainModel.Enums.Answers;
using ClinicalStudy.DomainModel.Enums.Display;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.Interface.FormData;
using ClinicalStudy.Site.Areas.Analytics.Models.Analytics;

namespace ClinicalStudy.Site.Areas.Analytics.Controllers {
	public class AnalyticsController : AnalyticsBaseController {
		private readonly IAdverseEventFormDataRepository aefdRepository;
		private readonly IDemographicFormDataRepository dfdRepository;
		private readonly IHappinessFormDataRepository hfdRepository;
		private readonly IPatientRepository patientRepository;
		private readonly IQueryRepository queryRepository;
		private readonly IVisitRepository visitRepository;

		public AnalyticsController(IPatientRepository patientRepository, IVisitRepository visitRepository,
		                           IDemographicFormDataRepository dfdRepository,
		                           IHappinessFormDataRepository hfdRepository, IAdverseEventFormDataRepository aefdRepository,
		                           IQueryRepository queryRepository) {
			this.patientRepository = patientRepository;
			this.visitRepository = visitRepository;
			this.dfdRepository = dfdRepository;
			this.hfdRepository = hfdRepository;
			this.aefdRepository = aefdRepository;
			this.queryRepository = queryRepository;
		}

		public ActionResult Happiness() {
			if (Request.IsAjaxRequest())
				return PartialView("_Happiness", GetPatientsHappinessChanges());
			return View("Happiness", GetPatientsHappinessChanges());
		}

		public PartialViewResult HappinessPartial() {
			return PartialView("_HappinessPivotGrid", GetPatientsHappinessChanges());
		}

		public ActionResult Queries() {
			if (Request.IsAjaxRequest())
				return PartialView("_Queries", GetQueriesAnalytics());
			return View("Queries", GetQueriesAnalytics());
		}

		public PartialViewResult QueriesPartial() {
			return PartialView("_QueriesPivotGrid", GetQueriesAnalytics());
		}

		public ActionResult AdverseEvents() {
			if (Request.IsAjaxRequest())
				return PartialView("_AdverseEvents", GetAdverseEventsAnalytics());
			return View("AdverseEvents", GetAdverseEventsAnalytics());
		}

		public PartialViewResult AdverseEventsPartial() {
			return PartialView("_AdverseEventsPivotGrid", GetAdverseEventsAnalytics());
		}

		private List<PatientsHappinessChangeViewModel> GetPatientsHappinessChanges() {
			IList<HappinessChangeDto> data = patientRepository.GetHappinessChangeData();

			return (from dto in data
			        let demog = dfdRepository.GetFormDataByFormId(dto.DemographicFormId)
			        let happiness1 = hfdRepository.GetFormDataByFormId(dto.HappinessDay1FormId).HappinessLevel
			        let happiness10 = hfdRepository.GetFormDataByFormId(dto.HappinessDay10FormId).HappinessLevel
			        where !String.IsNullOrEmpty(happiness1.Value) && !String.IsNullOrEmpty(happiness10.Value)
			        select new PatientsHappinessChangeViewModel {
			        	ClinicId = dto.ClinicId,
			        	ClinicName = dto.ClinicName,
			        	DoctorId = dto.DoctorId,
			        	DoctorName = dto.DoctorName,
			        	PatientId = dto.PatientId,
			        	PatientNumber = dto.PatientNumber,
			        	Gender = (demog != null) ? EnumHelper.GetDescription<Gender>(demog.Sex.Value) : "Unknown",
			        	Race = (demog != null) ? EnumHelper.GetDescription<Race>(demog.Race.Value) : "Unknown",
			        	HappinessChange = (decimal.Parse(happiness10.Value) - decimal.Parse(happiness1.Value))
			        	                  /100
			        }).ToList();
		}

		private List<QueriesAnalyticsViewModel> GetQueriesAnalytics() {
			IEnumerable<QueryReportDto> data = queryRepository.GetQueriesReportData();

			return (from dto in data
			        group dto by new {dto.ClinicName, dto.DoctorName, dto.FormType, dto.QuestionName}
			        into gr
			        select new QueriesAnalyticsViewModel {
			        	Clinic = gr.Key.ClinicName,
			        	Doctor = gr.Key.DoctorName,
			        	Crf = EnumHelper.GetDescription(gr.Key.FormType),
			        	Question = gr.Key.QuestionName,
			        	Queries = gr.Count(),
			        	OpenQueries = gr.Count(x => x.IsOpen)
			        }).ToList();
		}

		private List<AdverseEventsAnalyticsViewModel> GetAdverseEventsAnalytics() {
			IList<AeAnalyticsDto> data = visitRepository.GetAeAnalyticsData();

			return (from dto in data
			        let demog = dfdRepository.GetFormDataByFormId(dto.DemographicFormId)
			        let sex = (demog != null) ? EnumHelper.GetDescription<Gender>(demog.Sex.Value) : "Unknown"
			        let race = (demog != null) ? EnumHelper.GetDescription<Race>(demog.Race.Value) : "Unknown"
			        let aeForm = aefdRepository.GetFormDataByFormId(dto.AeFormId)
			        let intensity =
			        	(aeForm != null) ? EnumHelper.GetDescription<AdverseEventIntensity>(aeForm.Intensity.Value) : "Unknown"
			        let relationship =
			        	(aeForm != null)
			        		? EnumHelper.GetDescription<AdverseEventRelanshionship>(aeForm.RelationshipToInvestigationalDrug.Value)
			        		: "Unknown"
			        group dto by new {dto.ClinicName, dto.DoctorName, sex, race, intensity, relationship}
			        into gr
			        select new AdverseEventsAnalyticsViewModel {
			        	ClinicName = gr.Key.ClinicName,
			        	DoctorName = gr.Key.DoctorName,
			        	Gender = gr.Key.sex,
			        	Race = gr.Key.race,
			        	Intensity = gr.Key.intensity,
			        	RelationshipToInvestigationalDrug = gr.Key.relationship,
			        	AesCount = gr.Count()
			        }).ToList();
		}
	}
}
