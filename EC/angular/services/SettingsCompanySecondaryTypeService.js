(function () {

    'use strict';

    angular.module('EC')
        .service('SettingsCompanySecondaryTypeService', ['$resource', SettingsCompanySecondaryTypeService]);

    function SettingsCompanySecondaryTypeService($resource) {
        return $resource('/api/SettingsCompanySecondaryType', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
            delete: { method: 'POST', params: {}, isArray: false },
        });
    };
})();
