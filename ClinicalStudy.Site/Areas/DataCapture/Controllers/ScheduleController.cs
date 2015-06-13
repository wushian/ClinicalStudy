using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.DataCapture.Models.Schedule;

namespace ClinicalStudy.Site.Areas.DataCapture.Controllers {
	public class ScheduleController : DataCaptureBaseController {
		private readonly IVisitRepository visitRepository;

		public ScheduleController(IVisitRepository visitRepository) {
			this.visitRepository = visitRepository;
		}

		public ActionResult Index(DateTime? datetime) {
			DailyScheduleViewModel model = GetDailyScheduleModel(datetime);

			if (Request.IsAjaxRequest())
				return PartialView("_Index", model);
			return View(model);
		}

		public ActionResult DailyVisitsCallback(DateTime? datetime) {
			DailyScheduleViewModel model = GetDailyScheduleModel(datetime);

			return PartialView("_DailyVisitsCallbackPanel", model);
		}

		/// <summary>
		/// This action is used to support sorting in DX Grid 
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public ActionResult DailyVisitsGrid(DateTime? datetime) {
			DailyScheduleViewModel model = GetDailyScheduleModel(datetime);

			return PartialView("_DailyVisitsGrid", model);
		}

		public ActionResult Summary() {
			DailyScheduleViewModel model = GetDailyScheduleModel(DateTime.Today, 6);
			return PartialView("_Summary", model);
		}

		private DailyScheduleViewModel GetDailyScheduleModel(DateTime? datetime, int topCount = 0) {
			DateTime date = datetime.HasValue ? datetime.Value.Date : DateTime.Now.Date;
			string doctorLogin = User.Identity.Name;

			var model = new DailyScheduleViewModel {
				ScheduledVisits = new List<ScheduledVisitViewModel>()
			};
			var visits = topCount <= 0
			                            	? visitRepository.GetDailyVisits(doctorLogin, date)
			                            	: visitRepository.GetDailyVisits(doctorLogin, date).Take(topCount);

			if (visits != null) {
				foreach (Visit visit in visits) {
					model.ScheduledVisits.Add(
						new ScheduledVisitViewModel {
							VisitId = visit.Id,
							VisitState = visit.IsFullyCompleted ? "Completed" : "Incomplete",
							PatientInitials = visit.Patient.PatientInitials,
							PatientNumber = visit.Patient.PatientNumber,
							VisitCaption = visit.Caption,
                            IsPassed = visit.IsFullyCompleted
						});
				}
			}

           

			model.Date = date;

			if (date == DateTime.Now.Date)
				model.DateDescription = "Today";
			if (date == DateTime.Now.Date.AddDays(1))
				model.DateDescription = "Tomorrow";
			if (date == DateTime.Now.Date.AddDays(-1))
				model.DateDescription = "Yesterday";


			return model;
		}

	    public ActionResult Calendar()
	    {
            List<DateTime> model = new List<DateTime>();
            string doctorLogin = User.Identity.Name;
            foreach (Visit visit in visitRepository.GetAllVisits(doctorLogin))
            {
                model.Add(
                        visit.VisitDate.HasValue 
                        ? visit.VisitDate.Value 
                        : (visit.ExpectedVisitDate ?? DateTime.MinValue)
                    );
            }
	        return PartialView("_Calendar", model);
	    }
	}
}
