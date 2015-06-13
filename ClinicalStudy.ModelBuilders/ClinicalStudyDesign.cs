using System;
using System.Collections.Generic;
using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.DomainModel.ModelBuilders;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.Interface.FormData;

namespace ClinicalStudy.ModelBuilders {
	/// <summary>
	/// This class is responsible for constuction of Clinical Objects hierarchy, such as Visits, Forms and Questions
	/// </summary>
	public class ClinicalStudyDesign : IClinicalStudyDesign {
		//everyone from properties below is going to be set by Castle Windsor IoC framework
		//all we need to do is to create instance of ClinicalStudyDesign through IoC instead of direct creation
		public IClinicRepository ClinicRepository { get; set; }
		public IUserRepository UserRepository { get; set; }
		public IPatientRepository PatientRepository { get; set; }
		public IVisitRepository VisitRepository { get; set; }
		public IFormRepository FormRepository { get; set; }
		public IQuestionRepository QuestionRepository { get; set; }
		public IVitalsFormDataRepository VitalsFormDataRepository { get; set; }
		public IDemographicFormDataRepository DemographicFormDataRepository { get; set; }
		public IElectrocardiogramFormDataRepository ElectrocardiogramFormDataRepository { get; set; }
		public IHappinessFormDataRepository HappinessFormDataRepository { get; set; }
		public IInventoryFormDataRepository InventoryFormDataRepository { get; set; }
		public IAdverseEventFormDataRepository AdverseEventFormDataRepository { get; set; }

		public Patient CreatePatientForDoctor(string login) {
			var doctor = UserRepository.GetUserByLogin(login);
			return CreatePatient(doctor);
		}


		public Patient CreatePatient(User doctor) {
			var patient = new Patient();
			PatientRepository.Add(patient);

			//verify that patient number is unique
			//if it is not, then assign unique number
			var maxNumber = PatientRepository.GetMaxPatientNumber();
			patient.PatientNumber = maxNumber + 1;
			patient.Caption = string.Format("Subj {0}{1:D3}", "A", patient.PatientNumber);
			patient.Doctor = doctor;
			doctor.Patients.Add(patient);

			patient.Visits = new List<Visit>();
			patient.Visits.Add(CreateBaselineVisit(patient));
			patient.Visits.Add(Create1stDayVisit(patient));
			patient.Visits.Add(Create10thDayVisit(patient));
			foreach (var visit in patient.Visits) {
				VisitRepository.Add(visit);
				foreach (var form in visit.Forms) {
					FormRepository.Add(form);
				}
			}

			return patient;
		}

		private Visit CreateBaselineVisit(Patient patient) {
			var visit = new Visit() {
				Caption = "Baseline",
				OrderNo = 0,
				ExpectedVisitDate = DateTime.Now.Date,
				Patient = patient,
				VisitType = VisitType.Baseline
			};

			var demographicForm =
				new Form() {
					Caption = "Demographics",
					FormState = FormState.Incomplete,
					FormType = FormType.Demographics,
					OrderNo = 0,
					Visit = visit
				};
			visit.Forms.Add(demographicForm);
			AddDemographicFormData(demographicForm);

			var vitalsForm =
				new Form() {
					Caption = "Vitals",
					FormState = FormState.Incomplete,
					FormType = FormType.Vitals,
					OrderNo = 1,
					Visit = visit
				};
			visit.Forms.Add(vitalsForm);
			AddVitalsFormData(vitalsForm);

			return visit;
		}


		private Visit Create1stDayVisit(Patient patient) {
			var visit = new Visit() {
				Caption = "1st Day",
				OrderNo = 1,
				ExpectedVisitDate = DateTime.Now.AddDays(2).Date,
				Patient = patient,
				VisitType = VisitType.Day1
			};

			var happinessForm =
				new Form() {
					Caption = "Happiness",
					FormState = FormState.Incomplete,
					FormType = FormType.Happiness,
					OrderNo = 0,
					Visit = visit
				};
			visit.Forms.Add(happinessForm);
			AddHappinessFormData(happinessForm);


			var electroForm =
				new Form() {
					Caption = "Electrocardiogram",
					FormState = FormState.Incomplete,
					FormType = FormType.Electrocardiogram,
					OrderNo = 1,
					Visit = visit
				};
			visit.Forms.Add(electroForm);
			AddElectrocardiogramFormData(electroForm);

			var vitalsForm =
				new Form() {
					Caption = "Vitals",
					FormState = FormState.Incomplete,
					FormType = FormType.Vitals,
					OrderNo = 2,
					Visit = visit
				};
			visit.Forms.Add(vitalsForm);
			AddVitalsFormData(vitalsForm);

			return visit;
		}

		private Visit Create10thDayVisit(Patient patient) {
			var visit = new Visit() {
				Caption = "10th Day",
				OrderNo = 2,
				ExpectedVisitDate = DateTime.Now.AddDays(12).Date,
				Patient = patient,
				VisitType = VisitType.Day10
			};

			var happinessForm =
				new Form() {
					Caption = "Happiness",
					FormState = FormState.Incomplete,
					FormType = FormType.Happiness,
					OrderNo = 0,
					Visit = visit
				};
			visit.Forms.Add(happinessForm);
			AddHappinessFormData(happinessForm);


			var inventoryForm =
				new Form() {
					Caption = "Inventory",
					FormState = FormState.Incomplete,
					FormType = FormType.Inventory,
					OrderNo = 1,
					Visit = visit
				};
			visit.Forms.Add(inventoryForm);
			AddInventoryFormData(inventoryForm);

			var vitalsForm =
				new Form() {
					Caption = "Vitals",
					FormState = FormState.Incomplete,
					FormType = FormType.Vitals,
					OrderNo = 2,
					Visit = visit
				};
			visit.Forms.Add(vitalsForm);
			AddVitalsFormData(vitalsForm);

			return visit;
		}


		public Visit AddAdverseEventVisit(int patientId) {
			var patient = PatientRepository.GetByKey(patientId);

			if (patient == null)
				return null;

			var existingVisits = VisitRepository.GetVisitsForPatient(patientId);
			var maxVisitOrderNo = existingVisits.Max(v => v.OrderNo);
			var adverseEventsCount = existingVisits.Count(v => v.VisitType == VisitType.AdverseEventVisit);
			var caption = "Adverse Event";
			if (adverseEventsCount > 0)
				caption = string.Format("{0} {1}", caption, adverseEventsCount + 1);


			var aeVisit = new Visit {
				Caption = caption,
				ExpectedVisitDate = null,
				VisitDate = DateTime.Today.Date,
				OrderNo = maxVisitOrderNo + 1,
				VisitType = VisitType.AdverseEventVisit,
				Patient = patient
			};
			VisitRepository.Add(aeVisit);
			patient.Visits.Add(aeVisit);

			AddAdverseEventForm(aeVisit);

			VisitRepository.Save();
			return aeVisit;
		}

		private void AddAdverseEventForm(Visit visit) {
			var aeForm = new Form() {
				Caption = "Adverse Event",
				FormState = FormState.Incomplete,
				FormType = FormType.AdverseEvent,
				OrderNo = 0,
				Visit = visit
			};
			visit.Forms.Add(aeForm);
			FormRepository.Add(aeForm);

			AddAdverseEventFormData(aeForm);
		}


		private DemographicFormData AddDemographicFormData(Form form) {
			var formData = new DemographicFormData() {
				Form = form,
				DateOfBirth = new Question { Caption = "Date Of Birth", DataType = QuestionDataType.Date, Form = form },
				Other = new Question { Caption = "Other", DataType = QuestionDataType.String, Form = form },
				Race = new Question { Caption = "Race", DataType = QuestionDataType.Enum, Form = form },
				Sex = new Question { Caption = "Sex", DataType = QuestionDataType.Enum, Form = form }
			};
			DemographicFormDataRepository.Add(formData);
			QuestionRepository.Add(formData.DateOfBirth);
			QuestionRepository.Add(formData.Other);
			QuestionRepository.Add(formData.Race);
			QuestionRepository.Add(formData.Sex);
			return formData;
		}

		private VitalsFormData AddVitalsFormData(Form form) {
			var formData = new VitalsFormData() {
				Form = form,
				ActualTime = new Question { Caption = "Actual Time", DataType = QuestionDataType.Date, Form = form },
				Height = new Question { Caption = "Height", DataType = QuestionDataType.Number, Form = form },
				Weight = new Question { Caption = "Weight", DataType = QuestionDataType.Number, Form = form },
				Temperature = new Question { Caption = "Temperature", DataType = QuestionDataType.Number, Form = form },
				HeartRate = new Question { Caption = "Heart Rate", DataType = QuestionDataType.Integer, Form = form },
				BloodPressureSystolic = new Question { Caption = "Systolic Blood Pressure, mmHg", DataType = QuestionDataType.String, Form = form },
				BloodPressureDiastolic = new Question { Caption = "Diastolic Blood Pressure, mmHg", DataType = QuestionDataType.String, Form = form }
			};
			VitalsFormDataRepository.Add(formData);
			QuestionRepository.Add(formData.ActualTime);
			QuestionRepository.Add(formData.Height);
			QuestionRepository.Add(formData.Weight);
			QuestionRepository.Add(formData.Temperature);
			QuestionRepository.Add(formData.HeartRate);
			QuestionRepository.Add(formData.BloodPressureSystolic);
			QuestionRepository.Add(formData.BloodPressureDiastolic);
			return formData;
		}

		private HappinessFormData AddHappinessFormData(Form form) {
			var formData = new HappinessFormData() {
				Form = form,
				HappinessLevel = new Question { Caption = "Happiness", DataType = QuestionDataType.Integer, Form = form }
			};
			HappinessFormDataRepository.Add(formData);
			QuestionRepository.Add(formData.HappinessLevel);
			return formData;
		}

		private ElectrocardiogramFormData AddElectrocardiogramFormData(Form form) {
			var formData = new ElectrocardiogramFormData() {
				Form = form,
				ElectrocardiogramActualTime = new Question { Caption = "Actual Time", DataType = QuestionDataType.Date, Form = form },
				ElectrocardiogramAttachment = new Question { Caption = "Electrocardiogram Data File", DataType = QuestionDataType.Attachment, Form = form }
			};
			ElectrocardiogramFormDataRepository.Add(formData);
			QuestionRepository.Add(formData.ElectrocardiogramActualTime);
			QuestionRepository.Add(formData.ElectrocardiogramAttachment);
			return formData;
		}

		private InventoryFormData AddInventoryFormData(Form form) {
			var formData =
				new InventoryFormData() {
					Form = form,
					ShipDate = new Question { Caption = "Ship Date", DataType = QuestionDataType.Date, Form = form },
					BatchNumber = new Question { Caption = "Batch Number", DataType = QuestionDataType.Integer, Form = form },
					QuantityShipped = new Question { Caption = "Quantity Shipped", DataType = QuestionDataType.Number, Form = form },
					ReceiptDate = new Question { Caption = "Receipt Date", DataType = QuestionDataType.Date, Form = form },
					MedicationUsage = new List<RepeatableInventoryData>()
				};
			InventoryFormDataRepository.Add(formData);
			QuestionRepository.Add(formData.ShipDate);
			QuestionRepository.Add(formData.BatchNumber);
			QuestionRepository.Add(formData.QuantityShipped);
			QuestionRepository.Add(formData.ReceiptDate);
			return formData;
		}

		private AdverseEventFormData AddAdverseEventFormData(Form form) {
			var formData = new AdverseEventFormData() {
				Form = form,
				AdverseExperience =
					new Question { Caption = "Adverse Experience", DataType = QuestionDataType.String, Value = null, Form = form },
				OnsetDate =
					new Question { Caption = "Onset Date", DataType = QuestionDataType.Date, Value = null, Form = form },
				OnsetTime =
					new Question { Caption = "Onset Time", DataType = QuestionDataType.Time, Value = null, Form = form },
				EndDate =
					new Question { Caption = "End Date", DataType = QuestionDataType.Date, Value = null, Form = form },
				EndTime =
					new Question { Caption = "End Time", DataType = QuestionDataType.Time, Value = null, Form = form },
				Outcome =
					new Question { Caption = "Outcome", DataType = QuestionDataType.Enum, Value = null, Form = form },
				Intensity =
					new Question { Caption = "Intensity", DataType = QuestionDataType.Enum, Value = null, Form = form },
				RelationshipToInvestigationalDrug =
					new Question { Caption = "Relationship to Investigational Drug", DataType = QuestionDataType.Enum, Value = null, Form = form }
			};
			AdverseEventFormDataRepository.Add(formData);
			QuestionRepository.Add(formData.AdverseExperience);
			QuestionRepository.Add(formData.OnsetDate);
			QuestionRepository.Add(formData.OnsetTime);
			QuestionRepository.Add(formData.EndDate);
			QuestionRepository.Add(formData.EndTime);
			QuestionRepository.Add(formData.Outcome);
			QuestionRepository.Add(formData.Intensity);
			QuestionRepository.Add(formData.RelationshipToInvestigationalDrug);
			return formData;
		}
	}
}
