$('document').ready(function () {
    function reportSummaryShow() {
        $(".leftBlockContent .closeIcon").click(function () {
            $(".leftBlockContent").hide(300);
            $("#menu a").each(function (indx, element) {
                var temp = $(element).attr("href");
                temp = temp.replace('true', 'false');
                $(element).attr("href", temp);
            });
            $("#messages_menu a").each(function (indx, element) {
                var temp = $(element).attr("href");
                console.log(temp);

                temp = temp.replace('true', 'false');
                $(element).attr("href", temp);
            });
            $("#slider").show();
        });
        $('#slider').on('click', function () {
            $("#slider").hide();
            $(".leftBlockContent").show(300);
            if (window.location.href.indexOf('false') > -1) {
                //var temp = window.location.href;
                //temp.replace('false', 'true');
                $("#menu a").each(function (indx, element) {
                    var temp = $(element).attr("href");
                    temp = temp.replace('false', 'true');
                    $(element).attr("href", temp);
                });
                $("#messages_menu a").each(function (indx, element) {
                   
                    var temp = $(element).attr("href");
                    console.log(temp);
                    temp = temp.replace('false', 'true');
                    $(element).attr("href", temp);
                });
            }
        });
    }
    if (window.location.href.indexOf('false') > -1) {
        $(".leftBlockContent").hide(0);
        $("#slider").show();
    }
    reportSummaryShow();
});
