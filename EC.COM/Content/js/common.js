$(document).ready(function(){

	// OWL Carousel options

  $(".owl-carousel").owlCarousel({
  	"items": 							1,
  	"nav": 								true,
  	"loop": 							true,
  	"dots": 							false,
  	"navContainerClass": "owl-nav",
  	"autoplay": 					true,
  	"autoplayTimeout": 		4500
  });

  // Active input in contact form

  $(".learnMore-form__content-input input").focus(function(){
  	$( this ).parent().addClass("active");
  	$( this ).focusout(function(){
	  	$( this ).parent().removeClass("active");
	  });
  });

  // Scroll to element and focus

  $("[data-scroll-to]").click(function() {
	  var $this = $(this),
	      $toElement      = $this.attr('data-scroll-to'),
	      $focusElement   = $this.attr('data-scroll-focus'),
	      $offset         = $this.attr('data-scroll-offset') * 1 || 0,
	      $speed          = $this.attr('data-scroll-speed') * 1 || 500;

	  $('html, body').animate({
	    scrollTop: $($toElement).offset().top + $offset
	  }, $speed);
	  
	  if ($focusElement) $($focusElement).focus();
	});

});