(function () {

    'use strict';

    angular
        .module('EC')
        .controller('NewCaseInvestigationNotesController',
            ['$scope', '$filter', '$location', 'orderByFilter', 'NewCaseInvestigationNotesService', NewCaseInvestigationNotesController]);

    /*eslint max-statements: ["error", 40]*/
    function NewCaseInvestigationNotesController($scope, $filter, $location, orderByFilter, NewCaseInvestigationNotesService) {
        $scope.model = {};

        $scope.incidentTypeAddMode = false;
        $scope.mediatorAddMode = false;
        $scope.departmentAddMode = false;
        $scope.behavioralEditMode = false;
        $scope.externalInfluencesEditMode = false;
        $scope.isEditNote1 = false;
        $scope.isEditNote2 = false;

        $scope.report_id = $filter('parseUrl')($location.$$absUrl, 'report_id');

        $scope.refresh = function (data) {
            data.incidentTypeAdd = 0;
            data.mediatorAdd = 0;
            data.departmentAdd = 0;
            console.log(data.report_secondary_type_selected.length);
            for (var i = 0; i < data.report_secondary_type_selected.length; i++) {
                data.report_secondary_type_selected[i].inv_meth_bf_note = '';
                data.report_secondary_type_selected[i].inv_meth_ei_note = '';
                data.report_secondary_type_selected[i].inv_meth_ci_note = '';

                var r = $filter('filter')(data.report_investigation_methodology,
                    { 'report_secondary_type_id': data.report_secondary_type_selected[i].id }, true);
                if (r.length !== 0) {
                    data.report_secondary_type_selected[i].inv_meth_bf_note =
                        r[0].company_root_cases_behavioral_note === null ? '' : r[0].company_root_cases_behavioral_note;
                    data.report_secondary_type_selected[i].inv_meth_ei_note =
                        r[0].company_root_cases_external_note === null ? '' : r[0].company_root_cases_external_note;
                    data.report_secondary_type_selected[i].inv_meth_ci_note =
                        r[0].company_root_cases_organizational_note === null ? '' : r[0].company_root_cases_organizational_note;
                }
            }
            data.note1 = data.note1 || '';
            data.note2 = data.note2 || '';
            $scope.model = data;
        };

        $scope.noteP = function (note) {
            note = note || '';
            return note.split('\n').join('<br/>');
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

        $scope.mediatorDelete = function (id, mode) {
            var param = { report_id: $scope.report_id, mediator_delete: id, mode: mode };
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
            $scope.editNote1 = false;
            $scope.editNote2 = false;
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
            var r = $filter('filter')($scope.model.report_investigation_methodology, { 'report_secondary_type_id': item.id }, true);
            if (r.length !== 0) {
                r = $filter('filter')($scope.model.company_root_cases_behavioral, { 'id': r[0].company_root_cases_behavioral_id }, true);
                if (r.length !== 0) {
                    return r[0].name_en;
                }
            }
            return 'Select';
        };

        $scope.getExternalInfluences = function (item) {
            var r = $filter('filter')($scope.model.report_investigation_methodology, { 'report_secondary_type_id': item.id }, true);
            if (r.length !== 0) {
                r = $filter('filter')($scope.model.company_root_cases_external, { 'id': r[0].company_root_cases_external_id }, true);
                if (r.length !== 0) {
                    return r[0].name_en;
                }
            }

            return 'Select';
        };

        $scope.getCampusInfluences = function (item) {
            var r = $filter('filter')($scope.model.report_investigation_methodology, { 'report_secondary_type_id': item.id }, true);
            if (r.length !== 0) {
                r = $filter('filter')($scope.model.company_root_cases_organizational, { 'id': r[0].company_root_cases_organizational_id }, true);
                if (r.length !== 0) {
                    return r[0].name_en;
                }
            }

            if ($('#is_cc').val() === 'True') {
                return 'Select ';
            } else {
                return 'Select ';
            }
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

        $scope.addPerson = {
            Role: { id: 0, role_en: 'Select one' },
        };

        $scope.addNewPerson = function () {
            $scope.form.$setDirty(true);
            if ((!$scope.addPerson.FirstName) | (!$scope.addPerson.LastName) | (!$scope.addPerson.Title) | ($scope.addPerson.Role.id === 0)) {
                return;
            }
            $scope.mediatorAddMode = false;
            var param = {
                report_id: $scope.report_id,
                addPersonFirstName: $scope.addPerson.FirstName,
                addPersonLastName: $scope.addPerson.LastName,
                addPersonTitle: $scope.addPerson.Title,
                addPersonRole: $scope.addPerson.Role.id,
            };
            NewCaseInvestigationNotesService.post(param, function (data) {
                $scope.addPerson.FirstName = '';
                $scope.addPerson.LastName = '';
                $scope.addPerson.Title = '';
                $scope.addPerson.Role = { id: 0, role_en: 'Select one' };
                $scope.refresh(data);
            });
        };

        $scope.getRoleTitle = function (id) {
            var result = $.grep($scope.model.addNewPersonRoles, function (e) {
                return e.id === id;
            });
            return result.length === 0 ? '' : ' (' + result[0].role_en + ')';
        };
    }
}());
