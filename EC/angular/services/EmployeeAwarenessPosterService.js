(function () {

    'use strict';

    angular.module('EC')
        .service('EmployeeAwarenessPosterService', ['$resource', EmployeeAwarenessPosterService]);

    function EmployeeAwarenessPosterService($resource) {
        return $resource('/api/EmployeeAwarenessPoster', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();
