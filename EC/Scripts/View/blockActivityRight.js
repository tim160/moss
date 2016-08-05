$('document').ready(function () {
    function rightActivityShow() {
        var rightBlock = $("#rightRight");
        $(".positionActivityIcon").on('click', function () {
            if (!rightBlock.is(':empty')) {
                rightBlock.hide(500);
                rightBlock.empty();
                $('.positionActivityIcon').removeClass('active');
            }
            else {
                $.ajax(
                    {
                        method: "GET",
                        contentType: false,
                        processData: false,
                        cache: false,
                        url: "/Case/GetAjaxActivity/@Html.Raw(report_id)",
                    }
                   ).done(function (data) {//data from server
                       rightBlock.empty();
                       rightBlock.append(data);
                       rightBlock.show(500);
                       $('.positionActivityIcon').addClass('active');
                   }).fail(function (error) {
                       console.log(error);
                   });
            }
        });
    }


    rightActivityShow();
});
