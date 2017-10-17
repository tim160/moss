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
        $scope.behavioralEditMode = false;
        $scope.externalInfluencesEditMode = false;
        $scope.isEditNote1 = false;
        $scope.isEditNote2 = false;

        $scope.report_id = $location.search().report_id;

        $scope.refresh = function (data) {
            data.incidentTypeAdd = 0;
            data.mediatorAdd = 0;
            data.departmentAdd = 0;

            for (var i = 0; i < data.report_secondary_type_selected.length; i++) {
                data.report_secondary_type_selected[i].inv_meth_bf_note = '';
                data.report_secondary_type_selected[i].inv_meth_ei_note = '';
                data.report_secondary_type_selected[i].inv_meth_ci_note = '';

                var r = $filter('filter')(data.report_investigation_methodology,
                    { 'report_secondary_type_id': data.report_secondary_type_selected[i].id });
                if (r.length !== 0) {
                    data.report_secondary_type_selected[i].inv_meth_bf_note =
                        r[0].company_root_cases_behavioral_note === null ? '' : r[0].company_root_cases_behavioral_note;
                    data.report_secondary_type_selected[i].inv_meth_ei_note =
                        r[0].company_root_cases_external_note === null ? '' : r[0].company_root_cases_external_note;
                    data.report_secondary_type_selected[i].inv_meth_ci_note =
                        r[0].company_root_cases_organizational_note === null ? '' : r[0].company_root_cases_organizational_note;
                }
            }
            $scope.model = data;
        };

        NewCaseInvestigationNotesService.get({ report_id: $scope.report_id }, function (data) {
            $scope.refresh(data);
        });

        $scope.incidentTypeAdd = function (item) {
            $scope.incidentTypeAddMode = false;
            var param = { report_id: $scope.report_id, company_secondary_type_add: item.id };
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

        $scope.mediatorAdd = function (item) {
            $scope.mediatorAddMode = false;
            var param = { report_id: $scope.report_id, mediator_add: item.id };
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

        $scope.departmentAdd = function (item) {
            $scope.departmentAddMode = false;
            var param = { report_id: $scope.report_id, department_add: item.id };
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
            var param = type === 1 ?
                { report_id: $scope.report_id, note1: $scope.model.note1 } :
                { report_id: $scope.report_id, note2: $scope.model.note2 };

            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.refresh(data);

                if (type === 1) {
                    $scope.isEditNote1 = false;
                } else {
                    $scope.isEditNote2 = false;
                }
            });
        };

        $scope.getBehavioralFactors = function (item) {
            console.log(item);
            var r = $filter('filter')($scope.model.report_investigation_methodology, { 'report_secondary_type_id': item.id });
            if (r.length !== 0) {
                r = $filter('filter')($scope.model.company_root_cases_behavioral, { 'id': r[0].company_root_cases_behavioral_id });
                if (r.length !== 0) {
                    return r[0].name_en;
                }
            }
            return 'Select Behavioral Factors';
        };

        $scope.getExternalInfluences = function (item) {
            var r = $filter('filter')($scope.model.report_investigation_methodology, { 'report_secondary_type_id': item.id });
            if (r.length !== 0) {
                r = $filter('filter')($scope.model.company_root_cases_external, { 'id': r[0].company_root_cases_external_id });
                if (r.length !== 0) {
                    return r[0].name_en;
                }
            }

            return 'Select External Influences';
        };

        $scope.getCampusInfluences = function (item) {
            var r = $filter('filter')($scope.model.report_investigation_methodology, { 'report_secondary_type_id': item.id });
            if (r.length !== 0) {
                r = $filter('filter')($scope.model.company_root_cases_organizational, { 'id': r[0].company_root_cases_organizational_id });
                if (r.length !== 0) {
                    return r[0].name_en;
                }
            }

            return 'Select Campus Influences';
        };

        $scope.changeBehavioralFactors = function (item, item2) {
            var param = { report_id: $scope.report_id, inv_meth_st_id: item.id, inv_meth_bf_id: item2.id, };
            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.changeExternalInfluences = function (item, item2) {
            var param = { report_id: $scope.report_id, inv_meth_st_id: item.id, inv_meth_ei_id: item2.id, };
            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.changeCampusInfluences = function (item, item2) {
            var param = { report_id: $scope.report_id, inv_meth_st_id: item.id, inv_meth_ci_id: item2.id, };
            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.saveNoteBehavioralFactors = function (item) {
            item.behavioralEditMode = false;
            var param = { report_id: $scope.report_id, inv_meth_st_id: item.id, inv_meth_bf_note: item.inv_meth_bf_note, };
            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.saveNoteExternalInfluences = function (item) {
            item.externalInfluencesEditMode = false;
            var param = { report_id: $scope.report_id, inv_meth_st_id: item.id, inv_meth_ei_note: item.inv_meth_ei_note, };
            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.saveNoteCampusInfluences = function (item) {
            item.campusInfluencesEditMode = false;
            var param = { report_id: $scope.report_id, inv_meth_st_id: item.id, inv_meth_ci_note: item.inv_meth_ci_note, };
            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.refresh(data);
            });
        };
    }
}());
