'use strict';
components
    .directive('activeHover', function () {
        return function ($scope, $element, $attrs) {

            //Private State
            var el = $($element);
            var container = el.parent();
            var activeClass = "active";

            //Config
            if (angular.isDefined($attrs.activeHoverContainer)) {
                container = el.closest($attrs.activeHoverContainer);
            }
            if (angular.isDefined($attrs.activeHoverThis)) {
                container = el;
            }
            if (angular.isDefined($attrs.activeHoverClass)) {
                activeClass = $attrs.activeHoverClass;
            }
            el.hover(function () {
                container.addClass(activeClass);
            }, function () {
                container.removeClass(activeClass);
            });
        };
    });
