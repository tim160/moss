document.addEventListener('DOMContentLoaded', function () { 

    document.getElementById("passField").onkeyup = function () {
        document.getElementById("saveChangesButton").style.display = "block";
    }
    document.getElementById("ok").onclick = function () {
        var ReportId = document.getElementById('ReportId').value;
        window.location.href = "/Report/RedirectToReporterDashboard?reportId=" + ReportId;
        document.getElementById("loading").style.display = "block";
    }
    document.getElementsByClassName("close").onclick = function () {
        document.getElementById('modal-dialog').style.display = "block";
    }

    document.getElementById("saveChangesButton").onclick = function () {
        var pass = document.getElementById("passField").value;
        var userId = document.getElementById("UserId").value;
        updateLoginInfo(pass, userId);
    }

    function updateLoginInfo(pass, userId) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/Report/SaveLoginChanges', false);
        xhr.send();

        if (xhr.status != 200) {
        } else {
            $("#successPasswordChanged").modal();
            document.getElementById("saveChangesButton").style.display = "none";
        }
    }
    var linkRegistrReport = document.getElementById('linkRegistrationReport');
    if (linkRegistrReport) {
        linkRegistrReport.click();
    }
    document.getElementById('retry').onclick = function () {
        location.reload();
    }
});



$('document').ready(function () {

    $(".close").on('click', function () {
        $("#modal-dialog").toggle();
    });

});