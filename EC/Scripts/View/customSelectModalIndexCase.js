$('document').ready(function () {
    var selectedItem = "";
    $("#ddlOutcome").on("click", function (event) {
        var temp = $(event.currentTarget);
        temp.siblings('ul').slideDown(300);
    });

    $(".dropdown2 li").on("click", function (event) {
        var temp = $(event.currentTarget);
        var allOptions = temp.parents('ul ');
        allOptions.find('li').removeClass('selected');
        temp.addClass('selected');
        $(".dropdown2 #ddlOutcome").html(temp.text());
        $(".dropdown2 #ddlOutcome").attr('data-value', temp.attr('data-value'));
        $(".dropdown2 ul").slideUp();
    });

    $("#ddlReasonClosure").on("click", function (event) {
        var _temp = $(event.currentTarget);
        _temp.siblings('ul').slideDown(300);
    });

    $(".dropdown3 li").on("click", function (event) {
        var temp = $(event.currentTarget);
        var _allOptions = temp.parents('ul');
        _allOptions.find('li').removeClass('selected');
        temp.addClass('selected');
        var label = temp.parent().siblings('label');
        label.html(temp.text());
        label.attr('data-value', temp.attr('data-value'));
        if (label.attr('data-value') === 'yes') {
            $('.defaultHide').show();
        } else if (label.attr('data-value') === 'no') {
            $('.defaultHide').hide();
        }
        _allOptions.slideUp();
    });


    $("#anotherDialog .close1").on('click', function () {
        $("#anotherDialog").toggle();
    });
    var button = "";
    $("#not_resolve_btn").on('click', function (event) {
        button = $(event.currentTarget).val();
        sendAjax(button);
    });

    $("#rblIsCleryActCrime").on("click", function (event) {
        var _temp = $(event.currentTarget);
        _temp.siblings('ul').slideDown(300);
    });

    $("#ddlCrimeStatistics").on("click", function (event) {
        var _temp = $(event.currentTarget);
        _temp.siblings('ul').slideDown(300);
    });
    $("#ddlGeographicLocations").on("click", function (event) {
        var _temp = $(event.currentTarget);
        _temp.siblings('ul').slideDown(300);
    });

});