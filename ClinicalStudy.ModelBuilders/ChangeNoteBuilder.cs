using System;
using System.Collections.Generic;
using System.Globalization;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.ModelBuilders;
using ClinicalStudy.Repositories.Interface;

namespace ClinicalStudy.ModelBuilders {
	public class ChangeNoteBuilder : IChangeNoteBuilder {

		private IChangeNoteRepository changeNoteRepository;

		public ChangeNoteBuilder(IChangeNoteRepository changeNoteRepository) {
			this.changeNoteRepository = changeNoteRepository;
		}

		public ChangeNote CreateChangeNote(Question question, string originalValue, string newValue, string changeReason) {
			var changeNote = new ChangeNote {
				Question = question,
				ChangeReason = changeReason,
				ChangeDate = DateTime.UtcNow
			};

			switch (question.DataType) {
				case QuestionDataType.Date:
				case QuestionDataType.Time:
					DateTime newDate;
					if (
						!DateTime.TryParse(newValue, CultureInfo.InvariantCulture,
										   DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out newDate))
						return null;
					DateTime originalDate;
					if (
						!DateTime.TryParse(originalValue, CultureInfo.InvariantCulture,
										   DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out originalDate))
						return null;

					changeNote.NewValue = newDate.ToString(CultureInfo.InvariantCulture);
					changeNote.OriginalValue = originalDate.ToString(CultureInfo.InvariantCulture);
					break;
				case QuestionDataType.String:
					changeNote.NewValue = newValue;
					changeNote.OriginalValue = originalValue;
					break;
				case QuestionDataType.Integer:
				case QuestionDataType.Enum:
					int newInt;
					if (!int.TryParse(newValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out newInt))
						return null;
					int originalInt;
					if (!int.TryParse(originalValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out originalInt))
						return null;

					changeNote.NewValue = newInt.ToString(CultureInfo.InvariantCulture);
					changeNote.OriginalValue = originalInt.ToString(CultureInfo.InvariantCulture);
					break;
				case QuestionDataType.Number:
					decimal newNumber;
					if (!decimal.TryParse(newValue, NumberStyles.Number, CultureInfo.InvariantCulture, out newNumber))
						return null;
					decimal originalNumber;
					if (!decimal.TryParse(originalValue, NumberStyles.Number, CultureInfo.InvariantCulture, out originalNumber))
						return null;

					changeNote.NewValue = newNumber.ToString(CultureInfo.InvariantCulture);
					changeNote.OriginalValue = originalNumber.ToString(CultureInfo.InvariantCulture);
					break;
			}
			changeNoteRepository.Add(changeNote);
			changeNoteRepository.Save();
			
			question.ChangeNotes.Add(changeNote);

			return changeNote;
		}

	}
}
