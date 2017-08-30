(function () {

    'use strict';

    angular.module('EC')
        .service('NewCaseInvestigationNotesService', ['$resource', NewCaseInvestigationNotesService]);

    function NewCaseInvestigationNotesService($resource) {
        return $resource('/api/NewCaseInvestigationNotes', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();
