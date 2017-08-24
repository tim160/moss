(function () {

    'use strict';

    angular.module('EC')
        .service('SettingsCompanyCaseAdminDepartmentService', ['$resource', SettingsCompanyCaseAdminDepartmentService]);

    function SettingsCompanyCaseAdminDepartmentService($resource) {
        return $resource('/api/SettingsCompanyCaseAdminDepartment', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
            delete: { method: 'POST', params: {}, isArray: false },
        });
    };
})();
