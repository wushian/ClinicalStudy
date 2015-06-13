using System;
using System.Collections.Generic;
using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.Enums.Answers;
using ClinicalStudy.DomainModel.FormData;

namespace ClinicalStudy.DemoData.DemoBuilders {
	public class QueriesBuilder {
		public List<DemographicFormData> DemographicFormDatas;
		public List<HappinessFormData> HappinessFormDatas;
		public List<VitalsFormData> VitalsFormDatas;
		public List<InventoryFormData> InventoryFormDatas;
		public List<ElectrocardiogramFormData> ElectrocardiogramFormDatas;
		public List<AdverseEventFormData> AdverseEventFormDatas;
		public List<Patient> Patients;
		private readonly Random randomGenerator = new Random();
		private static int queryId = 1;

		public List<Query> BuildQueries(User doctor, User supervisor) {
			var list = new List<Query>();

			int firstPatientNumber = doctor.Patients.First().PatientNumber;


			var query = new Query {
				Id = queryId++,
				QueryText = "Please use Country Name, not Code",
				QueryAuthor = supervisor,
				QueryTime = DateTime.Now.AddDays(-1),
				AnswerAuthor = doctor,
				AnswerText = "Corrected from “CAR” to “Central African Republic”",
				AnswerTime = DateTime.Now.AddDays(-0.75)
			};
			AssociateQueryWithQuestion(query, firstPatientNumber, VisitType.Baseline, FormType.Demographics,
			                           (formId) => DemographicFormDatas.Find(d => d.Form.Id == formId).Other);

			list.Add(query);


			query = new Query {
				Id = queryId++,
				QueryText = "Please specify the reason for high temperature",
				QueryAuthor = supervisor,
				QueryTime = DateTime.Now.AddDays(-1),
				AnswerAuthor = doctor,
                AnswerText = "Patient had flu-like symptoms occurr a day before the visit",
				AnswerTime = DateTime.Now.AddDays(-0.75)
			};
			AssociateQueryWithQuestion(query, firstPatientNumber, VisitType.Day1, FormType.Vitals,
			                           (formId) => VitalsFormDatas.Find(d => d.Form.Id == formId).Temperature);

			list.Add(query);

			query = new Query {
				Id = queryId++,
				QueryText = "Cardiogram image is not full",
				QueryAuthor = supervisor,
				QueryTime = DateTime.Now.AddDays(-1),
				AnswerAuthor = null,
				AnswerText = null,
				AnswerTime = null
			};
			AssociateQueryWithQuestion(query, firstPatientNumber, VisitType.Day1, FormType.Electrocardiogram,
			                           (formId) =>
			                           ElectrocardiogramFormDatas.Find(d => d.Form.Id == formId).ElectrocardiogramAttachment);

			list.Add(query);


			query = new Query {
				Id = queryId++,
				QueryText = "Was the measurement performed in regular conditions?",
				QueryAuthor = supervisor,
				QueryTime = DateTime.Now.AddDays(-1),
				AnswerAuthor = null,
				AnswerText = null,
				AnswerTime = null
			};
			AssociateQueryWithQuestion(query, firstPatientNumber, VisitType.Day10, FormType.Vitals,
			                           (formId) => VitalsFormDatas.Find(d => d.Form.Id == formId).HeartRate);

			list.Add(query);

			AddTemparatureQueries(list);
			AddHappinessChangeQueries(list);
			AddDemographicQueries(list);
			AddInventoryQueries(list);
			return list;
		}

		private void AddInventoryQueries(List<Query> list) {
			Query query;
			int dice;
			foreach (var data in InventoryFormDatas.Where(d=> d.Form.FormStateValue == (int)FormState.Completed)) {
				dice = randomGenerator.Next(100);
				if (dice < 2) {
					query = new Query {
						Id = queryId++,
						QueryText = "This Batch Number is registered as Unshipped, please check",
						QueryAuthor = GetClinicSupervisor(data.Form.Visit.Patient),
						QueryTime = GetRandomQueryTime(data),
						AnswerAuthor = null,
						AnswerText = null,
						AnswerTime = null,
						Question = data.BatchNumber
					};

					dice = randomGenerator.Next(100);
					if (dice > 70) {
						query.AnswerAuthor = data.Form.Visit.Patient.Doctor;
						query.AnswerText = "Marked as Shipped";
						query.AnswerTime = query.QueryTime.AddHours(randomGenerator.Next(5) + 1);
					}
					if (dice > 60) {
						query.AnswerAuthor = data.Form.Visit.Patient.Doctor;
						query.AnswerText = "Batch Number was corrected";
						query.AnswerTime = query.QueryTime.AddHours(randomGenerator.Next(5) + 1);
					}
					list.Add(query);
				}
			}
		}

		private void AddDemographicQueries(List<Query> list) {
			Query query;
			int dice;
			foreach (var data in DemographicFormDatas) {
				dice = randomGenerator.Next(100);
				if (data.Race.Value == ((int)Race.Other).ToString() && (dice < 80)) {
					query = new Query {
						Id = queryId++,
						QueryText = "Please specify the Patient's Race in 'Other' field",
						QueryAuthor = GetClinicSupervisor(data.Form.Visit.Patient),
						QueryTime = GetRandomQueryTime(data),
						AnswerAuthor = null,
						AnswerText = null,
						AnswerTime = null,
						Question = data.Race
					};

					dice = randomGenerator.Next(100);
					if (dice > 80) {
						query.AnswerAuthor = data.Form.Visit.Patient.Doctor;
						query.AnswerText = "We will contact patient to confirm race";
						query.AnswerTime = query.QueryTime.AddHours(randomGenerator.Next(5) + 1);
					}
					list.Add(query);
				}
			}
		}

		private void AddHappinessChangeQueries(List<Query> list) {
			Query query;
			var happinessFormPairs = from day1Happiness in HappinessFormDatas
			                         from day10Happiness in HappinessFormDatas
			                         where day1Happiness.Form.Visit.VisitTypeValue == (int) VisitType.Day1
			                               && day10Happiness.Form.Visit.VisitTypeValue == (int) VisitType.Day10
										   && day1Happiness.Form.FormStateValue == (int)FormState.Completed
										   && day10Happiness.Form.FormStateValue == (int)FormState.Completed
										   && day1Happiness.Form.Visit.Patient.Id == day10Happiness.Form.Visit.Patient.Id
			                         select new {OriginalHappinessData = day1Happiness, ChangedHappinessData = day10Happiness};

			foreach (var happinessFormPair in happinessFormPairs) {
				int originalLevel;
				if(!int.TryParse(happinessFormPair.OriginalHappinessData.HappinessLevel.Value, out originalLevel))
					continue;
				int changedLevel;
				if(!int.TryParse(happinessFormPair.ChangedHappinessData.HappinessLevel.Value, out changedLevel))
					continue;
				
				if(changedLevel < originalLevel) {
					query = new Query {
						Id = queryId++,
						QueryText = "Has patient explained this drop of happiness level?",
						QueryAuthor = GetClinicSupervisor(happinessFormPair.ChangedHappinessData.Form.Visit.Patient),
						QueryTime = GetRandomQueryTime(happinessFormPair.ChangedHappinessData),
						AnswerAuthor = null,
						AnswerText = null,
						AnswerTime = null,
						Question = happinessFormPair.ChangedHappinessData.HappinessLevel
					};
					list.Add(query);
				}

				if(originalLevel == 0) {
					query = new Query {
						Id = queryId++,
						QueryText = "Has patient given the reason of so unhappy state?",
						QueryAuthor = GetClinicSupervisor(happinessFormPair.OriginalHappinessData.Form.Visit.Patient),
						QueryTime = GetRandomQueryTime(happinessFormPair.OriginalHappinessData),
						AnswerAuthor = null,
						AnswerText = null,
						AnswerTime = null,
						Question = happinessFormPair.OriginalHappinessData.HappinessLevel
					};
					list.Add(query);
				}
			}
		}

		private void AddTemparatureQueries(List<Query> list) {
			Query query;
			foreach (var formData in VitalsFormDatas) {
				if (formData.Temperature.Value != null) {
					double temperature = double.Parse(formData.Temperature.Value);
					int dice = randomGenerator.Next(100);
					if (temperature < 36.6 && dice < 90) {
						query = new Query {
							Id = queryId++,
							QueryText = "Please specify the reason for reduced body temperature",
							QueryAuthor = GetClinicSupervisor(formData.Form.Visit.Patient),
							QueryTime = GetRandomQueryTime(formData),
							AnswerAuthor = null,
							AnswerText = null,
							AnswerTime = null,
							Question = formData.Temperature
						};

						dice = randomGenerator.Next(100);
						if (dice > 60) {
							query.AnswerAuthor = formData.Form.Visit.Patient.Doctor;
                            query.AnswerText = "It is the patient's regular body temperature";
							query.AnswerTime = query.QueryTime.AddHours(randomGenerator.Next(5) + 1);
						}
						list.Add(query);
					}
					if (temperature > 36.6 && dice < 80) {
						query = new Query {
							Id = queryId++,
							QueryText = "Please specify the reason for high body temperature",
							QueryAuthor = GetClinicSupervisor(formData.Form.Visit.Patient),
							QueryTime = GetRandomQueryTime(formData),
							AnswerAuthor = null,
							AnswerText = null,
							AnswerTime = null,
							Question = formData.Temperature
						};

						dice = randomGenerator.Next(100);
						if (dice > 60) {
							query.AnswerAuthor = formData.Form.Visit.Patient.Doctor;
                            query.AnswerText = "Patient had flu-like symptoms occurr a day before the visit";
							query.AnswerTime = query.QueryTime.AddHours(randomGenerator.Next(5) + 1);
						}
						list.Add(query);
					}
				}
			}
		}

		private DateTime GetRandomQueryTime(BaseFormData baseFormData) {
			var time = baseFormData.Form.Visit.VisitDate ?? DateTime.Now.AddDays(-2);

			time = time.Date.AddDays(randomGenerator.Next(3) + 1);
			time = time.AddHours(randomGenerator.Next(8) + 8);
			time = time.AddMinutes(randomGenerator.Next(60));
			return time;
		}


		private User GetClinicSupervisor(Clinic clinic) {
			return clinic.Doctors.FirstOrDefault(u => u.Role == ClinicalStudyRoles.Supervisor);
		}
		private User GetClinicSupervisor(Patient patient) {
			return GetClinicSupervisor(patient.Doctor.Clinic);
		}

		private void AssociateQueryWithQuestion(Query query, int patientNumber, VisitType visitType, FormType formType,
		                                        Func<int, Question> questionFinder) {
			var patient = Patients.First(p => p.PatientNumber == patientNumber);
			var visit = patient.Visits.First(v => v.VisitType == visitType);
			var form = visit.Forms.First(f => f.FormType == formType);
			query.Question = questionFinder(form.Id);
		}
	}
}
