(function () {

    'use strict';

    angular.module('EC')
        .factory('validateSettingsUser',['$http','$q', function ($http, $q) {
            return {
                validate: function (value, rv) {
                    if (rv === undefined) {
                        if ((value === null) || (value === undefined) || (value.trim() === '')) {
                            return false;
                        }
                    } else {
                        if (value === null || value.trim() === '' || !rv.test(value.trim())) {
                            return false;
                        }
                    }
                    return true;
                }
            };
        }]);
}());
