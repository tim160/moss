(function () {

    'use strict';

    angular.module('EC')
        .service('NewCaseCaseClosureReportService', ['$resource', NewCaseCaseClosureReportService]);

    function NewCaseCaseClosureReportService($resource) {
        return $resource('/api/NewCaseCaseClosureReport', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
        });
    };
})();
