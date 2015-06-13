using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DemoData.DemoBuilders {
	public class UsersBuilder {
		private int entityIdIncrement = 1;
		private int currentMalePhotoIndex = 1;
		private int currentFemalePhotoIndex = 1;
		private const int DoctorsPerClinicNumber = 4;
		private Random randomGenerator;
		private const int MaxMalePhotoIndex = 28;
		private const int MaxFemalePhotoIndex = 28;
		private const string SupervisorPhoto = "~/Content/People/Supervisor.jpg";
		private const string DoctorPhoto = "~/Content/People/Doctor.jpg";
		private const string NoPhotoMale = "~/Content/People/Male/NoPhotoMale.png";
		private const string NoPhotoFemale = "~/Content/People/Female/NoPhotoFemale.png";

		public UsersBuilder() {
			randomGenerator = new Random();
		}

		public List<User> GetDemoUsers(Clinic clinic) {
			var users = new List<User>() {
				new User() {
					Id = entityIdIncrement++,
					FirstName = "Mark",
					LastName = "Oliver",
					Login = "moliver",
					Role = ClinicalStudyRoles.Supervisor,
					Photo = GetBytesFromImage(SupervisorPhoto),
					CanVisitWebSite = true,
					Patients = new List<Patient>(),
					Clinic = clinic
				},
				new User() {
					Id = entityIdIncrement++,
					FirstName = "Devid",
					LastName = "Mitchell",
					Login = "dmitchell",
					Role = ClinicalStudyRoles.Doctor,
					Photo = GetBytesFromImage(DoctorPhoto),
					CanVisitWebSite = true,
					Patients = new List<Patient>(),
					Clinic = clinic
				}
			};

			clinic.Doctors.AddRange(users);
			return users;
		}

		public List<User> GetRandomDemoUsers(List<Clinic> clinics) {
			var users = new List<User>();
			foreach (var clinic in clinics) {
				if(!clinic.Doctors.Any(d => d.Role == ClinicalStudyRoles.Supervisor))
					CreateUserRecord(clinic, users, ClinicalStudyRoles.Supervisor);
				
				var doctorsNumberInThisClinic = DoctorsPerClinicNumber + randomGenerator.Next(0, 2) - 1;
				for (int i = 0; i < doctorsNumberInThisClinic; i++) {
					CreateUserRecord(clinic, users, ClinicalStudyRoles.Doctor);
				}

				
			}

			return users;
		}

		private User CreateUserRecord(Clinic clinic, List<User>users, string role) {
			bool isMale = PeopleDataProvider.IsMale();

			var user = new User() {
				Id = entityIdIncrement++,
				FirstName = PeopleDataProvider.GetRandomFirstName(isMale),
				LastName = PeopleDataProvider.GetRandomLastName(isMale),
				Photo = GetBytesFromImage(GetNextPhotoUrl(isMale)),
				CanVisitWebSite = false,
				Clinic = clinic,
				Patients = new List<Patient>(),
				Role = role
			};
			CreateUserLoginName(user, users);
			users.Add(user);
			clinic.Doctors.Add(user);
			return user;
		}

		private void CreateUserLoginName(User user, List<User> users) {
			var baseLogin = user.FirstName.Substring(0, 1).ToLower() + user.LastName.ToLower();
			var login = baseLogin;
			int iterator = 1;
			while (users.Any(u => string.Equals(login, u.Login, StringComparison.InvariantCultureIgnoreCase))) {
				login = baseLogin + iterator.ToString();
				iterator++;
			}

			user.Login = login;
		}

		private string GetNextPhotoUrl(bool isMale) {
			var userHasPhoto = randomGenerator.Next(100)%3 != 0;
			if (!userHasPhoto) {
				if (isMale)
					return NoPhotoMale;
				else
					return NoPhotoFemale;
			}

			int currentIndex = isMale ? currentMalePhotoIndex : currentFemalePhotoIndex;
			int maxPhotoIndex = isMale ? MaxMalePhotoIndex : MaxFemalePhotoIndex;
			if (currentIndex > maxPhotoIndex) {
				if (isMale)
					return NoPhotoMale;
				else
					return NoPhotoFemale;
				
			}
			string subfolder = isMale ? "Male" : "Female";
			var url = string.Format("~/Content/People/{0}/photo{1}.jpg", subfolder, currentIndex);
			if (isMale)
				currentMalePhotoIndex++;
			else
				currentFemalePhotoIndex++;
			return url;
		}

		private byte[] GetBytesFromImage(string imagePath) {
			if (HttpContext.Current != null)
				imagePath = HttpContext.Current.Server.MapPath(imagePath);
			if(!File.Exists(imagePath))
				return new byte[0];
			Image image = Image.FromFile(imagePath);
			MemoryStream ms = new MemoryStream();
			image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
			return ms.ToArray();
		}
	}
}
