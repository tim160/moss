$(document).ready(function () {
    //START Open mini menu for mobile
    function blockActivityHeight() {
        if ($('#menu').height() < 50) {
            $('.positionActivityIcon').height(89);
        }
        else {
            $('.positionActivityIcon').height($('#casesHeader').height());
        }
    }
    $('.mainTitle').click(function () {
        $('.mainTitle + div').toggle();
        blockActivityHeight();
    });
});