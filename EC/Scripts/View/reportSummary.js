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
        var data = sendAjax(button);
    });
    function sendAjax(promotion_value) {
        var description = "";
        var _report_id = $("#_report_id").val();
        var user_id = $("#user_id").val();
        var outcome_id = $("#ddlOutcome").attr('data-value');
        var reason_id = $("#ddlReasonClosure").attr('data-value');
        

        if (reason_id == "") {
            $("#ddlReasonClosure").css("border", "2px solid red");
            $("#ddlReasonClosure").on('focusin', function () {
                $("#ddlReasonClosure").css("border", "none");
            });
        }
        else {
            $("#ddlReasonClosure").css("border", "none");
        }

        var sign_off_mediator_id = $("#ddlSignOffMediator").attr('data-value');

             if (_report_id > 0 && user_id > 0 && promotion_value != "") {
                $.ajax({
                    method: "POST",
                    url: "/Case/CloseCase",
                    data: {
                        user_id: user_id, report_id: _report_id, description: description, promotion_value: promotion_value,
                        case_closure_reason_id: reason_id, sign_off_mediator_id: sign_off_mediator_id
                    }
                }).done(function (data) {//data from server

                    if (data == -1) {
                        // need to re-login
                        //console.log('Need to login');
                    }

                    if (data == 1) {
                        var str = window.location.href;

                            if (promotion_value == 3)
                                str = '/cases/index';
                            if (promotion_value == 4)
                                str = '/cases/completed';
                            if (promotion_value == 9)
                                str = '/cases/closed';

                        window.location.href = str;
                    }
                    if (data == 0) {
                //        alert('Error: Please add a Case Executive to your team. Your case cannot be approved for resolution or closure unless a member of your executive leadership is brought on board as a Case Executive. If one of your existing Case Officers is an executive leader (i.e. CEO, CHRO, COO, CFO, CRO, CSO or CLO), then please change their role in their profile to Case Executive.');
                    }
                }).fail(function (error) {
                    console.log(error);
                    return -1;
                });

            
            }
            return 0;
    }
    /*end click modal window*/

});
