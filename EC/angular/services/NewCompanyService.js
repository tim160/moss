(function () {

    'use strict';

    angular.module('EC')
        .service('NewCompanyService', ['$resource', NewCompanyService]);

    function NewCompanyService($resource) {
        return $resource('/api/Trainer', {}, {
            calc: { url: '/api/NewCompany/Calc', method: 'POST', params: {}, isArray: false },
            create: { url: '/api/NewCompany/Create', method: 'POST', params: {}, isArray: false },
        });
    };
})();
