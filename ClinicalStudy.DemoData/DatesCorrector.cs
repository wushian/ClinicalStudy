using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.EntityFrameworkRepository;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.DemoData {
	public class DatesCorrector {
		public int GetDaysChange(ClinicalStudyContext dbContext) {
			var timepoint = dbContext.Timepoints.First();
			var timeSpan = DateTime.Now.Date - timepoint.DateAndTime.Date;
			return (int)timeSpan.TotalDays;
		}

		public void SetDatesToToday(int daysChange, PerSessionDataStorage storage) {
			Stopwatch stopwatch = Stopwatch.StartNew();

			foreach (var patient in storage.GetData<Patient>()) {
				patient.EnrollDate = AddDays(daysChange, patient.EnrollDate);
				patient.RandomisationDate = AddDays(daysChange, patient.RandomisationDate);
			}

			foreach (var visit in storage.GetData<Visit>()) {
				visit.VisitDate = AddDays(daysChange, visit.VisitDate);
				visit.ExpectedVisitDate = AddDays(daysChange, visit.ExpectedVisitDate);
			}

			foreach (var question in storage.GetData<RepeatableInventoryData>().Select(rid => rid.DateUsed)) {
				question.Value = AddDays(daysChange, question.Value);
			}

			foreach (var formData in storage.GetData<InventoryFormData>()) {
				formData.ReceiptDate.Value = AddDays(daysChange, formData.ReceiptDate.Value);
				formData.ShipDate.Value = AddDays(daysChange, formData.ShipDate.Value);
			}

			foreach (var query in storage.GetData<Query>()) {
				query.QueryTime = AddDays(daysChange, query.QueryTime);
				query.AnswerTime = AddDays(daysChange, query.AnswerTime);
			}

			foreach (var changeNote in storage.GetData<ChangeNote>()) {
				changeNote.ChangeDate = AddDays(daysChange, changeNote.ChangeDate);
			}
			stopwatch.Stop();
		}

		public void SetDatesToToday(int daysChange, ClinicalStudyContext dbContext) {
			Stopwatch stopwatch = Stopwatch.StartNew();

			foreach (var patient in dbContext.Set<Patient>()) {
				patient.EnrollDate = AddDays(daysChange, patient.EnrollDate);
				patient.RandomisationDate = AddDays(daysChange, patient.RandomisationDate);
			}

			foreach (var visit in dbContext.Set<Visit>()) {
				visit.VisitDate = AddDays(daysChange, visit.VisitDate);
				visit.ExpectedVisitDate = AddDays(daysChange, visit.ExpectedVisitDate);
			}

			foreach (var question in dbContext.Set<RepeatableInventoryData>().Select(rid => rid.DateUsed)) {
				question.Value = AddDays(daysChange, question.Value);
			}

			foreach (var formData in dbContext.Set<InventoryFormData>()) {
				formData.ReceiptDate.Value = AddDays(daysChange, formData.ReceiptDate.Value);
				formData.ShipDate.Value = AddDays(daysChange, formData.ShipDate.Value);
			}

			foreach (var query in dbContext.Set<Query>()) {
				query.QueryTime = AddDays(daysChange, query.QueryTime);
				query.AnswerTime = AddDays(daysChange, query.AnswerTime);
			}

			foreach (var changeNote in dbContext.Set<ChangeNote>()) {
				changeNote.ChangeDate = AddDays(daysChange, changeNote.ChangeDate);
			}

			dbContext.SaveChanges();
			stopwatch.Stop();
		}

		private static string AddDays(int daysChange, string dateString) {
			if (string.IsNullOrEmpty(dateString))
				return dateString;
			var date = DateTime.Parse(dateString);

			date = date.AddDays(daysChange);
			return date.ToString(CultureInfo.InvariantCulture);
		}
		private static DateTime? AddDays(int daysChange, DateTime? date) {
			if (date.HasValue)
				date = date.Value.AddDays(daysChange);
			return date;
		}
		private static DateTime AddDays(int daysChange, DateTime date) {
			date = date.AddDays(daysChange);
			return date;
		}

	}
}
