$(document).ready(function (event) {

    var enterYourEmail = $("#EnterYourEmail").val();
    var enterYourLogin = $("#EnterYourLogin").val();
    var updatePassEmailSent = $("#UpdatePassEmailSent").val();
    $('#submit').on('click', function (event) {
        event.preventDefault();
        if (($('input[name=Login]').val() != "") && $('input[name=Email]').val() != "") {
            ForgetPass($('input[name=Login]').val().replace(/</g, "&lt;").replace(/>/g, "&gt;"), $('input[name=Email]').val().replace(/</g, "&lt;").replace(/>/g, "&gt;"));
        }
        else {
            if ($('input[name=Login]').val() === "") {
                alert(enterYourLogin);
            }
            if ($('input[name=Email]').val() === "") {
                alert(enterYourEmail);
            }
        }
    });

    function ForgetPass(login, email) {

        $.ajax({
            method: "POST",
            url: "/Service/Email",
            data: { email: email, login: login }
        }).done(function (data) {//data from server
            if (data == 'success') {
                alert(updatePassEmailSent);
                $('input[name=Login]').val('');
                $('input[name=Email]').val('');
            }
            else {
                alert(data);
            }
        }).fail(function (error) {
            console.log(error);
        });
    }
});