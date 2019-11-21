$('document').ready(function () {
    $("#ok").on('click', function () {
        var ReportId = document.getElementById('ReportId').value;
        window.location.href = "/Report/RedirectToReporterDashboard?reportId=" + ReportId;
        $('#loading').show();
    });
    $(".pass").on("keyup", function () {
        $('.saveChanges').show();
    });
    $(".close").on('click', function () {
        $("#modal-dialog").toggle();
    });
    $("#retry").on('click', function () {
        location.reload();
    });
    $("#newReport").on('click', function () {
        var CompanyCode = document.getElementById('CompanyCode').value;
        window.location.href = "/Service/Disclaimer?companyCode=" + CompanyCode;
    });
    function setHoverBlock(item) {
        $(this).parent().find('img, input').addClass('hover');
    }
    function setUnHoverBlock(item) {
        $(this).parent().find('img, input').removeClass('hover');
    }
    var root = $("#summaryResponse");
    var summaryButtons = root.find(".summaryButtons");
    var printImage = root.find("#printImage");
    var printDashboard = root.find("#dashboardImage");

    summaryButtons.mouseenter(setHoverBlock);
    printImage.mouseenter(setHoverBlock);
    printDashboard.mouseenter(setHoverBlock);

    summaryButtons.mouseout(setUnHoverBlock);
    printImage.mouseout(setUnHoverBlock);
    printDashboard.mouseout(setUnHoverBlock);

    $(".saveChanges").on('click', function () {
        var pass = $(".accessInfo.pas").val();
        var userId = document.getElementById("UserId").value;
        updateLoginInfo(pass, userId);
    });

    function updateLoginInfo(pass, userId) {
        $.ajax({
            method: "POST",
            url: "/Report/SaveLoginChanges",
            dataType: "json",
            data: { pass: pass, userId: userId }
        }).done(function (data) {
            $("#successPasswordChanged").modal();
            $(".saveChanges").hide();
        });
    };

    var linkRegistrReport = document.getElementById('linkRegistrationReport');
    if (linkRegistrReport) {
        linkRegistrReport.click();
    }
});