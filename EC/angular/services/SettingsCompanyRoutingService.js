(function () {

    'use strict';

    angular.module('EC')
        .service('SettingsCompanyRoutingService', ['$resource', SettingsCompanyRoutingService]);

    function SettingsCompanyRoutingService($resource) {
        return $resource('/api/SettingsCompanyRouting', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
            delete: { method: 'POST', params: {}, isArray: false },
            put: { method: 'PUT', params: {}, isArray: false },
        });
    };
})();
