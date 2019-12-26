$(function () {
    $("#specialInput").autocomplete({
        minLength: 3,
        source: $("#SeekCompanyLocation").val(),
        select: function (event, ui) {
            $('#specialInput').val(ui.item.label);
            $('#companyCode').val(ui.item.value);
            return false;
        }
    });
});

$("#startBtn").on('click', function (event) {
    event.preventDefault();
    var companyCode = $('#companyCode').val().replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var language = $("#selectedLanguage").val();
    if (language == undefined) {
        language = "en-us"
    }
    if (companyCode != 'false' && language != "") {
        window.location.href = "/Service/Disclaimer?companyCode=" + companyCode;
    } else {
        TakeCompany();
    }
});

function TakeCompany(selname) {
    $.ajax({
        method: 'POST',
        url: $("#SeekCompanyLocation").val(),
        data: { term: $('#specialInput').val().replace(/</g, "&lt;").replace(/>/g, "&gt;") }
    }).done(function (data) {
        if (data.length > 0) {
            window.location.href = "/Service/Disclaimer?companyCode=" + data[0].value;
        }
    });
}
$(".loginFormLinks__loginBtn.btn").click(function (event) {
    var login = $("#Login").val().replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var Password = $("#Password").val().replace(/</g, "&lt;").replace(/>/g, "&gt;");
    $("#Login").val(login);
    $("#Password").val(Password);
});
