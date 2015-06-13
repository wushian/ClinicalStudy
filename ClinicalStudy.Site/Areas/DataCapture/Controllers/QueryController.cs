using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.DataCapture.Models.Query;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.Site.Areas.DataCapture.Controllers {
	public class QueryController : DataCaptureBaseController {
		private readonly IQueryRepository queryRepository;
		private readonly IUserRepository userRepository;

		public QueryController(IQueryRepository queryRepository, IUserRepository userRepository) {
			this.queryRepository = queryRepository;
			this.userRepository = userRepository;
		}


		public ActionResult Summary() {
			QueryListDataViewModel model = PopulateQueries(6);
			return PartialView("_Summary", model);
		}

		public ActionResult Index() {
			QueryListDataViewModel model = PopulateQueries();
			if (Request.IsAjaxRequest())
				return PartialView("_Index", model);
			return View("Index", model);
		}

		public ActionResult GridCallback() {
			QueryListDataViewModel model = PopulateQueries();
			return PartialView("_QueryGrid", model);
		}


        private static string GetAnswerAuthorRole(string role)
        {
            if (role == ClinicalStudyRoles.Doctor)
                role = "Dr.";
            return role;
        }
        public ActionResult ReplyQueryDialog(int queryId)
        {
			Query query = queryRepository.GetByKey(queryId);
			var model = new QueryViewModel {QueryId = queryId};
			if (query != null) {
                model.QueryAuthor = String.Format("{0} {1}", GetAnswerAuthorRole(query.QueryAuthor.Role), query.QueryAuthor.LastName);
				model.QueryAuthorImage = query.QueryAuthor.Photo;
				model.QueryText = query.QueryText;
				model.Question = query.Question.Caption;

				model.IsClosed = !string.IsNullOrEmpty(query.AnswerText);
				model.Answer = query.AnswerText;
				if (query.AnswerAuthor != null) {
                    model.AnswerAuthor = String.Format("{0} {1}", GetAnswerAuthorRole(query.AnswerAuthor.Role), query.AnswerAuthor.LastName);
					model.AnswerAuthorImage = query.AnswerAuthor.Photo;
				}
				model.FormCaption = query.Question.Form.Caption;
				model.VisitCaption = query.Question.Form.Visit.Caption;
				model.PatientCaption = query.Question.Form.Visit.Patient.Caption;
				model.PatientNumber = query.Question.Form.Visit.Patient.PatientNumber;
			}
			return PartialView("_ReplyQueryDialog", model);
		}

		[HttpPost]
		public ActionResult ReplyQueryDialogPost(QueryViewModel model) {
			if (ModelState.IsValid) {
				Query query = queryRepository.GetByKey(model.QueryId);
				Debug.Assert(query != null, "query != null", "Query was not found");
				User user = userRepository.GetUserByLogin(User.Identity.Name);

				Debug.Assert(user != null, "user != null", "User was not found");

				query.AnswerText = model.Answer;
				query.AnswerTime = DateTime.Now;
				query.AnswerAuthor = user;
				queryRepository.Edit(query);
				queryRepository.Save();
			}
			return PartialView("_ReplyQueryDialog", model);
		}


		private QueryListDataViewModel PopulateQueries(int topCount = 0) {
			string doctorLogin = User.Identity.Name;
			var queries = from q in queryRepository.GetAll()
			                                     where q.Question.Form.Visit.Patient.Doctor.Login == doctorLogin
			                                     orderby q.QueryTime
			                                     select new QueryViewModel {
			                                     	IsClosed = q.AnswerText != null,
			                                     	QueryAuthorImage = q.QueryAuthor.Photo,
			                                     	QueryId = q.Id,
			                                     	QueryText = q.QueryText,
			                                     	QueryAuthor = q.QueryAuthor.Role + " " + q.QueryAuthor.LastName,
			                                     	PatientNumber = q.Question.Form.Visit.Patient.PatientNumber,
			                                     	PatientCaption = q.Question.Form.Visit.Patient.Caption,
			                                     	VisitCaption = q.Question.Form.Visit.Caption,
			                                     	FormCaption = q.Question.Form.Caption,
			                                     	Question = q.Question.Caption
			                                     };


			var model = new QueryListDataViewModel(topCount <= 0 ? queries : queries.Take(topCount));
			model.OpenQueryNumber =
				queryRepository.FindBy(q => q.Question.Form.Visit.Patient.Doctor.Login == doctorLogin).Count(
					q => q.AnswerText == null);
			model.TotalQueryNumber =
				queryRepository.FindBy(q => q.Question.Form.Visit.Patient.Doctor.Login == doctorLogin).Count();
			return model;
		}
	}
}
