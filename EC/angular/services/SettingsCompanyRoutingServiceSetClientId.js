(function () {

    'use strict';

    angular.module('EC')
        .service('SettingsCompanyRoutingServiceSetClientId', ['$resource', SettingsCompanyRoutingServiceSetClientId]);

    function SettingsCompanyRoutingServiceSetClientId($resource) {
        return $resource('/api/SettingsCompanyClient', {}, {
            post: { method: 'POST', params: {}, isArray: false }
        });
    };
})();
