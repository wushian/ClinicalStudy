using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.ModelBuilders;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.DataCapture.Models;
using ClinicalStudy.Site.Areas.DataCapture.Models.Shared;
using DevExpress.Web.Mvc;

namespace ClinicalStudy.Site.Areas.DataCapture.Controllers {
	public class VisitController : DataCaptureBaseController {
		private readonly IClinicalStudyDesignFactory clinicalStudyDesignFactory;
		private readonly IVisitRepository visitRepository;

		public VisitController(IVisitRepository visitRepository, IClinicalStudyDesignFactory clinicalStudyDesignFactory) {
			this.visitRepository = visitRepository;
			this.clinicalStudyDesignFactory = clinicalStudyDesignFactory;
		}

		public ActionResult VisitDataContainer(int patientNumber, string visitName, string formName) {
			DataContainerViewModel model = CreateVisitDataModel(patientNumber, visitName, formName);

			return PartialView("_VisitDataContainer", model);
		}

		private DataContainerViewModel CreateVisitDataModel(int patientNumber, string visitName, string formName) {
			Visit visit = visitRepository.GetVisitByPatientNumberAndVisitName(patientNumber, visitName);
			if (visit == null)
				return null;

			var model = new DataContainerViewModel {
				Id = visit.Id,
				PatientNumber = patientNumber,
				SelectedVisitName = visitName,
				SelectedFormName = formName,
				Children = new List<ConteinerChildViewModel>(from form in visit.Forms
				                                             orderby form.OrderNo
				                                             select new ConteinerChildViewModel {
				                                             	Id = form.Id,
				                                             	Caption = form.Caption,
				                                             	OrderNo = form.OrderNo
				                                             })
			};
			return model;
		}


		public ActionResult ShowVisit(int patientNumber, string visitName) {
			return ViewEditVisit(null, patientNumber, visitName);
		}

		public ActionResult ViewVisit(int patientNumber, string visitName) {
			return ViewEditVisit(false, patientNumber, visitName);
		}

		public ActionResult EditVisit(int visitId) {
			return ViewEditVisit(true, visitId);
		}

		public ActionResult ViewEditVisit(bool? isVisitEditing, int visitId) {
			Visit visit = visitRepository.GetByKey(visitId);
			return ViewEditVisit(isVisitEditing, visit);
		}

		public ActionResult ViewEditVisit(bool? isVisitEditing, int patientNumber, string visitName) {
			Visit visit = visitRepository.GetVisitByPatientNumberAndVisitName(patientNumber, visitName);
			return ViewEditVisit(isVisitEditing, visit);
		}


		private ActionResult ViewEditVisit(bool? isVisitEditing, Visit visit) {
			if (visit == null) {
				var errorModel = new ErrorViewModel {
					Caption = "Visit is not found",
					ErrorMessage = string.Format("Visit is not found in clinical data")
				};
				return View("ErrorInfo", errorModel);
			}

			var model = new VisitViewModel {
				Id = visit.Id,
				Caption = visit.Caption,
				VisitType = visit.VisitType,
				ExpectedVisitDate = visit.ExpectedVisitDate,
				VisitDate = visit.VisitDate,
				VisitTime = visit.VisitTime,
				OrderNo = visit.OrderNo,
				NoExpectedVisitDate = visit.VisitType == VisitType.AdverseEventVisit
			};

			if (visit.IsCompleted && !(isVisitEditing ?? false))
				return PartialView("_ViewVisit", model);
			return PartialView("_EditVisit", model);
		}

		[HttpPost]
		public ActionResult EditVisit([ModelBinder(typeof (DevExpressEditorsBinder))] VisitViewModel model) {
			Visit visit = visitRepository.GetByKey(model.Id);
			if (visit == null) {
				var errorModel = new ErrorViewModel {
					Caption = "Visit is not found",
					ErrorMessage = string.Format("Visit '{0}' is not found in clinical data", model.Caption)
				};
				return View("ErrorInfo", errorModel);
			}

			visit.Caption = model.Caption;
			visit.ExpectedVisitDate = model.ExpectedVisitDate;
			visit.VisitDate = model.VisitDate;
			visit.VisitTime = model.VisitTime;
			EnsureSqlServer2005Compatibility(visit);
			if (visit.VisitDate.HasValue && visit.VisitTime.HasValue)
				visit.IsCompleted = true;
			visitRepository.Edit(visit);
			visitRepository.Save();

			return RedirectToAction("ShowVisit",
				new {
					patientNumber = visit.Patient.PatientNumber,
					visitName = visit.Caption,
					DXScript = Request["DXScript"]
				});
		}

		public ActionResult CreateAdverseEvent(int patientId) {
			IClinicalStudyDesign studyDesign = clinicalStudyDesignFactory.Create();

			Visit aeVisit = studyDesign.AddAdverseEventVisit(patientId);
			clinicalStudyDesignFactory.Release(studyDesign);


			if (aeVisit == null)
				return null;


			return Content(aeVisit.Caption);
		}

		private static void EnsureSqlServer2005Compatibility(Visit visit) {
			if (visit.VisitTime.HasValue && visit.VisitTime.Value.Year <= 1753)
				visit.VisitTime = visit.VisitTime.Value.AddYears(DateTime.Now.Year);
		}
	}
}
