$(document).ready(function(){
    $(".settingMenu ul li").click(function(){
        $('.settingMenu ul li').removeClass('clicked');
        $('.arrow').remove();
        $(this).addClass('clicked');
        $(this).append('<div class="arrow"></div> ');
    });
    $('.secondaryMenu ul li').click(function(){
        $('.secondaryMenu ul li').removeClass('active');
        $(this).addClass('active');
    });

});