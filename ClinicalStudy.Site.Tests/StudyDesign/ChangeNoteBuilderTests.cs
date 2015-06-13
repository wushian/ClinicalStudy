using System;
using System.Globalization;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.ModelBuilders;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.DataCapture.Models;
using Moq;
using NUnit.Framework;

namespace ClinicalStudy.Site.Tests.StudyDesign {
	public class ChangeNoteBuilderTests {
		private Mock<IChangeNoteRepository> cnRep;
		private ChangeNoteBuilder builder;

		[SetUp]
		public void TestSetup() {
			cnRep = new Mock<IChangeNoteRepository>();
			builder = new ChangeNoteBuilder(cnRep.Object);
		}




		[Test]
		public void Map_DateQuestion_Correct() {
			//Arrange
			var question = new Question {
				Id = 11,
				DataType = QuestionDataType.Date,
				Value = "Tue, 19 Feb 1980 00:00:00 GMT"
			};

			var model = new ChangeNoteViewModel {
				ChangeReason = "Hi",
				NewValue = "Thu, 04 Jan 1979 00:00:00 GMT",
				OriginalValue = "Tue, 19 Feb 1980 00:00:00 GMT",
				QuestionId = 11
			};

			//Act
			var result = builder.CreateChangeNote(question, model.OriginalValue, model.NewValue, model.ChangeReason);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Question, Is.EqualTo(question));
			Assert.That(result.ChangeDate.Date, Is.EqualTo(DateTime.UtcNow.Date));
			Assert.That(result.ChangeReason, Is.EqualTo(model.ChangeReason));
			Assert.That(result.OriginalValue, Is.EqualTo(new DateTime(1980, 2, 19).ToString(CultureInfo.InvariantCulture)));
			Assert.That(result.NewValue, Is.EqualTo(new DateTime(1979, 1, 4).ToString(CultureInfo.InvariantCulture)));
		}

		[Test]
		public void Map_DateQuestion_Incorrect() {
			//Arrange
			var question = new Question {
				Id = 11,
				DataType = QuestionDataType.Date,
				Value = "Tue, 19 Feb 1980 00:00:00 GMT"
			};

			var model = new ChangeNoteViewModel {
				ChangeReason = "Hi",
				NewValue = "This is not a date",
				OriginalValue = "Tue, 19 Feb 1980 00:00:00 GMT",
			};

			//Act
			var result = builder.CreateChangeNote(question, model.OriginalValue, model.NewValue, model.ChangeReason);

			//Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public void Map_TimeQuestion_Correct() {
			//Arrange
			var question = new Question {
				Id = 11,
				DataType = QuestionDataType.Time,
				Value = "Fri, 01 Jan 0100 11:05:00 GMT"
			};

			var model = new ChangeNoteViewModel {
				ChangeReason = "Hi",
				NewValue = "Fri, 01 Jan 0100 16:55:20 GMT",
				OriginalValue = "Fri, 01 Jan 0100 11:05:00 GMT",
				QuestionId = 11
			};

			//Act
			var result = builder.CreateChangeNote(question, model.OriginalValue, model.NewValue, model.ChangeReason);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Question, Is.EqualTo(question));
			Assert.That(result.ChangeDate.Date, Is.EqualTo(DateTime.UtcNow.Date));
			Assert.That(result.ChangeReason, Is.EqualTo(model.ChangeReason));
			Assert.That(result.OriginalValue,
						Is.EqualTo(new DateTime(100, 1, 1, 11, 05, 0, DateTimeKind.Utc).ToString(CultureInfo.InvariantCulture)));
			Assert.That(result.NewValue,
						Is.EqualTo(new DateTime(100, 1, 1, 16, 55, 20, DateTimeKind.Utc).ToString(CultureInfo.InvariantCulture)));
		}

		[Test]
		public void Map_TimeQuestion_Incorrect() {
			//Arrange
			var question = new Question {
				Id = 11,
				DataType = QuestionDataType.Time,
				Value = "Fri, 01 Jan 0100 11:05:00 GMT"
			};

			var model = new ChangeNoteViewModel {
				ChangeReason = "Hi",
				NewValue = "This is not a time",
				OriginalValue = "Fri, 01 Jan 0100 11:05:00 GMT",
				QuestionId = 11
			};

			//Act
			var result = builder.CreateChangeNote(question, model.OriginalValue, model.NewValue, model.ChangeReason);

			//Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public void Map_StringQuestion_Correct() {
			//Arrange
			var question = new Question {
				Id = 11,
				DataType = QuestionDataType.String,
				Value = "Just string"
			};

			var model = new ChangeNoteViewModel {
				ChangeReason = "Hi",
				NewValue = "New string",
				OriginalValue = "Just string",
				QuestionId = 11
			};

			//Act
			var result = builder.CreateChangeNote(question, model.OriginalValue, model.NewValue, model.ChangeReason);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Question, Is.EqualTo(question));
			Assert.That(result.ChangeDate.Date, Is.EqualTo(DateTime.UtcNow.Date));
			Assert.That(result.ChangeReason, Is.EqualTo(model.ChangeReason));
			Assert.That(result.OriginalValue, Is.EqualTo("Just string"));
			Assert.That(result.NewValue, Is.EqualTo("New string"));
		}

		[Test]
		public void Map_IntegerQuestion_Correct() {
			//Arrange
			var question = new Question {
				Id = 11,
				DataType = QuestionDataType.Integer,
				Value = "135"
			};

			var model = new ChangeNoteViewModel {
				ChangeReason = "Hi",
				NewValue = "357",
				OriginalValue = "135",
				QuestionId = 11
			};

			//Act
			var result = builder.CreateChangeNote(question, model.OriginalValue, model.NewValue, model.ChangeReason);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Question, Is.EqualTo(question));
			Assert.That(result.ChangeDate.Date, Is.EqualTo(DateTime.UtcNow.Date));
			Assert.That(result.ChangeReason, Is.EqualTo(model.ChangeReason));
			Assert.That(result.OriginalValue, Is.EqualTo("135"));
			Assert.That(result.NewValue, Is.EqualTo("357"));
		}

		[Test]
		public void Map_IntegerQuestion_Incorrect() {
			//Arrange
			var question = new Question {
				Id = 11,
				DataType = QuestionDataType.Integer,
				Value = "135"
			};

			var model = new ChangeNoteViewModel {
				ChangeReason = "Hi",
				NewValue = "This is incorrect integer string",
				OriginalValue = "135",
				QuestionId = 11
			};

			//Act
			var result = builder.CreateChangeNote(question, model.OriginalValue, model.NewValue, model.ChangeReason);

			//Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public void Map_NumberQuestion_Correct() {
			//Arrange
			var question = new Question {
				Id = 11,
				DataType = QuestionDataType.Number,
				Value = 3.5m.ToString(CultureInfo.InvariantCulture)
			};

			var model = new ChangeNoteViewModel {
				ChangeReason = "Hi",
				NewValue = 4.84m.ToString(CultureInfo.InvariantCulture),
				OriginalValue = 3.5m.ToString(CultureInfo.InvariantCulture),
				QuestionId = 11
			};

			//Act
			var result = builder.CreateChangeNote(question, model.OriginalValue, model.NewValue, model.ChangeReason);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Question, Is.EqualTo(question));
			Assert.That(result.ChangeDate.Date, Is.EqualTo(DateTime.UtcNow.Date));
			Assert.That(result.ChangeReason, Is.EqualTo(model.ChangeReason));
			Assert.That(result.OriginalValue, Is.EqualTo(3.5m.ToString(CultureInfo.InvariantCulture)));
			Assert.That(result.NewValue, Is.EqualTo(4.84m.ToString(CultureInfo.InvariantCulture)));
		}

		[Test]
		public void Map_NumberQuestion_Incorrect() {
			//Arrange
			var question = new Question {
				Id = 11,
				DataType = QuestionDataType.Number,
				Value = 3.5m.ToString(CultureInfo.InvariantCulture)
			};

			var model = new ChangeNoteViewModel {
				ChangeReason = "Hi",
				NewValue = "This is incorrect number string",
				OriginalValue = 3.5m.ToString(CultureInfo.InvariantCulture),
				QuestionId = 11
			};

			//Act
			ChangeNote result = builder.CreateChangeNote(question, model.OriginalValue, model.NewValue, model.ChangeReason);

			//Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public void Map_EnumQuestion_Correct() {
			//Arrange
			var question = new Question {
				Id = 11,
				DataType = QuestionDataType.Enum,
				Value = "1"
			};

			var model = new ChangeNoteViewModel {
				ChangeReason = "Hi",
				NewValue = "2",
				OriginalValue = "1",
				QuestionId = 11
			};

			//Act
			ChangeNote result = builder.CreateChangeNote(question, model.OriginalValue, model.NewValue, model.ChangeReason);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Question, Is.EqualTo(question));
			Assert.That(result.ChangeDate.Date, Is.EqualTo(DateTime.UtcNow.Date));
			Assert.That(result.ChangeReason, Is.EqualTo(model.ChangeReason));
			Assert.That(result.OriginalValue, Is.EqualTo("1"));
			Assert.That(result.NewValue, Is.EqualTo("2"));
		}

		[Test]
		public void Map_EnumQuestion_Incorrect() {
			//Arrange
			var question = new Question {
				Id = 11,
				DataType = QuestionDataType.Enum,
				Value = "1"
			};

			var model = new ChangeNoteViewModel {
				ChangeReason = "Hi",
				NewValue = "This is incorrect enum string",
				OriginalValue = "1",
				QuestionId = 11
			};

			//Act
			ChangeNote result = builder.CreateChangeNote(question, model.OriginalValue, model.NewValue, model.ChangeReason);

			//Assert
			Assert.That(result, Is.Null);
		} 
	}
}
