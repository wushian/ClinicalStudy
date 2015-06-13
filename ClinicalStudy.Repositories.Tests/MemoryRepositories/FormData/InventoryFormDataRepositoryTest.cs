using System.Collections.Generic;
using System.Linq;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using ClinicalStudy.Repositories.MemoryRepositories.FormData;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Repositories.Tests.MemoryRepositories.FormData {
	[TestFixture]
	public class InventoryFormDataRepositoryTest {
		[Test]
		public void MappingTest() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();
			var entity = new InventoryFormData() {
				Id = 1,
				QuantityShipped = new Question {Id = 2, Value = "2"},
				BatchNumber = new Question {Id = 3, Value = "3"},
				ReceiptDate = new Question {Id = 4, Value = "4"},
				ShipDate = new Question {Id = 5, Value = "5"},
				MedicationUsage = new List<RepeatableInventoryData> {
					new RepeatableInventoryData {
						Id = 1,
						DateUsed =
							new Question
							{Id = 6, Value = "6"},
						QuantityUsed =
							new Question
							{Id = 7, Value = "7"}
					}
				}
			};
			var updatedEntity = new InventoryFormData() {
				Id = 1,
				QuantityShipped = new Question {Id = 3, Value = "3"},
				BatchNumber = new Question {Id = 4, Value = "4"},
				ReceiptDate = new Question {Id = 5, Value = "5"},
				ShipDate = new Question {Id = 6, Value = "6"},
				MedicationUsage = new List<RepeatableInventoryData> {
					new RepeatableInventoryData {
						Id = 1,
						DateUsed =
							new Question
							{Id = 7, Value = "7"},
						QuantityUsed =
							new Question
							{Id = 8, Value = "8"}
					}
				}
			};
			var repository = new InventoryFormDataRepository(dataStorage.Object);

			dataStorage.Setup(ds => ds.GetData<InventoryFormData>()).Returns(new List<InventoryFormData> {entity});
			//Act
			repository.Edit(updatedEntity);
			//Assert
			//Values are equal
			Assert.That(entity.QuantityShipped.Value, Is.EqualTo(updatedEntity.QuantityShipped.Value));
			Assert.That(entity.BatchNumber.Value, Is.EqualTo(updatedEntity.BatchNumber.Value));
			Assert.That(entity.ReceiptDate.Value, Is.EqualTo(updatedEntity.ReceiptDate.Value));
			Assert.That(entity.ShipDate.Value, Is.EqualTo(updatedEntity.ShipDate.Value));
			Assert.That(entity.MedicationUsage[0].DateUsed.Value, Is.EqualTo(updatedEntity.MedicationUsage[0].DateUsed.Value));
			Assert.That(entity.MedicationUsage[0].QuantityUsed.Value,
			            Is.EqualTo(updatedEntity.MedicationUsage[0].QuantityUsed.Value));

			//Objects are not equal
			Assert.That(entity.QuantityShipped, Is.Not.EqualTo(updatedEntity.QuantityShipped));
			Assert.That(entity.BatchNumber, Is.Not.EqualTo(updatedEntity.BatchNumber));
			Assert.That(entity.ReceiptDate, Is.Not.EqualTo(updatedEntity.ReceiptDate));
			Assert.That(entity.ShipDate, Is.Not.EqualTo(updatedEntity.ShipDate));
			Assert.That(entity.MedicationUsage[0].DateUsed, Is.Not.EqualTo(updatedEntity.MedicationUsage[0].DateUsed));
			Assert.That(entity.MedicationUsage[0].QuantityUsed, Is.Not.EqualTo(updatedEntity.MedicationUsage[0].QuantityUsed));
		}

		[Test]
		public void UpdateMedicationUsage_SameQuestions() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();

			var entity = new InventoryFormData() {
				Id = 1,
				QuantityShipped = new Question {Id = 2, Value = "2"},
				BatchNumber = new Question {Id = 3, Value = "3"},
				ReceiptDate = new Question {Id = 4, Value = "4"},
				ShipDate = new Question {Id = 5, Value = "5"},
				MedicationUsage = new List<RepeatableInventoryData> {
					new RepeatableInventoryData {
						Id = 1,
						DateUsed =
							new Question
							{Id = 6, Value = "6"},
						QuantityUsed =
							new Question
							{Id = 7, Value = "7"}
					},
					new RepeatableInventoryData {
						Id = 2,
						DateUsed =
							new Question
							{Id = 8, Value = "8"},
						QuantityUsed =
							new Question
							{Id = 9, Value = "9"}
					}
				}
			};
			var updatedEntity = new InventoryFormData() {
				Id = 1,
				QuantityShipped = new Question {Id = 3, Value = "3"},
				BatchNumber = new Question {Id = 4, Value = "4"},
				ReceiptDate = new Question {Id = 5, Value = "5"},
				ShipDate = new Question {Id = 6, Value = "6"},
				MedicationUsage = new List<RepeatableInventoryData> {
					new RepeatableInventoryData {
						Id = 1,
						DateUsed =
							new Question {
								Id = 7,
								Value = "7"
							},
						QuantityUsed =
							new Question {
								Id = 8,
								Value = "8"
							}
					},
					new RepeatableInventoryData {
						Id = 2,
						DateUsed =
							new Question {
								Id = 9,
								Value = "9"
							},
						QuantityUsed =
							new Question {
								Id = 10,
								Value = "10"
							}
					}
				}
			};


			var repository = new InventoryFormDataRepository(dataStorage.Object);
			dataStorage.Setup(ds => ds.GetData<InventoryFormData>()).Returns(new List<InventoryFormData> {entity});

			//Act
			repository.Edit(updatedEntity);
			//Assert
			//Values are equal
			Assert.That(entity.MedicationUsage, Is.Not.Null);
			Assert.That(entity.MedicationUsage.Count, Is.EqualTo(updatedEntity.MedicationUsage.Count));
			Assert.That(entity.MedicationUsage.Count, Is.EqualTo(2));
			Assert.That(entity.MedicationUsage[0].Id, Is.EqualTo(updatedEntity.MedicationUsage[0].Id));
			Assert.That(entity.MedicationUsage[0].DateUsed.Value, Is.EqualTo(updatedEntity.MedicationUsage[0].DateUsed.Value));
			Assert.That(entity.MedicationUsage[0].QuantityUsed.Value,
			            Is.EqualTo(updatedEntity.MedicationUsage[0].QuantityUsed.Value));
			Assert.That(entity.MedicationUsage[1].Id, Is.EqualTo(updatedEntity.MedicationUsage[1].Id));
			Assert.That(entity.MedicationUsage[1].DateUsed.Value, Is.EqualTo(updatedEntity.MedicationUsage[1].DateUsed.Value));
			Assert.That(entity.MedicationUsage[1].QuantityUsed.Value,
			            Is.EqualTo(updatedEntity.MedicationUsage[1].QuantityUsed.Value));
		}

		[Test]
		public void UpdateMedicationUsage_AllNewQuestions() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();

			var entity = new InventoryFormData() {
				Id = 1,
				QuantityShipped = new Question {Id = 2, Value = "2"},
				BatchNumber = new Question {Id = 3, Value = "3"},
				ReceiptDate = new Question {Id = 4, Value = "4"},
				ShipDate = new Question {Id = 5, Value = "5"},
				MedicationUsage = new List<RepeatableInventoryData>()
			};
			var updatedEntity = new InventoryFormData() {
				Id = 1,
				QuantityShipped = new Question {Id = 3, Value = "3"},
				BatchNumber = new Question {Id = 4, Value = "4"},
				ReceiptDate = new Question {Id = 5, Value = "5"},
				ShipDate = new Question {Id = 6, Value = "6"},
				MedicationUsage = new List<RepeatableInventoryData> {
					new RepeatableInventoryData {
						Id = 1,
						DateUsed =
							new Question {
								Id = 7,
								Value = "7"
							},
						QuantityUsed =
							new Question {
								Id = 8,
								Value = "8"
							}
					},
					new RepeatableInventoryData {
						Id = 2,
						DateUsed =
							new Question {
								Id = 9,
								Value = "9"
							},
						QuantityUsed =
							new Question {
								Id = 10,
								Value = "10"
							}
					}
				}
			};


			var repository = new InventoryFormDataRepository(dataStorage.Object);
			dataStorage.Setup(ds => ds.GetData<InventoryFormData>()).Returns(new List<InventoryFormData> {entity});

			//Act
			repository.Edit(updatedEntity);
			//Assert
			//Values are equal
			Assert.That(entity.MedicationUsage, Is.Not.Null);
			Assert.That(entity.MedicationUsage.Count, Is.EqualTo(updatedEntity.MedicationUsage.Count));
			Assert.That(entity.MedicationUsage.Count, Is.EqualTo(2));
			Assert.That(entity.MedicationUsage[0].Id, Is.EqualTo(updatedEntity.MedicationUsage[0].Id));
			Assert.That(entity.MedicationUsage[0].DateUsed.Value, Is.EqualTo(updatedEntity.MedicationUsage[0].DateUsed.Value));
			Assert.That(entity.MedicationUsage[0].QuantityUsed.Value,
			            Is.EqualTo(updatedEntity.MedicationUsage[0].QuantityUsed.Value));
			Assert.That(entity.MedicationUsage[1].Id, Is.EqualTo(updatedEntity.MedicationUsage[1].Id));
			Assert.That(entity.MedicationUsage[1].DateUsed.Value, Is.EqualTo(updatedEntity.MedicationUsage[1].DateUsed.Value));
			Assert.That(entity.MedicationUsage[1].QuantityUsed.Value,
			            Is.EqualTo(updatedEntity.MedicationUsage[1].QuantityUsed.Value));
		}

		[Test]
		public void UpdateMedicationUsage_DeleteAllQuestions() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();

			var entity = new InventoryFormData() {
				Id = 1,
				QuantityShipped = new Question {Id = 2, Value = "2"},
				BatchNumber = new Question {Id = 3, Value = "3"},
				ReceiptDate = new Question {Id = 4, Value = "4"},
				ShipDate = new Question {Id = 5, Value = "5"},
				MedicationUsage = new List<RepeatableInventoryData> {
					new RepeatableInventoryData {
						Id = 1,
						DateUsed = new Question {
							Id = 7,
							Value = "7"
						},
						QuantityUsed = new Question {
							Id = 8,
							Value = "8"
						}
					},
					new RepeatableInventoryData {
						Id = 2,
						DateUsed = new Question {
							Id = 9,
							Value = "9"
						},
						QuantityUsed = new Question {
							Id = 10,
							Value = "10"
						}
					}
				}
			};
			var updatedEntity = new InventoryFormData() {
				Id = 1,
				QuantityShipped = new Question {Id = 3, Value = "3"},
				BatchNumber = new Question {Id = 4, Value = "4"},
				ReceiptDate = new Question {Id = 5, Value = "5"},
				ShipDate = new Question {Id = 6, Value = "6"},
				MedicationUsage = new List<RepeatableInventoryData>()
			};


			var repository = new InventoryFormDataRepository(dataStorage.Object);
			dataStorage.Setup(ds => ds.GetData<InventoryFormData>()).Returns(new List<InventoryFormData> {entity});

			//Act
			repository.Edit(updatedEntity);
			//Assert
			//Values are equal
			Assert.That(entity.MedicationUsage, Is.Not.Null);
			Assert.That(entity.MedicationUsage.Count, Is.EqualTo(updatedEntity.MedicationUsage.Count));
			Assert.That(entity.MedicationUsage.Count, Is.EqualTo(0));
		}

		[Test]
		public void UpdateMedicationUsage_AddOneDeleteOneQuestions() {
			//Arrange
			var dataStorage = new Mock<IDataStorage>();

			var entity = new InventoryFormData() {
				Id = 1,
				QuantityShipped = new Question {Id = 2, Value = "2"},
				BatchNumber = new Question {Id = 3, Value = "3"},
				ReceiptDate = new Question {Id = 4, Value = "4"},
				ShipDate = new Question {Id = 5, Value = "5"},
				MedicationUsage = new List<RepeatableInventoryData> {
					new RepeatableInventoryData {
						Id = 1,
						DateUsed =
							new Question {
								Id = 6,
								Value = "6"
							},
						QuantityUsed =
							new Question {
								Id = 7,
								Value = "7"
							}
					},
					new RepeatableInventoryData {
						Id = 2,
						DateUsed = new Question {
							Id = 8,
							Value = "8"
						},
						QuantityUsed =
							new Question {
								Id = 9,
								Value = "9"
							}
					}
				}
			};
			var updatedEntity = new InventoryFormData() {
				Id = 1,
				QuantityShipped = new Question {Id = 3, Value = "3"},
				BatchNumber = new Question {Id = 4, Value = "4"},
				ReceiptDate = new Question {Id = 5, Value = "5"},
				ShipDate = new Question {Id = 6, Value = "6"},
				MedicationUsage = new List<RepeatableInventoryData> {
					new RepeatableInventoryData {
						Id = 2,
						DateUsed =
							new Question {
								Id = 10,
								Value = "10"
							},
						QuantityUsed =
							new Question {
								Id = 11,
								Value = "11"
							}
					},
					new RepeatableInventoryData {
						Id = 0,
						DateUsed =
							new Question {
								Id = 12,
								Value = "12"
							},
						QuantityUsed =
							new Question {
								Id = 13,
								Value = "13"
							}
					}
				}
			};


			var repository = new InventoryFormDataRepository(dataStorage.Object);
			dataStorage.Setup(ds => ds.GetData<InventoryFormData>()).Returns(new List<InventoryFormData> {entity});

			//Act
			repository.Edit(updatedEntity);
			//Assert
			//Values are equal
			Assert.That(entity.MedicationUsage, Is.Not.Null);
			Assert.That(entity.MedicationUsage.Count, Is.EqualTo(2));
			Assert.That(entity.MedicationUsage.Count, Is.EqualTo(updatedEntity.MedicationUsage.Count));
			Assert.That(entity.MedicationUsage.Any(x => x.Id == 1), Is.False);
			Assert.That(entity.MedicationUsage.Any(x => x.Id == 2), Is.True);
			Assert.That(entity.MedicationUsage.Any(x => x.Id == 3), Is.True);
			Assert.That(entity.MedicationUsage[0].Id, Is.EqualTo(updatedEntity.MedicationUsage[0].Id));
			Assert.That(entity.MedicationUsage[0].DateUsed.Value, Is.EqualTo(updatedEntity.MedicationUsage[0].DateUsed.Value));
			Assert.That(entity.MedicationUsage[0].QuantityUsed.Value,
			            Is.EqualTo(updatedEntity.MedicationUsage[0].QuantityUsed.Value));
			Assert.That(entity.MedicationUsage[1].Id, Is.EqualTo(updatedEntity.MedicationUsage[1].Id));
			Assert.That(entity.MedicationUsage[1].DateUsed.Value, Is.EqualTo(updatedEntity.MedicationUsage[1].DateUsed.Value));
			Assert.That(entity.MedicationUsage[1].QuantityUsed.Value,
			            Is.EqualTo(updatedEntity.MedicationUsage[1].QuantityUsed.Value));
		}
	}
}
