

/// <reference path="jquery-1.4.4.js" />
/// <reference path="ASPxScriptIntelliSense.js" />
/// <reference path="clinicalStudy.js" />

(function ($) {
	$(document).ready(function () {
		$(".queryMark").live('click', showQuery);


		$('#patNavLoadingIndicator')
			.bind('beforeShow', function () {
			    if (window.patNavLoadingIndicator)
			        window.patNavLoadingIndicator.Show();
			})
			.bind('beforeHide', function () {
			    if (window.patNavLoadingIndicator)
			        window.patNavLoadingIndicator.Hide();
			});
	});
})(jQuery);

function electrocardiogramUploadComplete(s, e) {
    if (e.callbackData != '') {

        var result = jQuery.parseJSON(e.callbackData);
        $('#AttachmentId').val(result.id);
        var link = $('#hlUploadedFile');
        link.attr("href", result.link);
        link.text(result.name);
        if (window.ucAttachment)
        	window.ucAttachment.SetClientVisible(false);
        if (window.imgRemoveAttachment)
        	window.imgRemoveAttachment.SetVisible(true);
        link.show();
        //it makes sense to enforce validation to hide error message (if shown)
        $("form").validate().element("#AttachmentId");
    } 
}

function electrocardiogramRemoveFile() {
	var link = $('#hlUploadedFile');
	link.hide();
	if (window.ucAttachment)
		window.ucAttachment.SetVisible(true);
	if (window.imgRemoveAttachment)
		window.imgRemoveAttachment.SetVisible(false);
	$("#AttachmentId").val(null);
}

function PrepareValidationScripts(form) {
	if (!form)
		return;
	if (form.attr("data-executed"))
		return;
	$.validator.unobtrusive.parse(form);
	form.attr("data-executed", "true");
}

var suppressLoadingPanel = false;

function OnGridClick(s, e) {
	if (window.PatientDataPanel)
		if (window.PatientDataPanel.InCallback())
			return;
	suppressLoadingPanel = true;
	window.PatientListGrid.GetRowValues(e.visibleIndex, 'PatientNumber', OnGetRowValues);
}

function OnGetRowValues(values, visitName, formName) {
	var url = composeUrl(values, visitName, formName);
	loadContent(url, values, loadPatientHtml);
}

function PatientGridReloadWithPatient(patientNumber, visitName, formName) {
	selectedStudyData.needFindPatient = true;
	OnGetRowValues(patientNumber, visitName, formName);
}

function OnVisitTabClick(s,e) {
	selectedStudyData.selectedVisitName = e.tab.name;
	var url = composeUrl(selectedStudyData.selectedPatientNumber,
		selectedStudyData.selectedVisitName);
	pushHistoryState(url, url);
	if (window.PatientDataPanel)
		window.PatientDataPanel.PerformCallback();
}

var selectedStudyData = {
	selectedPatientNumber: null,
	patientsGridPerActivity: null,
	selectedVisitName: null,
	selectedQueryId: null,
	needFindPatient: false
};

function loadPatientHtml(patientNumber, data) {
	selectedStudyData.selectedPatientNumber = patientNumber;
	$("#contentPanel").html(data);
	afterPatientChanged(data);
	setActiveNavTab('patients');
	if (window.PatientListGrid) {
		suppressLoadingPanel = true;
		window.PatientListGrid.PerformCallback();
	}
}

function afterPatientChanged(data) {
	document.title = $(data).find(".headerGroup").attr("data-page-title");
}

function afterNewPatientRequest(data) {
	$("#contentPanel").html(data);
	
	afterPatientChanged(data);
	if (window.PatientListGrid) {
		suppressLoadingPanel = true;
		window.PatientListGrid.PerformCallback();
	}
}

function fillSelectedPatient(s, e) {
	e.customArgs["patientNumber"] = selectedStudyData.selectedPatientNumber;
	e.customArgs["activityState"] = selectedStudyData.patientsGridPerActivity;
	e.customArgs["needFindPatient"] = selectedStudyData.needFindPatient;
}

function patientsListGridBeginCallback(s, e) {
	if (!suppressLoadingPanel && window.patNavLoadingIndicator)
		window.patNavLoadingIndicator.Show();
	fillSelectedPatient(s, e);
}

function patientsListGridEndCallback(s, e) {
	suppressLoadingPanel = false;
	if (window.patNavLoadingIndicator)
		window.patNavLoadingIndicator.Hide();

	selectedStudyData.needFindPatient = false;
	setActiveInactiveNavTab(s.cpRequestedPatientActivity);
}

function refreshPatientListAfterEdit() {
	selectedStudyData.selectedPatientNumber = $("#PatientNumber").val();
	selectedStudyData.selectedVisitName = null;
	if (window.HeaderPatientInitialsLabel && window.PatientInitialsLabel)
	    $(window.HeaderPatientInitialsLabel).text($(window.PatientInitialsLabel).text());
	
	if (window.PatientListGrid)
		window.PatientListGrid.PerformCallback();
	if (window.PatientDataPanel)
		window.PatientDataPanel.PerformCallback();
	var url = composeUrl(selectedStudyData.selectedPatientNumber);
	pushHistoryState(url, url);
}
function showCreatedAdverseEvent(data, status, xhr) {
	selectedStudyData.selectedVisitName = data;
	if (window.PatientDataPanel)
	    window.PatientDataPanel.PerformCallback();
	var url = composeUrl(selectedStudyData.selectedPatientNumber,
		selectedStudyData.selectedVisitName);
	pushHistoryState(url, url);
}

function beforePatientPanelLoaded(s, e) {
	showLoadingIndicator();
}

function afterPatientPanelLoaded(s,e) {
	document.title = s.cpPageTitle;

	if (window.HeaderPatientInitialsLabel)
		$(window.HeaderPatientInitialsLabel).text(s.cpPatientInitials);
}

function beforeReloadPatientVisitsPanel(s, e) {
	showLoadingIndicator();
	e.customArgs["patientNumber"] = selectedStudyData.selectedPatientNumber;
	if (selectedStudyData.selectedVisitName)
		e.customArgs["visitName"] = selectedStudyData.selectedVisitName;
}

function afterReloadPatientVisitsPanel(s, e) {
	hideLoadingIndicator();
}


function updateDemographic(data, status, xhr) {
	if (window.PatientListGrid)
		window.PatientListGrid.PerformCallback();

	processDataChanges(data);
}

function processDataChanges(data) {
	var popupName = $(data).find(".dataChange").attr("data-popup");
	var popup = window[popupName];
	if (popup)
		popup.Show();
}

function composeUrl(patientNumber, visitName, formName) {
    var url = clinicalStudyUrlPrefix +  "DataCapture/Patients/" + patientNumber.toString();
	if (visitName)
		url = url + '/' + visitName;
	if (formName)
		url = url + '/' + formName;
	return url;
}

function showQuery() {
	selectedStudyData.selectedQueryId = $(this).attr("data-query-id");
	if (window.replyQueryPopup) {
		window.replyQueryPopup.PerformCallback();
		window.replyQueryPopup.Show();
	}
}
function onQueryDialogBeginCallback(s, e) {
	e.customArgs["queryId"] = selectedStudyData.selectedQueryId;
}

function onQueryAnswered() {
	if(window.replyQueryPopup)
		window.replyQueryPopup.PerformCallback(); 
	if(window.QueryGrid)
		window.QueryGrid.PerformCallback();
	$("#queryIcon" + selectedStudyData.selectedQueryId).attr("src", clinicalStudyUrlPrefix + "Content/Icons/MarkCompleted.png");
}

function activePatientsGridChange(s, e) {
	selectedStudyData.patientsGridPerActivity = e.tab.name;
	if(window.PatientListGrid)
		window.PatientListGrid.PerformCallback();
}

function rightNavTabClick(s, e, tabUrls) {
	if (window.navigationRightTab && window.navigationRightTab.GetActiveTab() && e.tab.name == window.navigationRightTab.GetActiveTab().name)
		return;
	if (e.tab.name == 'patients' && selectedStudyData.selectedPatientNumber) {
		PatientGridReloadWithPatient(selectedStudyData.selectedPatientNumber, selectedStudyData.selectedVisitName);
	}
	else {
		var url = tabUrls[e.tab.name];
		if (url) {
			buttonAjaxClick(s, e, url, { NeedHistoryUpdate : true });
		}
	}
	if (window.navigationRightTab)
	    window.navigationRightTab.SetActiveTab(e.tab);
}

function setActiveNavTab(tabName) {
    if (!window.navigationRightTab || !tabName)
		return;
    var tab = window.navigationRightTab.GetTabByName(tabName);
	if(tab)
	    window.navigationRightTab.SetActiveTab(tab);
}

function setActiveInactiveNavTab(active) {
	if (!window.navigationLeftTab)
		return;
	
	selectedStudyData.patientsGridPerActivity = 'active';
	if(active == false)
		selectedStudyData.patientsGridPerActivity = 'inactive';
	
	var tab = window.navigationLeftTab.GetTabByName(selectedStudyData.patientsGridPerActivity);
	if (tab)
		window.navigationLeftTab.SetActiveTab(tab);
}

function buttonAjaxClick(s, e, url, ajaxOptions) {
	if (!url)
		return false;
	if (ajaxOptions.NeedHistoryUpdate)
		pushHistoryState(url, url);

	var loading = $("#loadingIndicator");
	if (ajaxOptions.LoadingElementId)
		loading = $("#" + ajaxOptions.LoadingElementId);

	var duration = 500;

	var onSuccess = function (data) { $("#contentPanel").html(data); };
	if (ajaxOptions.OnSuccess)
		onSuccess = ajaxOptions.OnSuccess;
	else if (ajaxOptions.UpdateTargetId)
		onSuccess = function (data) { $("#" + ajaxOptions.UpdateTargetId).html(data); };

	var method = "GET";
	if (ajaxOptions.Method)
		method = ajaxOptions.Method;

	$.ajax({
		url: url,
		cache: false,
		type: method,
		beforeSend: function (xhr) {
			loading.show(duration);
		},
		complete: function () {
			loading.hide(duration);
		},
		success: function (data, status, xhr) {
			onSuccess(data);
		},
		failure: ajaxFailure
	});
	return true;
}

function submitCrf(formId, validationGroup) {
	var form = $(formId);
	if (form && form.length > 0) {
	    PrepareValidationScripts(form);
	    if(ASPxClientEdit.ValidateEditorsInContainer(form.get(0), validationGroup))
		    form.submit();
	}
}

var inventorySubmittingData = {
	tryInventorySubmit: false,
	inventoryFormId: null
};

function submitInventoryCrf(formId) {
	inventorySubmittingData.tryInventorySubmit = true;
	inventorySubmittingData.inventoryFormId = formId;

	checkRepetableInventoryDataEditing($(formId));
}

function checkRepetableInventoryDataEditing(form) {
	if (!window.gvRepeatableInventory || (window.gvRepeatableInventory && !window.gvRepeatableInventory.IsEditing())) {
		inventorySubmittingData.tryInventorySubmit = false;
		PrepareValidationScripts(form);
		form.submit();
		return;
	}

	if (window.gvRepeatableInventory.IsEditing()) {
		window.gvRepeatableInventory.UpdateEdit();
	}
	return;
}

function RepeatableInventoryBeginCallback(s,e) {
	showLoadingIndicator();
}

function RepeatableInventoryEndCallback(s, e) {
	hideLoadingIndicator();
	if (inventorySubmittingData.tryInventorySubmit && inventorySubmittingData.inventoryFormId) {
		var form = $(inventorySubmittingData.inventoryFormId);
		inventorySubmittingData.tryInventorySubmit = false;
		PrepareValidationScripts(form);
		if (form && !s.IsEditing()) {
			form.submit();
		}
	}
}


function prepareAndValidateDataChangePopup(popup) {
    PrepareValidationScripts($('#changeReasonForm'));
    var popupObject = window[popup];
    if(popupObject == null)
        return;
    var container =  popupObject.GetMainElement();
    if(ASPxClientEdit.ValidateEditorsInContainer(container)) {
        popupObject.Hide();
    }
}