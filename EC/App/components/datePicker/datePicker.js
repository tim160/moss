'use strict';
components
    .directive("datePicker",
     function () {
         return {
             require: '?ngModel',
             template: '<input type="text"><span class="add-on"><i class="icon-calendar"></i></span>',
            link: function ($scope, $element, $attrs, ngModel) {
                 if (!ngModel) { throw "No ngModel was specified for the datePicker directive"; }


                 var element = $($element);
                 var inputElement = element.find('input').addClass($attrs.datePickerInputClass);

                 //is the element disabled
                 if ($attrs.ngDisabled && $scope.$eval($attrs.ngDisabled)) {
                     //make the input disabled
                     element.find('input').attr('disabled', 'disabled').val($scope.$eval($attrs.ngModel));
                     return;
                 }
                 var xDateFormat = 'dd MMM yyyy';   //this will format as 29 MAR 2014
                 var datePickerFormat = 'dd M yyyy';    //this will also format as 29 MAR 2014
                 element.datepicker({
                     //the default time format
                     format:  datePickerFormat,
                     autoclose: true
                 });

                 var isInitialized = false;
                 //on change of the input
                 inputElement.on('blur change', function () {
                     if (!isInitialized) {
                         isInitialized = true;
                         return;
                     }

                     //has the value changed?
                     if (ngModel.$viewValue != undefined) {
                         if (inputElement.val() == ngModel.$viewValue.toString(xDateFormat)) { return; }
                     } else {
                         if (!inputElement.val()) { return; }
                     }
                     

                     $scope.$root.$safeApply(function () {
                         if (inputElement.val()) {
                             ngModel.$setViewValue(new XDate(inputElement.val() + ' GMT', true));
                         }
                         else {
                            ngModel.$setViewValue(null);
                         }
                         
                     });
                 });
                 

                 ////on render of the ngModel, update the datepicker value
                 ngModel.$render = function (value) {
                     if ($(document).find(element).length) {
                         //prevent it from doing double change
                         if (ngModel.$viewValue != undefined) {
                             if (inputElement.val() ==
                             ngModel.$viewValue.toString(xDateFormat) ||
                             (!inputElement.val() == !ngModel.$viewValue && !ngModel.$viewValue == true)) {
                                 return;
                             }
                              //update the date of the element
                              element.datepicker('setDate', ngModel.$viewValue.toString(xDateFormat));
                             
                         } else {
                             element.datepicker('setDate', "");
                         }

                         if ($attrs.ngChange) {
                            $scope.$eval($attrs.ngChange);
                         }
                     } else {

                         //the element is no longer on the page
                         ngModel.$render = angular.noop;
                     }
                 };
             }
         };
     });
