(function () {
    ROOT = $('base').attr('href');

    'use strict';

    angular.module('EC', [
        'ngResource',
        'ngAnimate',
        'ngSanitize',
        'nvd3',
        'ngFileUpload',
        'ui.calendar',

        'EC',
    ]);

    angular.module('EC').config(['$locationProvider',
        function () {
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

    angular.module('EC').filter('parseUrl', function () {
        return function (url, param) {
            var p = url.indexOf('#');
            if (p !== -1) {
                url = url.substring(0, p);
            }
            if (url.indexOf('?') === -1) {
                var s = url.split('/');
                return s[s.length - 1];
            }
            var params = url.split(/(\?|\&)([^=]+)\=([^&]+)/);
            return params[params.findIndex(function (x) {
                if (x === param) {
                    return true;
                }
            }) + 1];
        };
    });

    angular.module('EC').directive('dropbox', function () {
        var directive = {};

        directive.restrict = 'E';

        directive.template = function (elem, attr) {
            var html = '<div class="select" ng-init="expanded = false">';
            var expr = (attr.text || '{{textexpr}}');
            html += '<a href="#" class="slct" ng-click="expanded = !expanded" ng-class="{ active: expanded }">' + expr + '</a>';
            html += '<ul class="drop slide" ng-class="{ active: expanded }">';
            html += '<li ng-repeat="item in list" ng-click="onSelectFunction(item)">';
            html += '<a href="">' + attr.itemtext + '</a>';
            html += '</li></ul>';
            html += '</div>';
            return html;
        };

        directive.scope = {
            expanded: '&',
            list: '=ngDropboxList',
            textexpr: '=ngDropboxTextexpr',
            rootitem: '=ngDropboxRootitem',
            onSelect: '=ngDropboxOnSelect',
        };

        directive.link = function (scope, element) {
            $(element).hover(
                function () {
                },
                function () {
                    scope.$apply(function () {
                        scope.expanded = false;
                    });
            });
            scope.onSelectFunction = function (item) {
                scope.expanded = false;
                var getType = {};
                if (scope.onSelect && getType.toString.call(scope.onSelect) === '[object Function]') {
                    scope.onSelect(scope.rootitem || item, item);
                } else {
                    scope.onSelect = item;
                }
            };
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

    angular.module('EC').directive('repeatDone', function () {
        return function (scope, element, attrs) {
            if (scope.$last) {
                scope.$eval(attrs.repeatDone);
            }
        };
    });

    angular.module('EC').directive('stringToNumber', function() {
        return {
            require: 'ngModel',
            link: function(scope, element, attrs, ngModel) {
                ngModel.$parsers.push(function(value) {
                    return '' + value;
                });
                ngModel.$formatters.push(function(value) {
                    return parseFloat(value);
                });
            }
        };
    });

})();

