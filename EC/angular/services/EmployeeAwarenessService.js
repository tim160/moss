(function () {

    'use strict';

    angular.module('EC')
        .service('EmployeeAwarenessService', ['$resource', EmployeeAwarenessService]);

    function EmployeeAwarenessService($resource) {
        return $resource('/api/EmployeeAwareness', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();
