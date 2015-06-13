using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.DomainModel.ModelBuilders;
using ClinicalStudy.DemoData.DemoBuilders;
using ClinicalStudy.Repositories.EntityFrameworkRepository;
using ClinicalStudy.Repositories.EntityFrameworkRepository.FormData;

namespace ClinicalStudy.DemoData {
	public class DemoDataInitializer : DropCreateDatabaseIfModelChanges<ClinicalStudyContext> {
		private IClinicalStudyDesign clinicalStudyDesign;

		public DemoDataInitializer(IClinicalStudyDesign clinicalStudyDesign) {
			this.clinicalStudyDesign = clinicalStudyDesign;
		}


		protected override void Seed(ClinicalStudyContext context) {


			var clinicCreator = new ClinicsBuilder();
			var clinics = clinicCreator.GetClinics();
			

			UsersBuilder usersBuilder = new UsersBuilder();
			var users = usersBuilder.GetDemoUsers(clinics.FirstOrDefault());
			var moreUsers = usersBuilder.GetRandomDemoUsers(clinics);
			users = users.Concat(moreUsers).ToList();
			foreach (var user in users) {
				context.Users.Add(user);
			}
			var doctor = users.FirstOrDefault(u => u.Role == ClinicalStudyRoles.Doctor);
			

			//as any other entities are binded to Clinic, we do not need to add them explicitely
			//EF itself will take care about it
			foreach (var clinic in clinics) {
				context.Clinics.Add(clinic);
			}

			base.Seed(context);
			context.SaveChanges();

			IClinicalStudyContextFactory factory = new SingleContextFactory(context);
			PatientsBuilder patientsBuilder = new PatientsBuilder(
				new DemographicFormDataRepository(factory),
				new VitalsFormDataRepository(factory),
				new AdverseEventFormDataRepository(factory),
				new ElectrocardiogramFormDataRepository(factory),
				new HappinessFormDataRepository(factory),
				new InventoryFormDataRepository(factory),
				new AttachmentRepository(factory),
				clinicalStudyDesign);



			List<DoctorWithRequiredPatients> doctorsToGeneratePatients = new List<DoctorWithRequiredPatients>();

			foreach (var user in users.Where(u => u.Role == ClinicalStudyRoles.Doctor)) {
				doctorsToGeneratePatients.Add(new DoctorWithRequiredPatients() {
					Doctor = user,
					PatientsNumber = user == doctor ? 6 : PeopleDataProvider.GetPatientForDoctorNumber()
				});
			}

			bool firstPatient = true;
			int firstPatientNumber = 25;
			for (int i = 0; i < doctorsToGeneratePatients.Max(d => d.PatientsNumber); i++) {
				foreach (var doctorWithRequiredPatient in doctorsToGeneratePatients) {
					if (i < doctorWithRequiredPatient.PatientsNumber) {
						var patient = clinicalStudyDesign.CreatePatient(doctorWithRequiredPatient.Doctor);
						if(firstPatient) {
							patient.PatientNumber = firstPatientNumber;
							patient.Caption = "Subj A0" + firstPatientNumber.ToString();
							firstPatient = false;
						}
						context.SaveChanges();
						var demoPatientState = PeopleDataProvider.GetNewDemoPatientState();
						patientsBuilder.PopulatePatientInfoAndDemographics(patient, demoPatientState);
					}
				}
			}

			context.SaveChanges();

			QueriesBuilder builder = new QueriesBuilder {
				Patients = context.Patients.ToList(),
				AdverseEventFormDatas = context.AdverseEventFormDatas.ToList(),
				DemographicFormDatas = context.DemographicFormDatas.ToList(),
				ElectrocardiogramFormDatas = context.ElectrocardiogramFormDatas.ToList(),
				HappinessFormDatas = context.HappinessFormDatas.ToList(),
				InventoryFormDatas = context.InventoryFormDatas.ToList(),
				VitalsFormDatas = context.VitalsFormDatas.ToList()
			};
			var queries = builder.BuildQueries(users.Single(u => u.Id == 2), users.Single(u => u.Id == 1));
			foreach (var query in queries) {
				context.Queries.Add(query);
			}
			context.SaveChanges();


			context.Timepoints.Add(
				new Timepoint() { DateAndTime = DateTime.Now, WasCorrectedAfterCreation = false });
			context.SaveChanges();
		}

		private class SingleContextFactory : IClinicalStudyContextFactory {
			private ClinicalStudyContext context;

			public SingleContextFactory(ClinicalStudyContext context) {
				this.context = context;
			}


			public ClinicalStudyContext Retrieve() {
				return context;
			}
		}
		private struct DoctorWithRequiredPatients {
			public User Doctor;
			public int PatientsNumber;
		}
	}
}
