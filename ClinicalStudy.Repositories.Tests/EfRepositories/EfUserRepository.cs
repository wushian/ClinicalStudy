using System.Configuration;
using System.Data.Entity;
using System.Linq;
using ClinicalStudy.Repositories.EntityFrameworkRepository;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.EfRepositories {
	[TestFixture]
	public class EfUserRepository {
		[Test]
		[Explicit]
		public void TestCreation() {
			Database.SetInitializer(new DropCreateDatabaseAlways<ClinicalStudyContext>());
			var connectionString = ConfigurationManager.ConnectionStrings["ClinicalStudyContext"].ConnectionString;


			ClinicalStudyContext db = new ClinicalStudyContext(connectionString);
			var x = db.Users.FirstOrDefault();
		}
	}
}
