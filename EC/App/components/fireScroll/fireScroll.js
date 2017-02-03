'use strict';
components.directive('fireScroll',
        function () {
            return {
                restrict: 'A',
                link: function (scope, element, attr) {
                    $(element).on('scroll', function (eventData) {
                        scope.$apply(function () { scope.fire('scroll', eventData); });
                    });
                }
            };
        });
