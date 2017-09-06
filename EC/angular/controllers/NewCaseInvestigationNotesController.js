(function () {

    'use strict';

    angular
        .module('EC')
        .controller('NewCaseInvestigationNotesController',
            ['$scope', '$filter', '$location', 'orderByFilter', 'NewCaseInvestigationNotesService', NewCaseInvestigationNotesController]);

    function NewCaseInvestigationNotesController($scope, $filter, $location, orderByFilter, NewCaseInvestigationNotesService) {
        $scope.model = {};

        $scope.incidentTypeAddMode = false;
        $scope.mediatorAddMode = false;
        $scope.departmentAddMode = false;

        $scope.report_id = parseInt($location.absUrl().substring($location
            .absUrl().toLowerCase().indexOf('InvestigationNotes/'.toLowerCase()) + 'InvestigationNotes/'.toLowerCase().length));

        $scope.refresh = function (data) {
            data.report_secondary_type_selected_avilable.splice(0, 0, { id: 0, secondary_type_en: 'Please select' });
            data.mediator_all.splice(0, 0, { id: 0, first_nm: 'Please select' });
            data.departments_all.splice(0, 0, { id: 0, department_en: 'Please select' });

            data.incidentTypeAdd = 0;
            data.mediatorAdd = 0;
            data.departmentAdd = 0;

            $scope.model = data;
        };

        NewCaseInvestigationNotesService.get({ report_id: $scope.report_id }, function (data) {
            $scope.refresh(data);
        });

        $scope.incidentTypeAdd = function () {
            $scope.incidentTypeAddMode = false;
            var param = { report_id: $scope.report_id, company_secondary_type_add: $scope.model.incidentTypeAdd };
            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.incidentTypeDelete = function (id) {
            var param = { report_id: $scope.report_id, company_secondary_type_delete: id };
            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.mediatorAdd = function () {
            $scope.mediatorAddMode = false;
            var param = { report_id: $scope.report_id, mediator_add: $scope.model.mediatorAdd };
            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.mediatorDelete = function (id) {
            var param = { report_id: $scope.report_id, mediator_delete: id };
            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.departmentAdd = function () {
            $scope.departmentAddMode = false;
            var param = { report_id: $scope.report_id, department_add: $scope.model.departmentAdd };
            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.departmentDelete = function (id) {
            var param = { report_id: $scope.report_id, department_delete: id };
            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.saveNote = function (type) {
            var param = type === 1 ? { report_id: $scope.report_id, note1: $scope.model.note1 } : { report_id: $scope.report_id, note2: $scope.model.note2 };;
            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.refresh(data);
            });
        };
    }
}());
