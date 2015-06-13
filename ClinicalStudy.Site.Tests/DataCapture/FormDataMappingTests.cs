using System;
using System.Collections.Generic;
using System.Globalization;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.DataCapture.Controllers;
using ClinicalStudy.Site.Areas.DataCapture.Models.FormData;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Site.Tests.DataCapture {
	[TestFixture]
	public class FormDataMappingTests {
		private FormController formController;
		private Mock<IAttachmentRepository> attachmentRep;

		[SetUp]
		public void TestSetup() {
			attachmentRep = new Mock<IAttachmentRepository>();
			formController = new FormController(null, null, null, null, null, null, null, attachmentRep.Object, null);
		}

		[Test]
		public void DemographicFormDataToViewModel_FullData() {
			//Arrange 
			var dFormData = new DemographicFormData {
				Id = 1,
				Form =
					new Form {
						Id = 11,
						Caption = "Form11",
						FormState = FormState.Completed,
						Visit =
							new Visit {Caption = "Visit11", Patient = new Patient {PatientNumber = 110}}
					},
				DateOfBirth =
					new Question {
						Id = 3,
						DataType = QuestionDataType.Date,
						Value =
							new DateTime(1980, 1, 1).
								ToString(CultureInfo.InvariantCulture)
					},
				Other =
					new Question {
						Id = 4,
						DataType = QuestionDataType.String,
						Value = "Some description"
					},
				Race =
					new Question {Id = 5, DataType = QuestionDataType.Enum, Value = "1"},
				Sex =
					new Question {Id = 6, DataType = QuestionDataType.Enum, Value = "2"}
			};

			//Act
			var demog = formController.MapDemographicFormDataToViewModel(dFormData);

			//Assert
			Assert.That(demog.FormId, Is.EqualTo(11));
			Assert.That(demog.FormCaption, Is.EqualTo("Form11"));
			Assert.That(demog.IsCompleted, Is.True);
			Assert.That(demog.VisitName, Is.EqualTo("Visit11"));
			Assert.That(demog.PatientNumber, Is.EqualTo(110));
			Assert.That(demog.Id, Is.EqualTo(1));
			Assert.That(demog.DateOfBirth, Is.Not.Null);
			Assert.That(demog.DateOfBirth.Value.Date, Is.EqualTo(new DateTime(1980, 1, 1).Date));
			Assert.That(demog.Other, Is.EqualTo("Some description"));
			Assert.That(demog.Race, Is.EqualTo(1));
			Assert.That(demog.Sex, Is.EqualTo(2));

			Assert.That(demog.DateOfBirthQuestionId, Is.EqualTo(dFormData.DateOfBirth.Id));
			Assert.That(demog.OtherQuestionId, Is.EqualTo(dFormData.Other.Id));
			Assert.That(demog.SexQuestionId, Is.EqualTo(dFormData.Sex.Id));
			Assert.That(demog.RaceQuestionId, Is.EqualTo(dFormData.Race.Id));
		}

		[Test]
		public void DemographicViewModelToFormData_FullData() {
			//Arrange 
			var dViewModel = new DemographicFormViewModel {
				Id = 1,
				DateOfBirth = new DateTime(1978, 2, 3),
				Other = "Some description - updated",
				Race = 2,
				Sex = 3
			};
			var dFormData = CreateEmptyDemographicFormData();

			//Act
			formController.MapDemographicViewModelToFormData(dViewModel, dFormData);

			//Assert
			Assert.That(dFormData.DateOfBirth.Value, Is.EqualTo(new DateTime(1978, 2, 3).ToString(CultureInfo.InvariantCulture)));
			Assert.That(dFormData.Other.Value, Is.EqualTo("Some description - updated"));
			Assert.That(dFormData.Race.Value, Is.EqualTo("2"));
			Assert.That(dFormData.Sex.Value, Is.EqualTo("3"));
		}

		[Test]
		public void VitalsFormDataToViewModel_FullData() {
			//Arrange 
			var vFormData = new VitalsFormData {
				Id = 2,
				Form =
					new Form {
						Id = 12,
						Caption = "Form12",
						FormState = FormState.Completed,
						Visit =
							new Visit {Caption = "Visit12", Patient = new Patient {PatientNumber = 120}}
					},
				ActualTime =
					new Question {
						Id = 2,
						DataType = QuestionDataType.Date,
						Value =
							new DateTime(2012, 1, 1, 10, 11, 12).ToString(
								CultureInfo.InvariantCulture)
					},
				Height = new Question {Id = 3, DataType = QuestionDataType.Number, Value = "177"},
				Weight = new Question {Id = 4, DataType = QuestionDataType.Number, Value = "90"},
				Temperature =
					new Question {
						Id = 5,
						DataType = QuestionDataType.Number,
						Value = (36.6m).ToString(CultureInfo.InvariantCulture)
					},
				HeartRate = new Question {Id = 6, DataType = QuestionDataType.Integer, Value = "66"},
				BloodPressureSystolic =
					new Question {Id = 7, DataType = QuestionDataType.String, Value = "120"},
				BloodPressureDiastolic =
					new Question {Id = 8, DataType = QuestionDataType.String, Value = "70"}
			};
			//Act
			var vitals = formController.MapVitalsFormDataToViewModel(vFormData);

			//Assert
			Assert.That(vitals.FormId, Is.EqualTo(12));
			Assert.That(vitals.FormCaption, Is.EqualTo("Form12"));
			Assert.That(vitals.IsCompleted, Is.True);
			Assert.That(vitals.VisitName, Is.EqualTo("Visit12"));
			Assert.That(vitals.PatientNumber, Is.EqualTo(120));
			Assert.That(vitals.Id, Is.EqualTo(2));
			Assert.That(vitals.ActualTime, Is.Not.Null);
			Assert.That(vitals.ActualTime.Value.ToLongTimeString(),
			            Is.EqualTo(new DateTime(1980, 1, 1, 10, 11, 12).ToLongTimeString()));
			Assert.That(vitals.Height, Is.EqualTo(177));
			Assert.That(vitals.Weight, Is.EqualTo(90));
			Assert.That(vitals.Temperature, Is.EqualTo(36.6m));
			Assert.That(vitals.HeartRate, Is.EqualTo(66));
			Assert.That(vitals.BloodPressureSystolic, Is.EqualTo("120"));
			Assert.That(vitals.BloodPressureDiastolic, Is.EqualTo("70"));


			Assert.That(vitals.ActualTimeQuestionId, Is.EqualTo(vFormData.ActualTime.Id));
			Assert.That(vitals.HeightQuestionId, Is.EqualTo(vFormData.Height.Id));
			Assert.That(vitals.WeightQuestionId, Is.EqualTo(vFormData.Weight.Id));
			Assert.That(vitals.TemperatureQuestionId, Is.EqualTo(vFormData.Temperature.Id));
			Assert.That(vitals.HeartRateQuestionId, Is.EqualTo(vFormData.HeartRate.Id));
			Assert.That(vitals.BloodPressureSystolicQuestionId, Is.EqualTo(vFormData.BloodPressureSystolic.Id));
			Assert.That(vitals.BloodPressureDiastolicQuestionId, Is.EqualTo(vFormData.BloodPressureDiastolic.Id));
		}

		[Test]
		public void VitalsViewModelToFormData_FullData() {
			//Arrange
			var vViewModel = new VitalsFormViewModel {
				Id = 2,
				ActualTime = new DateTime(2011, 11, 12, 11, 12, 13),
				Height = 178,
				Weight = 91,
				Temperature = 37.6m,
				HeartRate = 67,
				BloodPressureSystolic = "125",
				BloodPressureDiastolic = "75"
			};
			var vFormData = CreateEmptyVitalsFormData();

			//Act
			formController.MapVitalsViewModelToFormData(vViewModel, vFormData);

			//Assert
			Assert.That(vFormData.ActualTime.Value,
			            Is.EqualTo(new DateTime(2011, 11, 12, 11, 12, 13).ToString(CultureInfo.InvariantCulture)));
			Assert.That(vFormData.Height.Value, Is.EqualTo("178"));
			Assert.That(vFormData.Weight.Value, Is.EqualTo("91"));
			Assert.That(vFormData.Temperature.Value, Is.EqualTo(37.6m.ToString(CultureInfo.InvariantCulture)));
			Assert.That(vFormData.HeartRate.Value, Is.EqualTo("67"));
			Assert.That(vFormData.BloodPressureSystolic.Value, Is.EqualTo("125"));
			Assert.That(vFormData.BloodPressureDiastolic.Value, Is.EqualTo("75"));
		}

		[Test]
		public void HappinessFormDataToViewModel_FullData() {
			//Arrange 
			var hFormData = new HappinessFormData {
				Id = 3,
				Form =
					new Form {
						Id = 13,
						Caption = "Form13",
						FormState = FormState.Completed,
						Visit =
							new Visit {Caption = "Visit13", Patient = new Patient {PatientNumber = 130}}
					},
				HappinessLevel = new Question {Id = 12, DataType = QuestionDataType.Integer, Value = "60"}
			};
			//Act
			var happiness = formController.MapHappinessFormDataToViewModel(hFormData);

			//Assert
			Assert.That(happiness.FormId, Is.EqualTo(13));
			Assert.That(happiness.FormCaption, Is.EqualTo("Form13"));
			Assert.That(happiness.IsCompleted, Is.True);
			Assert.That(happiness.VisitName, Is.EqualTo("Visit13"));
			Assert.That(happiness.PatientNumber, Is.EqualTo(130));
			Assert.That(happiness.Id, Is.EqualTo(3));
			Assert.That(happiness.HappinessLevel, Is.EqualTo(60));
			Assert.That(happiness.HappinessLevelQuestionId, Is.EqualTo(hFormData.HappinessLevel.Id));
		}

		[Test]
		public void HappinessViewModelToFormData_FullData() {
			//Arrange
			var hViewModel = new HappinessFormViewModel {
				Id = 3,
				HappinessLevel = 70
			};
			var hFormData = CreateEmptyHappinessFormData();

			//Act
			formController.MapHappinessViewModelToFormData(hViewModel, hFormData);

			//Assert
			Assert.That(hFormData.HappinessLevel.Value, Is.EqualTo("70"));
		}

		[Test]
		public void ElectrocardiogramFormDataToViewModel_FullData() {
			//Arrange
			var eFormData = new ElectrocardiogramFormData() {
				Id = 4,
				Form =
					new Form {
						Id = 14,
						Caption = "Form14",
						FormState = FormState.Completed,
						Visit =
							new Visit {
								Caption = "Visit14",
								Patient = new Patient {PatientNumber = 140}
							}
					},
				ElectrocardiogramActualTime =
					new Question {
						Id = 3,
						DataType = QuestionDataType.Date,
						Value =
							new DateTime(2012, 1, 1, 10, 11, 12).ToString(
								CultureInfo.InvariantCulture)
					},
				ElectrocardiogramAttachment = new Question() {
					DataType = QuestionDataType.Attachment,
					Id = 212,
					Value = null,
					File = new Attachment() {
						Id = 414,
						FileName = "test.pdf",
						MimeType = "application/pdf",
						FileSize = 4096
					}
				}
			};
			//Act
			var electrocardiogram = formController.MapElectrocardiogramFormDataToViewModel(eFormData);

			//Assert
			Assert.That(electrocardiogram.FormId, Is.EqualTo(14));
			Assert.That(electrocardiogram.FormCaption, Is.EqualTo("Form14"));
			Assert.That(electrocardiogram.IsCompleted, Is.True);
			Assert.That(electrocardiogram.VisitName, Is.EqualTo("Visit14"));
			Assert.That(electrocardiogram.PatientNumber, Is.EqualTo(140));
			Assert.That(electrocardiogram.Id, Is.EqualTo(4));
			Assert.That(electrocardiogram.ElectrocardiogramActualTime, Is.Not.Null);
			Assert.That(electrocardiogram.ElectrocardiogramActualTime.Value.ToLongTimeString(),
			            Is.EqualTo(new DateTime(1980, 1, 1, 10, 11, 12).ToLongTimeString()));
			Assert.That(electrocardiogram.AttachmentId, Is.EqualTo(414));
			Assert.That(electrocardiogram.AttachmentName, Is.EqualTo("test.pdf"));
			Assert.That(electrocardiogram.ElectrocardiogramValidationSettings, Is.Not.Null);


			Assert.That(electrocardiogram.ActualTimeQuestionId, Is.EqualTo(eFormData.ElectrocardiogramActualTime.Id));
			Assert.That(electrocardiogram.ElectrocardiogramAttachmentQuestionId,
			            Is.EqualTo(eFormData.ElectrocardiogramAttachment.Id));
		}

		[Test]
		public void ElectrocardiogramViewModelToFormData_FullData_WithoutAttachment() {
			//Arrange
			var eViewModel = new ElectrocardiogramFormViewModel() {
				Id = 4,
				ElectrocardiogramActualTime = new DateTime(2011, 10, 11, 9, 10, 11),
			};
			var eFormData = CreateEmptyElectrocardiogramFormData();

			//Act
			formController.MapElectrocardiogramViewModelToFormData(eViewModel, eFormData, null);

			//Assert
			Assert.That(eFormData.ElectrocardiogramActualTime.Value,
			            Is.EqualTo(new DateTime(2011, 10, 11, 9, 10, 11).ToString(CultureInfo.InvariantCulture)));
		}

		[Test]
		public void ElectrocardiogramViewModelToFormData_FullData_WithAttachment() {
			//Arrange
			var eViewModel = new ElectrocardiogramFormViewModel() {
				Id = 4,
				ElectrocardiogramActualTime = new DateTime(2011, 10, 11, 9, 10, 11),
				AttachmentId = 14,
			};
			var eFormData = CreateEmptyElectrocardiogramFormData();
			Attachment attachment = new Attachment {
				Id = 14,
				FileName = "test.pdf",
				FileSize = 1234,
				MimeType = "application/pdf",
				StorageFileName = "abcdef.pdf"
			};


			//Act
			formController.MapElectrocardiogramViewModelToFormData(eViewModel, eFormData, attachment);

			//Assert
			Assert.That(eFormData.ElectrocardiogramActualTime.Value,
			            Is.EqualTo(new DateTime(2011, 10, 11, 9, 10, 11).ToString(CultureInfo.InvariantCulture)));

			Assert.That(eFormData.ElectrocardiogramAttachment, Is.Not.Null);
			var file = eFormData.ElectrocardiogramAttachment.File;
			Assert.That(file, Is.Not.Null);
			Assert.That(file.Id, Is.EqualTo(attachment.Id));
			Assert.That(file.FileName, Is.EqualTo(attachment.FileName));
			Assert.That(file.FileSize, Is.EqualTo(attachment.FileSize));
			Assert.That(file.MimeType, Is.EqualTo(attachment.MimeType));
			Assert.That(file.StorageFileName, Is.EqualTo(attachment.StorageFileName));
		}

		[Test]
		public void InventoryFormDataToViewModel_FullData() {
			//Arrange
			var iFormData = new InventoryFormData {
				Id = 5,
				Form =
					new Form {
						Id = 15,
						Caption = "Form15",
						FormState = FormState.Completed,
						Visit =
							new Visit {Caption = "Visit15", Patient = new Patient {PatientNumber = 150}}
					},
				ShipDate =
					new Question {
						Id = 3,
						DataType = QuestionDataType.Date,
						Value =
							new DateTime(2011, 12, 21).ToString(
								CultureInfo.InvariantCulture)
					},
				BatchNumber =
					new Question {Id = 4, DataType = QuestionDataType.Integer, Value = "123"},
				QuantityShipped =
					new Question {
						Id = 5,
						DataType = QuestionDataType.Number,
						Value = (3.3m).ToString(CultureInfo.InvariantCulture)
					},
				ReceiptDate =
					new Question {
						Id = 6,
						DataType = QuestionDataType.Date,
						Value =
							new DateTime(2011, 12, 5).ToString(
								CultureInfo.InvariantCulture)
					},
				MedicationUsage =
					new List<RepeatableInventoryData> {
						new RepeatableInventoryData {
							Id = 1,
							DateUsed =
								new Question {
									Id = 114,
									DataType = QuestionDataType.Date,
									Value = DateTime.Today.ToString(CultureInfo.InvariantCulture)
								},
							QuantityUsed =
								new Question {
									Id = 115,
									DataType = QuestionDataType.Number,
									Value = (2.4m).ToString(CultureInfo.InvariantCulture)
								}
						},
						new RepeatableInventoryData {
							Id = 2,
							DateUsed = new Question {
								Id = 211,
								DataType = QuestionDataType.Date,
								Value = DateTime.Today.AddDays(-1).ToString(CultureInfo.InvariantCulture)
							},
							QuantityUsed =
								new Question {
									Id = 212,
									DataType = QuestionDataType.Number,
									Value = (0.4m).ToString(CultureInfo.InvariantCulture)
								}
						}
					}
			};
			//Act
			var inventory = formController.MapInventoryFormDataToViewModel(iFormData);

			//Assert
			Assert.That(inventory.FormId, Is.EqualTo(15));
			Assert.That(inventory.FormCaption, Is.EqualTo("Form15"));
			Assert.That(inventory.IsCompleted, Is.True);
			Assert.That(inventory.VisitName, Is.EqualTo("Visit15"));
			Assert.That(inventory.PatientNumber, Is.EqualTo(150));
			Assert.That(inventory.Id, Is.EqualTo(5));
			Assert.That(inventory.BatchNumber, Is.EqualTo(123));
			Assert.That(inventory.ShipDate, Is.Not.Null);
			Assert.That(inventory.ShipDate.Value.ToShortDateString(), Is.EqualTo(new DateTime(2011, 12, 21).ToShortDateString()));
			Assert.That(inventory.ReceiptDate, Is.Not.Null);
			Assert.That(inventory.ReceiptDate.Value.ToShortDateString(),
			            Is.EqualTo(new DateTime(2011, 12, 5).ToShortDateString()));
			Assert.That(inventory.QuantityShipped, Is.EqualTo(3.3m));


			Assert.That(inventory.BatchNumberQuestionId, Is.EqualTo(iFormData.BatchNumber.Id));
			Assert.That(inventory.ShipDateQuestionId, Is.EqualTo(iFormData.ShipDate.Id));
			Assert.That(inventory.ReceiptDateQuestionId, Is.EqualTo(iFormData.ReceiptDate.Id));
			Assert.That(inventory.QuantityShippedQuestionId, Is.EqualTo(iFormData.QuantityShipped.Id));


			Assert.That(inventory.MedicationUsage, Is.Not.Null);
			Assert.That(inventory.MedicationUsage.Count, Is.EqualTo(2));

			Assert.That(inventory.MedicationUsage[0].DateUsed, Is.Not.Null);
			Assert.That(inventory.MedicationUsage[0].DateUsed.Value.ToShortDateString(),
			            Is.EqualTo(DateTime.Today.ToShortDateString()));
			Assert.That(inventory.MedicationUsage[0].QuantityUsed, Is.EqualTo(2.4m));

			Assert.That(inventory.MedicationUsage[0].DateUsedQuestionId, Is.EqualTo(iFormData.MedicationUsage[0].DateUsed.Id));
			Assert.That(inventory.MedicationUsage[0].QuantityUsedQuestionId,
			            Is.EqualTo(iFormData.MedicationUsage[0].QuantityUsed.Id));

			Assert.That(inventory.MedicationUsage[1].DateUsed, Is.Not.Null);
			Assert.That(inventory.MedicationUsage[1].DateUsed.Value.ToShortDateString(),
			            Is.EqualTo(DateTime.Today.AddDays(-1).ToShortDateString()));
			Assert.That(inventory.MedicationUsage[1].QuantityUsed, Is.EqualTo(0.4m));

			Assert.That(inventory.MedicationUsage[1].DateUsedQuestionId, Is.EqualTo(iFormData.MedicationUsage[1].DateUsed.Id));
			Assert.That(inventory.MedicationUsage[1].QuantityUsedQuestionId,
			            Is.EqualTo(iFormData.MedicationUsage[1].QuantityUsed.Id));
		}

		[Test]
		public void InventoryViewModelToFormData_FullData() {
			//Arrange
			var iViewModel = new InventoryFormViewModel {
				Id = 5,
				ShipDate = new DateTime(2012, 1, 2),
				BatchNumber = 120,
				QuantityShipped = 4.5m,
				ReceiptDate = new DateTime(2012, 2, 1)
			};
			var iFormData = CreateEmptyInventoryFormData();

			//Act
			formController.MapInventoryViewModelToFormData(iViewModel, iFormData);

			//Assert
			Assert.That(iFormData.BatchNumber.Value, Is.EqualTo("120"));
			Assert.That(iFormData.ShipDate.Value, Is.EqualTo(new DateTime(2012, 1, 2).ToString(CultureInfo.InvariantCulture)));
			Assert.That(iFormData.ReceiptDate.Value, Is.EqualTo(new DateTime(2012, 2, 1).ToString(CultureInfo.InvariantCulture)));
			Assert.That(iFormData.QuantityShipped.Value, Is.EqualTo(4.5m.ToString(CultureInfo.InvariantCulture)));
		}

		[Test]
		public void RepeatableInventoryDataViewModelToFormData_FullData() {
			//Arrange
			var iViewModel =
				new List<RepeatableInventoryDataViewModel> {
					new RepeatableInventoryDataViewModel {
						DateUsed =
							DateTime.Today.
								AddDays(-1),
						QuantityUsed =
							3.1m
					},
					new RepeatableInventoryDataViewModel {
						DateUsed =
							DateTime.Today.
								AddDays(1),
						QuantityUsed =
							1.1m
					}
				};
			var iFormData = new List<RepeatableInventoryData>();
			//Act
			formController.MapRepeatableInventoryViewModelToFormData(iViewModel, iFormData);

			//Assert
			Assert.That(iFormData, Is.Not.Null);
			Assert.That(iFormData.Count, Is.EqualTo(2));

			Assert.That(iFormData[0].DateUsed.Value,
			            Is.EqualTo(DateTime.Today.AddDays(-1).ToString(CultureInfo.InvariantCulture)));
			Assert.That(iFormData[0].QuantityUsed.Value, Is.EqualTo(3.1m.ToString(CultureInfo.InvariantCulture)));

			Assert.That(iFormData[1].DateUsed.Value,
			            Is.EqualTo(DateTime.Today.AddDays(1).ToString(CultureInfo.InvariantCulture)));
			Assert.That(iFormData[1].QuantityUsed.Value, Is.EqualTo(1.1m.ToString(CultureInfo.InvariantCulture)));
		}

		[Test]
		public void AdverseEventFormDataToViewModel_FullData() {
			//Arrange 
			var aFormData = new AdverseEventFormData {
				Id = 6,
				Form =
					new Form {
						Id = 16,
						Caption = "Form16",
						FormState = FormState.Completed,
						Visit =
							new Visit {Caption = "Visit16", Patient = new Patient {PatientNumber = 160}}
					},
				AdverseExperience =
					new Question {Id = 3, DataType = QuestionDataType.String, Value = "Broken legs"},
				OnsetDate =
					new Question {
						Id = 4,
						DataType = QuestionDataType.Date,
						Value =
							new DateTime(2011, 11, 8, 9, 10, 11).ToString(
								CultureInfo.InvariantCulture)
					},
				OnsetTime =
					new Question {
						Id = 5,
						DataType = QuestionDataType.Time,
						Value =
							new DateTime(2011, 11, 8, 9, 10, 11).ToString(
								CultureInfo.InvariantCulture)
					},
				EndDate =
					new Question {
						Id = 6,
						DataType = QuestionDataType.Date,
						Value =
							new DateTime(2012, 1, 10, 11, 12, 13).ToString(
								CultureInfo.InvariantCulture)
					},
				EndTime =
					new Question {
						Id = 7,
						DataType = QuestionDataType.Date,
						Value =
							new DateTime(2012, 1, 10, 11, 12, 13).ToString(
								CultureInfo.InvariantCulture)
					},
				Outcome = new Question {Id = 8, DataType = QuestionDataType.Enum, Value = "1"},
				Intensity = new Question {Id = 9, DataType = QuestionDataType.Enum, Value = "2"},
				RelationshipToInvestigationalDrug = new Question {Id = 10, DataType = QuestionDataType.Enum, Value = "3"}
			};
			//Act
			AdverseEventFormViewModel adverseEvent = formController.MapAdverseEventFormDataToViewModel(aFormData);

			//Assert
			Assert.That(adverseEvent.FormId, Is.EqualTo(16));
			Assert.That(adverseEvent.FormCaption, Is.EqualTo("Form16"));
			Assert.That(adverseEvent.IsCompleted, Is.True);
			Assert.That(adverseEvent.VisitName, Is.EqualTo("Visit16"));
			Assert.That(adverseEvent.PatientNumber, Is.EqualTo(160));
			Assert.That(adverseEvent.Id, Is.EqualTo(6));
			Assert.That(adverseEvent.AdverseExperience, Is.EqualTo("Broken legs"));

			Assert.That(adverseEvent.OnsetDate, Is.Not.Null);
			Assert.That(adverseEvent.OnsetDate.Value.ToShortDateString(),
			            Is.EqualTo(new DateTime(2011, 11, 8).ToShortDateString()));
			Assert.That(adverseEvent.OnsetTime, Is.Not.Null);
			Assert.That(adverseEvent.OnsetTime.Value.ToLongTimeString(),
			            Is.EqualTo(new DateTime(2011, 12, 5, 9, 10, 11).ToLongTimeString()));

			Assert.That(adverseEvent.EndDate, Is.Not.Null);
			Assert.That(adverseEvent.EndDate.Value.ToShortDateString(),
			            Is.EqualTo(new DateTime(2012, 1, 10).ToShortDateString()));
			Assert.That(adverseEvent.EndTime, Is.Not.Null);
			Assert.That(adverseEvent.EndTime.Value.ToLongTimeString(),
			            Is.EqualTo(new DateTime(2011, 12, 5, 11, 12, 13).ToLongTimeString()));

			Assert.That(adverseEvent.Outcome, Is.EqualTo(1));
			Assert.That(adverseEvent.Intensity, Is.EqualTo(2));
			Assert.That(adverseEvent.RelationshipToInvestigationalDrug, Is.EqualTo(3));


			Assert.That(adverseEvent.AdverseExperienceQuestionId, Is.EqualTo(aFormData.AdverseExperience.Id));
			Assert.That(adverseEvent.OnsetDateQuestionId, Is.EqualTo(aFormData.OnsetDate.Id));
			Assert.That(adverseEvent.OnsetTimeQuestionId, Is.EqualTo(aFormData.OnsetTime.Id));
			Assert.That(adverseEvent.EndDateQuestionId, Is.EqualTo(aFormData.EndDate.Id));
			Assert.That(adverseEvent.EndTimeQuestionId, Is.EqualTo(aFormData.EndTime.Id));
			Assert.That(adverseEvent.OutcomeQuestionId, Is.EqualTo(aFormData.Outcome.Id));
			Assert.That(adverseEvent.IntensityQuestionId, Is.EqualTo(aFormData.Intensity.Id));
			Assert.That(adverseEvent.RelationshipToInvestigationalDrugQuestionId,
			            Is.EqualTo(aFormData.RelationshipToInvestigationalDrug.Id));
		}

		[Test]
		public void AdverseEventViewModelToFormData_FullData() {
			//Arrange
			var aViewModel = new AdverseEventFormViewModel {
				Id = 6,
				AdverseExperience = "Broken arm",
				OnsetDate = new DateTime(2012, 3, 4),
				OnsetTime = new DateTime(2012, 4, 5, 12, 13, 14),
				EndDate = new DateTime(2011, 2, 3),
				EndTime = new DateTime(2011, 4, 5, 1, 2, 3),
				Outcome = 2,
				Intensity = 3,
				RelationshipToInvestigationalDrug = 4
			};
			var aFormData = CreateEmptyAdverseEventFormData();

			//Act
			formController.MapAdverseEventViewModelToFormData(aViewModel, aFormData);

			//Assert
			Assert.That(aFormData.AdverseExperience.Value, Is.EqualTo("Broken arm"));
			Assert.That(aFormData.OnsetDate.Value,
			            Is.EqualTo(new DateTime(2012, 3, 4).ToString(CultureInfo.InvariantCulture)));
			Assert.That(aFormData.OnsetTime.Value,
			            Is.EqualTo(new DateTime(2012, 4, 5, 12, 13, 14).ToString(CultureInfo.InvariantCulture)));
			Assert.That(aFormData.EndDate.Value,
			            Is.EqualTo(new DateTime(2011, 2, 3).ToString(CultureInfo.InvariantCulture)));
			Assert.That(aFormData.EndTime.Value,
			            Is.EqualTo(new DateTime(2011, 4, 5, 1, 2, 3).ToString(CultureInfo.InvariantCulture)));
			Assert.That(aFormData.Outcome.Value, Is.EqualTo("2"));
			Assert.That(aFormData.Intensity.Value, Is.EqualTo("3"));
			Assert.That(aFormData.RelationshipToInvestigationalDrug.Value, Is.EqualTo("4"));
		}


		[Test]
		public void DemographicFormDataToViewModel_EmptyData() {
			//Arrange 
			var dFormData = CreateEmptyDemographicFormData();

			//Act
			var demog = formController.MapDemographicFormDataToViewModel(dFormData);

			//Assert
			Assert.That(String.IsNullOrEmpty(demog.FormCaption), Is.True);
			Assert.That(demog.IsCompleted, Is.False);
			Assert.That(String.IsNullOrEmpty(demog.VisitName), Is.True);
			Assert.That(demog.PatientNumber, Is.EqualTo(0));
			Assert.That(demog.FormId, Is.EqualTo(0));
			Assert.That(demog.DateOfBirth, Is.Null);
			Assert.That(String.IsNullOrEmpty(demog.Other), Is.True);
			Assert.That(demog.Race, Is.EqualTo(0));
			Assert.That(demog.Sex, Is.EqualTo(0));
		}

		[Test]
		public void DemographicViewModelToFormData_EmptyData() {
			//Arrange 
			var dViewModel = CreateEmptyDemographicFormViewModel();
			var dFormData = CreateEmptyDemographicFormData();

			//Act
			formController.MapDemographicViewModelToFormData(dViewModel, dFormData);

			//Assert
			Assert.That(String.IsNullOrEmpty(dFormData.DateOfBirth.Value), Is.True);
			Assert.That(String.IsNullOrEmpty(dFormData.Other.Value), Is.True);
			Assert.That(dFormData.Race.Value, Is.EqualTo("0"));
			Assert.That(dFormData.Sex.Value, Is.EqualTo("0"));
		}

		[Test]
		public void VitalsFormDataToViewModel_EmptyData() {
			//Arrange 
			var vFormData = CreateEmptyVitalsFormData();
			//Act
			var vitals = formController.MapVitalsFormDataToViewModel(vFormData);

			//Assert
			Assert.That(String.IsNullOrEmpty(vitals.FormCaption), Is.True);
			Assert.That(vitals.IsCompleted, Is.False);
			Assert.That(String.IsNullOrEmpty(vitals.VisitName), Is.True);
			Assert.That(vitals.PatientNumber, Is.EqualTo(0));
			Assert.That(vitals.FormId, Is.EqualTo(0));
			Assert.That(vitals.ActualTime, Is.Null);
			Assert.That(vitals.Height, Is.EqualTo(0));
			Assert.That(vitals.Weight, Is.EqualTo(0));
			Assert.That(vitals.Temperature, Is.EqualTo(0));
			Assert.That(vitals.HeartRate, Is.EqualTo(0));
			Assert.That(String.IsNullOrEmpty(vitals.BloodPressureSystolic), Is.True);
			Assert.That(String.IsNullOrEmpty(vitals.BloodPressureDiastolic), Is.True);
		}

		[Test]
		public void VitalsViewModelToFormData_EmptyData() {
			//Arrange
			var vViewModel = CreateEmptyVitalsFormViewModel();
			var vFormData = CreateEmptyVitalsFormData();

			//Act
			formController.MapVitalsViewModelToFormData(vViewModel, vFormData);

			//Assert
			Assert.That(String.IsNullOrEmpty(vFormData.ActualTime.Value), Is.True);
			Assert.That(vFormData.Height.Value, Is.EqualTo("0"));
			Assert.That(vFormData.Weight.Value, Is.EqualTo("0"));
			Assert.That(vFormData.Temperature.Value, Is.EqualTo("0"));
			Assert.That(vFormData.HeartRate.Value, Is.EqualTo("0"));
			Assert.That(String.IsNullOrEmpty(vFormData.BloodPressureSystolic.Value), Is.True);
			Assert.That(String.IsNullOrEmpty(vFormData.BloodPressureDiastolic.Value), Is.True);
		}

		[Test]
		public void HappinessFormDataToViewModel_EmptyData() {
			//Arrange 
			var hFormData = CreateEmptyHappinessFormData();
			//Act
			var happiness = formController.MapHappinessFormDataToViewModel(hFormData);

			//Assert
			Assert.That(String.IsNullOrEmpty(happiness.FormCaption), Is.True);
			Assert.That(happiness.IsCompleted, Is.False);
			Assert.That(String.IsNullOrEmpty(happiness.VisitName), Is.True);
			Assert.That(happiness.PatientNumber, Is.EqualTo(0));
			Assert.That(happiness.FormId, Is.EqualTo(0));
			Assert.That(happiness.HappinessLevel, Is.EqualTo(0));
		}

		[Test]
		public void HappinessViewModelToFormData_EmptyData() {
			//Arrange
			var hViewModel = CreateEmptyHappinessFormViewModel();
			var hFormData = CreateEmptyHappinessFormData();

			//Act
			formController.MapHappinessViewModelToFormData(hViewModel, hFormData);

			//Assert
			Assert.That(hFormData.HappinessLevel.Value, Is.EqualTo("0"));
		}

		[Test]
		public void ElectrocardiogramFormDataToViewModel_EmptyData() {
			//Arrange
			var eFormData = CreateEmptyElectrocardiogramFormData();
			//Act
			var electrocardiogram = formController.MapElectrocardiogramFormDataToViewModel(eFormData);

			//Assert
			Assert.That(String.IsNullOrEmpty(electrocardiogram.FormCaption), Is.True);
			Assert.That(electrocardiogram.IsCompleted, Is.False);
			Assert.That(String.IsNullOrEmpty(electrocardiogram.VisitName), Is.True);
			Assert.That(electrocardiogram.PatientNumber, Is.EqualTo(0));
			Assert.That(electrocardiogram.FormId, Is.EqualTo(0));
			Assert.That(electrocardiogram.ElectrocardiogramActualTime, Is.Null);
			Assert.That(electrocardiogram.AttachmentId, Is.Null);
			Assert.That(electrocardiogram.AttachmentName, Is.Null);
			Assert.That(electrocardiogram.ElectrocardiogramValidationSettings, Is.Not.Null);
		}

		[Test]
		public void ElectrocardiogramViewModelToFormData_EmptyData() {
			//Arrange
			var eViewModel = CreateEmptyElectrocardiogramFormViewModel();
			var eFormData = CreateEmptyElectrocardiogramFormData();

			//Act
			formController.MapElectrocardiogramViewModelToFormData(eViewModel, eFormData, null);

			//Assert
			Assert.That(String.IsNullOrEmpty(eFormData.ElectrocardiogramActualTime.Value), Is.True);
		}

		[Test]
		public void InventoryFormDataToViewModel_EmptyData() {
			//Arrange
			var iFormData = CreateEmptyInventoryFormData();
			//Act
			var inventory = formController.MapInventoryFormDataToViewModel(iFormData);

			//Assert
			Assert.That(String.IsNullOrEmpty(inventory.FormCaption), Is.True);
			Assert.That(inventory.IsCompleted, Is.False);
			Assert.That(String.IsNullOrEmpty(inventory.VisitName), Is.True);
			Assert.That(inventory.PatientNumber, Is.EqualTo(0));
			Assert.That(inventory.FormId, Is.EqualTo(0));
			Assert.That(inventory.BatchNumber, Is.EqualTo(0));
			Assert.That(inventory.ShipDate, Is.Null);
			Assert.That(inventory.ReceiptDate, Is.Null);
			Assert.That(inventory.QuantityShipped, Is.EqualTo(0));

			Assert.That(inventory.MedicationUsage, Is.Not.Null);
			Assert.That(inventory.MedicationUsage.Count, Is.EqualTo(0));
		}

		[Test]
		public void InventoryViewModelToFormData_EmptyData() {
			//Arrange
			var iViewModel = CreateEmptyInventoryFormViewModel();
			var iFormData = CreateEmptyInventoryFormData();

			//Act
			formController.MapInventoryViewModelToFormData(iViewModel, iFormData);

			//Assert
			Assert.That(iFormData.BatchNumber.Value, Is.EqualTo("0"));
			Assert.That(String.IsNullOrEmpty(iFormData.ShipDate.Value), Is.True);
			Assert.That(String.IsNullOrEmpty(iFormData.ReceiptDate.Value), Is.True);
			Assert.That(iFormData.QuantityShipped.Value, Is.EqualTo("0"));

			Assert.That(iFormData.MedicationUsage, Is.Not.Null);
			Assert.That(iFormData.MedicationUsage.Count, Is.EqualTo(0));
		}

		[Test]
		public void AdverseEventFormDataToViewModel_EmptyData() {
			//Arrange 
			var aFormData = CreateEmptyAdverseEventFormData();
			//Act
			AdverseEventFormViewModel adverseEvent = formController.MapAdverseEventFormDataToViewModel(aFormData);

			//Assert
			Assert.That(String.IsNullOrEmpty(adverseEvent.FormCaption), Is.True);
			Assert.That(adverseEvent.IsCompleted, Is.False);
			Assert.That(String.IsNullOrEmpty(adverseEvent.VisitName), Is.True);
			Assert.That(adverseEvent.PatientNumber, Is.EqualTo(0));
			Assert.That(adverseEvent.FormId, Is.EqualTo(0));
			Assert.That(String.IsNullOrEmpty(adverseEvent.AdverseExperience), Is.True);
			Assert.That(adverseEvent.OnsetDate, Is.Null);
			Assert.That(adverseEvent.OnsetTime, Is.Null);
			Assert.That(adverseEvent.EndDate, Is.Null);
			Assert.That(adverseEvent.EndTime, Is.Null);
			Assert.That(adverseEvent.Outcome, Is.EqualTo(0));
			Assert.That(adverseEvent.Intensity, Is.EqualTo(0));
			Assert.That(adverseEvent.RelationshipToInvestigationalDrug, Is.EqualTo(0));
		}

		[Test]
		public void AdverseEventViewModelToFormData_EmptyData() {
			//Arrange
			var aViewModel = CreateEmptyAdverseEventFormViewModel();
			var aFormData = CreateEmptyAdverseEventFormData();

			//Act
			formController.MapAdverseEventViewModelToFormData(aViewModel, aFormData);

			//Assert
			Assert.That(String.IsNullOrEmpty(aFormData.AdverseExperience.Value), Is.True);
			Assert.That(String.IsNullOrEmpty(aFormData.OnsetDate.Value), Is.True);
			Assert.That(String.IsNullOrEmpty(aFormData.OnsetTime.Value), Is.True);
			Assert.That(String.IsNullOrEmpty(aFormData.EndDate.Value), Is.True);
			Assert.That(String.IsNullOrEmpty(aFormData.EndTime.Value), Is.True);
			Assert.That(aFormData.Outcome.Value, Is.EqualTo("0"));
			Assert.That(aFormData.Intensity.Value, Is.EqualTo("0"));
			Assert.That(aFormData.RelationshipToInvestigationalDrug.Value, Is.EqualTo("0"));
		}

		private DemographicFormData CreateEmptyDemographicFormData() {
			return new DemographicFormData {
				DateOfBirth = new Question(),
				Other = new Question(),
				Race = new Question(),
				Sex = new Question(),
				Form = new Form {Visit = new Visit {Patient = new Patient()}}
			};
		}

		private DemographicFormViewModel CreateEmptyDemographicFormViewModel() {
			return new DemographicFormViewModel();
		}

		private VitalsFormData CreateEmptyVitalsFormData() {
			return new VitalsFormData {
				Form = new Form {Visit = new Visit {Patient = new Patient()}},
				ActualTime = new Question(),
				Height = new Question(),
				Weight = new Question(),
				Temperature = new Question(),
				HeartRate = new Question(),
				BloodPressureSystolic = new Question(),
				BloodPressureDiastolic = new Question()
			};
		}

		private VitalsFormViewModel CreateEmptyVitalsFormViewModel() {
			return new VitalsFormViewModel();
		}

		private HappinessFormData CreateEmptyHappinessFormData() {
			return new HappinessFormData {
				Form = new Form {Visit = new Visit {Patient = new Patient()}},
				HappinessLevel = new Question()
			};
		}

		private HappinessFormViewModel CreateEmptyHappinessFormViewModel() {
			return new HappinessFormViewModel();
		}

		private ElectrocardiogramFormData CreateEmptyElectrocardiogramFormData() {
			return new ElectrocardiogramFormData {
				Form = new Form {Visit = new Visit {Patient = new Patient()}},
				ElectrocardiogramActualTime = new Question(),
				ElectrocardiogramAttachment = new Question()
			};
		}

		private ElectrocardiogramFormViewModel CreateEmptyElectrocardiogramFormViewModel() {
			return new ElectrocardiogramFormViewModel();
		}

		private InventoryFormData CreateEmptyInventoryFormData() {
			return new InventoryFormData {
				Form = new Form {Visit = new Visit {Patient = new Patient()}},
				BatchNumber = new Question(),
				QuantityShipped = new Question(),
				ReceiptDate = new Question(),
				ShipDate = new Question(),
				MedicationUsage = new List<RepeatableInventoryData>()
			};
		}

		private InventoryFormViewModel CreateEmptyInventoryFormViewModel() {
			return new InventoryFormViewModel {MedicationUsage = new List<RepeatableInventoryDataViewModel>()};
		}

		private AdverseEventFormData CreateEmptyAdverseEventFormData() {
			return new AdverseEventFormData {
				Form = new Form {Visit = new Visit {Patient = new Patient()}},
				AdverseExperience = new Question(),
				EndDate = new Question(),
				EndTime = new Question(),
				Intensity = new Question(),
				OnsetDate = new Question(),
				OnsetTime = new Question(),
				Outcome = new Question(),
				RelationshipToInvestigationalDrug = new Question()
			};
		}

		private AdverseEventFormViewModel CreateEmptyAdverseEventFormViewModel() {
			return new AdverseEventFormViewModel();
		}
	}
}
