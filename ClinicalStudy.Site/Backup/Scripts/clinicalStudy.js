/// <reference path="jquery-1.4.4.js" />
/// <reference path="ASPxScriptIntelliSense.js" />


var clinicalStudyUrlPrefix = '/';

var storedHash = '';
var refreshSessionIntervalInMinutes = 15;
var refreshSessionInterval = 1000 * 60 * refreshSessionIntervalInMinutes;


(function ($) {
    
    if((location.hash || '') != '') {
        dataCaptureHistoryBack(location.hash.substr(1, location.hash.length));
    }
    var initialUrl = location.href;

    $(document).ready(function () {
        $(".dcLink").live('click', dataCaptureLinkClick);
        
        
        if (window.addEventListener) {
            window.addEventListener('popstate', function (e) {
                if (history) {
                    if (history.state) {
                        dataCaptureHistoryBack(e.state.path);
                    }
                    else {
                        if (initialUrl == location.href) {
                            return;
                        }
                        dataCaptureHistoryBack(location.href);
                    }
                }
            }, false);
        }
        
        scheduleRefreshSessionPing();
    });


    function dataCaptureLinkClick(sender) {
        var link = $(sender.target);
        var href = link.attr("href");
        if (!href)
            href = link.parent().attr("href");
        pushHistoryState(href, link.text());

    }

    function dataCaptureHistoryBack(path) {
        location = path;
        showLoadingIndicator();
    }
    

    if (!Array.prototype.indexOf) {
        Array.prototype.indexOf = function (obj, start) {
            for (var i = (start || 0), j = this.length; i < j; i++) {
                if (this[i] === obj) { return i; }
            }
            return -1;
        }
    }

})(jQuery);


function pushHistoryState(uri, text) {
    if (window.history.pushState) {
        window.history.pushState({ path: uri, title: text }, text, uri);
    }
    else {
        storedHash = uri;
        location.hash = uri;
    }

}


//workaround for Loading Panel extension
//http://stackoverflow.com/questions/1225102/jquery-event-to-trigger-action-when-a-div-is-made-visible
jQuery(function ($) {

    var _oldShow = $.fn.show;
    var _oldHide = $.fn.hide;

    $.fn.show = function (speed, oldCallback) {
        return $(this).each(function () {
            var
				obj = $(this),
				newCallback = function () {
				    if ($.isFunction(oldCallback)) {
				        oldCallback.apply(obj);
				    }
				};

            // you can trigger a before show if you want
            obj.trigger('beforeShow');

            // now use the old function to show the element passing the new callback
            _oldShow.apply(obj, [speed, newCallback]);
        });
    };

    $.fn.hide = function (speed, oldCallback) {
        return $(this).each(function () {
            var
				obj = $(this),
				newCallback = function () {
				    if ($.isFunction(oldCallback)) {
				        oldCallback.apply(obj);
				    }
				};
            var e = jQuery.Event('beforeHide');

            var prevented = false;
            obj.trigger(e);

            if (e)
                prevented = e.isDefaultPrevented();
            if (!prevented) {
                // now use the old function to show the element passing the new callback
                _oldHide.apply(obj, [speed, newCallback]);
            }
        });
    };

    $('#loadingIndicator')
		.bind('beforeShow', function () {
		    showLoadingIndicator();
		})
		.bind('beforeHide', function (event) {
		    return hideLoadingIndicator(event);
		});
});




function loadContent(url, values, onSuccess) {
    var loading = $("#loadingIndicator");
    var duration = 500;

    pushHistoryState(url, url);

    $.ajax({
        url: url,
        type: "GET",
        cache: false,
        beforeSend: function (xhr) {
            loading.show(duration);
        },
        complete: function () {
            loading.hide(duration);
        },
        success: function (data, status, xhr) {
            onSuccess(values, data);
        },
        failure: ajaxFailure
    });

}


var loadingIndicatorClientsCounter = 0;
function showLoadingIndicator() {
    loadingIndicatorClientsCounter++;
    if (window.loadingIndicator)
        window.loadingIndicator.Show();
}

function hideLoadingIndicator(event) {
    loadingIndicatorClientsCounter--;
    if (loadingIndicatorClientsCounter === 0) {
        if (window.loadingIndicator)
            window.loadingIndicator.Hide();
    }
    else {
        if(event)
            event.preventDefault();
    }
}
function ajaxFailure(xhr, status, error) {
    debugger;
    alert(xhr);
}

function scheduleRefreshSessionPing() {
    window.setTimeout('pingSession()', refreshSessionInterval);
}

function pingSession() {
    $.get(clinicalStudyUrlPrefix + 'RefreshSession');
    scheduleRefreshSessionPing();
}