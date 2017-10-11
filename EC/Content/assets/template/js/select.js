//selects

$(document).ready(function () {
	$('.slct').click(function () {
	    var dropBlock = $(this).parent().find('.drop');

		if (dropBlock.is(':hidden')) {
			dropBlock.slideDown();

			$(this).addClass('active');

			$('.drop').find('li').click(function () {

				var selectResult = $(this).html();

				$(this).parent().parent().find('input').val(selectResult);

				//$(this).parent().parent().find('.slct').removeClass('active').html(selectResult);

				dropBlock.slideUp();
			});

		} else {
			$(this).removeClass('active');
			dropBlock.slideUp();
		}

		return false;
	});
});


// Slidemenu 
$(window).on("load", function () {

	$(".ninja-btn, .panel-overlay, .panel li a").click(function () {
		$(".ninja-btn, .panel-overlay, .panel").toggleClass("active");
		/* Check panel overlay */
		if ($(".panel-overlay").hasClass("active")) {
			$(".panel-overlay").fadeIn(100);
		} else {
			$(".panel-overlay").fadeOut(100);
		}
	});

});
