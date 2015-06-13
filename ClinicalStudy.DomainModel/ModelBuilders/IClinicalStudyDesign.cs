namespace ClinicalStudy.DomainModel.ModelBuilders {
	public interface IClinicalStudyDesign {
		Visit AddAdverseEventVisit(int patientId);
		Patient CreatePatientForDoctor(string login);
		Patient CreatePatient(User doctor);
	}
}
