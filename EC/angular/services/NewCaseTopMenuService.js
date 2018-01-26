(function () {

    'use strict';

    angular.module('EC')
        .service('NewCaseTopMenuService', ['$resource', NewCaseTopMenuService]);

    function NewCaseTopMenuService($resource) {
        return $resource('/api/NewCaseTopMenu', {}, {
            get: { url: '/api/NewCaseTopMenu/get', method: 'GET', params: {}, isArray: false },
            setLifeThreating: { url: '/api/NewCaseTopMenu/setLifeThreating', method: 'POST', params: {}, isArray: false },
        });
    };
})();
