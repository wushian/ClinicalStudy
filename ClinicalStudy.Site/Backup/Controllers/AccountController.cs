using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Models.Account;

namespace ClinicalStudy.Site.Controllers {

	[RedirectOnOldBrowser(Controller = "Info", Action = "OldBrowser")]
	public class AccountController : Controller {
		private IUserRepository _userRepository;

		public AccountController(IUserRepository userRepository) {
			_userRepository = userRepository;
		}

		public ActionResult LogOn() {




			var usersQuery = from user in _userRepository.GetAll().OrderBy(u => u.Role).ThenBy(u => u.LastName)
			                 where user.CanVisitWebSite == true
			                 select new LogOnViewModel {
			                 	UserName = user.Login,
			                 	FullName = user.FirstName + " " + user.LastName,
			                 	Role = user.Role,
			                 	Photo = user.Photo
			                 };
			return View(usersQuery.AsEnumerable());
		}

		[HttpPost]
		public ActionResult LogOn(LogOnViewModel model, string returnUrl) {
			if (ModelState.IsValid) {
				if (Membership.ValidateUser(model.UserName, string.Empty)) {
					bool rememberMe = false;
					FormsAuthentication.SetAuthCookie(model.UserName, rememberMe);
					if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
					    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")) {
						return Redirect(returnUrl);
					}
					else {
						var user = _userRepository.GetUserByLogin(model.UserName);
						switch (user.Role) {
							case ClinicalStudyRoles.Doctor:
								return RedirectToAction("Index", "Dashboard", new {area = "DataCapture"});
							default:
								return RedirectToAction("Index", "Dashboard", new { area = "Analytics" });
						}
					}
				}
				else {
					ModelState.AddModelError("", "The user name or password provided is incorrect.");
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		public ActionResult LogOff() {
			FormsAuthentication.SignOut();
			return RedirectToAction("LogOn");
		}

		public ActionResult LoggedUserInfo() {
			if (User == null)
				return null;
			if (!User.Identity.IsAuthenticated)
				return null;

			var user = _userRepository.GetUserByLogin(User.Identity.Name);
			if (user == null)
				return null;
			return PartialView("_LoggedUserInfo", user.LastName);
		}

		public ActionResult RefreshSession() {
			return Content("Refreshed");
		}
	}
}
