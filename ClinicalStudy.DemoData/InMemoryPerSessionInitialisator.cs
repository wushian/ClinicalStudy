using System.Diagnostics;
using System.Linq;
using ClinicalStudy.Repositories.EntityFrameworkRepository;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;

namespace ClinicalStudy.DemoData {
	public class InMemoryPerSessionInitialisator {

		private readonly ClinicalStudyContext dbContext;

		public InMemoryPerSessionInitialisator(ClinicalStudyContext dbContext) {
			this.dbContext = dbContext;
		}

		public void InitialisePerSessionData() {


			var stopwatch = Stopwatch.StartNew();




			dbContext.Configuration.AutoDetectChangesEnabled = false;
			dbContext.Configuration.LazyLoadingEnabled = false;
			dbContext.Configuration.ProxyCreationEnabled = false;
			PerSessionDataStorage storage = new PerSessionDataStorage();


			storage.SaveData(dbContext.Clinics.ToList());
			storage.SaveData(dbContext.Users.ToList());
			storage.SaveData(dbContext.Patients.ToList());
			storage.SaveData(dbContext.Visits.ToList());
			storage.SaveData(dbContext.Forms.ToList());
			storage.SaveData(dbContext.AdverseEventFormDatas.ToList());
			storage.SaveData(dbContext.DemographicFormDatas.ToList());
			storage.SaveData(dbContext.VitalsFormDatas.ToList());
			storage.SaveData(dbContext.ElectrocardiogramFormDatas.ToList());
			storage.SaveData(dbContext.InventoryFormDatas.ToList());
			storage.SaveData(dbContext.HappinessFormDatas.ToList());
			storage.SaveData(dbContext.Questions.ToList());
			storage.SaveData(dbContext.Attachments.ToList());
			storage.SaveData(dbContext.ChangeNotes.ToList());
			storage.SaveData(dbContext.RepeatableInventoryDatas.ToList());
			storage.SaveData(dbContext.Queries.ToList());
			stopwatch.Stop();
			DatesCorrector corrector = new DatesCorrector();
			int datesChange = corrector.GetDaysChange(dbContext);
			corrector.SetDatesToToday(datesChange, storage);
		}

	}
}
