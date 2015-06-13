using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web.Mvc;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.DomainModel.ModelBuilders;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.Interface.FormData;
using ClinicalStudy.Site.Areas.DataCapture.Models;
using ClinicalStudy.Site.Areas.DataCapture.Models.Patient;
using ClinicalStudy.Site.Areas.DataCapture.Models.Shared;
using DevExpress.Web.Mvc;

namespace ClinicalStudy.Site.Areas.DataCapture.Controllers {
	public class PatientController : DataCaptureBaseController {
		private readonly IClinicalStudyDesignFactory clinicalStudyDesignFactory;
		private readonly IDemographicFormDataRepository demographicFormDataRepository;
		private readonly IPatientRepository patientRepository;
		private readonly IUserRepository userRepository;

		public PatientController(IUserRepository userRepository,
		                         IPatientRepository patientRepository,
		                         IDemographicFormDataRepository demographicFormDataRepository,
		                         IClinicalStudyDesignFactory clinicalStudyDesignFactory) {
			this.userRepository = userRepository;
			this.patientRepository = patientRepository;
			this.demographicFormDataRepository = demographicFormDataRepository;
			this.clinicalStudyDesignFactory = clinicalStudyDesignFactory;
		}

		public ActionResult PatientHeaderGroup(int patientNumber, string visitName) {
			Patient patient = patientRepository.GetPatientByUniqueNumber(patientNumber);
			HeaderGroupViewModel model = GetHeaderModel(patient, visitName);


			return PartialView("_PatientHeaderGroup", model);
		}

		private HeaderGroupViewModel GetHeaderModel(Patient patient, string visitName) {
			HeaderGroupViewModel model;
			if (patient == null) {
				model = new HeaderGroupViewModel {PatientCaption = "New Patient"};
			}
			else {
				model = new HeaderGroupViewModel {
					PatientNumber = patient.PatientNumber,
					PatientCaption = patient.Caption,
					PatientInitials = patient.PatientInitials,
					ClinicCaption = patient.Doctor.Clinic.Caption,
					VisitName = visitName
				};
			}
			return model;
		}

		public ActionResult PatientGrid(int? patientNumber = null, string activityState = null, bool needFindPatient = false,
		                                int pageSize = 8) {
			User doctor = userRepository.GetUserByLogin(User.Identity.Name);
			if (doctor == null)
				return null;

			int selectedPatientNumber = patientNumber ?? -1;
			if (RouteData.Values["patientNumber"] != null) {
				int.TryParse(RouteData.Values["patientNumber"].ToString(), out selectedPatientNumber);
			}
			bool patientActivityState = true;
			if (string.Equals("inactive", activityState))
				patientActivityState = false;

			if (needFindPatient && selectedPatientNumber > 0) {
				Patient pat = patientRepository.GetPatientByUniqueNumber(selectedPatientNumber);
				if (pat != null) {
					patientActivityState = pat.IsActive;
				}
			}

			var formDataQuery = from formData in demographicFormDataRepository.GetAll()
			                                                select formData;

			var patients = (from patient in patientRepository.GetAll()
			                                       from visit in patient.Visits
			                                       from form in visit.Forms
			                                       from formData in formDataQuery
			                                       where patient.Doctor.Id == doctor.Id
			                                             && visit.VisitTypeValue == (int) VisitType.Baseline
			                                             && form.FormTypeValue == (int) FormType.Demographics
			                                             && form.Id == formData.Form.Id
			                                             && patient.IsActive == patientActivityState
			                                       orderby patient.Caption descending
			                                       select new PatientLinkViewModel {
			                                       	Id = patient.Id,
			                                       	Caption = patient.Caption,
			                                       	PatientNumber = patient.PatientNumber,
			                                       	Sex = formData.Sex.Value,
			                                       	DateOfBirth = formData.DateOfBirth.Value,
			                                       	IsSelected = patient.PatientNumber == selectedPatientNumber
			                                       }).ToList();

			var model = new PatientsListViewModel {
				PatientsList = patients,
				CertainPatientRequested = needFindPatient,
				CertainPatientIsActive = patientActivityState,
				CertainPatientPageIndex =
					needFindPatient ? (patients.FindIndex(x => x.PatientNumber == selectedPatientNumber)/pageSize) : -1
			};

			return PartialView("_PatientGrid", model);
		}

		public ActionResult PatientDataContainer(int patientNumber, string visitName, string formName) {
			DataContainerViewModel model = CreatePatientDataModel(patientNumber, visitName, formName);
			if (model == null) {
				throw new SecurityException("Unauthorized access to patient's data");
			}

			if (Request.IsAjaxRequest())
				return PartialView("_PatientDataContainer", model);
			return View(model);
		}

		private DataContainerViewModel CreatePatientDataModel(int patientNumber, string visitName, string formName) {
			Patient patient = patientRepository.GetPatientByUniqueNumber(patientNumber);
			if (patient == null)
				return new DataContainerViewModel {
					Id = 0,
					PatientNumber = patientRepository.GetMaxPatientNumber() + 1,
					Children = new List<ConteinerChildViewModel>()
				};

			if (patient.Doctor.Login != User.Identity.Name)
				return null;

			var model = new DataContainerViewModel {
				Id = patient.Id,
				PatientCaption = patient.Caption,
				PatientNumber = patient.PatientNumber,
				PatientInitials = patient.PatientInitials,
				SelectedVisitName = visitName,
				SelectedFormName = formName,
				Children = new List<ConteinerChildViewModel>(
					from v in patient.Visits
					orderby v.OrderNo
					select new ConteinerChildViewModel {
						Id = v.Id,
						Caption = v.Caption,
						OrderNo = v.OrderNo
					}
					)
			};
			return model;
		}

		public ActionResult PatientDataPages(int patientNumber, string visitName, string formName) {
			DataContainerViewModel model = CreatePatientDataModel(patientNumber, visitName, formName);
			return PartialView("_PatientDataPages", model);
		}

		public ActionResult PatientDataPanel(int patientNumber, string visitName, string formName) {
			DataContainerViewModel model = CreatePatientDataModel(patientNumber, visitName, formName);
			return PartialView("_PatientDataPagesPanel", model);
		}

		public ActionResult CreatePatient() {
			int maxNumber = patientRepository.GetMaxPatientNumber();
			var model = new PatientViewModel {
				Id = 0,
				PatientNumber = maxNumber + 1,
                Caption = "New Patient",
				IsActive = true
			};

			return PartialView("_EditPatient", model);
		}

		public ActionResult ShowPatient(int patientNumber) {
			return ViewEditPatient(null, patientNumber);
		}

		public ActionResult ViewPatient(int patientNumber) {
			return ViewEditPatient(false, patientNumber);
		}

		public ActionResult EditPatient(int patientNumber) {
			return ViewEditPatient(true, patientNumber);
		}

		public ActionResult ViewEditPatient(bool? isPatientEditing, int patientNumber) {
			Patient patient = patientRepository.GetPatientByUniqueNumber(patientNumber);

			if (patient == null) {
				return RedirectToAction("CreatePatient");
			}

			var model = new PatientViewModel {
				Id = patient.Id,
				Caption = patient.Caption,
				PatientNumber = patient.PatientNumber,
				EnrollDate = patient.EnrollDate,
				IsActive = patient.IsActive,
				IsEnrolled = patient.IsEnrolled,
				PatientInitials = patient.PatientInitials,
				RandomisationDate = patient.RandomisationDate,
				RandomisationNumber = patient.RandomisationNumber
			};


			if (patient.IsCompleted && !(isPatientEditing ?? false))
				return PartialView("_ViewPatient", model);
			return PartialView("_EditPatient", model);
		}


		[HttpPost]
		public ActionResult EditPatient([ModelBinder(typeof (DevExpressEditorsBinder))] PatientViewModel model) {
			Patient patient = patientRepository.GetByKey(model.Id);
			if (patient == null) {
				IClinicalStudyDesign studyDesign = clinicalStudyDesignFactory.Create();
				patient = studyDesign.CreatePatientForDoctor(User.Identity.Name);
				clinicalStudyDesignFactory.Release(studyDesign);
			}
			patient.IsActive = model.IsActive;
			patient.IsEnrolled = model.IsEnrolled;
			patient.EnrollDate = model.EnrollDate;
			patient.PatientInitials = model.PatientInitials;
			patient.RandomisationNumber = model.RandomisationNumber;
			patient.RandomisationDate = model.RandomisationDate;
			patient.IsCompleted = true;
			patientRepository.Edit(patient);
			patientRepository.Save();

			return RedirectToAction("ShowPatient", new {patientNumber = patient.PatientNumber, DXScript = Request["DXScript"]});
		}
	}
}
