(function () {

    'use strict';

    angular.module('EC')
        .service('AnalyticsRootCauseAnalysisService', ['$resource', AnalyticsRootCauseAnalysisService]);

    function AnalyticsRootCauseAnalysisService($resource) {
        return $resource('/api/AnalyticsRootCauseAnalysis', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();
