namespace ClinicalStudy.DomainModel.ModelBuilders {
	public interface IChangeNoteBuilder {
		ChangeNote CreateChangeNote(Question question, string originalValue, string newValue, string changeReason);
	}
}
