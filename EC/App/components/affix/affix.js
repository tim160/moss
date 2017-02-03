'use strict';
components
    .directive('affix', function () {
        return function ($scope, $element, $attrs) {

            //Private State
            var el = $($element);

            //affix this element
            el.affix();
        };
    });