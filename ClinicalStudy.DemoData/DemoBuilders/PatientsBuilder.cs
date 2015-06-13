using System;
using System.Collections.Generic;
using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.Enums.Answers;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.DomainModel.ModelBuilders;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.Interface.FormData;

namespace ClinicalStudy.DemoData.DemoBuilders {
	internal class PatientsBuilder {
		private readonly Random randomGenerator = new Random();
		private readonly IDemographicFormDataRepository demographicFormDataRepository;
		private readonly IVitalsFormDataRepository vitalsFormDataRepository;
		private readonly IAdverseEventFormDataRepository adverseEventFormDataRepository;
		private readonly IElectrocardiogramFormDataRepository electrocardiogramFormDataRepository;
		private readonly IHappinessFormDataRepository happinessFormDataRepository;
		private readonly IInventoryFormDataRepository inventoryFormDataRepository;
		private readonly IAttachmentRepository attachmentRepository;
		private readonly IClinicalStudyDesign clinicalStudyDesign;


		private const int FormIsFilledChance = 94;

		public PatientsBuilder(IDemographicFormDataRepository demographicFormDataRepository,
		                       IVitalsFormDataRepository vitalsFormDataRepository,
		                       IAdverseEventFormDataRepository adverseEventFormDataRepository,
		                       IElectrocardiogramFormDataRepository electrocardiogramFormDataRepository,
		                       IHappinessFormDataRepository happinessFormDataRepository,
		                       IInventoryFormDataRepository inventoryFormDataRepository,
		                       IAttachmentRepository attachmentRepository,
							IClinicalStudyDesign clinicalStudyDesign) {
			this.demographicFormDataRepository = demographicFormDataRepository;
			this.vitalsFormDataRepository = vitalsFormDataRepository;
			this.adverseEventFormDataRepository = adverseEventFormDataRepository;
			this.electrocardiogramFormDataRepository = electrocardiogramFormDataRepository;
			this.happinessFormDataRepository = happinessFormDataRepository;
			this.inventoryFormDataRepository = inventoryFormDataRepository;
			this.attachmentRepository = attachmentRepository;
			this.clinicalStudyDesign = clinicalStudyDesign;
		}


		public void PopulatePatientInfoAndDemographics(Patient patient, DemoPatientState patientState) {
			var isMale = PeopleDataProvider.IsMale();
			var firstName = PeopleDataProvider.GetRandomFirstName(isMale);
			var lastName = PeopleDataProvider.GetRandomLastName(isMale);
			var initials = firstName.Substring(0, 1) + lastName.Substring(0, 1);
			initials = initials.ToUpperInvariant();

			patient.PatientInitials = initials;

			if (patientState == DemoPatientState.NotEnrolled)
				return;
			int dice = randomGenerator.Next(100);
			if(dice < 97)
				patient.IsActive = true;


			//TODO: change per patient state
			patient.IsEnrolled = true;
			patient.EnrollDate = DateTime.Now.AddDays(- randomGenerator.Next(10)*(int) patientState);

			patient.RandomisationDate = patient.EnrollDate.Value.AddDays(randomGenerator.Next(0, 2));
			patient.RandomisationNumber = randomGenerator.Next(10000);
			patient.IsCompleted = true;


			//Baseline visit 
			var baselineVisit = patient.Visits.First(v => v.VisitType == VisitType.Baseline);
			baselineVisit.ExpectedVisitDate = patient.RandomisationDate;
			if (patientState >= DemoPatientState.BaselineOnly)
				SetRealVisitDateAndTime(baselineVisit);
			//Day 1 Visit
			var day1Day = patient.Visits.First(v => v.VisitType == VisitType.Day1);
			day1Day.ExpectedVisitDate =
				(baselineVisit.VisitDate ?? baselineVisit.ExpectedVisitDate).Value.AddDays(randomGenerator.Next(3));
			if (patientState >= DemoPatientState.Day1Only) {
				SetRealVisitDateAndTime(day1Day);

				//Adverse Event section only for patients passed 1st day
				dice = randomGenerator.Next(100);
				if (dice > 85)
					AddAdverseEventVisit(patient);
				if (dice > 95)
					AddAdverseEventVisit(patient);
			}


			//Day 10 visit
			var day10Day = patient.Visits.First(v => v.VisitType == VisitType.Day10);
			day10Day.ExpectedVisitDate = (day1Day.VisitDate ?? day1Day.ExpectedVisitDate).Value.AddDays(10);
			if (patientState >= DemoPatientState.AllVisits)
				SetRealVisitDateAndTime(day10Day);


			PopulateBaselineVisit(baselineVisit, isMale);
			if (patientState == DemoPatientState.BaselineOnly)
				return;
			PopulateDay1Visit(day1Day, baselineVisit);

			if (patientState == DemoPatientState.Day1Only)
				return;
			PopulateDay10Visit(day10Day, day1Day);
		}

		private void PopulateBaselineVisit(Visit baselineVisit, bool isMale) {

			var year = DateTime.Now.Year - randomGenerator.Next(29) - 22;
			var month = randomGenerator.Next(12) + 1;
			var day = randomGenerator.Next(28) + 1;
			var dateOfBirth = new DateTime(year, month, day);


			var dice = randomGenerator.Next(100);
			if (dice < FormIsFilledChance) {
				var demographicForm = baselineVisit.Forms.First(f => f.FormType == FormType.Demographics);
				demographicForm.FormState = FormState.Completed;
				var demographicFormData = demographicFormDataRepository.GetFormDataByFormId(demographicForm.Id);
				demographicFormData.Sex.Value = isMale ? "0" : "1";
				demographicFormData.Race.Value = randomGenerator.Next(6).ToString();

				demographicFormData.DateOfBirth.Value = dateOfBirth.ToString();
				demographicFormData.Other.Value = string.Empty;
			}

			dice = randomGenerator.Next(100);
			if (dice < FormIsFilledChance) {
				var vitalsForm = baselineVisit.Forms.First(f => f.FormType == FormType.Vitals);
				vitalsForm.FormState = FormState.Completed;
				var vitalsFormData = vitalsFormDataRepository.GetFormDataByFormId(vitalsForm.Id);

				vitalsFormData.ActualTime.Value = baselineVisit.VisitTime.Value.AddMinutes(randomGenerator.Next(18)*10).ToString();
				double weight = Math.Round(((isMale ? 86.6 : 74.4) + randomGenerator.Next(10) - 5), 1);
				vitalsFormData.Weight.Value = weight.ToString();
				double height = Math.Round(((isMale ? 177d : 163d) + randomGenerator.Next(20) - 10), 0);
				vitalsFormData.Height.Value = height.ToString();

				var temperature = GetRandomTemperature();
				vitalsFormData.Temperature.Value = temperature.ToString();

				var heartRate = GetRandomHeartRate();
				vitalsFormData.HeartRate.Value = heartRate.ToString();
				int age = DateTime.Now.Year - dateOfBirth.Year;
				double systolic = 109d + (0.5*age) + (0.1d*weight);
				double diastolic = 63d + (0.1*age) + (0.15d*weight);
				vitalsFormData.BloodPressureSystolic.Value = ((int) systolic/5*5).ToString();
				vitalsFormData.BloodPressureDiastolic.Value = ((int) diastolic/5*5).ToString();
			}
		}


		public void PopulateDay1Visit(Visit day1Visit, Visit baselineVisit) {
			//happiness form
			int dice;
			dice = randomGenerator.Next(100);
			if (dice < FormIsFilledChance) {
				var happinessForm = day1Visit.Forms.First(f => f.FormType == FormType.Happiness);
				happinessForm.FormState = FormState.Completed;
				var happinessFormData = happinessFormDataRepository.GetFormDataByFormId(happinessForm.Id);
				int happinessLevel;
				dice = randomGenerator.Next(100);
				if (dice < 5)
					happinessLevel = 0;
				else if (dice < 25)
					happinessLevel = 25;
				else if (dice < 90)
					happinessLevel = 50;
				else if (dice < 98)
					happinessLevel = 75;
				else
					happinessLevel = 100;
				happinessFormData.HappinessLevel.Value = happinessLevel.ToString();
			}
			//electrocardiogram form
			dice = randomGenerator.Next(100);
			if (dice < FormIsFilledChance) {
				var electrocardiogramForm = day1Visit.Forms.First(f => f.FormType == FormType.Electrocardiogram);
				electrocardiogramForm.FormState = FormState.Completed;
				var electrocardiogramFormData = electrocardiogramFormDataRepository.GetFormDataByFormId(electrocardiogramForm.Id);
				electrocardiogramFormData.ElectrocardiogramAttachment.File = new Attachment {
					FileName = string.Format("{0}_cardiogramm.png", day1Visit.Patient.Caption),
					FileSize = 0,
					Id = electrocardiogramForm.Id,
					MimeType = "image/png",
					StorageFileName = "ecg1.png"
				};
				attachmentRepository.Add(electrocardiogramFormData.ElectrocardiogramAttachment.File);
			}
			//vitals form
			
			dice = randomGenerator.Next(100);
			if (dice < FormIsFilledChance) {
				var vitalsOriginalForm = baselineVisit.Forms.First(f => f.FormType == FormType.Vitals);
				if (vitalsOriginalForm.FormState == FormState.Completed) {
					var vitalsOriginalFormData = vitalsFormDataRepository.GetFormDataByFormId(vitalsOriginalForm.Id);

					var vitalsForm = day1Visit.Forms.First(f => f.FormType == FormType.Vitals);
					vitalsForm.FormState = FormState.Completed;
					var vitalsFormData = vitalsFormDataRepository.GetFormDataByFormId(vitalsForm.Id);


					vitalsFormData.ActualTime.Value = day1Visit.VisitTime.Value.AddMinutes(randomGenerator.Next(18)*10).ToString();


					FillVitalsBasedOnPreviousVisit(vitalsOriginalFormData, vitalsFormData, vitalsForm);
				}
			}
		}

		private void PopulateDay10Visit(Visit day10Visit, Visit day1Visit) {
			//happiness form
			int dice = randomGenerator.Next(100);
			if (dice < FormIsFilledChance) {
				var happinessOriginalForm = day1Visit.Forms.First(f => f.FormType == FormType.Happiness);
				if (happinessOriginalForm.FormState == FormState.Completed) {
					var happinessOriginalFormData = happinessFormDataRepository.GetFormDataByFormId(happinessOriginalForm.Id);

					var happinessForm = day10Visit.Forms.First(f => f.FormType == FormType.Happiness);
					var happinessFormData = happinessFormDataRepository.GetFormDataByFormId(happinessForm.Id);

					int originalHappinessLevel = int.Parse(happinessOriginalFormData.HappinessLevel.Value);

					dice = randomGenerator.Next(100);
					int happinessLevel = 0;
					if (dice < 50)
						happinessLevel = originalHappinessLevel + 25;
					else if (dice < 75)
						happinessLevel = originalHappinessLevel + 50;
					else if (dice < 95)
						happinessLevel = originalHappinessLevel;
					else
						happinessLevel = originalHappinessLevel - 25;

					happinessLevel = Math.Min(happinessLevel, 100);
					happinessLevel = Math.Max(happinessLevel, 0);
					happinessFormData.HappinessLevel.Value = happinessLevel.ToString();

					happinessForm.FormState = FormState.Completed;
				}
			}
			//inventory form
			dice = randomGenerator.Next(100);
			if (dice < FormIsFilledChance) {
				var inventoryForm = day10Visit.Forms.First(f => f.FormType == FormType.Inventory);
				inventoryForm.FormState = FormState.Completed;
				var inventoryFormData = inventoryFormDataRepository.GetFormDataByFormId(inventoryForm.Id);
				var quantityShipped = Math.Round((double) randomGenerator.Next(8)/2, 1) + 4;
				var shipDate = day1Visit.VisitDate.Value.AddDays(randomGenerator.Next(2));
				inventoryFormData.QuantityShipped.Value = quantityShipped.ToString();
				inventoryFormData.ReceiptDate.Value = day1Visit.VisitDate.ToString();
				inventoryFormData.ShipDate.Value = shipDate.ToString();
				inventoryFormData.BatchNumber.Value = randomGenerator.Next(1000).ToString();

				FillInvetnoryFormMedicationUsage(quantityShipped, shipDate, inventoryFormData);
			}

			//vitals form
			dice = randomGenerator.Next(100);
			if (dice < FormIsFilledChance) {
				var vitalsOriginalForm = day1Visit.Forms.First(f => f.FormType == FormType.Vitals);
				if (vitalsOriginalForm.FormState == FormState.Completed) {

					var vitalsOriginalFormData = vitalsFormDataRepository.GetFormDataByFormId(vitalsOriginalForm.Id);

					var vitalsForm = day10Visit.Forms.First(f => f.FormType == FormType.Vitals);
					vitalsForm.FormState = FormState.Completed;
					var vitalsFormData = vitalsFormDataRepository.GetFormDataByFormId(vitalsForm.Id);


					vitalsFormData.ActualTime.Value = day10Visit.VisitTime.Value.AddMinutes(randomGenerator.Next(18)*10).ToString();

					FillVitalsBasedOnPreviousVisit(vitalsOriginalFormData, vitalsFormData, vitalsForm);
				}
			}
		}

		private void FillVitalsBasedOnPreviousVisit(VitalsFormData vitalsOriginalFormData, VitalsFormData vitalsFormData,
		                                            Form vitalsForm) {
			vitalsForm.FormState = FormState.Completed;
			vitalsFormData.Height.Value = vitalsOriginalFormData.Height.Value;
			double weight = double.Parse(vitalsOriginalFormData.Weight.Value);
			weight = Math.Round(weight + randomGenerator.NextDouble()*3 - 1.5d, 1);
			vitalsFormData.Weight.Value = weight.ToString();

			vitalsFormData.BloodPressureDiastolic.Value = vitalsOriginalFormData.BloodPressureDiastolic.Value;
			vitalsFormData.BloodPressureSystolic.Value = vitalsOriginalFormData.BloodPressureSystolic.Value;

			var temperature = GetRandomTemperature();
			vitalsFormData.Temperature.Value = temperature.ToString();
			var heartRate = GetRandomHeartRate();
			vitalsFormData.HeartRate.Value = heartRate.ToString();
		}

		private void FillInvetnoryFormMedicationUsage(double quantityShipped, DateTime shipDate,
		                                              InventoryFormData inventoryFormData) {

			int usagesCount = randomGenerator.Next(1, 4);
			double quantityPerTime = Math.Round(quantityShipped/usagesCount, 1);
			DateTime nextUsage = shipDate.AddDays(randomGenerator.Next(4));
			inventoryFormData.MedicationUsage = new List<RepeatableInventoryData>();

			for (int i = 0; i < usagesCount - 1; i++) {
				inventoryFormData.MedicationUsage.Add(new RepeatableInventoryData {
					DateUsed =
						new Question() {Caption = "Date Used", DataType = QuestionDataType.Date, Value = nextUsage.ToString()},
					QuantityUsed =
						new Question() {Caption = "Quantity Used", DataType = QuestionDataType.Number, Value = quantityPerTime.ToString()}
				});
				nextUsage = nextUsage.AddDays(randomGenerator.Next(1, 3));
			}
			double restOfMedication = quantityShipped - Math.Round(quantityPerTime*(usagesCount - 1), 1);
			restOfMedication = Math.Round(restOfMedication, 1);
			inventoryFormData.MedicationUsage.Add(new RepeatableInventoryData {
				DateUsed =
					new Question() { Caption = "Date Used", DataType = QuestionDataType.Date, Value = nextUsage.ToString() },
				QuantityUsed =
					new Question() { Caption = "Quantity Used", DataType = QuestionDataType.Number, Value = restOfMedication.ToString() }
			});
		}

		private void SetRealVisitDateAndTime(Visit visit) {
			var visitDate = visit.ExpectedVisitDate.Value.AddDays(randomGenerator.Next(100) > 95 ? 1 : 0);
			SetRealVisitDateAndTime(visit, visitDate);
		}

		private void SetRealVisitDateAndTime(Visit visit, DateTime visitDate) {
			visit.VisitDate = visitDate;
			int visitHour = randomGenerator.Next(0, 8) + 8;
			int visitMinute = randomGenerator.Next(0, 60);
			visitMinute = visitMinute - visitMinute%10;

			visit.VisitTime = visit.VisitDate.Value.Date;
			visit.VisitTime = visit.VisitTime.Value.AddHours(visitHour);
			visit.VisitTime = visit.VisitTime.Value.AddMinutes(visitMinute);
			visit.IsCompleted = true;
		}


		private int GetRandomHeartRate() {
			int heartRate = 75 + randomGenerator.Next(30) - 15;
			return heartRate;
		}

		private decimal GetRandomTemperature() {
			decimal temperature = 36.6m;
			int dice = randomGenerator.Next(100);
			if (dice > 97)
				temperature = temperature - (decimal) randomGenerator.Next(10)/10;
			else if (dice > 92)
				temperature = temperature + (decimal) randomGenerator.Next(15)/10;
			return temperature;
		}



		public void AddAdverseEventVisit(Patient patient) {
			var visit = clinicalStudyDesign.AddAdverseEventVisit(patient.Id);

			var day1Date = patient.Visits.FirstOrDefault(v => v.VisitTypeValue == (int) VisitType.Day1).VisitDate;
			SetRealVisitDateAndTime(visit, day1Date.Value.AddDays(randomGenerator.Next(9)));
			//vitals form
			var form = visit.Forms.ToList().OrderBy(f => f.OrderNo).Last(f => f.FormType == FormType.AdverseEvent);
			form.FormState = FormState.Completed;
			var formData = adverseEventFormDataRepository.GetFormDataByFormId(form.Id);
			

			


			formData.AdverseExperience.Value = GetRandomAdverseEvent();
			formData.RelationshipToInvestigationalDrug.Value =
				((int)(AdverseEventRelanshionship)(randomGenerator.Next(4) + 1)).ToString();
			formData.Outcome.Value =
				((int)(AdverseEventOutcome)(randomGenerator.Next(2) + 1)).ToString();
			formData.Intensity.Value =
				((int)(AdverseEventIntensity)(randomGenerator.Next(3) + 1)).ToString();

			int onsetHour = randomGenerator.Next(0, 23);
			int onsetMinute = randomGenerator.Next(0, 60);
			onsetMinute = onsetMinute - onsetMinute % 10;
			var onsetDate = visit.VisitDate.Value.AddDays(-(randomGenerator.Next(3) + 1)).Date;
			onsetDate = onsetDate.AddHours(onsetHour);
			onsetDate = onsetDate.AddMinutes(onsetMinute);
			formData.OnsetDate.Value = onsetDate.Date.ToString();
			formData.OnsetTime.Value = onsetDate.ToString();

			var endDate = onsetDate.AddMinutes(randomGenerator.Next(3, 36)*10);
			formData.EndDate.Value = endDate.Date.ToString();
			formData.EndTime.Value = endDate.ToString();
		}

		private string GetRandomAdverseEvent() {
			int dice = randomGenerator.Next(100);
			if (dice < 20)
				return "Pain in legs";
			if (dice < 30)
				return "Sleep disturbance";
			if (dice < 35)
				return "Depression";
			if (dice < 50)
				return "Fever";
			if (dice < 55)
				return "A fit of anger";
			if (dice < 75)
				return "Headache";
			if (dice < 85)
				return "Violation of the digestive system";
			if (dice < 95)
				return "Dystaxia";
			return "Visual impairment";
		}
	}
}
