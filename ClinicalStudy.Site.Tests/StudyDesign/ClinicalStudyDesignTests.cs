using System;
using System.Collections.Generic;
using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.ModelBuilders;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.Interface.FormData;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Site.Tests.StudyDesign {
	[TestFixture]
	public class ClinicalStudyDesignTests {
		private ClinicalStudyDesign studyDesign;
		private Mock<IClinicRepository> clinicRepository;
		private Mock<IUserRepository> userRepository;
		private Mock<IPatientRepository> patientRepository;
		private Mock<IVisitRepository> visitRepository;
		private Mock<IFormRepository> formRepository;
		private Mock<IQuestionRepository> questionRepository;
		private Mock<IVitalsFormDataRepository> vitalsFormDataRepository;
		private Mock<IDemographicFormDataRepository> demogFormDataRepository;
		private Mock<IElectrocardiogramFormDataRepository> electroFormDataRepository;
		private Mock<IHappinessFormDataRepository> happinessFormDataRepository;
		private Mock<IInventoryFormDataRepository> inventoryFormDataRepository;
		private Mock<IAdverseEventFormDataRepository> aeFormDataRepository;

		[SetUp]
		public void Setup() {
			studyDesign = new ClinicalStudyDesign();
			clinicRepository = new Mock<IClinicRepository>();
			userRepository = new Mock<IUserRepository>();
			patientRepository = new Mock<IPatientRepository>();
			visitRepository = new Mock<IVisitRepository>();
			formRepository = new Mock<IFormRepository>();
			questionRepository = new Mock<IQuestionRepository>();
			vitalsFormDataRepository = new Mock<IVitalsFormDataRepository>();
			demogFormDataRepository = new Mock<IDemographicFormDataRepository>();
			electroFormDataRepository = new Mock<IElectrocardiogramFormDataRepository>();
			happinessFormDataRepository = new Mock<IHappinessFormDataRepository>();
			inventoryFormDataRepository = new Mock<IInventoryFormDataRepository>();
			aeFormDataRepository = new Mock<IAdverseEventFormDataRepository>();

			studyDesign.ClinicRepository = clinicRepository.Object;
			studyDesign.UserRepository = userRepository.Object;
			studyDesign.PatientRepository = patientRepository.Object;
			studyDesign.VisitRepository = visitRepository.Object;
			studyDesign.FormRepository = formRepository.Object;
			studyDesign.QuestionRepository = questionRepository.Object;
			studyDesign.DemographicFormDataRepository = demogFormDataRepository.Object;
			studyDesign.ElectrocardiogramFormDataRepository = electroFormDataRepository.Object;
			studyDesign.VitalsFormDataRepository = vitalsFormDataRepository.Object;
			studyDesign.HappinessFormDataRepository = happinessFormDataRepository.Object;
			studyDesign.InventoryFormDataRepository = inventoryFormDataRepository.Object;
			studyDesign.AdverseEventFormDataRepository = aeFormDataRepository.Object;
		}

		[Test]
		public void CreatePatientForDoctor() {
			//Arrange
			const string doctorName = "doctorName";
			var doctor = new User() {Id = 15, Role = ClinicalStudyRoles.Doctor, Patients = new List<Patient>()};
			userRepository.Setup(r => r.GetUserByLogin(doctorName)).Returns(doctor);
			//Act
			var result = studyDesign.CreatePatientForDoctor(doctorName);
			//Assert
			userRepository.Verify(r => r.GetUserByLogin(doctorName), Times.Once());
		}


		[Test]
		public void CreatePatient_PatientVerification() {
			//Arrange
			var doctor = new User() {Id = 15, Role = ClinicalStudyRoles.Doctor, Patients = new List<Patient>()};
			//Act
			var result = studyDesign.CreatePatient(doctor);
			//Assert
			Assert.That(result, Is.Not.Null, "Patient was created");
			Assert.That(result.Doctor, Is.Not.Null);
			Assert.That(result.Doctor.Id, Is.EqualTo(doctor.Id));
			Assert.That(doctor.Patients.Exists(p => p.Id == result.Id), "Clinic patients list does not include created patient");
		}


		[Test]
		public void CreatePatient_PatientVisitsVerification() {
			//Arrange
			var doctor = new User() {Id = 15, Role = ClinicalStudyRoles.Doctor, Patients = new List<Patient>()};
			//Act
			var result = studyDesign.CreatePatient(doctor);
			//Assert
			Assert.That(result, Is.Not.Null, "Patient was created");
			Assert.That(result.Visits, Is.Not.Null, "Visits were not created");
			Assert.That(result.Visits.Count, Is.EqualTo(3));
			Assert.That(result.Visits.Any(v => v.VisitType == VisitType.Baseline), "Baseline visit was not scheduled");
			Assert.That(result.Visits.Any(v => v.VisitType == VisitType.Day1), "1st Day visist was not scheduled");
			Assert.That(result.Visits.Any(v => v.VisitType == VisitType.Day10), "10th Day visist was not scheduled");
		}


		[Test]
		public void CreatePatient_BaselineVisitVerification() {
			//Arrange
			var doctor = new User() {Id = 15, Role = ClinicalStudyRoles.Doctor, Patients = new List<Patient>()};
			//Act
			var result = studyDesign.CreatePatient(doctor);
			//Assert
			Assert.That(result, Is.Not.Null, "Patient was created");
			Assert.That(result.Visits, Is.Not.Null, "Visits were not created");
			var visit = result.Visits.FirstOrDefault(v => v.VisitType == VisitType.Baseline);
			Assert.That(visit, Is.Not.Null, "Baseline visit was not scheduled");

			Assert.That(visit.Caption, Is.EqualTo("Baseline"));
			Assert.That(visit.ExpectedVisitDate, Is.EqualTo(DateTime.Now.Date));
			Assert.That(visit.OrderNo, Is.EqualTo(0));
			Assert.That(visit.Patient, Is.Not.Null);
			Assert.That(visit.VisitDate, Is.Null);
			Assert.That(visit.VisitTime, Is.Null);
			Assert.That(visit.Forms, Is.Not.Null);
			visitRepository.Verify(r => r.Add(visit), Times.Once());
		}

		[Test]
		public void CreatePatient_BaselineVisit_FormsVerification() {
			//Arrange
			var doctor = new User() {Id = 15, Role = ClinicalStudyRoles.Doctor, Patients = new List<Patient>()};
			var vitalsFormDatas = new List<VitalsFormData>();
			var demogFormDatas = new List<DemographicFormData>();
			vitalsFormDataRepository
				.Setup(r => r.Add(It.IsAny<VitalsFormData>()))
				.Callback<VitalsFormData>(data => vitalsFormDatas.Add(data));
			demogFormDataRepository
				.Setup(r => r.Add(It.IsAny<DemographicFormData>()))
				.Callback<DemographicFormData>(data => demogFormDatas.Add(data));


			//Act
			var result = studyDesign.CreatePatient(doctor);
			//Assert
			Assert.That(result, Is.Not.Null, "Patient was created");
			Assert.That(result.Visits, Is.Not.Null, "Visits were not created");
			var visit = result.Visits.FirstOrDefault(v => v.VisitType == VisitType.Baseline);
			Assert.That(visit, Is.Not.Null, "Baseline visit was not scheduled");
			Assert.That(visit.Forms, Is.Not.Null, "Forms were not created");

			//demographics CRF
			var demographicsForm = visit.Forms.FirstOrDefault(f => f.FormType == FormType.Demographics);
			Assert.That(demographicsForm, Is.Not.Null);
			Assert.That(demographicsForm.Caption, Is.EqualTo("Demographics"));
			Assert.That(demographicsForm.FormState, Is.EqualTo(FormState.Incomplete));
			Assert.That(demographicsForm.FormType, Is.EqualTo(FormType.Demographics));
			Assert.That(demographicsForm.OrderNo, Is.EqualTo(0));
			formRepository.Verify(r => r.Add(demographicsForm), Times.Once());

			var demogFormData = demogFormDatas.FirstOrDefault(data => data.Form == demographicsForm);
			Assert.That(demogFormData, Is.Not.Null);
			Assert.That(demogFormData.DateOfBirth, Is.Not.Null);
			Assert.That(demogFormData.Race, Is.Not.Null);
			Assert.That(demogFormData.Sex, Is.Not.Null);
			Assert.That(demogFormData.Other, Is.Not.Null);


			//vitals CRF
			var vitalsForm = visit.Forms.FirstOrDefault(f => f.FormType == FormType.Vitals);
			Assert.That(vitalsForm, Is.Not.Null);
			Assert.That(vitalsForm.Caption, Is.EqualTo("Vitals"));
			Assert.That(vitalsForm.FormState, Is.EqualTo(FormState.Incomplete));
			Assert.That(vitalsForm.FormType, Is.EqualTo(FormType.Vitals));
			Assert.That(vitalsForm.OrderNo, Is.EqualTo(1));
			formRepository.Verify(r => r.Add(vitalsForm), Times.Once());

			var vitalsFormData = vitalsFormDatas.FirstOrDefault(data => data.Form == vitalsForm);
			Assert.That(vitalsFormData, Is.Not.Null);
			Assert.That(vitalsFormData.ActualTime, Is.Not.Null);
			Assert.That(vitalsFormData.BloodPressureDiastolic, Is.Not.Null);
			Assert.That(vitalsFormData.BloodPressureSystolic, Is.Not.Null);
			Assert.That(vitalsFormData.HeartRate, Is.Not.Null);
			Assert.That(vitalsFormData.Height, Is.Not.Null);
			Assert.That(vitalsFormData.Weight, Is.Not.Null);
			Assert.That(vitalsFormData.Temperature, Is.Not.Null);
		}


		[Test]
		public void CreatePatient_Day1VisitVerification() {
			//Arrange
			var doctor = new User() {Id = 15, Role = ClinicalStudyRoles.Doctor, Patients = new List<Patient>()};
			//Act
			var result = studyDesign.CreatePatient(doctor);
			//Assert
			Assert.That(result, Is.Not.Null, "Patient was created");
			Assert.That(result.Visits, Is.Not.Null, "Visits were not created");
			var visit = result.Visits.FirstOrDefault(v => v.VisitType == VisitType.Day1);
			Assert.That(visit, Is.Not.Null, "1st Day visit was not scheduled");

			Assert.That(visit.Caption, Is.EqualTo("1st Day"));
			Assert.That(visit.ExpectedVisitDate, Is.EqualTo(DateTime.Now.Date.AddDays(2)));
			Assert.That(visit.OrderNo, Is.EqualTo(1));
			Assert.That(visit.Patient, Is.Not.Null);
			Assert.That(visit.VisitDate, Is.Null);
			Assert.That(visit.VisitTime, Is.Null);
			Assert.That(visit.Forms, Is.Not.Null);
			visitRepository.Verify(r => r.Add(visit), Times.Once());
		}

		[Test]
		public void CreatePatient_1stDayVisit_FormsVerification() {
			//Arrange
			var doctor = new User() {Id = 15, Role = ClinicalStudyRoles.Doctor, Patients = new List<Patient>()};
			var vitalsFormDatas = new List<VitalsFormData>();
			var happinessFormDatas = new List<HappinessFormData>();
			var electrocardiogramFormDatas = new List<ElectrocardiogramFormData>();
			vitalsFormDataRepository
				.Setup(r => r.Add(It.IsAny<VitalsFormData>()))
				.Callback<VitalsFormData>(data => vitalsFormDatas.Add(data));
			happinessFormDataRepository
				.Setup(r => r.Add(It.IsAny<HappinessFormData>()))
				.Callback<HappinessFormData>(data => happinessFormDatas.Add(data));
			electroFormDataRepository
				.Setup(r => r.Add(It.IsAny<ElectrocardiogramFormData>()))
				.Callback<ElectrocardiogramFormData>(data => electrocardiogramFormDatas.Add(data));


			//Act
			var result = studyDesign.CreatePatient(doctor);
			//Assert
			Assert.That(result, Is.Not.Null, "Patient was created");
			Assert.That(result.Visits, Is.Not.Null, "Visits were not created");
			var visit = result.Visits.FirstOrDefault(v => v.VisitType == VisitType.Day1);
			Assert.That(visit, Is.Not.Null, "Baseline visit was not scheduled");
			Assert.That(visit.Forms, Is.Not.Null, "Forms were not created");

			//happiness CRF
			var happinessForm = visit.Forms.FirstOrDefault(f => f.FormType == FormType.Happiness);
			Assert.That(happinessForm, Is.Not.Null);
			Assert.That(happinessForm.Caption, Is.EqualTo("Happiness"));
			Assert.That(happinessForm.FormState, Is.EqualTo(FormState.Incomplete));
			Assert.That(happinessForm.FormType, Is.EqualTo(FormType.Happiness));
			Assert.That(happinessForm.OrderNo, Is.EqualTo(0));
			formRepository.Verify(r => r.Add(happinessForm), Times.Once());

			var happinessFormData = happinessFormDatas.FirstOrDefault(data => data.Form == happinessForm);
			Assert.That(happinessFormData, Is.Not.Null);
			Assert.That(happinessFormData.HappinessLevel, Is.Not.Null);


			//electrocardiogram CRF
			var electroForm = visit.Forms.FirstOrDefault(f => f.FormType == FormType.Electrocardiogram);
			Assert.That(electroForm, Is.Not.Null);
			Assert.That(electroForm.Caption, Is.EqualTo("Electrocardiogram"));
			Assert.That(electroForm.FormState, Is.EqualTo(FormState.Incomplete));
			Assert.That(electroForm.FormType, Is.EqualTo(FormType.Electrocardiogram));
			Assert.That(electroForm.OrderNo, Is.EqualTo(1));
			formRepository.Verify(r => r.Add(electroForm), Times.Once());

			var electrocardiogramFormData = electrocardiogramFormDatas.FirstOrDefault(data => data.Form == electroForm);
			Assert.That(electrocardiogramFormData, Is.Not.Null);
			Assert.That(electrocardiogramFormData.ElectrocardiogramActualTime, Is.Not.Null);
			Assert.That(electrocardiogramFormData.ElectrocardiogramAttachment, Is.Not.Null);


			//vitals CRF
			var vitalsForm = visit.Forms.FirstOrDefault(f => f.FormType == FormType.Vitals);
			Assert.That(vitalsForm, Is.Not.Null);
			Assert.That(vitalsForm.Caption, Is.EqualTo("Vitals"));
			Assert.That(vitalsForm.FormState, Is.EqualTo(FormState.Incomplete));
			Assert.That(vitalsForm.FormType, Is.EqualTo(FormType.Vitals));
			Assert.That(vitalsForm.OrderNo, Is.EqualTo(2));
			formRepository.Verify(r => r.Add(vitalsForm), Times.Once());

			var vitalsFormData = vitalsFormDatas.FirstOrDefault(data => data.Form == vitalsForm);
			Assert.That(vitalsFormData, Is.Not.Null);
			Assert.That(vitalsFormData.ActualTime, Is.Not.Null);
			Assert.That(vitalsFormData.BloodPressureDiastolic, Is.Not.Null);
			Assert.That(vitalsFormData.BloodPressureSystolic, Is.Not.Null);
			Assert.That(vitalsFormData.HeartRate, Is.Not.Null);
			Assert.That(vitalsFormData.Height, Is.Not.Null);
			Assert.That(vitalsFormData.Weight, Is.Not.Null);
			Assert.That(vitalsFormData.Temperature, Is.Not.Null);
		}


		[Test]
		public void CreatePatient_Day10VisitVerification() {
			//Arrange
			var doctor = new User() {Id = 15, Role = ClinicalStudyRoles.Doctor, Patients = new List<Patient>()};
			//Act
			var result = studyDesign.CreatePatient(doctor);
			//Assert
			Assert.That(result, Is.Not.Null, "Patient was created");
			Assert.That(result.Visits, Is.Not.Null, "Visits were not created");
			var visit = result.Visits.FirstOrDefault(v => v.VisitType == VisitType.Day10);
			Assert.That(visit, Is.Not.Null, "10th Day visit was not scheduled");

			Assert.That(visit.Caption, Is.EqualTo("10th Day"));
			Assert.That(visit.ExpectedVisitDate, Is.EqualTo(DateTime.Now.Date.AddDays(12)));
			Assert.That(visit.OrderNo, Is.EqualTo(2));
			Assert.That(visit.Patient, Is.Not.Null);
			Assert.That(visit.VisitDate, Is.Null);
			Assert.That(visit.VisitTime, Is.Null);
			Assert.That(visit.Forms, Is.Not.Null);
			visitRepository.Verify(r => r.Add(visit), Times.Once());
		}

		[Test]
		public void CreatePatient_10thDayVisit_FormsVerification() {
			//Arrange
			var doctor = new User() {Id = 15, Role = ClinicalStudyRoles.Doctor, Patients = new List<Patient>()};
			var vitalsFormDatas = new List<VitalsFormData>();
			var happinessFormDatas = new List<HappinessFormData>();
			var inventoryFormDatas = new List<InventoryFormData>();
			vitalsFormDataRepository
				.Setup(r => r.Add(It.IsAny<VitalsFormData>()))
				.Callback<VitalsFormData>(data => vitalsFormDatas.Add(data));
			happinessFormDataRepository
				.Setup(r => r.Add(It.IsAny<HappinessFormData>()))
				.Callback<HappinessFormData>(data => happinessFormDatas.Add(data));
			inventoryFormDataRepository
				.Setup(r => r.Add(It.IsAny<InventoryFormData>()))
				.Callback<InventoryFormData>(data => inventoryFormDatas.Add(data));


			//Act
			var result = studyDesign.CreatePatient(doctor);
			//Assert
			Assert.That(result, Is.Not.Null, "Patient was created");
			Assert.That(result.Visits, Is.Not.Null, "Visits were not created");
			var visit = result.Visits.FirstOrDefault(v => v.VisitType == VisitType.Day10);
			Assert.That(visit, Is.Not.Null, "Baseline visit was not scheduled");
			Assert.That(visit.Forms, Is.Not.Null, "Forms were not created");

			//happiness CRF
			var happinessForm = visit.Forms.FirstOrDefault(f => f.FormType == FormType.Happiness);
			Assert.That(happinessForm, Is.Not.Null);
			Assert.That(happinessForm.Caption, Is.EqualTo("Happiness"));
			Assert.That(happinessForm.FormState, Is.EqualTo(FormState.Incomplete));
			Assert.That(happinessForm.FormType, Is.EqualTo(FormType.Happiness));
			Assert.That(happinessForm.OrderNo, Is.EqualTo(0));
			formRepository.Verify(r => r.Add(happinessForm), Times.Once());

			var happinessFormData = happinessFormDatas.FirstOrDefault(data => data.Form == happinessForm);
			Assert.That(happinessFormData, Is.Not.Null);
			Assert.That(happinessFormData.HappinessLevel, Is.Not.Null);


			//electrocardiogram CRF
			var inventoryForm = visit.Forms.FirstOrDefault(f => f.FormType == FormType.Inventory);
			Assert.That(inventoryForm, Is.Not.Null);
			Assert.That(inventoryForm.Caption, Is.EqualTo("Inventory"));
			Assert.That(inventoryForm.FormState, Is.EqualTo(FormState.Incomplete));
			Assert.That(inventoryForm.FormType, Is.EqualTo(FormType.Inventory));
			Assert.That(inventoryForm.OrderNo, Is.EqualTo(1));
			formRepository.Verify(r => r.Add(inventoryForm), Times.Once());

			var inventoryFormData = inventoryFormDatas.FirstOrDefault(data => data.Form == inventoryForm);
			Assert.That(inventoryFormData, Is.Not.Null);
			Assert.That(inventoryFormData.BatchNumber, Is.Not.Null);
			Assert.That(inventoryFormData.QuantityShipped, Is.Not.Null);
			Assert.That(inventoryFormData.ReceiptDate, Is.Not.Null);
			Assert.That(inventoryFormData.ShipDate, Is.Not.Null);
			Assert.That(inventoryFormData.MedicationUsage, Is.Not.Null);


			//vitals CRF
			var vitalsForm = visit.Forms.FirstOrDefault(f => f.FormType == FormType.Vitals);
			Assert.That(vitalsForm, Is.Not.Null);
			Assert.That(vitalsForm.Caption, Is.EqualTo("Vitals"));
			Assert.That(vitalsForm.FormState, Is.EqualTo(FormState.Incomplete));
			Assert.That(vitalsForm.FormType, Is.EqualTo(FormType.Vitals));
			Assert.That(vitalsForm.OrderNo, Is.EqualTo(2));
			formRepository.Verify(r => r.Add(vitalsForm), Times.Once());

			var vitalsFormData = vitalsFormDatas.FirstOrDefault(data => data.Form == vitalsForm);
			Assert.That(vitalsFormData, Is.Not.Null);
			Assert.That(vitalsFormData.ActualTime, Is.Not.Null);
			Assert.That(vitalsFormData.BloodPressureDiastolic, Is.Not.Null);
			Assert.That(vitalsFormData.BloodPressureSystolic, Is.Not.Null);
			Assert.That(vitalsFormData.HeartRate, Is.Not.Null);
			Assert.That(vitalsFormData.Height, Is.Not.Null);
			Assert.That(vitalsFormData.Weight, Is.Not.Null);
			Assert.That(vitalsFormData.Temperature, Is.Not.Null);
		}

		#region CreateAdverseEvent tests

		[Test]
		public void CreateFirstAdverseEvent_VisitVerification() {
			//Arrange
			const int testPatientId = 3;


			var patient = new Patient() {Id = testPatientId, Visits = new List<Visit>()};
			patientRepository.Setup(r => r.GetByKey(testPatientId)).Returns(patient);
			visitRepository.Setup(r => r.GetVisitsForPatient(testPatientId)).Returns(new List<Visit>() {
				new Visit() {
					Id = 15,
					Caption = "Baseline",
					OrderNo = 1
				}
			});

			//as checking of saved visit is quite complicated, we will save the passed object and inspect it later
			Visit savedVisit = null;
			visitRepository.Setup(r => r.Add(It.IsAny<Visit>())).Callback<Visit>(v => savedVisit = v);
			//Act
			var result = studyDesign.AddAdverseEventVisit(testPatientId);


			//Assert
			patientRepository.Verify(r => r.GetByKey(testPatientId), Times.Once());
			visitRepository.Verify(r => r.Save(), Times.Once());
			visitRepository.Verify(r => r.Add(It.IsAny<Visit>()), Times.Once());
			Assert.That(savedVisit, Is.Not.Null, "Visit was not passed");
			Assert.That(savedVisit.OrderNo, Is.EqualTo(2));
			Assert.That(savedVisit.Caption, Is.EqualTo("Adverse Event"));
			Assert.That(savedVisit.ExpectedVisitDate, Is.Null);
			Assert.That(savedVisit.VisitType, Is.EqualTo(VisitType.AdverseEventVisit));

			Assert.That(savedVisit.Patient, Is.Not.Null);
			Assert.That(savedVisit.Patient.Id, Is.EqualTo(patient.Id));
			Assert.That(patient.Visits.Contains(savedVisit));

			Assert.That(savedVisit.Id, Is.EqualTo(result.Id));
		}

		[Test]
		public void CreateSecondAdverseEvent_VisitVerification() {
			//Arrange
			const int testPatientId = 3;

			var patient = new Patient() {Id = testPatientId};
			var visits = new List<Visit>() {
				new Visit() {
					Id = 15,
					Caption = "Baseline",
					OrderNo = 1,
					VisitType = VisitType.Baseline,
					Patient = patient
				},
				new Visit() {
					Id = 16,
					Caption = "AE - pain in leg",
					OrderNo = 2,
					VisitType = VisitType.AdverseEventVisit,
					Patient = patient
				}
			};
			patient.Visits = visits;
			patientRepository.Setup(r => r.GetByKey(testPatientId)).Returns(patient);
			visitRepository
				.Setup(r => r.GetVisitsForPatient(testPatientId))
				.Returns(visits);

			//as checking of saved visit is quite complicated, we will save the passed object and inspect it later
			Visit savedVisit = null;
			visitRepository.Setup(r => r.Add(It.IsAny<Visit>())).Callback<Visit>(v => savedVisit = v);
			//Act
			var result = studyDesign.AddAdverseEventVisit(testPatientId);


			//Assert
			patientRepository.Verify(r => r.GetByKey(testPatientId), Times.Once());
			visitRepository.Verify(r => r.Save(), Times.Once());
			visitRepository.Verify(r => r.Add(It.IsAny<Visit>()), Times.Once());
			Assert.That(savedVisit, Is.Not.Null, "Visit was not passed");
			Assert.That(savedVisit.OrderNo, Is.EqualTo(3));
			Assert.That(savedVisit.Caption, Is.EqualTo("Adverse Event 2"));
			Assert.That(savedVisit.ExpectedVisitDate, Is.Null);
			Assert.That(savedVisit.VisitType, Is.EqualTo(VisitType.AdverseEventVisit));

			Assert.That(savedVisit.Patient, Is.Not.Null);
			Assert.That(savedVisit.Patient.Id, Is.EqualTo(patient.Id));
			Assert.That(patient.Visits.Contains(savedVisit));

			Assert.That(savedVisit.Id, Is.EqualTo(result.Id));
		}


		[Test]
		public void CreateAdverseEvent_FormVerification() {
			//Arrange
			const int testPatientId = 3;

			var patient = new Patient() {Id = testPatientId, Visits = new List<Visit>()};
			patientRepository.Setup(r => r.GetByKey(testPatientId)).Returns(patient);
			visitRepository.Setup(r => r.GetVisitsForPatient(testPatientId)).Returns(new List<Visit>() {
				new Visit() {
					Id = 15,
					Caption = "Baseline",
					OrderNo = 1
				}
			});

			//as checking of saved visit is quite complicated, we will save the passed object and inspect it later
			Visit savedVisit = null;
			Form savedForm = null;

			visitRepository.Setup(r => r.Add(It.IsAny<Visit>())).Callback<Visit>(v => savedVisit = v);
			formRepository.Setup(r => r.Add(It.IsAny<Form>())).Callback<Form>(f => savedForm = f);

			//Act
			var result = studyDesign.AddAdverseEventVisit(testPatientId);


			//Assert
			patientRepository.Verify(r => r.GetByKey(testPatientId), Times.Once());
			visitRepository.Verify(r => r.Save(), Times.Once());
			visitRepository.Verify(r => r.Add(It.IsAny<Visit>()), Times.Once());
			formRepository.Verify(r => r.Add(It.IsAny<Form>()), Times.Once());
			Assert.That(savedVisit, Is.Not.Null, "Visit was not passed");
			Assert.That(savedForm, Is.Not.Null, "Form was not passed");

			Assert.That(savedForm.Visit, Is.EqualTo(savedVisit));
			Assert.That(savedVisit.Forms.Contains(savedForm));

			Assert.That(savedForm.Caption, Is.EqualTo("Adverse Event"));
			Assert.That(savedForm.FormState, Is.EqualTo(FormState.Incomplete));
			Assert.That(savedForm.FormType, Is.EqualTo(FormType.AdverseEvent));
			Assert.That(savedForm.OrderNo, Is.EqualTo(0));
		}

		[Test]
		public void CreateAdverseEvent_FormDataVerification() {
			//Arrange
			const int testPatientId = 3;

			var patient = new Patient() {Id = testPatientId, Visits = new List<Visit>()};
			patientRepository.Setup(r => r.GetByKey(testPatientId)).Returns(patient);
			visitRepository.Setup(r => r.GetVisitsForPatient(testPatientId)).Returns(new List<Visit>() {
				new Visit() {
					Id = 15,
					Caption = "Baseline",
					OrderNo = 1
				}
			});

			//as checking of saved visit is quite complicated, we will save the passed object and inspect it later
			Form savedForm = null;
			AdverseEventFormData savedFormData = null;

			formRepository.Setup(r => r.Add(It.IsAny<Form>())).Callback<Form>(f => savedForm = f);
			aeFormDataRepository.Setup(r => r.Add(It.IsAny<AdverseEventFormData>())).Callback<AdverseEventFormData>(
				f => savedFormData = f);

			//Act
			var result = studyDesign.AddAdverseEventVisit(testPatientId);


			//Assert
			patientRepository.Verify(r => r.GetByKey(testPatientId), Times.Once());
			visitRepository.Verify(r => r.Save(), Times.Once());

			formRepository.Verify(r => r.Add(It.IsAny<Form>()), Times.Once());
			aeFormDataRepository.Verify(r => r.Add(It.IsAny<AdverseEventFormData>()), Times.Once());

			Assert.That(savedForm, Is.Not.Null, "Form was not passed");
			Assert.That(savedFormData, Is.Not.Null, "Form Data was not passed");

			Assert.That(savedFormData.Form, Is.EqualTo(savedForm));

			Assert.That(savedFormData.AdverseExperience, Is.Not.Null);
			Assert.That(savedFormData.OnsetDate, Is.Not.Null);
			Assert.That(savedFormData.OnsetTime, Is.Not.Null);
			Assert.That(savedFormData.EndDate, Is.Not.Null);
			Assert.That(savedFormData.EndTime, Is.Not.Null);
			Assert.That(savedFormData.Intensity, Is.Not.Null);
			Assert.That(savedFormData.Outcome, Is.Not.Null);
			Assert.That(savedFormData.RelationshipToInvestigationalDrug, Is.Not.Null);
		}

		#endregion CreateAdverseEvent tests
	}
}
