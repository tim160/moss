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
    var arrayLang =
        [
            { label: "English", value: "en-US" },
            { label: "Español", value: "es-ES" }
            //'عربى(Arabic)', '中文(Chinese)', 'Deutsch', 'Filipino', 'Français',
            //'Italiano', '日本人(Japanese)', '한국어(Korean)', 'Nederlands', 'Português', 'Русский(Russian)', 'Tiếng Việt(Vietnamese)'];
        ]
    $("#languages").autocomplete({
        minLength: 0,
        source: arrayLang,
        select: function (event, ui) {
            $('#languages').val(ui.item.label);
            $('#selectedLanguage').val(ui.item.value);
            return false;
        }
    });
    $("#startBtn").on('click', function (event) {
        event.preventDefault();
        var companyCode = $('#companyCode').val();
        var language = $("#selectedLanguage").val();
        if (language == undefined) {
            language = "en-us"
        }
        if (companyCode != 'false' && language != "") {
            window.location.href = "/Service/Disclaimer?companyCode=" + companyCode + "&lang=" + language;
        } else {
            TakeCompany();
        }
    });
});

function TakeCompany(selname) {
    $.ajax({
        method: 'POST',
        url: $("#SeekCompanyLocation").val(),
        data: { term: $('#specialInput').val() }
    }).done(function (data) {
        if (data.length > 0) {
            window.location.href = "/Service/Disclaimer?companyCode=" + data[0].value;
        }
    });
}