$(document).ready(function () {
    $("#PayByCard").hide();

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
        var valCode = $("#code");
        var valCompanyName = $("#company_name");
        var valLocation = $("#location");
        var valNumber = $("#number");
        var valFirst = $("#first");
        var valLast = $("#last");
        var valEmail = $("#email");
        var valTitle = $("#title");

        var csv = $("#csv");
        var cardname = $("#cardname");
        var cardnumber = $("#cardnumber");
        var selectedMonth = $("#selectedMonth");
        var selectedYear = $("#selectedYear");

        var rv_onlyText = /^[a-zA-Zа-яА-Я\. ]+$/;
        var rv_email = /^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$/;
        //var rv_tel = /[0-9]{5,20}$/;

        valTB(valCode);
        valTB(valCompanyName);
        valTB(valLocation);
        valTB(valNumber);
        valTB(valFirst);
        valTB(valLast);
        valTB_rv(valEmail, rv_email);
        valTB(valTitle);

        valTB(csv);
        valTB(cardname);
        valTB(cardnumber);
        valTB(selectedMonth);
        valTB(selectedYear);

        if (!valCode.hasClass('error') &&
            !valCompanyName.hasClass('error') &&
            !valLocation.hasClass('error') &&
            !valNumber.hasClass('error') &&
            !valFirst.hasClass('error') &&
            !valLast.hasClass('error') &&
            !valEmail.hasClass('error') &&

            !csv.hasClass('error') &&
            !cardname.hasClass('error') &&
            !cardnumber.hasClass('error') &&
            !selectedMonth.hasClass('error') &&
            !selectedYear.hasClass('error') &&

            !valTitle.hasClass('error') && ($("#company_exist").val().length > 0)) {
            createCompany();
        }
    }

    function clickCreate() {
        $('.updateProfileBtn input').click(function () {
            createCompany();
            //validationForm();
            $('html, body').animate({ scrollTop: 0 }, 500);
        });
    }

    function companyInUse(company_nm) {
        $.ajax({
            cache: false,
            method: "POST",
            url: "/New/IsCompanyAvailable",
            data: { company_nm: company_nm }
        }).done(function (data) {//data from server
            if ((company_nm.length > 0) && (data == 1)) {
                alert('Company name is already in use.');
                $("#company_name").parent().css('border', '2px solid red');
                $("#company_exist").val('');
            }
            if ((company_nm.length == 0) || (data == 1)) {
                $("#company_name").parent().css('border', '2px solid red');
                $("#company_exist").val('');

            }
            else {
                // $("#company_name").parent().css('border', '');
                $("#company_exist").val('1');

            }
            return 0;

        }).fail(function (error) {
            alert('Error connecting to server ' + error);
            $("#company_name").parent().css('border', '2px solid red');
            return 1;
        });
        return 1;

    }

    function companyNameChange() {
        $("#company_name").on('change', function (e) {
            var in_use = companyInUse($("#company_name").val().trim());
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

    function createCompany() {
        $.ajax({
            method: "POST",
            url: "/New/CreateCompany",
            data: {
                code: $("#code").val().trim(),
                location: $("#location").val().trim(),
                company_name: $("#company_name").val().trim(),
                number: $("#number").val().trim(),
                first: $("#first").val().trim(),
                last: $("#last").val().trim(),
                email: $("#email").val().trim(),
                title: $("#title").val().trim(),
                csv: $("#csv").val().trim(),
                cardname: $("#cardname").val().trim(),
                cardnumber: $("#cardnumber").val().trim(),
                selectedMonth: $("#selectedMonth").val(),
                selectedYear: $("#selectedYear").val(),
                //description: $("#description").val().trim()
                description: $("#description").text().trim(),

            }
        }).done(function (data) {//data from server
            if (data != 'completed') {
                alert(data);
            }
            else {
                //    location.reload();
                window.location.href = "/new/success?show=1";
            }
        }).fail(function (error) {
            console.log(error);
        });
    }

    //Main function
    companyNameChange();
    editStyleInput();
    clickCreate();

    

    var changeTimer = false;

    $("#amount").on('change', function (event) {
        var temp = $(event.currentTarget);
        parseInt(temp);
    });
    $("#code").on("change", function (event) {

        if (changeTimer !== false) clearTimeout(changeTimer);

        changeTimer = setTimeout(function () {
            CheckCode($("#code").val(), $("#number").val());
            changeTimer = false;
        }, 3000);
    });
    $("#number").on("change", function (event) {

        if (changeTimer !== false) clearTimeout(changeTimer);

        changeTimer = setTimeout(function () {
            CheckCode($("#code").val(), $("#number").val());
            changeTimer = false;
        }, 3000);
    });



    function CheckCode(code, emplquant) {
        let temp = window.location.protocol + "/new/ReturnAmount";
        
        $.ajax({
            method: "POST",
            url: temp,
            data: { code: code, emplquant: emplquant }
        }).done(function (data) {//data from server
            var amount = parseInt(data);
            if (amount == 0) {
                  $("#PayByCard").hide();
                  $("#amount").val(0);
            } else if (amount > 0) {
                  $("#amount").val(data);
                  $("#PayByCard").show();
                //validationForm();
                  $('.updateProfileBtn input').unbind();
                  $('.updateProfileBtn input').click(function () {
                      validationForm();
                      $('html, body').animate({ scrollTop: 0 }, 500);
                  });
              }
        }).fail(function (error) {
            console.log(error);
        });
    }
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