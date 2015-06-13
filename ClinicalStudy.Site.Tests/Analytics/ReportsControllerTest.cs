using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Castle.Components.DictionaryAdapter;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.Analytics.Controllers;
using ClinicalStudy.Site.Areas.Analytics.Models.REPORTS;
using ClinicalStudy.Site.Areas.Analytics.Reports;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Site.Tests.Analytics {
	[TestFixture]
	[SetCulture("en-US")]
	[SetUICulture("en-US")]
	public class ReportsControllerTest {
		[TestFixture]
		public class PatientsReportTest : ControllerTestsBase {
            //5 patients (Ids: 111, 121, 122, 211, 212)
			//each patient has 2 visits (Ids: patId+1,2)  with 2 forms (Ids: visitId+1,2)

			private Patient patient111;
			private Visit visit1111, visit1112;
			private Form form11111, form11112, form11121, form11122;
			
            private Patient patient121, patient122;
			private Visit visit1211, visit1212, visit1221, visit1222;
			private Form form12111, form12112, form12121, form12122, form12211, form12212, form12221, form12222;

			private Patient patient211, patient212;
			private Visit visit2111, visit2112, visit2121, visit2122;
			private Form form21111, form21112, form21121, form21122, form21211, form21212, form21221, form21222;

			private void PreparePatientsReportData() {
				form11111 = new Form {Id = 11112, FormState = FormState.Incomplete};
				form11112 = new Form {Id = 11112, FormState = FormState.Completed};
				form11121 = new Form {Id = 11122, FormState = FormState.Completed};
				form11122 = new Form {Id = 11122, FormState = FormState.Incomplete};

				visit1111 = new Visit {
					Id = 1111,
					Caption = "Visit1111",
                    VisitDate = DateTime.Today.AddDays(-11),
					Forms = new List<Form> {form11111, form11112}
				};
				visit1112 = new Visit {
					Id = 1112,
					Caption = "Visit1112",
                    VisitDate = DateTime.Today.AddDays(-12),
					Forms = new List<Form> {form11121, form11122}
				};

				patient111 = new Patient {
					Id = 111,
					PatientInitials = "P111",
					RandomisationDate = DateTime.Today.AddDays(-11),
					RandomisationNumber = 111,
					EnrollDate = DateTime.Today.AddDays(11),
					IsEnrolled = true,
					IsActive = true,
					Visits = new List<Visit> {visit1111, visit1112}
				};
				visit1111.Patient = visit1112.Patient = patient111;

				form12111 = new Form {Id = 12111, FormState = FormState.Incomplete};
				form12112 = new Form {Id = 12112, FormState = FormState.Completed};
				form12121 = new Form {Id = 12121, FormState = FormState.Incomplete};
				form12122 = new Form {Id = 12122, FormState = FormState.Incomplete};
				form12211 = new Form {Id = 12211, FormState = FormState.Completed};
				form12212 = new Form {Id = 12212, FormState = FormState.Completed};
				form12221 = new Form {Id = 12221, FormState = FormState.Incomplete};
				form12222 = new Form {Id = 12222, FormState = FormState.Completed};

				visit1211 = new Visit {
					Id = 1211,
					Caption = "Visit1211",
                    VisitDate = DateTime.Today.AddDays(-11),
					Forms = new List<Form> {form12111, form12112}
				};
				visit1212 = new Visit {
					Id = 1212,
					Caption = "Visit1212",
                    VisitDate = DateTime.Today.AddDays(-12),
					Forms = new List<Form> {form12121, form12122}
				};
				visit1221 = new Visit {
					Id = 1221,
					Caption = "Visit1221",
                    VisitDate = DateTime.Today.AddDays(-21),
					Forms = new List<Form> {form12211, form12212}
				};
				visit1222 = new Visit {
					Id = 1222,
					Caption = "Visit1222",
                    VisitDate = DateTime.Today.AddDays(-22),
					Forms = new List<Form> {form12221, form12222}
				};

				patient121 = new Patient {
					Id = 121,
					PatientInitials = "P121",
					RandomisationDate = DateTime.Today.AddDays(-21),
					RandomisationNumber = 121,
					EnrollDate = DateTime.Today.AddDays(21),
					IsEnrolled = true,
					IsActive = true,
					Visits = new List<Visit> {visit1211, visit1212}
				};
				visit1211.Patient = visit1212.Patient = patient121;

				patient122 = new Patient {
					Id = 122,
					PatientInitials = "P122",
					RandomisationDate = DateTime.Today.AddDays(-22),
					RandomisationNumber = 122,
					EnrollDate = DateTime.Today.AddDays(22),
					IsEnrolled = true,
					IsActive = true,
					Visits = new List<Visit> {visit1221, visit1222}
				};
				visit1221.Patient = visit1222.Patient = patient122;

				form21111 = new Form {Id = 21111, FormState = FormState.Incomplete};
				form21112 = new Form {Id = 21112, FormState = FormState.Completed};
				form21121 = new Form {Id = 21121, FormState = FormState.Completed};
				form21122 = new Form {Id = 21122, FormState = FormState.Incomplete};
				form21211 = new Form {Id = 21211, FormState = FormState.Incomplete};
				form21212 = new Form {Id = 21212, FormState = FormState.Completed};
				form21221 = new Form {Id = 21221, FormState = FormState.Completed};
				form21222 = new Form {Id = 21222, FormState = FormState.Incomplete};

				visit2111 = new Visit {
					Id = 2111,
					Caption = "Visit2111",
					VisitDate = DateTime.Today.AddDays(-11),
					Forms = new List<Form> {form21111, form21112}
				};
				visit2112 = new Visit {
					Id = 2112,
					Caption = "Visit2112",
					VisitDate = DateTime.Today.AddDays(-12),
					Forms = new List<Form> {form21121, form21122}
				};
				visit2121 = new Visit {
					Id = 2121,
					Caption = "Visit2121",
					VisitDate = DateTime.Today.AddDays(-11),
					Forms = new List<Form> {form21211, form21212}
				};
				visit2122 = new Visit {
					Id = 2122,
					Caption = "Visit2122",
					VisitDate = DateTime.Today.AddDays(-12),
					Forms = new List<Form> {form21221, form21222}
				};

				patient211 = new Patient {
					Id = 211,
					PatientInitials = "P211",
					RandomisationDate = DateTime.Today.AddDays(-11),
					RandomisationNumber = 211,
					EnrollDate = DateTime.Today.AddDays(11),
					IsEnrolled = true,
					IsActive = true,
					Visits = new List<Visit> {visit2111, visit2112}
				};
				visit2111.Patient = visit2112.Patient = patient211;

				patient212 = new Patient {
					Id = 212,
					PatientInitials = "P212",
					RandomisationDate = DateTime.Today.AddDays(-11),
					RandomisationNumber = 212,
					EnrollDate = DateTime.Today.AddDays(11),
					IsEnrolled = true,
					IsActive = true,
					Visits = new List<Visit> {visit2121, visit2122}
				};
				visit2121.Patient = visit2122.Patient = patient212;
			}

			[Test]
			public void PatientsReport() {
				//Arrange
				Mock<IPatientRepository> pRepository;
				Mock<IVisitRepository> vRepository;
				ReportsController controller;
				ArrangePatientsTestData(out controller, out pRepository, out vRepository);
				EmulateControllerContext(controller, false);

				//Act
				var result = controller.Patients();

				//Assert
				AssertPatientsReport(false, result, "Report", pRepository, vRepository);
			}

			[Test]
			public void PatientsReport_Ajax() {
				//Arrange
				Mock<IPatientRepository> pRepository;
				Mock<IVisitRepository> vRepository;
				ReportsController controller;
				ArrangePatientsTestData(out controller, out pRepository, out vRepository);
				EmulateControllerContext(controller, true);

				//Act
				var result = controller.Patients();

				//Assert
				AssertPatientsReport(true, result, "_Report", pRepository, vRepository);
			}

			[Test]
			public void PatientsReportViewer() {
				//Arrange
				Mock<IPatientRepository> pRepository;
				Mock<IVisitRepository> vRepository;
				ReportsController controller;
				ArrangePatientsTestData(out controller, out pRepository, out vRepository);

				//Act
				var result = controller.PatientsReportViewer();

				//Assert
				AssertPatientsReport(false, result, "_ReportViewer", pRepository, vRepository);
			}

			private void ArrangePatientsTestData(out ReportsController controller, out Mock<IPatientRepository> pRepository,
			                                     out Mock<IVisitRepository> vRepository) {
				pRepository = new Mock<IPatientRepository>();
				vRepository = new Mock<IVisitRepository>();
				controller = new ReportsController(pRepository.Object, vRepository.Object, null);

				PreparePatientsReportData();

				pRepository.Setup(r => r.GetAll()).Returns(
				                                           (new List<Patient>
				                                            {patient111, patient121, patient122, patient211, patient212}).
				                                           	AsQueryable());
				vRepository.Setup(r => r.GetAll()).Returns(
				                                           (new List<Visit> {
				                                           	visit1111,
				                                           	visit1112,
				                                           	visit1211,
				                                           	visit1212,
				                                           	visit1221,
				                                           	visit1222,
				                                           	visit2111,
				                                           	visit2112,
				                                           	visit2121,
				                                           	visit2122
				                                           }).
				                                           	AsQueryable());
			}

			private void AssertPatientsReport(bool isAjaxRequest, ActionResult result, string viewName,
			                                  Mock<IPatientRepository> pRepository, Mock<IVisitRepository> vRepository) {
				Assert.That(result, Is.Not.Null);
				if (isAjaxRequest)
					Assert.That(result is PartialViewResult);
				else
					Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo(viewName));
				Assert.That(viewResultBase.Model is PatientsReportViewModel);
				var model = viewResultBase.Model as PatientsReportViewModel;
				Assert.That(model.ReportName, Is.EqualTo("Patients"));
				Assert.That(model.Report, Is.Not.Null);

				Assert.That(model.Report is PatientsReport);
				var report = model.Report as PatientsReport;

				var patientsTable = report.PatientsReportDataSet.Patients;
				Assert.That(patientsTable.Count, Is.EqualTo(5));
				CheckPatientData(patientsTable.Rows[0], patient111);
				CheckPatientData(patientsTable.Rows[1], patient121);
				CheckPatientData(patientsTable.Rows[2], patient122);
				CheckPatientData(patientsTable.Rows[3], patient211);
				CheckPatientData(patientsTable.Rows[4], patient212);

				var visitsTable = report.PatientsReportDataSet.Visits;
				Assert.That(visitsTable.Count, Is.EqualTo(10));
				CheckVisitData(visitsTable.Rows[0], patient111, visit1111);
				CheckVisitData(visitsTable.Rows[1], patient111, visit1112);
				CheckVisitData(visitsTable.Rows[2], patient121, visit1211);
				CheckVisitData(visitsTable.Rows[3], patient121, visit1212);
				CheckVisitData(visitsTable.Rows[4], patient122, visit1221);
				CheckVisitData(visitsTable.Rows[5], patient122, visit1222);
				CheckVisitData(visitsTable.Rows[6], patient211, visit2111);
				CheckVisitData(visitsTable.Rows[7], patient211, visit2112);
				CheckVisitData(visitsTable.Rows[8], patient212, visit2121);
				CheckVisitData(visitsTable.Rows[9], patient212, visit2122);

				pRepository.Verify(r => r.GetAll(), Times.Once());
				vRepository.Verify(r => r.GetAll(), Times.Once());
			}

			private void CheckPatientData(DataRow dataRow, Patient patient) {
				Assert.That(dataRow is PatientsReportDataSet.PatientsRow);
				var row = dataRow as PatientsReportDataSet.PatientsRow;
				Assert.That(row, Is.Not.Null);
				Assert.That(row.PatientId, Is.EqualTo(patient.Id));
				Assert.That(row.PatientInitials, Is.EqualTo(patient.PatientInitials));
				Assert.That(row.PatientNumber, Is.EqualTo(patient.PatientNumber));
				Assert.That(row.RandomizationDate, Is.EqualTo(patient.RandomisationDate));
				Assert.That(row.RandomizationNumber, Is.EqualTo(patient.RandomisationNumber));
				Assert.That(row.EnrollDate, Is.EqualTo(patient.EnrollDate));
				Assert.That(row.Enrolled, Is.EqualTo(patient.IsEnrolled));
				Assert.That(row.Active, Is.EqualTo(patient.IsActive));
			}

			private void CheckVisitData(DataRow dataRow, Patient patient, Visit visit) {
				Assert.That(dataRow is PatientsReportDataSet.VisitsRow);
				var row = dataRow as PatientsReportDataSet.VisitsRow;
				Assert.That(row, Is.Not.Null);
				Assert.That(row.VisitId, Is.EqualTo(visit.Id));
				Assert.That(row.PatientId, Is.EqualTo(patient.Id));
				Assert.That(row.VisitDate, Is.EqualTo(visit.VisitDate));
				Assert.That(row.VisitName, Is.EqualTo(visit.Caption));
				Assert.That(row.Progress, Is.EqualTo(visit.Forms.Average(x => x.FormState == FormState.Completed ? 1 : 0)));
			}
		}

		[TestFixture]
		public class QueriesReportTest : ControllerTestsBase
		{
			//2 supervisors (Ids: 1000, 2000)
			//3 doctors (Ids: 1, 2, 3)
			//1st doctor has 3 CRFs (Ids: 11, 12, 13)
			//2nd doctor has 2 CRFs (Ids: 21, 22)
			//3rd doctor has 5 CRFs (Ids: 31, 32, 33, 34, 35)
			//11 form has 1 question with query w/ reply (QId: 111)
			//12 form has 2 questions with query w/ reply (QIds: 121, 122)
			//13 form has 0 questions with query
			//21 form has 3 questions with query w/ reply (QId: 211, 212, 213)
			//22 form has 0 question with query
			//each CRF of 3rd doctor has 1 query w/o reply

			private User supervisor1000, supervisor2000;
			private User doctor1, doctor2, doctor3;

			private Form form11, form12, form13;
			private Form form21, form22;
			private Form form31, form32, form33, form34, form35;

			private Question question111, question121, question122, question211, question212, question213;
			private Question question311, question321, question331, question341, question351;

			private Query query1110, query1210, query1220, query2110, query2120, query2130;
			private Query query3110, query3210, query3310, query3410, query3510;

			private void PrepareQueriesReportData() {
				supervisor1000 = new User {Id = 1000, FirstName = "First1000", LastName = "Last1000"};
				supervisor2000 = new User {Id = 2000, FirstName = "First2000", LastName = "Last2000"};

				doctor1 = new User {Id = 1, FirstName = "First1", LastName = "Last1"};
				doctor2 = new User {Id = 2, FirstName = "First2", LastName = "Last2"};
				doctor3 = new User {Id = 3, FirstName = "First3", LastName = "Last3"};

				form11 = new Form {Id = 11, Caption = "CRF11"};
				form12 = new Form {Id = 12, Caption = "CRF12"};
				form13 = new Form {Id = 13, Caption = "CRF13"};

				form21 = new Form {Id = 21, Caption = "CRF21"};
				form22 = new Form {Id = 22, Caption = "CRF22"};

				form31 = new Form {Id = 31, Caption = "CRF31"};
				form32 = new Form {Id = 32, Caption = "CRF32"};
				form33 = new Form {Id = 33, Caption = "CRF33"};
				form34 = new Form {Id = 34, Caption = "CRF34"};
				form35 = new Form {Id = 35, Caption = "CRF35"};

				question111 = new Question {Id = 111, Caption = "Question111", Form = form11};

				question121 = new Question {Id = 121, Caption = "Question121", Form = form12};
				question122 = new Question {Id = 122, Caption = "Question122", Form = form12};

				question211 = new Question {Id = 211, Caption = "Question211", Form = form21};
				question212 = new Question {Id = 212, Caption = "Question212", Form = form21};
				question213 = new Question {Id = 213, Caption = "Question213", Form = form21};

				question311 = new Question {Id = 311, Caption = "Question311", Form = form31};
				question321 = new Question {Id = 321, Caption = "Question321", Form = form32};
				question331 = new Question {Id = 331, Caption = "Question331", Form = form33};
				question341 = new Question {Id = 341, Caption = "Question341", Form = form34};
				question351 = new Question {Id = 351, Caption = "Question351", Form = form35};

				query1110 = new Query { Id = 1110, Question = question111, QueryAuthor = supervisor1000, QueryTime = DateTime.Today.AddDays(-1110), QueryText = "Query1110", AnswerAuthor = doctor1, AnswerText = "Reply1110", AnswerTime = DateTime.Today.AddDays(1110)};
				query1210 = new Query { Id = 1210, Question = question121, QueryAuthor = supervisor2000, QueryTime = DateTime.Today.AddDays(-1210), QueryText = "Query1210", AnswerAuthor = doctor1, AnswerText = "Reply1210", AnswerTime = DateTime.Today.AddDays(1210) };
				query1220 = new Query { Id = 1220, Question = question122, QueryAuthor = supervisor1000, QueryTime = DateTime.Today.AddDays(-1220), QueryText = "Query1220", AnswerAuthor = doctor1, AnswerText = "Reply1220", AnswerTime = DateTime.Today.AddDays(1220) };
				query2110 = new Query { Id = 2110, Question = question211, QueryAuthor = supervisor2000, QueryTime = DateTime.Today.AddDays(-2110), QueryText = "Query2110", AnswerAuthor = doctor2, AnswerText = "Reply2110", AnswerTime = DateTime.Today.AddDays(2110) };
				query2120 = new Query { Id = 2120, Question = question212, QueryAuthor = supervisor1000, QueryTime = DateTime.Today.AddDays(-2120), QueryText = "Query2120", AnswerAuthor = doctor2, AnswerText = "Reply2120", AnswerTime = DateTime.Today.AddDays(2120) };
				query2130 = new Query { Id = 2130, Question = question213, QueryAuthor = supervisor2000, QueryTime = DateTime.Today.AddDays(-2130), QueryText = "Query2130", AnswerAuthor = doctor2, AnswerText = "Reply2130", AnswerTime = DateTime.Today.AddDays(2130) };
				query3110 = new Query { Id = 3110, Question = question311, QueryAuthor = supervisor1000, QueryTime = DateTime.Today.AddDays(-3110), QueryText = "Query3110"};
				query3210 = new Query { Id = 3210, Question = question321, QueryAuthor = supervisor2000, QueryTime = DateTime.Today.AddDays(-3210), QueryText = "Query3210"};
				query3310 = new Query { Id = 3310, Question = question331, QueryAuthor = supervisor1000, QueryTime = DateTime.Today.AddDays(-3310), QueryText = "Query3310"};
				query3410 = new Query { Id = 3410, Question = question341, QueryAuthor = supervisor2000, QueryTime = DateTime.Today.AddDays(-3410), QueryText = "Query3410"};
				query3510 = new Query { Id = 3510, Question = question351, QueryAuthor = supervisor1000, QueryTime = DateTime.Today.AddDays(-3510), QueryText = "Query3510"};
			}

			[Test]
			public void QueriesReport()
			{
				//Arrange
				Mock<IQueryRepository> qRepository;
				ReportsController controller;
				ArrangeQueriesTestData(out controller, out qRepository);
				EmulateControllerContext(controller, false);

				//Act
				var result = controller.Queries();

				//Assert
				AssertQueriesReport(false, result, "Report", qRepository);
			}

			[Test]
			public void QueriesReport_Ajax()
			{
				//Arrange
				Mock<IQueryRepository> qRepository;
				ReportsController controller;
				ArrangeQueriesTestData(out controller, out qRepository);
				EmulateControllerContext(controller, true);

				//Act
				var result = controller.Queries();

				//Assert
				AssertQueriesReport(true, result, "_Report", qRepository);
			}

			[Test]
			public void QueriesReportViewer()
			{
				//Arrange
				Mock<IQueryRepository> qRepository;
				ReportsController controller;
				ArrangeQueriesTestData(out controller, out qRepository);

				//Act
				var result = controller.QueriesReportViewer();

				//Assert
				AssertQueriesReport(false, result, "_ReportViewer", qRepository);
			}

			private void ArrangeQueriesTestData(out ReportsController controller, out Mock<IQueryRepository> qRepository)
			{
				qRepository = new Mock<IQueryRepository>();
				controller = new ReportsController(null, null, qRepository.Object);

				PrepareQueriesReportData();

				qRepository.Setup(r => r.GetAll()).Returns(
				                                           (new List<Query> {
				                                           	query1110,
				                                           	query1210,
				                                           	query1220,
				                                           	query2110,
				                                           	query2120,
				                                           	query2130,
				                                           	query3110,
				                                           	query3210,
				                                           	query3310,
				                                           	query3410,
				                                           	query3510
				                                           }).AsQueryable());
			}

			private void AssertQueriesReport(bool isAjaxRequest, ActionResult result, string viewName, Mock<IQueryRepository> qRepository)
			{
				Assert.That(result, Is.Not.Null);
				if (isAjaxRequest)
					Assert.That(result is PartialViewResult);
				else
					Assert.That(result is ViewResultBase);
				var viewResultBase = result as ViewResultBase;
				Assert.That(viewResultBase.Model, Is.Not.Null);
				Assert.That(viewResultBase.ViewName, Is.EqualTo(viewName));
				Assert.That(viewResultBase.Model is QueriesReportViewModel);
				var model = viewResultBase.Model as QueriesReportViewModel;
				Assert.That(model.ReportName, Is.EqualTo("Queries"));
				Assert.That(model.Report, Is.Not.Null);

				Assert.That(model.Report is QueriesReport);
				var report = model.Report as QueriesReport;

				var queriesTable = report.QueriesReportDataSet.Queries;
				Assert.That(queriesTable.Count, Is.EqualTo(11));
				CheckQueryData(queriesTable.Rows[0], query1110);
				CheckQueryData(queriesTable.Rows[1], query1210);
				CheckQueryData(queriesTable.Rows[2], query1220);
				CheckQueryData(queriesTable.Rows[3], query2110);
				CheckQueryData(queriesTable.Rows[4], query2120);
				CheckQueryData(queriesTable.Rows[5], query2130);
				CheckQueryData(queriesTable.Rows[6], query3110, false);
				CheckQueryData(queriesTable.Rows[7], query3210, false);
				CheckQueryData(queriesTable.Rows[8], query3310, false);
				CheckQueryData(queriesTable.Rows[9], query3410, false);
				CheckQueryData(queriesTable.Rows[10], query3510, false);

				qRepository.Verify(r => r.GetAll(), Times.Once());
			}

			private void CheckQueryData(DataRow dataRow, Query query, bool wReply = true)
			{
				Assert.That(dataRow is QueriesReportDataSet.QueriesRow);
				var row = dataRow as QueriesReportDataSet.QueriesRow;
				Assert.That(row, Is.Not.Null);
				Assert.That(row.QueryId, Is.EqualTo(query.Id));
				Assert.That(row.CRF, Is.EqualTo(query.Question.Form.Caption));
				Assert.That(row.Question, Is.EqualTo(query.Question.Caption));
				Assert.That(row.QueryAuthor, Is.EqualTo(String.Format("{0} {1}", query.QueryAuthor.FirstName, query.QueryAuthor.LastName)));
				Assert.That(row.QueryTime, Is.EqualTo(query.QueryTime.ToString("MM/dd/yyyy HH:mm")));
				Assert.That(row.QueryText, Is.EqualTo(query.QueryText));

				if (wReply) {
					Assert.That(row.ReplyAuthor, Is.EqualTo(String.Format("{0} {1}", query.AnswerAuthor.FirstName, query.AnswerAuthor.LastName)));
					Assert.That(row.ReplyTime, Is.EqualTo(query.AnswerTime.Value.ToString("MM/dd/yyyy HH:mm")));
					Assert.That(row.ReplyText, Is.EqualTo(query.AnswerText));
				}
				else {
					Assert.That(String.IsNullOrEmpty(row.ReplyAuthor), Is.True);
					Assert.That(row.ReplyTime, Is.EqualTo(String.Empty));
					Assert.That(String.IsNullOrEmpty(row.ReplyText), Is.True);
				}
			}
		}
	}
}
