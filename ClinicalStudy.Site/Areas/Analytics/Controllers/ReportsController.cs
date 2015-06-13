using System;
using System.Linq;
using System.Web.Mvc;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.Analytics.Models.REPORTS;
using ClinicalStudy.Site.Areas.Analytics.Reports;
using DevExpress.Web.Mvc;

namespace ClinicalStudy.Site.Areas.Analytics.Controllers {
	public class ReportsController : AnalyticsBaseController {
		private readonly IPatientRepository patientRepository;
		private readonly IQueryRepository queryRepository;
		private readonly IVisitRepository visitRepository;

		public ReportsController(IPatientRepository patientRepository, IVisitRepository visitRepository,
		                         IQueryRepository queryRepository) {
			this.patientRepository = patientRepository;
			this.visitRepository = visitRepository;
			this.queryRepository = queryRepository;
		}

		public ActionResult Patients() {
			return Report(new PatientsReportViewModel {Report = GetPatientsReport()});
		}

		public ActionResult PatientsReportViewer() {
			return ReportViewer(new PatientsReportViewModel {Report = GetPatientsReport()});
		}

		public ActionResult PatientsReportViewerExportTo() {
			return ReportViewerExtension.ExportTo(GetPatientsReport());
		}

		public ActionResult Queries() {
			return Report(new QueriesReportViewModel {Report = GetQueriesReport()});
		}

		public ActionResult QueriesReportViewer() {
			return ReportViewer(new QueriesReportViewModel {Report = GetQueriesReport()});
		}

		public ActionResult QueriesReportViewerExportTo() {
			return ReportViewerExtension.ExportTo(GetQueriesReport());
		}

		private ActionResult Report(ReportViewModel model) {
			if (Request.IsAjaxRequest())
				return PartialView("_Report", model);
			else
				return View("Report", model);
		}

		private PartialViewResult ReportViewer(ReportViewModel model) {
			return PartialView("_ReportViewer", model);
		}

		private PatientsReport GetPatientsReport() {
			IQueryable<Patient> patients = patientRepository.GetAll();
			IQueryable<Visit> visits = visitRepository.GetAll();

			var report = new PatientsReport();

		    PatientsReportDataSet.PatientsDataTable patientsTable = report.PatientsReportDataSet.Patients;
			foreach (Patient patient in patients) {
				PatientsReportDataSet.PatientsRow row = patientsTable.NewPatientsRow();

				row.PatientId = patient.Id;
				row.PatientInitials = patient.PatientInitials;
				row.PatientNumber = patient.PatientNumber;
				if (patient.RandomisationDate.HasValue)
					row.RandomizationDate = patient.RandomisationDate.Value;
				row.RandomizationNumber = patient.RandomisationNumber ?? 0;
				if (patient.EnrollDate.HasValue)
					row.EnrollDate = patient.EnrollDate.Value;
				row.Enrolled = patient.IsEnrolled;
				row.Active = patient.IsActive;

				patientsTable.Rows.Add(row);
			}

			PatientsReportDataSet.VisitsDataTable visitsTable = report.PatientsReportDataSet.Visits;
			foreach (Visit visit in visits.OrderBy(v => v.VisitDate != null ? v.VisitDate : v.ExpectedVisitDate)) {
				PatientsReportDataSet.VisitsRow row = visitsTable.NewVisitsRow();

				row.VisitId = visit.Id;
				row.PatientId = visit.Patient.Id;
				if (visit.VisitDate.HasValue)
					row.VisitDate = visit.VisitDate.Value;
				row.VisitName = visit.Caption;
				row.Progress = visit.Forms.Average(x => x.FormState == FormState.Completed ? 1 : 0);

				visitsTable.Rows.Add(row);
			}

			return report;
		}

		private QueriesReport GetQueriesReport() {
			var queries = queryRepository.GetAll();

			var report = new QueriesReport();

			var queriesTable = report.QueriesReportDataSet.Queries;
			foreach (Query query in queries) {
				var row = queriesTable.NewQueriesRow();

				row.QueryId = query.Id;
				row.CRF = query.Question.Form.Caption;
				row.Question = query.Question.Caption;
				row.QueryAuthor = String.Format("{0} {1}", query.QueryAuthor.FirstName, query.QueryAuthor.LastName);
				row.QueryTime = query.QueryTime.ToString("MM/dd/yyyy HH:mm");
				row.QueryText = query.QueryText;

				if (query.AnswerAuthor != null) {
					row.ReplyAuthor = String.Format("{0} {1}", query.AnswerAuthor.FirstName, query.AnswerAuthor.LastName);
					row.ReplyText = query.AnswerText;
					row.ReplyTime = query.AnswerTime.HasValue ? query.AnswerTime.Value.ToString("MM/dd/yyyy HH:mm") : String.Empty;
				}
				queriesTable.Rows.Add(row);
			}

			return report;
		}
	}
}
