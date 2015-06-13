﻿using ClinicalStudy.DomainModel.Enums;

namespace ClinicalStudy.DomainModel.DTOs {
	public class QueryDto
	{
		public string QuestionText { get; set; }
		public FormType FormType { get; set; }
		public string ClinicName { get; set; }
		public string DoctorName { get; set; }
		public int PatientNumber { get; set; }
		public string VisitName { get; set; }
	}
}
