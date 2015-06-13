using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.Enums;
using ClinicalStudy.DomainModel.Enums.Answers;
using ClinicalStudy.DomainModel.Enums.Display;
using ClinicalStudy.DomainModel.FormData;
using ClinicalStudy.DomainModel.ModelBuilders;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.Interface.FormData;
using ClinicalStudy.Site.Areas.DataCapture.ControlHelper;
using ClinicalStudy.Site.Areas.DataCapture.Models;
using ClinicalStudy.Site.Areas.DataCapture.Models.FormData;
using ClinicalStudy.Site.Areas.DataCapture.Models.Query;
using ClinicalStudy.Site.Areas.DataCapture.Models.Shared;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.Mvc;

namespace ClinicalStudy.Site.Areas.DataCapture.Controllers {
	public class FormController : DataCaptureBaseController {
		public const string ChangeNoteRequest = "Please specify the Change Reason for every data change you have made";
		private readonly IAdverseEventFormDataRepository adverseEventFormDataRepository;
		private readonly IAttachmentRepository attachmentRepository;
		private readonly IChangeNoteBuilder changeNoteBuilder;
		private readonly IDemographicFormDataRepository demographicFormDataRepository;
		private readonly IElectrocardiogramFormDataRepository electrocardiogramFormDataRepository;
		private readonly IFormRepository formRepository;
		private readonly IHappinessFormDataRepository happinessFormDataRepository;
		private readonly IInventoryFormDataRepository inventoryFormDataRepository;
		private readonly IQueryRepository queryRepository;
		private readonly IVitalsFormDataRepository vitalsFormDataRepository;

		public FormController(IFormRepository formRepository,
		                      IDemographicFormDataRepository demographicFormDataRepository,
		                      IVitalsFormDataRepository vitalsFormDataRepository,
		                      IHappinessFormDataRepository happinessFormDataRepository,
		                      IElectrocardiogramFormDataRepository electrocardiogramFormDataRepository,
		                      IInventoryFormDataRepository inventoryFormDataRepository,
		                      IAdverseEventFormDataRepository adverseEventFormDataRepository,
		                      IAttachmentRepository attachmentRepository,
		                      IQueryRepository queryRepository,
		                      IChangeNoteBuilder changeNoteBuilder = null) {
			this.formRepository = formRepository;
			this.demographicFormDataRepository = demographicFormDataRepository;
			this.vitalsFormDataRepository = vitalsFormDataRepository;
			this.happinessFormDataRepository = happinessFormDataRepository;
			this.electrocardiogramFormDataRepository = electrocardiogramFormDataRepository;
			this.inventoryFormDataRepository = inventoryFormDataRepository;
			this.adverseEventFormDataRepository = adverseEventFormDataRepository;
			this.attachmentRepository = attachmentRepository;
			this.queryRepository = queryRepository;
			this.changeNoteBuilder = changeNoteBuilder;
			HtmlEditorAdapter = new HtmlEditorAdapter();
		}

		public IHtmlEditorAdapter HtmlEditorAdapter { get; set; }

		public ActionResult EditForm(int patientNumber, string visitName, string formName) {
			return ViewEditForm(true, patientNumber, visitName, formName);
		}

		public ActionResult ViewForm(int patientNumber, string visitName, string formName) {
			return ViewEditForm(false, patientNumber, visitName, formName);
		}


		public ActionResult ShowForm(int patientNumber, string visitName, string formName) {
			return ViewEditForm(null, patientNumber, visitName, formName);
		}

		[HttpPost]
		public ActionResult EditDemographicForm(
			[ModelBinder(typeof (DevExpressEditorsBinder))] DemographicFormViewModel viewModel) {
			if (!ModelState.IsValid)
				return PartialView("_EditDemographicForm", viewModel);


			Form form = formRepository.GetByKey(viewModel.FormId);
			if (form == null)
				return ComposeErrorInfoView(viewModel.FormCaption, viewModel.FormId);

			DemographicFormData formData = demographicFormDataRepository.GetByKey(viewModel.Id);
			if (formData == null)
				return ComposeErrorInfoView(viewModel.FormCaption, viewModel.FormId);


			if (form.FormState == FormState.Completed) {
				viewModel.IsCompleted = true;
				CheckDemograpihcFormChangeReasons(viewModel, formData);
				if (viewModel.DataChangeReasonRequired) {
					ModelState.AddModelError(string.Empty, ChangeNoteRequest);
					return PartialView("_EditDemographicForm", viewModel);
				}
			}

			MapDemographicViewModelToFormData(viewModel, formData);
			SaveFormDataChangeReasons(viewModel, formData);
			demographicFormDataRepository.Edit(formData);
			demographicFormDataRepository.Save();

			return SaveFormAsCompletedAndRedirect(form);
		}


		private static void CheckDemograpihcFormChangeReasons(DemographicFormViewModel viewModel, DemographicFormData formData) {
			Debug.Assert(viewModel.DateOfBirth.HasValue);

			if (viewModel.ChangeInfos == null)
				viewModel.ChangeInfos = new List<ChangeNoteViewModel>();

			int oldRaceId;
			int.TryParse(formData.Race.Value, out oldRaceId);
			int oldSexId;
			int.TryParse(formData.Sex.Value, out oldSexId);
			DateTime originalDateOfBirth;
			DateTime.TryParse(formData.DateOfBirth.Value, out originalDateOfBirth);

			AddChangeReasonRequestIfRequired(formData.DateOfBirth, originalDateOfBirth.ToShortDateString(),
				viewModel.DateOfBirth.Value.ToShortDateString(), viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.Race, EnumHelper.GetDescription((Race) oldRaceId),
				EnumHelper.GetDescription((Race) viewModel.Race), viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.Sex, EnumHelper.GetDescription((Gender) oldSexId),
				EnumHelper.GetDescription((Gender) viewModel.Sex), viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.Other, formData.Other.Value, viewModel.Other, viewModel.ChangeInfos);

			if (viewModel.ChangeInfos.Any(ci => string.IsNullOrWhiteSpace(ci.ChangeReason)))
				viewModel.DataChangeReasonRequired = true;
		}


		[HttpPost]
		public ActionResult EditVitalsForm([ModelBinder(typeof (DevExpressEditorsBinder))] VitalsFormViewModel viewModel) {
			if (!ModelState.IsValid)
				return PartialView("_EditVitalsForm", viewModel);

			Form form = formRepository.GetByKey(viewModel.FormId);
			if (form == null)
				return ComposeErrorInfoView(viewModel.FormCaption, viewModel.FormId);

			VitalsFormData formData = vitalsFormDataRepository.GetByKey(viewModel.Id);
			if (formData == null)
				return ComposeErrorInfoView(viewModel.FormCaption, viewModel.FormId);

			if (form.FormState == FormState.Completed) {
				viewModel.IsCompleted = true;
				CheckVitalsFormChangeReasons(viewModel, formData);
				if (viewModel.DataChangeReasonRequired) {
					ModelState.AddModelError(string.Empty, ChangeNoteRequest);
					return PartialView("_EditVitalsForm", viewModel);
				}
			}

			MapVitalsViewModelToFormData(viewModel, formData);
			SaveFormDataChangeReasons(viewModel, formData);
			vitalsFormDataRepository.Edit(formData);
			vitalsFormDataRepository.Save();

			return SaveFormAsCompletedAndRedirect(form);
		}

		private void CheckVitalsFormChangeReasons(VitalsFormViewModel viewModel, VitalsFormData formData) {
			Debug.Assert(viewModel.ActualTime.HasValue);

			if (viewModel.ChangeInfos == null)
				viewModel.ChangeInfos = new List<ChangeNoteViewModel>();

			DateTime originalActualTime;
			DateTime.TryParse(formData.ActualTime.Value, out originalActualTime);

			AddChangeReasonRequestIfRequired(formData.ActualTime, originalActualTime.ToShortTimeString(),
				viewModel.ActualTime.Value.ToShortTimeString(), viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.Weight, formData.Weight.Value, viewModel.Weight.ToString(),
				viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.Height, formData.Height.Value, viewModel.Height.ToString(),
				viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.HeartRate, formData.HeartRate.Value, viewModel.HeartRate.ToString(),
				viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.Temperature, formData.Temperature.Value, viewModel.Temperature.ToString(),
				viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.BloodPressureSystolic, formData.BloodPressureSystolic.Value,
				viewModel.BloodPressureSystolic, viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.BloodPressureDiastolic, formData.BloodPressureDiastolic.Value,
				viewModel.BloodPressureDiastolic, viewModel.ChangeInfos);

			if (viewModel.ChangeInfos.Any(ci => string.IsNullOrWhiteSpace(ci.ChangeReason)))
				viewModel.DataChangeReasonRequired = true;
		}

		[HttpPost]
		public ActionResult EditHappinessForm([ModelBinder(typeof (DevExpressEditorsBinder))] HappinessFormViewModel viewModel) {
			if (!ModelState.IsValid)
				return PartialView("_EditHappinessForm", viewModel);

			Form form = formRepository.GetByKey(viewModel.FormId);
			if (form == null)
				return ComposeErrorInfoView(viewModel.FormCaption, viewModel.FormId);

			HappinessFormData formData = happinessFormDataRepository.GetByKey(viewModel.Id);
			if (formData == null)
				return ComposeErrorInfoView(viewModel.FormCaption, viewModel.FormId);

			if (form.FormState == FormState.Completed) {
				viewModel.IsCompleted = true;
				CheckHappinessFormChangeReasons(viewModel, formData);
				if (viewModel.DataChangeReasonRequired) {
					ModelState.AddModelError(string.Empty, ChangeNoteRequest);
					return PartialView("_EditHappinessForm", viewModel);
				}
			}


			MapHappinessViewModelToFormData(viewModel, formData);
			SaveFormDataChangeReasons(viewModel, formData);
			happinessFormDataRepository.Edit(formData);
			happinessFormDataRepository.Save();

			return SaveFormAsCompletedAndRedirect(form);
		}

		private void CheckHappinessFormChangeReasons(HappinessFormViewModel viewModel, HappinessFormData formData) {
			if (viewModel.ChangeInfos == null)
				viewModel.ChangeInfos = new List<ChangeNoteViewModel>();

			int originalHappinessLevel;
			int.TryParse(formData.HappinessLevel.Value, out originalHappinessLevel);
			AddChangeReasonRequestIfRequired(formData.HappinessLevel,
				EnumHelper.GetDescription((HappinessLevel) originalHappinessLevel),
				EnumHelper.GetDescription((HappinessLevel) viewModel.HappinessLevel), viewModel.ChangeInfos);

			if (viewModel.ChangeInfos.Any(ci => string.IsNullOrWhiteSpace(ci.ChangeReason)))
				viewModel.DataChangeReasonRequired = true;
		}

		[HttpPost]
		public ActionResult EditElectrocardiogramForm(
			[ModelBinder(typeof (DevExpressEditorsBinder))] ElectrocardiogramFormViewModel viewModel) {
			if (!ModelState.IsValid)
				return PartialView("_EditElectrocardiogramForm", viewModel);

			Form form = formRepository.GetByKey(viewModel.FormId);
			if (form == null)
				return ComposeErrorInfoView(viewModel.FormCaption, viewModel.FormId);

			ElectrocardiogramFormData formData = electrocardiogramFormDataRepository.GetByKey(viewModel.Id);
			if (formData == null)
				return ComposeErrorInfoView(viewModel.FormCaption, viewModel.FormId);

			Attachment attachment = null;
			if (viewModel.AttachmentId.HasValue) {
				attachment = attachmentRepository.GetByKey(viewModel.AttachmentId.Value);
			}

			if (form.FormState == FormState.Completed) {
				viewModel.IsCompleted = true;
				CheckElectrocardiogramFormChangeReasons(viewModel, formData, attachment);
				if (viewModel.DataChangeReasonRequired) {
					ModelState.AddModelError(string.Empty, ChangeNoteRequest);
					return PartialView("_EditElectrocardiogramForm", viewModel);
				}
			}


			MapElectrocardiogramViewModelToFormData(viewModel, formData, attachment);
			SaveFormDataChangeReasons(viewModel, formData);
			electrocardiogramFormDataRepository.Edit(formData);
			electrocardiogramFormDataRepository.Save();

			return SaveFormAsCompletedAndRedirect(form);
		}

		private void CheckElectrocardiogramFormChangeReasons(ElectrocardiogramFormViewModel viewModel,
		                                                     ElectrocardiogramFormData formData, Attachment attachment) {
			Debug.Assert(viewModel.ElectrocardiogramActualTime.HasValue);
			if (viewModel.ChangeInfos == null)
				viewModel.ChangeInfos = new List<ChangeNoteViewModel>();

			DateTime originalActualTime;
			DateTime.TryParse(formData.ElectrocardiogramActualTime.Value, out originalActualTime);
			string newFileName = attachment != null ? attachment.FileName : "N/A";
			string originalFileName = formData.ElectrocardiogramAttachment.File != null
			                          	? formData.ElectrocardiogramAttachment.File.FileName
			                          	: "N/A";


			AddChangeReasonRequestIfRequired(formData.ElectrocardiogramActualTime, originalActualTime.ToShortTimeString(),
				viewModel.ElectrocardiogramActualTime.Value.ToShortTimeString(), viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.ElectrocardiogramAttachment, originalFileName, newFileName,
				viewModel.ChangeInfos);

			if (viewModel.ChangeInfos.Any(ci => string.IsNullOrWhiteSpace(ci.ChangeReason)))
				viewModel.DataChangeReasonRequired = true;
		}

		[HttpPost]
		public ActionResult EditInventoryForm([ModelBinder(typeof (DevExpressEditorsBinder))] InventoryFormViewModel viewModel) {
			if (!ModelState.IsValid)
				return PartialView("_EditInventoryForm", viewModel);

			Form form = formRepository.GetByKey(viewModel.FormId);
			if (form == null)
				return ComposeErrorInfoView(viewModel.FormCaption, viewModel.FormId);

			InventoryFormData formData = inventoryFormDataRepository.GetByKey(viewModel.Id);
			if (formData == null)
				return ComposeErrorInfoView(viewModel.FormCaption, viewModel.FormId);

			var repeatableInventoryViewModel = Session[String.Format("ird_{0}", viewModel.FormId)];

			viewModel.MedicationUsage = repeatableInventoryViewModel != null ? repeatableInventoryViewModel as List<RepeatableInventoryDataViewModel> : new List<RepeatableInventoryDataViewModel>();

			if (form.FormState == FormState.Completed) {
				viewModel.IsCompleted = true;
				CheckInventoryFormChangeReasons(viewModel, formData);
				if (viewModel.DataChangeReasonRequired) {
					ModelState.AddModelError(string.Empty, ChangeNoteRequest);
					return PartialView("_EditInventoryForm", viewModel);
				}
			}

			MapInventoryViewModelToFormData(viewModel, formData);
			MapRepeatableInventoryViewModelToFormData(viewModel.MedicationUsage, formData.MedicationUsage);
			SaveFormDataChangeReasons(viewModel, formData);
			inventoryFormDataRepository.Edit(formData);
			inventoryFormDataRepository.Save();

			return SaveFormAsCompletedAndRedirect(form);
		}

		private void CheckInventoryFormChangeReasons(InventoryFormViewModel viewModel, InventoryFormData formData) {
			Debug.Assert(viewModel.ShipDate.HasValue);
			Debug.Assert(viewModel.ReceiptDate.HasValue);

			if (viewModel.ChangeInfos == null)
				viewModel.ChangeInfos = new List<ChangeNoteViewModel>();

			DateTime originalShipDate = DateTime.Parse(formData.ShipDate.Value);
			DateTime originalReceiptDate = DateTime.Parse(formData.ReceiptDate.Value);

			AddChangeReasonRequestIfRequired(formData.ShipDate, originalShipDate.ToShortDateString(),
				viewModel.ShipDate.Value.ToShortDateString(), viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.ReceiptDate, originalReceiptDate.ToShortDateString(),
				viewModel.ReceiptDate.Value.ToShortDateString(), viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.BatchNumber, formData.BatchNumber.Value, viewModel.BatchNumber.ToString(),
				viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.QuantityShipped, formData.QuantityShipped.Value,
				viewModel.QuantityShipped.ToString(), viewModel.ChangeInfos);

			if (viewModel.ChangeInfos.Any(ci => string.IsNullOrWhiteSpace(ci.ChangeReason)))
				viewModel.DataChangeReasonRequired = true;
		}

	
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult EditAdverseEventForm(
			[ModelBinder(typeof (DevExpressEditorsBinder))] AdverseEventFormViewModel viewModel) {
			if (!ModelState.IsValid)
				return PartialView("_EditAdverseEventForm", viewModel);

			Form form = formRepository.GetByKey(viewModel.FormId);
			if (form == null)
				return ComposeErrorInfoView(viewModel.FormCaption, viewModel.FormId);

			AdverseEventFormData formData = adverseEventFormDataRepository.GetByKey(viewModel.Id);
			if (formData == null)
				return ComposeErrorInfoView(viewModel.FormCaption, viewModel.FormId);


			bool isValid;
			ViewData["ActiveView"] = HtmlEditorAdapter.GetActiveView("AdverseExperience");
			string htmlValue = HtmlEditorAdapter.GetHtmlView("AdverseExperience", null, null, null, out isValid);
			ViewData["Html"] = htmlValue;
			if (!isValid)
				return PartialView("_EditAdverseEventForm", viewModel);

			viewModel.AdverseExperience = htmlValue;

			if (form.FormState == FormState.Completed) {
				viewModel.IsCompleted = true;
				CheckAdverseEventFormChangeReasons(viewModel, formData);
				if (viewModel.DataChangeReasonRequired) {
					ModelState.AddModelError(string.Empty, ChangeNoteRequest);
					return PartialView("_EditAdverseEventForm", viewModel);
				}
			}

			MapAdverseEventViewModelToFormData(viewModel, formData);
			SaveFormDataChangeReasons(viewModel, formData);
			adverseEventFormDataRepository.Edit(formData);
			adverseEventFormDataRepository.Save();

			return SaveFormAsCompletedAndRedirect(form);
		}

		private void CheckAdverseEventFormChangeReasons(AdverseEventFormViewModel viewModel, AdverseEventFormData formData) {
			Debug.Assert(viewModel.OnsetDate.HasValue);
			Debug.Assert(viewModel.OnsetTime.HasValue);
			Debug.Assert(viewModel.EndDate.HasValue);
			Debug.Assert(viewModel.EndTime.HasValue);

			if (viewModel.ChangeInfos == null)
				viewModel.ChangeInfos = new List<ChangeNoteViewModel>();

			DateTime originalOnsetDate = DateTime.Parse(formData.OnsetDate.Value);
			DateTime originalEndDate = DateTime.Parse(formData.EndDate.Value);
			int originalInensityId;
			int.TryParse(formData.Intensity.Value, out originalInensityId);
			int originalOutcomeId;
			int.TryParse(formData.Outcome.Value, out originalOutcomeId);
			int originalRelanshionshipId;
			int.TryParse(formData.RelationshipToInvestigationalDrug.Value, out originalRelanshionshipId);

			DateTime originalOnsetTime = DateTime.Parse(formData.OnsetTime.Value);
			DateTime originalEndTime = DateTime.Parse(formData.EndTime.Value);


			AddChangeReasonRequestIfRequired(formData.OnsetDate, originalOnsetDate.ToShortDateString(),
				viewModel.OnsetDate.Value.ToShortDateString(), viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.OnsetTime, originalOnsetTime.ToShortTimeString(),
				viewModel.OnsetTime.Value.ToShortTimeString(), viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.EndDate, originalEndDate.ToShortDateString(),
				viewModel.EndDate.Value.ToShortDateString(), viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.EndTime, originalEndTime.ToShortTimeString(),
				viewModel.EndTime.Value.ToShortTimeString(), viewModel.ChangeInfos);

			AddChangeReasonRequestIfRequired(formData.AdverseExperience, formData.AdverseExperience.Value,
				viewModel.AdverseExperience, viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.Intensity,
				EnumHelper.GetDescription((AdverseEventIntensity) originalInensityId),
				EnumHelper.GetDescription((AdverseEventIntensity) viewModel.Intensity), viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.Outcome, EnumHelper.GetDescription((AdverseEventOutcome) originalOutcomeId),
				EnumHelper.GetDescription((AdverseEventOutcome) viewModel.Outcome), viewModel.ChangeInfos);
			AddChangeReasonRequestIfRequired(formData.RelationshipToInvestigationalDrug,
				EnumHelper.GetDescription((AdverseEventRelanshionship) originalRelanshionshipId),
				EnumHelper.GetDescription((AdverseEventRelanshionship) viewModel.RelationshipToInvestigationalDrug),
				viewModel.ChangeInfos);


			if (viewModel.ChangeInfos.Any(ci => string.IsNullOrWhiteSpace(ci.ChangeReason)))
				viewModel.DataChangeReasonRequired = true;
		}

		public ActionResult AdverseExperienceHtml() {
			return PartialView("_EditAdverseExperienceData");
		}

		[ValidateInput(false)]
		public ActionResult InlineViewingRepeatableInventoryData(int? inventoryFormId) {
			InventoryFormViewModel iModel = GetInventoryViewModel(inventoryFormId);
			if (iModel == null) {
				ViewData["EditError"] = "Sorry, internal error occured!";
				return PartialView("_ViewRepeatableInventoryData", new InventoryFormViewModel{ FormId = inventoryFormId??0, MedicationUsage = new List<RepeatableInventoryDataViewModel>()});
			}

			return PartialView("_ViewRepeatableInventoryData", iModel);
		}

		[ValidateInput(false)]
		public ActionResult InlineEditingRepeatableInventoryData(int? inventoryFormId) {

			var model = Session[String.Format("ird_{0}", inventoryFormId ?? 0)];
			if(model == null) {
				ViewData["EditError"] = "Sorry, internal error occured!";
				return PartialView("_EditRepeatableInventoryData", new InventoryFormViewModel{FormId = inventoryFormId ?? 0, MedicationUsage = new List<RepeatableInventoryDataViewModel>()});
			}

			return PartialView("_EditRepeatableInventoryData", new InventoryFormViewModel { FormId = inventoryFormId ?? 0, MedicationUsage = model as List<RepeatableInventoryDataViewModel> });
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult InlineEditingAddNewRepeatableInventoryData(int? inventoryFormId,
		                                                               [ModelBinder(typeof (DevExpressEditorsBinder))] RepeatableInventoryDataViewModel inventoryData) {
			var model = Session[String.Format("ird_{0}", inventoryFormId ?? 0)];
			if (model == null)
			{
				ViewData["EditError"] = "Sorry, internal error occured!";
				return PartialView("_EditRepeatableInventoryData", new InventoryFormViewModel { FormId = inventoryFormId ?? 0, MedicationUsage = new List<RepeatableInventoryDataViewModel>() });
			}

			var iModel = model as List<RepeatableInventoryDataViewModel>;

			if (ModelState.IsValid) {
				inventoryData.InnerId = iModel.Count > 0 ? iModel.Max(x => x.InnerId) + 1 : 1;
				iModel.Add(inventoryData);
			}
			else {
				ViewData["EditError"] = "Please, correct all errors.";
			}

			return PartialView("_EditRepeatableInventoryData", new InventoryFormViewModel { FormId = inventoryFormId ?? 0, MedicationUsage = iModel });
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult InlineEditingUpdateRepeatableInventoryData(int? inventoryFormId,
		                                                               [ModelBinder(typeof (DevExpressEditorsBinder))] RepeatableInventoryDataViewModel inventoryData) {

			var model = Session[String.Format("ird_{0}", inventoryFormId ?? 0)];
			if (model == null)
			{
				ViewData["EditError"] = "Sorry, internal error occured!";
				return PartialView("_EditRepeatableInventoryData", new InventoryFormViewModel { FormId = inventoryFormId ?? 0, MedicationUsage = new List<RepeatableInventoryDataViewModel>() });
			}

			var iModel = model as List<RepeatableInventoryDataViewModel>;
			if (ModelState.IsValid) {

				var data = iModel.FirstOrDefault(x => x.InnerId == inventoryData.InnerId);


				if (data != null) {
					data.DateUsed = inventoryData.DateUsed;
					data.QuantityUsed = inventoryData.QuantityUsed;
				}
				else {
					inventoryData.InnerId = inventoryData.InnerId == 0 ? (iModel.Count > 0 ? iModel.Max(x => x.InnerId) + 1 : 1) : inventoryData.InnerId;
					iModel.Add(inventoryData);
				}
			}
			else {
				ViewData["EditError"] = "Please, correct all errors.";
			}
			return PartialView("_EditRepeatableInventoryData", new InventoryFormViewModel { FormId = inventoryFormId ?? 0, MedicationUsage = iModel });
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult InlineEditingDeleteRepeatableInventoryData(int? inventoryFormId, int innerId) {
			var model = Session[String.Format("ird_{0}", inventoryFormId ?? 0)];
			if (model == null)
			{
				ViewData["EditError"] = "Sorry, internal error occured!";
				return PartialView("_EditRepeatableInventoryData", new InventoryFormViewModel { FormId = inventoryFormId ?? 0, MedicationUsage = new List<RepeatableInventoryDataViewModel>() });
			}

			var iModel = model as List<RepeatableInventoryDataViewModel>;
			if (ModelState.IsValid)
			{
				iModel.RemoveAll(x => x.InnerId == innerId);
			}
			else
			{
				ViewData["EditError"] = "Please, correct all errors.";
			}
			return PartialView("_EditRepeatableInventoryData", new InventoryFormViewModel { FormId = inventoryFormId ?? 0, MedicationUsage = iModel });
		}

		public DemographicFormViewModel MapDemographicFormDataToViewModel(DemographicFormData dFormData) {
			return new DemographicFormViewModel {
				Id = dFormData.Id,
				FormId = dFormData.Form.Id,
				FormCaption = dFormData.Form.Caption,
				IsCompleted = dFormData.Form.FormState == FormState.Completed,
				VisitName = dFormData.Form.Visit.Caption,
				PatientNumber = dFormData.Form.Visit.Patient.PatientNumber,
				DateOfBirth = ParseToNullableDateTime(dFormData.DateOfBirth.Value),
				DateOfBirthQuestionId = dFormData.DateOfBirth.Id,
				Other = dFormData.Other.Value,
				OtherQuestionId = dFormData.Other.Id,
				Race = ParseToInt(dFormData.Race.Value),
				RaceQuestionId = dFormData.Race.Id,
				Sex = ParseToInt(dFormData.Sex.Value),
				SexQuestionId = dFormData.Sex.Id
			};
		}

		public void MapDemographicViewModelToFormData(DemographicFormViewModel dViewModel, DemographicFormData dFormData) {
			dFormData.DateOfBirth.Value = CorrectDateTimeString(dViewModel.DateOfBirth);
			dFormData.Other.Value = dViewModel.Other;
			dFormData.Race.Value = CorrectIntString(dViewModel.Race);
			dFormData.Sex.Value = CorrectIntString(dViewModel.Sex);
		}

		public VitalsFormViewModel MapVitalsFormDataToViewModel(VitalsFormData vFormData) {
			return new VitalsFormViewModel {
				Id = vFormData.Id,
				FormId = vFormData.Form.Id,
				FormCaption = vFormData.Form.Caption,
				IsCompleted = vFormData.Form.FormState == FormState.Completed,
				VisitName = vFormData.Form.Visit.Caption,
				PatientNumber = vFormData.Form.Visit.Patient.PatientNumber,
				ActualTime = ParseToNullableDateTime(vFormData.ActualTime.Value),
				ActualTimeQuestionId = vFormData.ActualTime.Id,
				Height = ParseToDecimal(vFormData.Height.Value),
				HeightQuestionId = vFormData.Height.Id,
				Weight = ParseToDecimal(vFormData.Weight.Value),
				WeightQuestionId = vFormData.Weight.Id,
				Temperature = ParseToDecimal(vFormData.Temperature.Value),
				TemperatureQuestionId = vFormData.Temperature.Id,
				HeartRate = ParseToInt(vFormData.HeartRate.Value),
				HeartRateQuestionId = vFormData.HeartRate.Id,
				BloodPressureSystolic = vFormData.BloodPressureSystolic.Value,
				BloodPressureSystolicQuestionId = vFormData.BloodPressureSystolic.Id,
				BloodPressureDiastolic = vFormData.BloodPressureDiastolic.Value,
				BloodPressureDiastolicQuestionId = vFormData.BloodPressureDiastolic.Id,
			};
		}

		public void MapVitalsViewModelToFormData(VitalsFormViewModel vViewModel, VitalsFormData vFormData) {
			vFormData.ActualTime.Value = CorrectDateTimeString(vViewModel.ActualTime);
			vFormData.Height.Value = CorrectDecimalString(vViewModel.Height);
			vFormData.Weight.Value = CorrectDecimalString(vViewModel.Weight);
			vFormData.Temperature.Value = CorrectDecimalString(vViewModel.Temperature);
			vFormData.HeartRate.Value = CorrectIntString(vViewModel.HeartRate);
			vFormData.BloodPressureSystolic.Value = vViewModel.BloodPressureSystolic;
			vFormData.BloodPressureDiastolic.Value = vViewModel.BloodPressureDiastolic;
		}

		public HappinessFormViewModel MapHappinessFormDataToViewModel(HappinessFormData hFormData) {
			return new HappinessFormViewModel {
				Id = hFormData.Id,
				FormId = hFormData.Form.Id,
				FormCaption = hFormData.Form.Caption,
				IsCompleted = hFormData.Form.FormState == FormState.Completed,
				VisitName = hFormData.Form.Visit.Caption,
				PatientNumber = hFormData.Form.Visit.Patient.PatientNumber,
				HappinessLevel = ParseToInt(hFormData.HappinessLevel.Value),
				HappinessLevelQuestionId = hFormData.HappinessLevel.Id
			};
		}

		public void MapHappinessViewModelToFormData(HappinessFormViewModel hViewModel, HappinessFormData hFormData) {
			hFormData.HappinessLevel.Value = CorrectIntString(hViewModel.HappinessLevel);
		}

		public ElectrocardiogramFormViewModel MapElectrocardiogramFormDataToViewModel(ElectrocardiogramFormData eFormData) {
			var model = new ElectrocardiogramFormViewModel {
				Id = eFormData.Id,
				FormId = eFormData.Form.Id,
				FormCaption = eFormData.Form.Caption,
				IsCompleted = eFormData.Form.FormState == FormState.Completed,
				VisitName = eFormData.Form.Visit.Caption,
				PatientNumber = eFormData.Form.Visit.Patient.PatientNumber,
				ElectrocardiogramActualTime = ParseToNullableDateTime(eFormData.ElectrocardiogramActualTime.Value),
				ActualTimeQuestionId = eFormData.ElectrocardiogramActualTime.Id,
				ElectrocardiogramValidationSettings = AttachmentController.ElectrocardiogramValidationSettings,
			};
			if (eFormData.ElectrocardiogramAttachment != null) {
				model.ElectrocardiogramAttachmentQuestionId = eFormData.ElectrocardiogramAttachment.Id;
				Attachment attachment = eFormData.ElectrocardiogramAttachment.File;
				if (attachment != null) {
					model.AttachmentId = attachment.Id;
					model.AttachmentName = attachment.FileName;
				}
			}

			return model;
		}

		public void MapElectrocardiogramViewModelToFormData(ElectrocardiogramFormViewModel eViewModel,
		                                                    ElectrocardiogramFormData eFormData,
		                                                    Attachment attachment) {
			eFormData.ElectrocardiogramActualTime.Value = CorrectDateTimeString(eViewModel.ElectrocardiogramActualTime);
			eFormData.ElectrocardiogramAttachment.File = attachment;
		}

		public InventoryFormViewModel MapInventoryFormDataToViewModel(InventoryFormData iFormData) {
			var model = new InventoryFormViewModel {
				Id = iFormData.Id,
				FormId = iFormData.Form.Id,
				FormCaption = iFormData.Form.Caption,
				IsCompleted = iFormData.Form.FormState == FormState.Completed,
				VisitName = iFormData.Form.Visit.Caption,
				PatientNumber = iFormData.Form.Visit.Patient.PatientNumber,
				BatchNumber = ParseToInt(iFormData.BatchNumber.Value),
				BatchNumberQuestionId = iFormData.BatchNumber.Id,
				ReceiptDate = ParseToNullableDateTime(iFormData.ReceiptDate.Value),
				ReceiptDateQuestionId = iFormData.ReceiptDate.Id,
				ShipDate = ParseToNullableDateTime(iFormData.ShipDate.Value),
				ShipDateQuestionId = iFormData.ShipDate.Id,
				QuantityShipped = ParseToDecimal(iFormData.QuantityShipped.Value),
				QuantityShippedQuestionId = iFormData.QuantityShipped.Id,
				MedicationUsage = new List<RepeatableInventoryDataViewModel>()
			};
			if (iFormData.MedicationUsage != null) {
				var cnt = 1;
				foreach (RepeatableInventoryData repeatableInventoryData in iFormData.MedicationUsage) {
					model.MedicationUsage.Add(
						new RepeatableInventoryDataViewModel {
							Id = repeatableInventoryData.Id,
							InnerId = cnt++,
							DateUsed =
								ParseToNullableDateTime(
									repeatableInventoryData.DateUsed.Value),
							DateUsedQuestionId = repeatableInventoryData.DateUsed.Id,
							QuantityUsed =
								ParseToDecimal(
									repeatableInventoryData.QuantityUsed.Value),
							QuantityUsedQuestionId = repeatableInventoryData.QuantityUsed.Id
						});
				}
			}
			return model;
		}

		public void MapInventoryViewModelToFormData(InventoryFormViewModel iViewModel, InventoryFormData iFormData) {
			iFormData.BatchNumber.Value = CorrectIntString(iViewModel.BatchNumber);
			iFormData.ReceiptDate.Value = CorrectDateTimeString(iViewModel.ReceiptDate);
			iFormData.ShipDate.Value = CorrectDateTimeString(iViewModel.ShipDate);
			iFormData.QuantityShipped.Value = CorrectDecimalString(iViewModel.QuantityShipped);
		}

		public void MapRepeatableInventoryViewModelToFormData(List<RepeatableInventoryDataViewModel> iViewModel, List<RepeatableInventoryData> iFormData) {

			iFormData.RemoveAll(x => !iViewModel.Select(y => y.Id).Contains(x.Id));

			foreach (var repeatableInventoryData in iFormData) {
				var viewModel = iViewModel.FirstOrDefault(x => x.Id == repeatableInventoryData.Id);
				if(viewModel != null) {
					repeatableInventoryData.DateUsed.Value = CorrectDateTimeString(viewModel.DateUsed);
					repeatableInventoryData.QuantityUsed.Value = CorrectDecimalString(viewModel.QuantityUsed);
				}
			}

			var newData = iViewModel.Where(x=>x.Id == 0).Select(viewModel => new RepeatableInventoryData
			{
				Id = viewModel.Id,
				DateUsed =
					new Question {
						DataType = QuestionDataType.Date,
						Value =
							CorrectDateTimeString(viewModel.DateUsed)
					},
				QuantityUsed =
					new Question {
						DataType = QuestionDataType.Number,
						Value =
							CorrectDecimalString(viewModel.QuantityUsed)
					}
			}).ToList();

			iFormData.AddRange(newData);
		}

		public AdverseEventFormViewModel MapAdverseEventFormDataToViewModel(AdverseEventFormData aFormData) {
			return new AdverseEventFormViewModel {
				Id = aFormData.Id,
				FormId = aFormData.Form.Id,
				FormCaption = aFormData.Form.Caption,
				IsCompleted = aFormData.Form.FormState == FormState.Completed,
				VisitName = aFormData.Form.Visit.Caption,
				PatientNumber = aFormData.Form.Visit.Patient.PatientNumber,
				AdverseExperience = aFormData.AdverseExperience.Value,
				AdverseExperienceQuestionId = aFormData.AdverseExperience.Id,
				OnsetDate = ParseToNullableDateTime(aFormData.OnsetDate.Value),
				OnsetDateQuestionId = aFormData.OnsetDate.Id,
				OnsetTime = ParseToNullableDateTime(aFormData.OnsetTime.Value),
				OnsetTimeQuestionId = aFormData.OnsetTime.Id,
				EndDate = ParseToNullableDateTime(aFormData.EndDate.Value),
				EndDateQuestionId = aFormData.EndDate.Id,
				EndTime = ParseToNullableDateTime(aFormData.EndTime.Value),
				EndTimeQuestionId = aFormData.EndTime.Id,
				Outcome = ParseToInt(aFormData.Outcome.Value),
				OutcomeQuestionId = aFormData.Outcome.Id,
				Intensity = ParseToInt(aFormData.Intensity.Value),
				IntensityQuestionId = aFormData.Intensity.Id,
				RelationshipToInvestigationalDrug =
					ParseToInt(aFormData.RelationshipToInvestigationalDrug.Value),
				RelationshipToInvestigationalDrugQuestionId = aFormData.RelationshipToInvestigationalDrug.Id
			};
		}

		public void MapAdverseEventViewModelToFormData(AdverseEventFormViewModel aViewModel, AdverseEventFormData aFormData) {
			aFormData.AdverseExperience.Value = aViewModel.AdverseExperience;
			aFormData.OnsetDate.Value = CorrectDateTimeString(aViewModel.OnsetDate);
			aFormData.OnsetTime.Value = CorrectDateTimeString(aViewModel.OnsetTime);
			aFormData.EndDate.Value = CorrectDateTimeString(aViewModel.EndDate);
			aFormData.EndTime.Value = CorrectDateTimeString(aViewModel.EndTime);
			aFormData.Outcome.Value = CorrectIntString(aViewModel.Outcome);
			aFormData.Intensity.Value = CorrectIntString(aViewModel.Intensity);
			aFormData.RelationshipToInvestigationalDrug.Value = CorrectIntString(aViewModel.RelationshipToInvestigationalDrug);
		}

		private string CorrectDateTimeString(DateTime? value) {
			return value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
		}

		private string CorrectDecimalString(decimal value) {
			return value.ToString(CultureInfo.InvariantCulture);
		}

		private string CorrectIntString(int value) {
			return value.ToString(CultureInfo.InvariantCulture);
		}

		private int ParseToInt(string value) {
			int parsed;
			int.TryParse(value, out parsed);
			return parsed;
		}

		private decimal ParseToDecimal(string value) {
			decimal parsed;
			decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out parsed);
			return parsed;
		}

		private DateTime? ParseToNullableDateTime(string value) {
			DateTime parsedValue;
			bool isParsed = DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedValue);
			return isParsed ? parsedValue : (DateTime?) null;
		}

		private ViewResult ComposeErrorInfoView(int patientNumber, string visitName, string formName) {
			return View("ErrorInfo", new ErrorViewModel {
				Caption = "Form is not found",
				ErrorMessage =
					string.Format(
						"Form '{0}' in Visit '{1}' for Patient with Number {2} is not found in clinical data",
						formName, visitName, patientNumber)
			});
		}

		private ViewResult ComposeErrorInfoView(string formName, int formId) {
			return View("ErrorInfo", new ErrorViewModel {
				Caption = "Form is not found",
				ErrorMessage =
					string.Format(
						"Form '{0}' with id='{1}' is not found in clinical data",
						formName, formId)
			});
		}

		private InventoryFormData GetInventoryFormData(int? inventoryFormId) {
			if (!inventoryFormId.HasValue || inventoryFormId.Value == 0)
				return null;

			return inventoryFormDataRepository.GetFormDataByFormId(inventoryFormId.Value);
		}

		private InventoryFormViewModel GetInventoryViewModel(int? inventoryFormId) {
			InventoryFormData ifd = GetInventoryFormData(inventoryFormId);
			return ifd == null ? null : MapInventoryFormDataToViewModel(ifd);
		}

		private RedirectToRouteResult SaveFormAsCompletedAndRedirect(Form form) {
			form.FormState = FormState.Completed;
			formRepository.Edit(form);
			formRepository.Save();

			return RedirectToAction("ViewForm", "Form",
				new RouteValueDictionary {
					{"patientNumber", form.Visit.Patient.PatientNumber},
					{"visitName", form.Visit.Caption},
					{"formName", form.Caption},
					{"DXScript", Request["DXScript"]}
				});
		}

		private ActionResult ViewEditForm(bool? isFormEditing, int patientNumber, string visitName, string formName) {
			Form form = formRepository.GetForm(patientNumber, visitName, formName);
			if (form == null) {
				return ComposeErrorInfoView(patientNumber, visitName, formName);
			}

			bool isEditing = isFormEditing ?? form.FormState == FormState.Incomplete;

			switch (form.FormType) {
				case FormType.Demographics:
					DemographicFormData dfd = demographicFormDataRepository.GetFormDataByFormId(form.Id);
					if (dfd == null)
						return ComposeErrorInfoView(patientNumber, visitName, formName);
					DemographicFormViewModel dModel = MapDemographicFormDataToViewModel(dfd);
					dModel.QuestionsWithQueries = PopulateQueriesInfoForQuestions(GetQuestionIds(dModel));
					return PartialView(isEditing ? "_EditDemographicForm" : "_ViewDemographicForm", dModel);

				case FormType.Vitals:
					VitalsFormData vfd = vitalsFormDataRepository.GetFormDataByFormId(form.Id);
					if (vfd == null)
						return ComposeErrorInfoView(patientNumber, visitName, formName);
					VitalsFormViewModel vModel = MapVitalsFormDataToViewModel(vfd);
					vModel.QuestionsWithQueries = PopulateQueriesInfoForQuestions(GetQuestionIds(vModel));
					return PartialView(isEditing ? "_EditVitalsForm" : "_ViewVitalsForm", vModel);

				case FormType.Happiness:
					HappinessFormData hfd = happinessFormDataRepository.GetFormDataByFormId(form.Id);
					if (hfd == null)
						return ComposeErrorInfoView(patientNumber, visitName, formName);
					HappinessFormViewModel hModel = MapHappinessFormDataToViewModel(hfd);
					hModel.QuestionsWithQueries = PopulateQueriesInfoForQuestions(GetQuestionIds(hModel));
					return PartialView(isEditing ? "_EditHappinessForm" : "_ViewHappinessForm", hModel);

				case FormType.Electrocardiogram:
					ElectrocardiogramFormData efd = electrocardiogramFormDataRepository.GetFormDataByFormId(form.Id);
					if (efd == null)
						return ComposeErrorInfoView(patientNumber, visitName, formName);
					ElectrocardiogramFormViewModel eModel = MapElectrocardiogramFormDataToViewModel(efd);
					eModel.QuestionsWithQueries = PopulateQueriesInfoForQuestions(GetQuestionIds(eModel));
					return PartialView(isEditing ? "_EditElectrocardiogramForm" : "_ViewElectrocardiogramForm", eModel);

				case FormType.Inventory:
					InventoryFormData ifd = inventoryFormDataRepository.GetFormDataByFormId(form.Id);
					if (ifd == null)
						return ComposeErrorInfoView(patientNumber, visitName, formName);
					InventoryFormViewModel iModel = MapInventoryFormDataToViewModel(ifd);

					if(isEditing)
						Session.Add(String.Format("ird_{0}", iModel.FormId), iModel.MedicationUsage);
					else
						Session.Remove(String.Format("ird_{0}", iModel.FormId));

					iModel.QuestionsWithQueries = PopulateQueriesInfoForQuestions(GetQuestionIds(iModel));
					return PartialView(isEditing ? "_EditInventoryForm" : "_ViewInventoryForm", iModel);

				case FormType.AdverseEvent:
					AdverseEventFormData afd = adverseEventFormDataRepository.GetFormDataByFormId(form.Id);
					if (afd == null)
						return ComposeErrorInfoView(patientNumber, visitName, formName);
					AdverseEventFormViewModel aModel = MapAdverseEventFormDataToViewModel(afd);
					aModel.QuestionsWithQueries = PopulateQueriesInfoForQuestions(GetQuestionIds(aModel));

					ViewData["ActiveView"] = HtmlEditorView.Design;
					ViewData["Html"] = aModel.AdverseExperience;

					return PartialView(isEditing ? "_EditAdverseEventForm" : "_ViewAdverseEventForm", aModel);
			}

			return ComposeErrorInfoView(patientNumber, visitName, formName);
		}

		private static IList<int> GetQuestionIds(DemographicFormViewModel model) {
			return new List<int> {
				model.DateOfBirthQuestionId,
				model.RaceQuestionId,
				model.SexQuestionId,
				model.OtherQuestionId
			};
		}

		private static IList<int> GetQuestionIds(ElectrocardiogramFormViewModel model) {
			return new List<int> {
				model.ActualTimeQuestionId,
				model.ElectrocardiogramAttachmentQuestionId
			};
		}

		private static IList<int> GetQuestionIds(VitalsFormViewModel model) {
			return new List<int> {
				model.ActualTimeQuestionId,
				model.HeightQuestionId,
				model.WeightQuestionId,
				model.TemperatureQuestionId,
				model.HeartRateQuestionId,
				model.BloodPressureSystolicQuestionId,
				model.BloodPressureDiastolicQuestionId
			};
		}

		private static IList<int> GetQuestionIds(HappinessFormViewModel model) {
			return new List<int> {
				model.HappinessLevelQuestionId
			};
		}

		private static IList<int> GetQuestionIds(InventoryFormViewModel model) {
			return new List<int> {
				model.QuantityShippedQuestionId,
				model.BatchNumberQuestionId,
				model.ReceiptDateQuestionId,
				model.ShipDateQuestionId
			};
		}

		private static IList<int> GetQuestionIds(AdverseEventFormViewModel model) {
			return new List<int> {
				model.AdverseExperienceQuestionId,
				model.OnsetDateQuestionId,
				model.OnsetTimeQuestionId,
				model.EndDateQuestionId,
				model.EndTimeQuestionId,
				model.OutcomeQuestionId,
				model.IntensityQuestionId,
				model.RelationshipToInvestigationalDrugQuestionId
			};
		}

		private Dictionary<int, QueryShortViewModel> PopulateQueriesInfoForQuestions(IEnumerable<int> questionIds) {
			var result = new Dictionary<int, QueryShortViewModel>();
			if (queryRepository == null)
				return result;
			var queries = queryRepository.GetQueriesForQuestions(questionIds);
			foreach (Query query in queries) {
				result.Add(query.Question.Id,
					new QueryShortViewModel {
						QueryId = query.Id,
						QueryText = query.QueryText,
						IsCompleted = !string.IsNullOrEmpty(query.AnswerText)
					});
			}

			return result;
		}


		private static void AddChangeReasonRequestIfRequired(Question question, string oldValue, string newValue,
		                                                     List<ChangeNoteViewModel> changeInfos) {
			bool valueChanged = (oldValue ?? string.Empty) != (newValue ?? string.Empty);
			ChangeNoteViewModel existingChangeInfo = changeInfos.FirstOrDefault(ci => ci.QuestionId == question.Id);
			if (valueChanged) {
				if (existingChangeInfo == null) {
					changeInfos.Add(
						new ChangeNoteViewModel {
							QuestionName = question.Caption,
							NewValue = newValue,
							OriginalValue = oldValue,
							QuestionId = question.Id,
							ChangeReason = null
						});
				}
			}
			else if (existingChangeInfo != null) {
				changeInfos.Remove(existingChangeInfo);
			}
		}

		private void SaveFormDataChangeReasons(BaseFormDataViewModel viewModel, BaseFormData formData) {
			if (viewModel.ChangeInfos == null)
				return;
			foreach (ChangeNoteViewModel changeInfo in viewModel.ChangeInfos) {
				var question = formData.AllQuestions.FirstOrDefault(q => q.Id == changeInfo.QuestionId);
				if (question == null)
					continue;
				changeNoteBuilder.CreateChangeNote(question, changeInfo.OriginalValue, changeInfo.NewValue, changeInfo.ChangeReason);
			}
		}
	}
}
