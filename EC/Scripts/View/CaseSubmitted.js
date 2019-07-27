$(document).ready(function () {
    summaryPage();
    if (document.getElementById('linkRegistrationReport')) {
        document.getElementById('linkRegistrationReport').click();
    }

    var ReportId = document.getElementById('ReportId').value;
    var CompanyCode = document.getElementById('CompanyCode').value;


    $("#ok").on('click', function () {
        window.location.href = "/Report/RedirectToReporterDashboard?reportId=" + ReportId;
        $('#loading').show();
    });

    function summaryPage() {
        function setHoverBlock(item) {
            $(this).parent().find('img, input').addClass('hover');
        }

        function setUnHoverBlock(item) {
            $(this).parent().find('img, input').removeClass('hover');
        }

        var root = $('#summareResponce');
        var summaryButtons = root.find('.summaryButtons');
        var printImage = root.find('#printImage');
        var printDashboard = root.find('#dashboardImage');
        summaryButtons.mouseenter(setHoverBlock)
        printImage.mouseenter(setHoverBlock);
        printDashboard.mouseenter(setHoverBlock);

        summaryButtons.mouseout(setUnHoverBlock)
        printImage.mouseout(setUnHoverBlock);
        printDashboard.mouseout(setUnHoverBlock);
    }

    $('.saveChanges').click(function () {
        var pass = $('.accessInfo.pass').val();
        var userId = document.getElementById('UserId').value;
        updateLoginInfo(pass, userId);
    });

    function updateLoginInfo(pass, userId) {
        $.ajax({
            method: "POST",
            url: "/Report/SaveLoginChanges",
            dataType: 'json',
            data: { pass: pass, userId: userId }
        }).done(function (data) {
            //alert(data);
            //new modal window
            $("#successPasswordChanged").modal();
            $('.saveChanges').hide();
        });
    };
    $('.pass').keyup(function () {
        $('.saveChanges').show();
    });

    $("#printReport").on('click', function () {
        printElement(document.getElementById("modal-dialog"));
        $(".fullContainer").hide();
        $(".footer").hide();
        window.print();
        $(".fullContainer").show();
        $(".footer").show();
    });
    $(".close").on('click', function () {
        $("#modal-dialog").toggle();
    });
    function printElement(elem, append, delimiter) {
        var domClone = elem.cloneNode(true);

        var $printSection = document.getElementById("printSection");

        if (!$printSection) {
            $printSection = document.createElement("div");
            $printSection.id = "printSection";
            document.body.appendChild($printSection);
        }
        if (append !== true) {
            $printSection.innerHTML = "";
        }
        else if (append === true) {
            if (typeof (delimiter) === "string") {
                $printSection.innerHTML += delimiter;
            }
            else if (typeof (delimiter) === "object") {
                $printSection.appendChlid(delimiter);
            }
        }
        $printSection.appendChild(domClone);
    }


    $("#retry").on('click', function () {
        location.reload();
    });
    $("#newReport").on('click', function () {
        window.location.href = "/Service/Disclaimer?companyCode=" + CompanyCode;
    });
});