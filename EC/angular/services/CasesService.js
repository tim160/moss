(function () {

    'use strict';

    angular.module('EC')
        .service('CasesService', ['$resource', CasesService]);

    function CasesService($resource) {
        return $resource('/api/Cases', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();
