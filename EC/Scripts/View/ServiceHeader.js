﻿'use strict';
$("#languageMenuItem").on('click', function (event) {
    event.preventDefault();
    $('#manuLanguages').toggle();
});
$('#manuLanguages td').on('click', function (event) {
    var selectedLanguageElement = event.currentTarget;
    document.getElementById('languageMenuItem').value = selectedLanguageElement.innerHTML;
    var valueSelected = selectedLanguageElement.getAttribute('value');
    console.log('language Selected =' + valueSelected);
    $("#fullNameLanguage").val(selectedLanguageElement.innerHTML);
    $('#selectedLanguage').val(valueSelected);
    $("#submitChangeLanguage").click();
});
window.addEventListener('mouseup', function (event) {
    var manuLanguages = document.getElementById('manuLanguages');
    if (event.target != manuLanguages) {
        manuLanguages.style.display = 'none';
    }
});

    //var arrayLang =
    //    [
    //        { label: "English", value: "en-US" },
    //        { label: "Español", value: "es-ES" },
    //        { label: "Arabic", value: "ar-SA" }
    //        { label: "French", value: "fr-FR" }
    //        { label: "Russian", value: "ru" }
    //        //'عربى(Arabic)', '中文(Chinese)', 'Deutsch', 'Filipino', 'Français',
    //        //'Italiano', '日本人(Japanese)', '한국어(Korean)', 'Nederlands', 'Português', 'Русский(Russian)', 'Tiếng Việt(Vietnamese)'];
    //    ]