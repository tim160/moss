// This is an attribute directive that emits an angular event when
// another directive has completed rendering. This code comes from
// http://stackoverflow.com/questions/15207788/calling-a-function-when-ng-repeat-has-finished
//
// The use of the angular $timeout capability is necessary here. This causes 
// 

'use strict';

components.directive('onFinishRender', ['$timeout', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            if (scope.$last === true) {
                $timeout(function () {
                    scope.$eval(attr.onFinishRender);
                });
            }
        }
    };
}]);

