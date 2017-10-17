(function () {
    ROOT = $('base').attr('href');

    'use strict';

    angular.module('EC', [
        'ngResource',
        'ngAnimate',

        'EC',
    ]);

    angular.module('EC').config(['$locationProvider',
        function ($locationProvider) {
            $locationProvider.html5Mode({
                enabled: true,
                requireBase: false
            });
        }])
    .run(function () {
    });

    angular.module('EC').filter('getBy', function () {
        return function (arr, prop, val, valprop, array) {
            if (arr === undefined) {
                return null;
            }
            if (array) {
                array = [];
            }
            var i = 0, len = arr.length;
            for (; i < len; i++) {
                var cval;
                if ((prop === undefined) | (prop === '')) {
                    cval = arr[i];
                } else {
                    cval = arr[i][prop];
                }
                if (cval === val) {
                    if (valprop !== undefined) {
                        if (array !== undefined) {
                            array.push(arr[i][valprop]);
                        } else {
                            return arr[i][valprop];
                        }
                    } else {
                        if (array !== undefined) {
                            array.push(arr[i][valprop]);
                        } else {
                            return arr[i];
                        }
                    }
                }
            }
            if (array !== undefined) {
                return array;
            }
            return null;
        };
    });

    angular.module('EC').directive('dropbox', function () {
        var directive = {};

        directive.restrict = 'E';

        directive.template = function (elem, attr) {
            var html = '<div class="select" ng-click="active = !active">';
            html += '<a href="#" class="slct" ng-class="{ active: active }">' + (attr.text || '{{textexpr}}') + '</a>';
            html += '<ul class="drop slide" ng-class="{ active: active }">';
            html += '<li ng-repeat="item in list" ng-click="onSelect(rootitem == null ? item : rootitem, item)">';
            html += '<a href="">' + attr.itemtext + '</a>';
            html += '</li></ul>';
            html += '</div>';
            return html;
        };

        directive.scope = {
            list: '=ngDropboxList',
            textexpr: '=ngDropboxTextexpr',
            rootitem: '=ngDropboxRootitem',
            onSelect: '=ngDropboxOnSelect',
        };

        return directive;
    });

    angular.module('EC').animation('.slide', function () {
        var NG_HIDE_CLASS = 'active';
        return {
            beforeAddClass: function (element, className, done) {
                if (className === NG_HIDE_CLASS) {
                    element.slideDown(done);
                }
            },
            removeClass: function (element, className, done) {
                if (className === NG_HIDE_CLASS) {
                    element.slideUp(done);
                }
            }
        };
    });
})();

