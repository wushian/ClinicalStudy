using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using ClinicalStudy.Site.Areas.Analytics.Models;

namespace ClinicalStudy.Site.Areas.Analytics.Controllers {
	public class AdminNavBarController : Controller {
		public ActionResult AdminNavBar() {
			var model = new AdminNavigationViewModel {
				NavigationCategories = new ItemsData(new List<ItemData> {
					ItemDataFactory.GetParentItem("StudyProgress", "Study Progress", "StudyProgress_A.png", "StudyProgress_I.png"),
					ItemDataFactory.GetParentItem("Analytics", "Analytics", "Analytics_A.png", "Analytics_I.png"),
					ItemDataFactory.GetParentItem("Reports", "Reports", "Reports_A.png", "Reports_I.png"),
					ItemDataFactory.GetParentItem("StudyAdministration", "Study Administration", "StudyAdministration_A.png",
						"StudyAdministration_I.png")
				})
			};

			FillChildrenFullData(model.NavigationCategories);

			return PartialView("_AdminNavigation", model);
		}

		private void FillChildrenFullData(ItemsData items) {
			ItemData category = items.GetItem("StudyProgress");
			if (category != null) {
				category.HasChildren = true;
				category.Children = new ItemsData(new List<ItemData> {
					ItemDataFactory.GetChildItem("PatientsPerVisit", "Patients Progress per Visit",
						UrlHelper.GenerateUrl("", "PatientsPerVisit", "Charts", null, RouteTable.Routes, Request.RequestContext, false)),
					ItemDataFactory.GetChildItem(string.Empty, "Unfinished CRFs",
						UrlHelper.GenerateUrl("", "UnfinishedCrfsPerClinic", "Charts", null, RouteTable.Routes, Request.RequestContext,
							false)),
					ItemDataFactory.GetChildItem(string.Empty, "Queries per CRF",
						UrlHelper.GenerateUrl("", "OpenQueries", "Charts", null, RouteTable.Routes, Request.RequestContext, false))
				});
			}

			category = items.GetItem("Analytics");
			if (category != null) {
				category.HasChildren = true;
				category.Children = new ItemsData(
					new List<ItemData> {
						ItemDataFactory.GetChildItem("HappinessChange", "Happiness Change",
							UrlHelper.GenerateUrl("", "Happiness", "Analytics", null, RouteTable.Routes, Request.RequestContext, false)),
						ItemDataFactory.GetChildItem(string.Empty, "Adverse Events",
							UrlHelper.GenerateUrl("", "AdverseEvents", "Analytics", null, RouteTable.Routes, Request.RequestContext, false)),
						ItemDataFactory.GetChildItem(string.Empty, "Queries per CRF",
							UrlHelper.GenerateUrl("", "Queries", "Analytics", null, RouteTable.Routes, Request.RequestContext, false))
					});
			}
			category = items.GetItem("Reports");
			if (category != null) {
				category.HasChildren = true;
				category.Children = new ItemsData(new List<ItemData> {
					ItemDataFactory.GetChildItem("PatientsReport", "Patients Report",
						UrlHelper.GenerateUrl("", "Patients", "Reports", null, RouteTable.Routes, Request.RequestContext, false)),
					ItemDataFactory.GetChildItem(string.Empty, "Queries Report",
						UrlHelper.GenerateUrl("", "Queries", "Reports", null, RouteTable.Routes, Request.RequestContext, false))
				});
			}
			category = items.GetItem("StudyAdministration");
			if (category != null) {
				category.HasChildren = true;
				category.Children = new ItemsData(new List<ItemData> {
					ItemDataFactory.GetChildItem("ManageClinics", "Manage Clinics",
						UrlHelper.GenerateUrl("", "Clinics", "Administration", null, RouteTable.Routes, Request.RequestContext, false))
				});
			}
		}
	}
}
