$('document').ready(function () {
    function reportSummaryShow() {
        $(".leftBlockContent .closeIcon").click(function () {
            $(".leftBlockContent").hide(300);
            $("#menu a").each(function (indx, element) {
                var temp = $(element).attr("href");
                temp = temp.replace('true', 'false');
                $(element).attr("href", temp);
            });
            $("#messages_menu a").each(function (indx, element) {
                var temp = $(element).attr("href");
                temp = temp.replace('true', 'false');
                $(element).attr("href", temp);
            });
            $("#slider").show();
        });
        $('#slider').on('click', function () {
            $("#slider").hide();
            $(".leftBlockContent").show(300);
            if (window.location.href.indexOf('false') > -1) {
                //var temp = window.location.href;
                //temp.replace('false', 'true');
                $("#menu a").each(function (indx, element) {
                    var temp = $(element).attr("href");
                    temp = temp.replace('false', 'true');
                    $(element).attr("href", temp);
                });
                $("#messages_menu a").each(function (indx, element) {
                    var temp = $(element).attr("href");
                    temp = temp.replace('false', 'true');
                    $(element).attr("href", temp);
                });
            }
        });
    }
    if (window.location.href.indexOf('false') > -1) {
        $(".leftBlockContent").hide(0);
        $("#slider").show();
    }
    reportSummaryShow();

    /*modal window on click resolve button*/
    $("#resoveCompletedTopBtn").on('click', function (el) {
        $("#anotherDialog").toggle();
    });

    $("#resolve_btn").on('click', function (event) {
        button = $(event.currentTarget).val();
        sendAjax(button);
    });
    function sendAjax(promotion_value) {
        var description = "";
        var _report_id = $("#_report_id").val();
        var user_id = $("#user_id").val();
        var outcome_id = $("#ddlOutcome").attr('data-value');
        var reason_id = $("#ddlReasonClosure").attr('data-value');
        
        var outcome = $("#txtOutcome").val();
        if (outcome == "") {
            $("#txtOutcome").css("border", "2px solid red");
            $("#txtOutcome").on('focusin', function () {
                $("#txtOutcome").css("border", "none");
            });
        }
        if (outcome_id == "") {
            $("#ddlOutcome").css("border", "2px solid red");
            $("#ddlOutcome").on('focusin', function () {
                $("#ddlOutcome").css("border", "none");
            });
        } else {
            $("#ddlOutcome").css("border", "none");
        }

        if (reason_id == "") {
            $("#ddlReasonClosure").css("border", "2px solid red");
            $("#ddlReasonClosure").on('focusin', function () {
                $("#ddlReasonClosure").css("border", "none");
            });
        }
        else {
            $("#ddlReasonClosure").css("border", "none");
        }
        console.log('r', reason_id);
        console.log('o', outcome_id);

        var executive_summary = $("#txtExecutiveSummary").val();
        var facts_established = $("#txtFactsEstablished").val();
        var investigation_methodology = $("#txtInvestigationMethodology").val();
        var description_outcome = $("#txtDescriptionOutcome").val();
        var recommended_actions = $("#txtRecommendedActions").val();

        if (outcome != "") {

            if (_report_id > 0 && user_id > 0 && promotion_value != "") {
                $.ajax({
                    method: "POST",
                    url: "/Case/CloseCase",
                    data: {
                        user_id: user_id, report_id: _report_id, description: description, promotion_value: promotion_value, outcome_id: outcome_id, outcome: outcome,
                        case_closure_reason_id: reason_id, executive_summary: executive_summary, facts_established: facts_established, investigation_methodology: investigation_methodology, description_outcome: description_outcome, recommended_actions: recommended_actions
                    }
                }).done(function (data) {//data from server

                    if (data == -1) {
                        // need to re-login
                        //console.log('Need to login');
                    }

                    if (data == 1) {
                        var str = window.location.href;
                        window.location.href = str;
                    }
                    if (data == 0) {
                        alert('Error: Please add a Case Executive to your team. Your case cannot be approved for resolution or closure unless a member of your executive leadership is brought on board as a Case Executive. If one of your existing Case Officers is an executive leader (i.e. CEO, CHRO, COO, CFO, CRO, CSO or CLO), then please change their role in their profile to Case Executive.');
                    }
                }).fail(function (error) {
                    console.log(error);
                });

            }
        } else {
            $("#txtOutcome").css({ "border": "2px solid red" });
            $("#txtOutcome").on('focusin', function () {
                $("#txtOutcome").css({ "border": "2px solid transparent" });
            });
        }
    }
    /*end click modal window*/

});
