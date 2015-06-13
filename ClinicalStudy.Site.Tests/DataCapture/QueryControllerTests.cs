using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.DataCapture.Controllers;
using ClinicalStudy.Site.Areas.DataCapture.Models.Query;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Site.Tests.DataCapture {
	public class QueryControllerTests : ControllerTestsBase {
		private QueryController controller;
		private Mock<IQueryRepository> queryRepository;
		private Mock<IUserRepository> userRepository;

		[SetUp]
		public void Setup() {
			queryRepository = new Mock<IQueryRepository>();
			userRepository = new Mock<IUserRepository>();
			controller = new QueryController(queryRepository.Object, userRepository.Object);
		}
		[Test]
		public void ReplyQueryDialogViewTest() {
			//Arrange
			int queryId = 55;


			//Act
			var result = controller.ReplyQueryDialog(queryId);
			//Assert

			Assert.That(result, Is.Not.Null);
			Assert.That(result is PartialViewResult);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			Assert.That(viewResultBase.ViewName, Is.EqualTo("_ReplyQueryDialog"));
		}

		[Test]
		public void ReplyQueryDialog_NonAnsweredQuery_ModelTest() {
			//Arrange
			int queryId = 55;

			var query = GetStandardQuery(queryId);
			query.AnswerText = null;
			query.AnswerAuthor = null;
			queryRepository.Setup(r => r.GetByKey(queryId)).Returns(query);
			//Act

			var result = controller.ReplyQueryDialog(queryId);
			//Assert

			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			QueryViewModel model = viewResultBase.Model as QueryViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.QueryId, Is.EqualTo(queryId));

			Assert.That(model.IsClosed, Is.False);

			Assert.That(model.QueryText, Is.EqualTo(query.QueryText));
			Assert.That(model.QueryAuthor, Is.EqualTo("Supervisor Smith"));
			Assert.That(model.QueryAuthorImage, Is.EqualTo(new byte[] { 1, 2, 3 }));


			Assert.That(model.Question, Is.EqualTo("Temperature"));
			Assert.That(model.FormCaption, Is.EqualTo("Demographics"));
			Assert.That(model.VisitCaption, Is.EqualTo("Baseline"));
			Assert.That(model.PatientCaption, Is.EqualTo("Subj A025"));
			Assert.That(model.PatientNumber, Is.EqualTo(25));

			Assert.That(model.Answer, Is.Null);
			Assert.That(model.AnswerAuthor, Is.Null);
			Assert.That(model.AnswerAuthorImage, Is.Null);

		}
		[Test]
		public void ReplyQueryDialog_AnsweredQuery_ModelTest() {
			//Arrange
			int queryId = 55;

			var query = GetStandardQuery(queryId);
			queryRepository.Setup(r => r.GetByKey(queryId)).Returns(query);
			//Act

			var result = controller.ReplyQueryDialog(queryId);
			//Assert

			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			QueryViewModel model = viewResultBase.Model as QueryViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.QueryId, Is.EqualTo(queryId));
			
			Assert.That(model.IsClosed, Is.True);
			
			Assert.That(model.QueryText, Is.EqualTo(query.QueryText));
			Assert.That(model.QueryAuthor, Is.EqualTo("Supervisor Smith"));
			Assert.That(model.QueryAuthorImage, Is.EqualTo(new byte[] { 1, 2, 3 }));


			Assert.That(model.Question, Is.EqualTo("Temperature"));
			Assert.That(model.FormCaption, Is.EqualTo("Demographics"));
			Assert.That(model.VisitCaption, Is.EqualTo("Baseline"));
			Assert.That(model.PatientCaption, Is.EqualTo("Subj A025"));
			Assert.That(model.PatientNumber, Is.EqualTo(25));

			Assert.That(model.Answer, Is.EqualTo(query.AnswerText));
			Assert.That(model.AnswerAuthor, Is.EqualTo("Doctor Johnson"));
			Assert.That(model.AnswerAuthorImage, Is.EqualTo(new byte[] { 11, 12, 13 }));

		}

		[Test]
		public void Index_Counters_Model() {
			//Arrange
			EmulateControllerContext(controller, false);
			int queryId = 55;
			var query = GetStandardQuery(queryId);
			query.AnswerText = null;
			
			queryRepository
				.Setup(r => r.FindBy(It.IsAny<Expression<Func<Query, bool>>>()))
				.Returns((new List<Query>{ query}).AsQueryable() );
			//Act
			var result = controller.Index();
			//Assert

			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			QueryListDataViewModel model = viewResultBase.Model as QueryListDataViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.TotalQueryNumber, Is.EqualTo(1), "Total Query Number");
			Assert.That(model.OpenQueryNumber, Is.EqualTo(1), "Open Query Number");
		}

		[Test]
		public void Index_Query_Model() {
			//Arrange
			EmulateControllerContext(controller, false);
			int queryId = 55;
			var query = GetStandardQuery(queryId);

			queryRepository
				.Setup(r => r.GetAll())
				.Returns((new List<Query> { query }).AsQueryable());
			//Act

			var result = controller.Index();
			//Assert

			Assert.That(result, Is.Not.Null);
			Assert.That(result is ViewResultBase);
			var viewResultBase = result as ViewResultBase;
			Assert.That(viewResultBase.Model, Is.Not.Null);
			QueryListDataViewModel model = viewResultBase.Model as QueryListDataViewModel;
			Assert.That(model, Is.Not.Null);
		
			Assert.That(model.Count, Is.EqualTo(1));
			var queryModel = model[0];

			Assert.That(queryModel.IsClosed, Is.True);

			Assert.That(queryModel.QueryText, Is.EqualTo(query.QueryText));
			//Assert.That(queryModel.QueryAuthor, Is.EqualTo("Supervisor Smith"));
			//Assert.That(queryModel.QueryAuthorImageUrl, Is.EqualTo(@"~\image1.jpg"));


			Assert.That(queryModel.Question, Is.EqualTo("Temperature"));
			Assert.That(queryModel.FormCaption, Is.EqualTo("Demographics"));
			Assert.That(queryModel.VisitCaption, Is.EqualTo("Baseline"));
			Assert.That(queryModel.PatientCaption, Is.EqualTo("Subj A025"));
			Assert.That(queryModel.PatientNumber, Is.EqualTo(25));

			//Assert.That(queryModel.Answer, Is.EqualTo(query.AnswerText));
			//Assert.That(queryModel.AnswerAuthor, Is.EqualTo("Doctor Johnson"));
			//Assert.That(queryModel.AnswerAuthorImageUrl, Is.EqualTo(@"~\image2.jpg"));

		}

		private static Query GetStandardQuery(int queryId) {
			var query = new Query {
				Id = queryId,
				QueryText = "text",
				QueryAuthor = new User() {
					Role = ClinicalStudyRoles.Supervisor,
					LastName = "Smith",
					Photo = new byte[]{1,2,3},
				},
				AnswerText = "Big and accurate answer",
				AnswerAuthor = new User() {
					Role = ClinicalStudyRoles.Doctor,
					LastName = "Johnson",
					Photo = new byte[]{11,12,13}
				},
				Question = new Question {
					Id = 11,
					Caption = "Temperature",
					Form = new Form() {
						Caption = "Demographics",
						Visit = new Visit {
							Caption = "Baseline",
							Patient = new Patient {
								Caption = "Subj A025",
								PatientNumber = 25,
								Doctor = new User { Login = CommonEmulatedUserName }
							}
						}
					}
				}
			};
			return query;
		}
	}
}
