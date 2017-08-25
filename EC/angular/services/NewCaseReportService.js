(function () {

    'use strict';

    angular.module('EC')
        .service('NewCaseReportService', ['$resource', NewCaseReportService]);

    function NewCaseReportService($resource) {
        return $resource('/api/NewCaseReport', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();
