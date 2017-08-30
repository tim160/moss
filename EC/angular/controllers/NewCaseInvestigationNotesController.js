(function () {

    'use strict';

    angular
        .module('EC')
        .controller('NewCaseInvestigationNotesController',
            ['$scope', '$filter', 'orderByFilter', 'NewCaseInvestigationNotesService', NewCaseInvestigationNotesController]);

    function NewCaseInvestigationNotesController($scope, $filter, orderByFilter, NewCaseInvestigationNotesService) {
        NewCaseInvestigationNotesService.get({}, function() {
        });
    }
}());
