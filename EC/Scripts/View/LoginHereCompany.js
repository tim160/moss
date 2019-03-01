$(document).ready(function () {
    $('#submit').on('click', function () {
        if (($('input[name=login]').val() != "") && ($('input[name=password]').val() != "")) {

            LoginHere($('input[name=login]').val(), $('input[name=password]').val());
        }
    });

    function LoginHere(login, password) {
        $.ajax({
            method: "POST",
            url: "/Service/Login",
            data: { login: login, password: password }
        }).done(function (data) {//data from server

            if (data != '') {
                var str = window.location.href;
                str = str.toLocaleLowerCase().split("login");

                window.location.href = str[0] + data;
            }
            else
                $('input[name=password]').val() = '';
        }).fail(function (error) {
            console.log(error);
        });
    }
});