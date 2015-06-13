using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinicalStudy.Repositories.EntityFrameworkRepository;

namespace ClinicalStudy.DemoData {
	public class FirstStartInitializer {
		
		private readonly ClinicalStudyContext dbContext;

		public FirstStartInitializer(ClinicalStudyContext dbContext) {
			this.dbContext = dbContext;
		}


		public void ReinitializeDateIfRequired() {

			var timepoint = dbContext.Timepoints.FirstOrDefault();
			if(timepoint == null)
				throw new InvalidOperationException("Database was not initialized properly");

			if(timepoint.WasCorrectedAfterCreation)
				return;
			
			DatesCorrector corrector=new DatesCorrector();
			int daysChange = corrector.GetDaysChange(dbContext);

			corrector.SetDatesToToday(daysChange, dbContext);
			timepoint.WasCorrectedAfterCreation = true;
			dbContext.SaveChanges();

		}
	}
}
