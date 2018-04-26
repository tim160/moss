(function () {

    'use strict';

    angular.module('EC')
        .service('SettingsDisclaimerService', ['$resource', SettingsDisclaimerService]);

    function SettingsDisclaimerService($resource) {
        return $resource('/api/SettingsDisclaimer', {}, {
            get: { url: '/api/SettingsDisclaimer/Get', method: 'GET', params: {}, isArray: false },
            save1: { url: '/api/SettingsDisclaimer/Save1', method: 'POST', params: {}, isArray: false },
            save2: { url: '/api/SettingsDisclaimer/Save2', method: 'POST', params: {}, isArray: false },
        });
    };
})();
