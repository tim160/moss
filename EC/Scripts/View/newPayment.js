$(document).ready(function () {

    function valTB_rv(personInfo, rv) {
        if (personInfo.val().trim() == '' || !rv.test(personInfo.val().trim())) {
            personInfo.addClass('error');
            personInfo.parent().css('border', '2px solid red');
        }
        else {
            personInfo.removeClass('error');
        }
    }

    function valTB(personInfo) {
        if (typeof personInfo.val() != 'undefined' && personInfo.val() != null) {
            if (personInfo.val().trim() == '') {
                personInfo.addClass('error');
                personInfo.parent().css('border', '2px solid red');
            }
            else {
                personInfo.removeClass('error');
            }
        } else {
            if (personInfo.hasClass('blockPersonalSettings')) {
                personInfo.addClass('error');
            } else {
                personInfo.addClass('error');
                personInfo.parents('.blockPersonalSettings').css('border', '2px solid red');
            }

        }

    }

    function validationForm() {

        var csv = $("#csv");
        var cardname = $("#cardname");
        var cardnumber = $("#cardnumber");
        var selectedMonth = $("#selectedMonth");
        var selectedYear = $("#selectedYear");

        var rv_onlyText = /^[a-zA-Zа-яА-Я\. ]+$/;
        var rv_email = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        //var rv_email = /^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$/;
        //var rv_tel = /[0-9]{5,20}$/;

        valTB(csv);
        valTB(cardname);
        valTB(cardnumber);
        valTB(selectedMonth);
        valTB(selectedYear);

        if (
            !csv.hasClass('error') &&
            !cardname.hasClass('error') &&
            !cardnumber.hasClass('error') &&
            !selectedMonth.hasClass('error') &&
            !selectedYear.hasClass('error')) {
            makePayment();
        }
    }

    function clickCreate() {
        $('.updateProfileBtn input').click(function () {
           // makePayment();
            validationForm();
            $('html, body').animate({ scrollTop: 0 }, 500);
        });
    }


    $(".blockPersonalSettings select").on('change', function (event) {
        var temp = $(event.currentTarget);
        temp.parents('.blockPersonalSettings').css({ 'border': '1px solid #e0e5e6' });
    });
    function editStyleInput() {
        $('.blockPersonalSettings input, .blockPersonalSettings select')
      .focus(function () {
          $(this).parent('.blockPersonalSettings').css({ 'border': '2px solid #05b5a2', 'width': '99.8%' });
      })
       .focusout(function () {
           $(this).parent('.blockPersonalSettings').css({ 'border': '1px solid #e0e5e6', 'width': '100%' });
       });
    }

    function makePayment() {
        $.ajax({
            method: "POST",
            url: "/Payment/Pay",
            data: {
                csv: $("#csv").val().trim(),
                cardname: $("#cardname").val().trim(),
                cardnumber: $("#cardnumber").val().trim(),
                selectedMonth: $("#selectedMonth").val(),
                selectedYear: $("#selectedYear").val(),
                amount: $("#amount").val().trim(),
                description: $("#description").text().trim(),

            }
        }).done(function (data) {//data from server
            if (data != 'completed') {
                alert(data);
            }
            else {
                //    location.reload();
                window.location.href = "/Payment/History";
            }
        }).fail(function (error) {
            console.log(error);
        });
    }

    //Main function
    editStyleInput();
    clickCreate();

    $("#csv").on('keypress', function (event) {
        //if ($(event.currentTarget).val().length == 4) {
        //    event.preventDefault();
        //}
        var key_code = event.keyCode;
        if ((key_code >= 48 && key_code <= 57) || (key_code >= 96 && key_code <= 105)) {
            return;
        } else if ((key_code === 8) || (key_code >= 35 && key_code <= 40) || key_code === 46 || key_code === 9 || key_code === 13) {  // Backspace, cursor keys, PgUp, PgDown, Delete, Tab, Enter
            return;
        }
    });
});