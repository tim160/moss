'use strict';
components
    .directive('scrollFixedToContainer', function () {
        return function ($scope, $element, $attrs) {

            //Private State
            var el = $($element);
            var container = el.parent();
            var leftConfigured = false;
            var topConfiguered = false;
            var left = 0;
            var top = 0;

            //config
            if (angular.isDefined($attrs.scrollFixedToContainer)) {
                container = el.closest($attrs.scrollFixedToContainer);
            }
            //left configuration
            if (angular.isDefined($attrs.scrollFixedToContainerLeft)) {
                left = parseInt($attrs.scrollFixedToContainerLeft, 10);
                leftConfigured = true;
            }

            //top configuration
            if (angular.isDefined($attrs.scrollFixedToContainerTop)) {
                top = parseInt($attrs.scrollFixedToContainerTop, 10);
                topConfiguered = true;
            }

            var updateElementPostion = function () {
                var style = {
                    position: 'absolute'
                };

                if (leftConfigured) {
                    style.left = (left + container.scrollLeft()) + 'px';
                } else if (topConfiguered) {
                    style.top = (top + container.scrollTop()) + 'px';
                }

                el.css(style);
            }

            //on scroll of the container update the element position
            container.scroll(updateElementPostion);
        };
    });