
'use strict';
components
    .directive("timePicker",
     function () {
         return {
             require: '?ngModel',
             link: function ($scope, $element, $attrs, ngModel) {
                 var element = $($element);
                 element.timepicker({
                     disableFocus: true,
                     showSeconds: false,
                     showMeridian: false,
                     minuteStep: 1,
                     showWidgetOnAddonClick: false
                 });

                 //bootstrap-timepicker work differently than datepicker that we use
                 //to keep it consistent we make it so that when the time textbox is focus we show the timepicker too
                 element.focus(function () {
                     element.timepicker('showWidget');
                 });

                 var isInitialized = false;
                 //on change of the input
                 element.on('blur change', function () {
                     if (!isInitialized) {
                         isInitialized = true;
                         return;
                     }
                     //has the value changed?
                     if (element.val() == ngModel.$viewValue) { return; }

                     $scope.$root.$safeApply(function () {                         
                             ngModel.$setViewValue(element.val());
                     });
                 });

                 //also the button that come after timepicker textbox will also popup the timepicker
                 element.next().click(function () {

                     if ($('.bootstrap-timepicker-widget').length) {
                         element.timepicker('hideWidget');
                     } else {
                         element.focus();
                     }

                     return false;
                 })
             }
         };
     });