
/// <reference path="jquery-1.4.4.js" />
/// <reference path="ASPxScriptIntelliSense.js" />
/// <reference path="clinicalStudy.js" />

function loadChart(_, data) {
	$("#contentPanel").html(data);
   }

var curNavigateItem = {
	navigateUrl: null,
   	name: null
   };

function anbItemClick(s,e) {
	var curNavigateUrl = e.item.GetNavigateUrl();
	e.item.SetNavigateUrl('javascript:void(0)');

	navigateItemAjax(curNavigateUrl, e.item);
}

function navigateItemAjax(url, navItem) {
	if (!url)
		return false;

	pushHistoryState(url, url);

	var loading = $("#loadingIndicator");
	var duration = 500;
	var onFinish = function (data) { $("#contentPanel").html(data); if(navItem) document.title = navItem.GetText(); };
	var method = "GET";
	
	$.ajax({
		url: url,
		cache: false,
		type: method,
		beforeSend: function (xhr) {
			loading.show(duration);
		},
		complete: function (s,status) {
			loading.hide(duration);
			if(navItem)
				navItem.SetNavigateUrl(url);
			if(status != 'success')
				onFinish('<h2>Analytics Dashboard</h2><p>Sorry, an error occured during the current operation</p>');
		},
		success: function (data, status, xhr) {
			onFinish(data);
		},
		failure: ajaxFailure
	});
	return true;
}

function setActiveNavBarItem(groupName, itemName) {
    if (!window.nbAdmin)
        return;
    if (groupName) {
        var group = window.nbAdmin.GetGroupByName(groupName);
        if (group)
            group.SetExpanded(true);
    }

    if (itemName) {
        var item = window.nbAdmin.GetItemByName(itemName);
        if (item)
            window.nbAdmin.SetSelectedItem(item);
    }
}
