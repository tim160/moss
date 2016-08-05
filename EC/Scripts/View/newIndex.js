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
        if (personInfo.val().trim() == '') {
            personInfo.addClass('error');
            personInfo.parent().css('border', '2px solid red');
        }
        else {
            personInfo.removeClass('error');
        }
    }

    function validationForm() {
        var valCode = $('#code');
        var valFirst = $('#first');
        var valLast = $('#last');
        var valEmail = $('#email');
        var valTitle = $('#title');

        var rv_onlyText = /^[a-zA-Zа-яА-Я\. ]+$/;
        var rv_email = /^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$/;
        //var rv_tel = /[0-9]{5,20}$/;

        valTB_rv(valEmail, rv_email);
        valTB_rv(valFirst, rv_onlyText);
        valTB_rv(valLast, rv_onlyText);
        valTB(valTitle);
        valTB(valCode);

        if (!valCode.hasClass('error') &&
            !valFirst.hasClass('error') &&
            !valLast.hasClass('error') &&
            !valEmail.hasClass('error') &&
            !valTitle.hasClass('error')) {
            console.log(11);
            updateProfile();
        }
    }

    function clickCreate() {
        $('.updateProfileBtn input').click(function () {
            validationForm();
            $('html, body').animate({ scrollTop: 0 }, 500);
        });
    }

    function updateProfile() {
        $.ajax({
            method: "POST",
            url: "/New/CreateUser",
            data: {
                code: $("#code").val().trim(),
                first: $("#first").val().trim(),
                last: $("#last").val().trim(),
                email: $("#email").val().trim(),
                title: $("#title").val().trim(),
                description: $("#description").val().trim()
            }
        }).done(function (data) {//data from server
            if (data != 'completed') {
                alert(data);
                //console.log(data);
            }
            else {
                //    location.reload();
                window.location.href = "/new/success";
            }
        }).fail(function (error) {
            console.log(error);
        });
    }

    function editStyleInput() {
        $('.blockPersonalSettings input')
      .focus(function () {
          $(this).parent('.blockPersonalSettings').css({ 'border': '2px solid #05b5a2', 'width': '99.8%' });
      })
       .focusout(function () {
           $(this).parent('.blockPersonalSettings').css({ 'border': '1px solid #e0e5e6', 'width': '100%' });
       });
    }

    //Main function
    editStyleInput();
    clickCreate();

});