$(document).ready(function(){

	// OWL Carousel options
  var active_class = "active";

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


  fieldActiveOnFocus( ".learnMore-form__content-input input" );
  fieldActiveOnFocus( ".loginForm__input input" );

  function fieldActiveOnFocus ( inputSelector ){
    $( inputSelector ).focus(function(){
      $( this ).parent().addClass(active_class);
      $( this ).focusout(function(){
        $( this ).parent().removeClass(active_class);
      });
    });
  }

  // Scroll to element and focus


  $(' a[href*="#"]')
  // Remove links that don't actually link to anything
  .not('[href="#"]')
  .not('[href="#0"]')
  .click(function(event) {
    // On-page links
    if (
      location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') 
      && 
      location.hostname == this.hostname
    ) {
      // Figure out element to scroll to
      var target = $(this.hash);
      
      headerHeight = $(".main-headerBg").height(); // Get fixed header height

      target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
      // Does a scroll target exist?
      if (target.length) {
        // Only prevent default if animation is actually gonna happen
        event.preventDefault();
        $('html, body').animate({
          scrollTop: target.offset().top - headerHeight
        }, 1000 );
        //return false;
      }
    }
  });

  // Mobile btn
  $(".main-headerLogo-mobile-burgerBtn").click(function(e){
    e.preventDefault();
    $(".main-headerMenu").toggleClass("on");
    $(document).mouseup(function(e) 
      {
          var container = $(".main-header");

          // if the target of the click isn't the container nor a descendant of the container
          if (!container.is(e.target) && container.has(e.target).length === 0 )
          {
            $(".main-headerMenu").removeClass("on");
          }
      });
    $(".main-headerMenu__nav").mouseup(function(){
      $(".main-headerMenu").removeClass("on");
    });
  });

  // Fixed main menu on scroll

  $(window).scroll(function(){
    fixedHeader();
  });
  fixedHeader();
  

  function fixedHeader(){
    if ($(window).scrollTop() >= 36) { // 36px height of top header
       $(".main-headerBg").addClass("fixed-header");
       $(".main").addClass("fixed-header");
    }
    else {
       $(".main-headerBg").removeClass("fixed-header");
       $(".main").removeClass("fixed-header");
    }
  }

  // About page drop info

  var leaderShipSection         = ".leaderShipSection",
      leaderShipBlock           = ".leaderShipBlock",
      leaderShipBlock_Active    = ".leaderShipBlock.active",
      leaderShipBlock_FullInfo  = ".leaderShipBlock-infoContainerBg";


  leadeShipInfo_Open( ".leaderShipBlock__link" );
  leadeShipInfo_Close(".leaderShipBlock-infoContainer__close");

  $('html').click(function(e) {
    //if clicked element is not your element and parents aren't your div
    if (e.target.id != leaderShipBlock && $(e.target).parents('.leaderShipSection__container').length == 0) {
      //do stuff
      $(leaderShipSection).removeClass(active_class);
      $(leaderShipBlock).css( "marginBottom", "0" ).removeClass(active_class);
      $(leaderShipBlock_FullInfo).fadeOut();
    }
  });

  function leadeShipInfo_Open ( leadeShipInfo_linkSelector ){
    $ ( leadeShipInfo_linkSelector ).click(function( event ){
      event.preventDefault();

      var $this = $( this );
      var dropBlock_height = $this.next().height();

      if (!( $this.parent(leaderShipBlock).hasClass(active_class) )){
        $(leaderShipSection).addClass(active_class);

        $(leaderShipBlock_Active).toggleClass(active_class)
          .css( "marginBottom", "0" )
          .children(leaderShipBlock_FullInfo).fadeOut();
        $this.parents(leaderShipBlock).toggleClass(active_class)
          .css( "marginBottom", dropBlock_height+"px" )
          .children(leaderShipBlock_FullInfo).fadeIn();
      }
    });
  }

  function leadeShipInfo_Close ( leadeShipInfo_closeLinkSelector ){
    $ ( leadeShipInfo_closeLinkSelector ).click(function( event ){
      event.preventDefault();

      var $this = $( this );

      $this.closest(leaderShipBlock_FullInfo).fadeOut();
      $this.closest(leaderShipBlock).css( "marginBottom", "0" );
      $(leaderShipSection).removeClass(active_class);
      $this.closest(leaderShipBlock).removeClass(active_class);
    });
  }

});
