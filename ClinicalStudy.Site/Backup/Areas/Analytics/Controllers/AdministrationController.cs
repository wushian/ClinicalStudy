using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ClinicalStudy.DomainModel;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Site.Areas.Analytics.Models;

namespace ClinicalStudy.Site.Areas.Analytics.Controllers {
	public class AdministrationController : AnalyticsBaseController {
		private readonly IClinicRepository clinicRepository;

		public AdministrationController(IClinicRepository clinicRepository) {
			this.clinicRepository = clinicRepository;
		}

		public ActionResult Clinics() {
			if (Request.IsAjaxRequest())
				return PartialView("_Clinics", GetClinicsModel());
			else
				return View("Clinics", GetClinicsModel());
		}

		public PartialViewResult ClinicsPartial() {
			return PartialView("_ClinicsGrid", GetClinicsModel());
		}

		public ActionResult ClinicDoctors(int? clinicId) {
			if (clinicId.HasValue) {
				Clinic clinic = clinicRepository.GetByKey(clinicId.Value);
				if (clinic != null) {
					var doctors = (from doctor in clinic.Doctors
					                                     select new ClinicDoctorInfoDto {
					                                     	ClinicId = clinic.Id,
					                                     	DoctorId = doctor.Id,
					                                     	FirstName = doctor.FirstName,
					                                     	LastName = doctor.LastName,
					                                     	Role = doctor.Role,
					                                     	Login = doctor.Login,
					                                     	Photo = doctor.Photo,
					                                     	PatientsCount = doctor.Patients.Count
					                                     }).ToList();

					var model = new ClinicDetailsViewModel {ClinicId = clinic.Id, Doctors = doctors};

					return PartialView("_ClinicDoctorsGrid", model);
				}
			}

			var errorModel = new ErrorViewModel {
				Caption = "Clinic is not found",
				ErrorMessage =
					string.Format(
						"Clinic with Id {0} is not found in clinical data", clinicId)
			};
			return PartialView("_ErrorInfo", errorModel);
		}

		private ClinicMasterViewModel GetClinicsModel() {
			var clinics = clinicRepository.GetAll();

			var data = (from clinic in clinics
			                                  select new ClinicMasterInfoDto {
			                                  	ClinicId = clinic.Id,
			                                  	ClinicName = clinic.Caption,
			                                  	DoctorsCount = clinic.Doctors.Count
			                                  }).ToList();

			return new ClinicMasterViewModel {Clinics = data};
		}
	}
}
