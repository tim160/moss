(function () {

    'use strict';

    angular.module('EC')
        .service('AnalyticsCACSService', ['$resource', AnalyticsCACSService]);

    function AnalyticsCACSService($resource) {
        return $resource('/api/AnalyticsCACS', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();
