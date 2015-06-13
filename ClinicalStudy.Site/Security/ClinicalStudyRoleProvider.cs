using System.Web.Security;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.IoC;

namespace ClinicalStudy.Site.Security {
	public class ClinicalStudyRoleProvider : RoleProvider {
		public static string[] EmptyRolesArray = new string[] {};

		public override string[] GetRolesForUser(string username) {
			//as this class is created by ASP.NET environment
			//we should explicitely call IoC container here 
			IUserRepository userRepository = IoCContainer.Instance.Resolve<IUserRepository>();

			var user = userRepository.GetUserByLogin(username);
			if (user == null)
				return EmptyRolesArray;

			return new string[] {user.Role};
		}

		public override string ApplicationName { get; set; }

		#region Not used methods

		public override bool IsUserInRole(string username, string roleName) {
			throw new System.NotImplementedException();
		}


		public override void CreateRole(string roleName) {
			throw new System.NotImplementedException();
		}

		public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) {
			throw new System.NotImplementedException();
		}

		public override bool RoleExists(string roleName) {
			throw new System.NotImplementedException();
		}

		public override void AddUsersToRoles(string[] usernames, string[] roleNames) {
			throw new System.NotImplementedException();
		}

		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {
			throw new System.NotImplementedException();
		}

		public override string[] GetUsersInRole(string roleName) {
			throw new System.NotImplementedException();
		}

		public override string[] GetAllRoles() {
			throw new System.NotImplementedException();
		}

		public override string[] FindUsersInRole(string roleName, string usernameToMatch) {
			throw new System.NotImplementedException();
		}

		#endregion
	}
}
