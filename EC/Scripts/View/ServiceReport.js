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
            { label: "English", value: "en-us" },
            { label: "Español", value: "es-mx" }
            //'عربى(Arabic)', '中文(Chinese)', 'Deutsch', 'Filipino', 'Français',
            //'Italiano', '日本人(Japanese)', '한국어(Korean)', 'Nederlands', 'Português', 'Русский(Russian)', 'Tiếng Việt(Vietnamese)'];
        ]
    $("#languages").autocomplete({
        minLength: 0,
        source: arrayLang,
        select: function (event, ui) {
            $('#specialInput').val(ui.item.label);
            $('#selectedLanguage').val(ui.item.value);
            return false;
        }
    });
    $("#startBtn").on('click', function () {
        var companyCode = $('#companyCode').val();
        var language = $("#selectedLanguage").val();
        if (language == "") {
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
        url: '@Url.Action("SeekCompany", "Index")',
        data: { term: $('#specialInput').val() }
    }).done(function (data) {
        if (data.length > 0) {
            window.location.href = "/Service/Disclaimer?companyCode=" + data[0].value;
        }
    });
}