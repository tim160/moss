$(document).ready(function () {
    "use strict";
    $.ajax({
        method: "GET",
        url: "/api/UserStatusButtons?user_id=" + $("#MediatorId").val(),
    }).done(function (data) {

        if ("undefined" === typeof (data.UserStatusViewModel.active_button_status)) {
            console.log("DOESN'T exists data.UserStatusViewModel.active_button_status");
        } else {
            if (data.UserStatusViewModel.active_button_status == 1) {
                makeActiveButton($(".oneIncidentActive"));
            } else {
                makeDisabledButton($(".oneIncidentActive"));
            }
        }
        if ("undefined" === typeof (data.UserStatusViewModel.inactive_button_status)) {
            console.log("DOESN'T exists data.UserStatusViewModel.inactive_button_status");
        } else {
            if (data.UserStatusViewModel.inactive_button_status == 1) {
                makeActiveButton($(".twoIncidentInactive"));
            } else {
                makeDisabledButton($(".twoIncidentInactive"));
            }
        }
        if ("undefined" === typeof (data.UserStatusViewModel.pending_button_status)) {
            console.log("DOESN'T exists data.UserStatusViewModel.pending_button_status");
        } else {
            if (data.UserStatusViewModel.pending_button_status == 1) {
                makeActiveButton($(".threeIncidentPending"));
            } else {
                makeDisabledButton($(".threeIncidentPending"));
            }
        }
    }).fail(function (error) {
        console.log(error);
    });

    function makeActiveButton(jqueryInput) {
        jqueryInput.removeClass('notAllowed');
    }
    function makeDisabledButton(jqueryInput) {
        jqueryInput.addClass('notAllowed');
        jqueryInput.unbind('click');
    }
});