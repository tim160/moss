(function () {

    'use strict';

    angular.module('EC')
        .service('NewCaseTeamService', ['$resource', NewCaseTeamService]);

    function NewCaseTeamService($resource) {
        return $resource('/api/NewCaseTeam', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
        });
    };
})();
