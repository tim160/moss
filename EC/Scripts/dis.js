(function () {
    ROOT = $('base').attr('href');

    'use strict';

    angular.module('EC', [
        'ngResource',
        'ngAnimate',

        'EC',
    ]);

    angular.module('EC').config(['$locationProvider',
        function () {
        }])
    .run(function () {
    });

    angular.module('EC').filter('getBy', function () {
        return function (arr, prop, val, valprop, array) {
            if (arr === undefined) {
                return null;
            }
            if (array) {
                array = [];
            }
            var i = 0, len = arr.length;
            for (; i < len; i++) {
                var cval;
                if ((prop === undefined) | (prop === '')) {
                    cval = arr[i];
                } else {
                    cval = arr[i][prop];
                }
                if (cval === val) {
                    if (valprop !== undefined) {
                        if (array !== undefined) {
                            array.push(arr[i][valprop]);
                        } else {
                            return arr[i][valprop];
                        }
                    } else {
                        if (array !== undefined) {
                            array.push(arr[i][valprop]);
                        } else {
                            return arr[i];
                        }
                    }
                }
            }
            if (array !== undefined) {
                return array;
            }
            return null;
        };
    });

    angular.module('EC').filter('parseUrl', function () {
        return function (url, param) {
            var params = url.split(/(\?|\&)([^=]+)\=([^&]+)/);
            return params[params.findIndex(function (x) {
                if (x === param) {
                    return true;
                }
            }) + 1];
        };
    });

    angular.module('EC').directive('dropbox', function () {
        var directive = {};

        directive.restrict = 'E';

        directive.template = function (elem, attr) {
            var html = '<div class="select" ng-init="expanded = false">';
            var expr = (attr.text || '{{textexpr}}');
            html += '<a href="#" class="slct" ng-click="expanded = !expanded" ng-class="{ active: expanded }">' + expr + '</a>';
            //var style = "{ 'display': expanded ? 'block' : 'none' }";
            html += '<ul class="drop slide" ng-class="{ active: expanded }">';
            html += '<li ng-repeat="item in list" ng-click="onSelectFunction(item)">';
            html += '<a href="">' + attr.itemtext + '</a>';
            html += '</li></ul>';
            html += '</div>';
            return html;
        };

        directive.scope = {
            expanded: '&',
            list: '=ngDropboxList',
            textexpr: '=ngDropboxTextexpr',
            rootitem: '=ngDropboxRootitem',
            onSelect: '=ngDropboxOnSelect',
        };

        directive.link = function (scope, element) {
            $(document).on('click', function () {
                var isChild = $(element).has(event.target).length > 0;
                var isSelf = element[0] === event.target;
                var isInside = isChild || isSelf;
                if (!isInside) {
                    scope.$apply(function () {
                        scope.expanded = false;
                    });
                }
            });
            scope.onSelectFunction = function (item) {
                scope.expanded = false;
                var getType = {};
                if (scope.onSelect && getType.toString.call(scope.onSelect) === '[object Function]') {
                    scope.onSelect(scope.rootitem || item, item);
                } else {
                    scope.onSelect = item;
                }
            };
        };

        return directive;
    });

    angular.module('EC').animation('.slide', function () {
        var NG_HIDE_CLASS = 'active';
        return {
            beforeAddClass: function (element, className, done) {
                if (className === NG_HIDE_CLASS) {
                    element.slideDown(done);
                }
            },
            removeClass: function (element, className, done) {
                if (className === NG_HIDE_CLASS) {
                    element.slideUp(done);
                }
            }
        };
    });
})();


(function () {

    'use strict';

    angular
        .module('EC')
        .controller('CasesController',
            ['$scope', '$filter', 'orderByFilter', 'CasesService', CasesController]);

    function CasesController($scope, $filter, orderByFilter, CasesService) {
        $scope.showAsGrid = true;
        $scope.mode = 1;
        $scope.reports = [];
        $scope.sortColumn = 'case_number';
        $scope.sortColumnDesc = true;

        $scope.counts = {
            Active: 0,
            Completed: 0,
            Spam: 0,
            Closed: 0,
        };

        $scope.refresh = function (mode, preload) {
            CasesService.get({ ReportFlag: mode, Preload: preload }, function (data) {
                $('.headerBlockTextRight > span').text(data.Title);
                for (var i = 0; i < data.Reports.length; i++) {
                    var r = $filter('filter')(data.ReportsAdv, { 'id': data.Reports[i].report_id }, true);
                    if ((r != null) && (r.length > 0)) {
                        data.Reports[i].AdvInfo = r[0];
                        data.Reports[i].total_days = r[0].total_days;
                        data.Reports[i].case_dt_s = r[0].case_dt_s;
                        data.Reports[i].cc_is_life_threating = r[0].cc_is_life_threating;
                        data.Reports[i].mediators = r[0].mediators;
                        data.Reports[i].severity_s = r[0].severity_s;
                    }

                    var r = $filter('filter')(data.Users, { 'id': data.Reports[i].last_sender_id }, true);
                    r = r.length === 0 ? null : r[0];
                    if (r === null) {
                        r = {
                            photo_path: '/Content/Icons/noPhoto.png',
                            first_nm: '',
                            last_nm: '',
                        };
                    }
                    r.sb_full_name = (r.first_nm + ' ' + r.last_nm).replace(' ', '_');
                    data.Reports[i].Last_sender = r;

                    var r = $filter('filter')(data.Users, { 'id': data.Reports[i].previous_sender_id }, true);
                    r = r.length === 0 ? null : r[0];
                    if (r === null) {
                        r = {
                            photo_path: '/Content/Icons/noPhoto.png',
                            first_nm: '',
                            last_nm: '',
                        };
                    }
                    r.sb_full_name = (r.first_nm + ' ' + r.last_nm).replace(' ', '_');
                    data.Reports[i].Previous_sender = r;
                }

                $scope.reports = data.Reports;
                $scope.mode = data.Mode;
                $scope.counts = data.Counts;
            });
        };
        $scope.refresh($scope.mode);

        $scope.openCase = function (id) {
            window.location ='/Case/Index/' + id;
        };

        $scope.sort = function (column) {
            if (column === $scope.sortColumn) {
                $scope.sortColumnDesc = !$scope.sortColumnDesc;
            } else {
                $scope.sortColumn = column;
                $scope.sortColumnDesc = false;
            }

            $scope.reports = orderByFilter($scope.reports, $scope.sortColumn, $scope.sortColumnDesc);
        };
    }
}());

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('EmployeeAwarenessController',
            ['$scope', '$filter', 'EmployeeAwarenessService', EmployeeAwarenessController]);

    function EmployeeAwarenessController($scope, $filter, EmployeeAwarenessService) {
        $scope.posters = [];
        $scope.categories = [];
        $scope.messages = [];
        $scope.languages = [];
        $scope.avaibleFormats = [];

        EmployeeAwarenessService.get({}, function (data) {
            $scope.posters = data.posters;
            $scope.categories = data.categories;
            $scope.messages = data.messages;
            $scope.avaibleFormats = data.avaibleFormats;
            $scope.languages = data.languages;
        });

        $scope.categoryName = function (category) {
            if (category === null) {
                return '';
            }
            if (typeof (category) === 'object') {
                return category.industry_posters_en;
            } else {
                return $scope.categoryName($filter('getBy')($scope.categories, 'id', category));
            }
        };

        $scope.categoryById = function () {
            return category.industry_posters_en;
        };

        $scope.messageName = function (message) {
            return message.message_posters_en;
        };

        $scope.selectedItemsCount = function () {
            var count = 0;

            var isFilter = false;
            angular.forEach($scope.categories, function (category) {
                if (category.Selected) {
                    isFilter = true;
                }
            });
            angular.forEach($scope.messages, function (message) {
                if (message.Selected) {
                    isFilter = true;
                }
            });

            angular.forEach($scope.posters, function (poster) {
                if (!isFilter) {
                    poster.IsVisible = true;
                } else {
                    poster.IsVisible = false;

                    angular.forEach($scope.categories, function (category) {
                        if (category.Selected) {
                            if ($filter('getBy')(poster.posterCategoryNames, 'id', category.id) != null) {
                                poster.IsVisible = true;
                            }
                        }
                    });

                    angular.forEach($scope.messages, function (message) {
                        if (message.Selected) {
                            if (poster.poster.poster_message_posters_id === message.id) {
                                poster.IsVisible = true;
                            }
                        }
                    });
                }

                count += poster.IsVisible ? 1 : 0;
            });
            return count;
        };
    }
}());

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('EmployeeAwarenessPosterController',
            ['$scope', '$filter', '$location', 'EmployeeAwarenessPosterService', EmployeeAwarenessPosterController]);

    function EmployeeAwarenessPosterController($scope, $filter, $location, EmployeeAwarenessPosterService) {
        $scope.SelectedSize = 1;
        $scope.SelectedLogo = 1;
        $scope.mainImage = '';
        $scope.id = parseInt($location.absUrl().substring($location.absUrl().indexOf('poster/') + 'poster/'.length));

        EmployeeAwarenessPosterService.get({ id: $scope.id }, function (data) {
            $scope.mainImage = data.mainImage;
        });

        $scope.Process = function () {
            console.log($scope.id, $scope.SelectedSize, $scope.SelectedLogo);
            EmployeeAwarenessPosterService.post({ type: $scope.id, size: $scope.SelectedSize, logo: $scope.SelectedLogo }, function (data) {
                window.location = data.file;
            });

            return true;
        };
    }
}());

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('NewCaseCaseClosureReportController',
            ['$scope', '$filter', 'orderByFilter', '$location', 'NewCaseCaseClosureReportService', NewCaseCaseClosureReportController]);

    function NewCaseCaseClosureReportController($scope, $filter, orderByFilter, $location, NewCaseCaseClosureReportService) {
        $scope.model = { };
        $scope.report_id = $filter('parseUrl')($location.$$absUrl, 'report_id');

        $scope.refresh = function (data) {
            data.outcomes.splice(0, 0, { id: 0, outcome_en: 'Please select' });

            data.report_cc_crime.cc_is_clear_act_crime = '' + data.report_cc_crime.cc_is_clear_act_crime;
            for (var i = 0; i < data.report_case_closure_outcomes.length; i++) {
                var r = $filter('filter')(data.report_non_mediator_involveds,
                    { 'id': data.report_case_closure_outcomes[i].non_mediator_involved_id }, true);
                if (r.length !== 0) {
                    data.report_case_closure_outcomes[i].user = r[0];
                }
                data.report_case_closure_outcomes[i].outcome_id =
                    data.report_case_closure_outcomes[i].outcome_id == null ? 0 : data.report_case_closure_outcomes[i].outcome_id;
            }

            data.reporter.outcome_id = data.reporter.outcome_id == null ? 0 : data.reporter.outcome_id;
            $scope.model = data;
        };

        NewCaseCaseClosureReportService.get({ report_id: $scope.report_id }, function (data) {
            $scope.refresh(data);
        });

        $scope.saveCrime = function () {
            NewCaseCaseClosureReportService.post({ report_id: $scope.report_id, report_cc_crime: $scope.model.report_cc_crime }, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.saveItem = function (user) {
            NewCaseCaseClosureReportService.post({ report_id: $scope.report_id, report_case_closure_outcome: user }, function (data) {
                $scope.refresh(data);
            });
        };
    }
}());

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

        $scope.report_id = $filter('parseUrl')($location.$$absUrl, 'report_id');

        $scope.refresh = function (data) {
            data.incidentTypeAdd = 0;
            data.mediatorAdd = 0;
            data.departmentAdd = 0;

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
            return 'Select Behavioral Factors';
        };

        $scope.getExternalInfluences = function (item) {
            var r = $filter('filter')($scope.model.report_investigation_methodology, { 'report_secondary_type_id': item.id }, true);
            if (r.length !== 0) {
                r = $filter('filter')($scope.model.company_root_cases_external, { 'id': r[0].company_root_cases_external_id }, true);
                if (r.length !== 0) {
                    return r[0].name_en;
                }
            }

            return 'Select External Influences';
        };

        $scope.getCampusInfluences = function (item) {
            var r = $filter('filter')($scope.model.report_investigation_methodology, { 'report_secondary_type_id': item.id }, true);
            if (r.length !== 0) {
                r = $filter('filter')($scope.model.company_root_cases_organizational, { 'id': r[0].company_root_cases_organizational_id }, true);
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

        $scope.addPerson = {
            Role: { id: 0, role_en: 'Select one' },
        };
        $scope.addNewPerson = function () {
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
                $scope.addPerson.Role = { id: 0, role_en: 'Select one' };
                $scope.refresh(data);
            });
        };
    }
}());

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('NewCaseReportController',
            ['$scope', '$filter', 'orderByFilter', '$location', 'NewCaseReportService', NewCaseReportController]);

    function NewCaseReportController($scope, $filter, orderByFilter, $location, NewCaseReportService) {
        $scope.report_id = parseInt(parseInt($location.absUrl().substring($location.absUrl().indexOf('report_id=') + 'report_id='.length)));
        $scope.model = {
            reportingFrom: 'Canada',
            reporterWouldLike: 'Contact Info Shared',
            reporterName: 'First Last',
            reporterIs: 'Member of the public',
            incidentHappenedIn: 'Toronto, ON, Canada',
            affectedDepartment: 'Marketing',
            partiesInvolvedName: '(Margot) Cooper',
            partiesInvolvedTitle: 'CFO',
            partiesInvolvedType: 'Case Administrators excluded',
            reportingAbout: 'Breach of Legal Obligations',
            incidentDate: 'Nov 1, 2016',
        };

        NewCaseReportService.get({ id: $scope.report_id }, function (data) {
            $scope.model = data;
        });
    }
}());

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('SettingsCompanyCaseAdminDepartmentController',
            ['$scope', '$filter', 'orderByFilter', 'SettingsCompanyCaseAdminDepartmentService', SettingsCompanyCaseAdminDepartmentController]);

    function SettingsCompanyCaseAdminDepartmentController($scope, $filter, orderByFilter, SettingsCompanyCaseAdminDepartmentService) {
        $scope.case_admin_departments = [];
        $scope.addMode = false;
        $scope.addName = '';

        $scope.refresh = function (data) {
            $scope.case_admin_departments = data.case_admin_departments;
        };

        SettingsCompanyCaseAdminDepartmentService.get({}, function (data) {
            $scope.refresh(data);
        });

        $scope.add = function () {
            $scope.addMode = false;
            SettingsCompanyCaseAdminDepartmentService.post({ addName: $scope.addName, addType: $scope.addType }, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.delete = function (id) {
            $scope.addMode = false;
            SettingsCompanyCaseAdminDepartmentService.delete({ DeleteId: id }, function (data) {
                $scope.refresh(data);
            });
        };
    }
}());

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('SettingsCompanyRoutingController',
            ['$scope', '$filter', 'orderByFilter', '$http', 'SettingsCompanyRoutingService', SettingsCompanyRoutingController]);

    function SettingsCompanyRoutingController($scope, $filter, orderByFilter, $http, SettingsCompanyRoutingService) {
        $scope.types = [];
        $scope.departments = [];
        $scope.users = [];
        $scope.usersLocation = [];
        $scope.scopes = [];
        $scope.items = [];
        $scope.files = [];
        $scope.uploadLine = 0;
        $scope.locations = [];
        $scope.locationItems = [];

        $scope.onShow = function () {
            SettingsCompanyRoutingService.get({}, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.refresh = function (data) {
            $scope.types = data.types;
            $scope.departments = data.departments;
            $scope.users = Array.from(data.users);
            $scope.usersLocation = Array.from(data.users);
            $scope.scopes = data.scopes;
            $scope.items = data.items;
            $scope.files = data.files;

            $scope.departments.splice(0, 0, { id: 0, department_en: 'Select' });
            $scope.users.splice(0, 0, { id: 0, first_nm: 'Select', last_nm: '' });
            $scope.scopes.splice(0, 0, { id: 0, scope_en: 'Select' });

            $scope.usersLocation.splice(0, 0, { id: 0, first_nm: 'Select Case Administrator', last_nm: '' });
            $scope.locations = data.locations;
            $scope.locationItems = data.locationItems;
        };

        $scope.onShow();

        $scope.delete = function (id) {
            SettingsCompanyRoutingService.delete({ DeleteId: id }, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.userName = function (item) {
            return item.first_nm + ' ' + item.last_nm;
        };

        $scope.typeName = function (id) {
            return $filter('filter')($scope.types, { 'id': id }, true)[0];
        };

        $scope.locationName = function (id) {
            return $filter('filter')($scope.locations, { 'id': id }, true)[0];
        };

        $scope.changeLine = function (line) {
            SettingsCompanyRoutingService.post({ model: line }, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.changeLineLocation = function (line) {
            SettingsCompanyRoutingService.post({ modelLocation: line }, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.upload = function (elem) {
            var data = new FormData();
            data.append('model', JSON.stringify($scope.uploadLine));
            data.append('file', elem.files[0]);

            $http({
                url: '/api/SettingsCompanyRouting',
                method: 'POST',
                data: data,
                headers: { 'Content-Type': undefined }, //this is important
                transformRequest: angular.identity //also important
            }).then(function (data) {
                $scope.refresh(data.data);
            }).catch(function(){
            });
        };

        $scope.fileSelect = function (line) {
            $scope.uploadLine = line;
        };
    }
}());

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('SettingsCompanySecondaryTypeController',
            ['$scope', '$filter', 'orderByFilter', 'SettingsCompanySecondaryTypeService', SettingsCompanySecondaryTypeController]);

    function SettingsCompanySecondaryTypeController($scope, $filter, orderByFilter, SettingsCompanySecondaryTypeService) {
        $scope.secondaryTypes = [];
        $scope.third_level_types = [];
        $scope.addMode = false;
        $scope.addName = '';
        $scope.addType = 0;

        $scope.refresh = function (data) {
            $scope.secondaryTypes = data.secondaryTypes;
            $scope.addType = data.secondaryTypes[0].id;
            $scope.third_level_types = data.third_level_types;
        };

        SettingsCompanySecondaryTypeService.get({}, function (data) {
            $scope.refresh(data);
        });

        $scope.add = function () {
            $scope.addMode = false;
            SettingsCompanySecondaryTypeService.post({ addName: $scope.addName, addType: $scope.addType }, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.findST = function (id) {
            for (var i = 0; i < $scope.secondaryTypes.length; i++) {
                if ($scope.secondaryTypes[i].id === id) {
                    return $scope.secondaryTypes[i].secondary_type_en;
                }
            }
            return '';
        };

        $scope.delete = function (id) {
            $scope.addMode = false;
            SettingsCompanySecondaryTypeService.delete({ DeleteId: id }, function (data) {
                $scope.refresh(data);
            });
        };
    }
}());

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('SettingsUserEditController',
            ['$scope', '$filter', '$location', 'SettingsUserEditService', SettingsUserEditController]);

    function SettingsUserEditController($scope, $filter, $location, SettingsUserEditService) {
        $scope.first_nm = '';
        $scope.last_nm = '';
        $scope.title_ds = '';
        $scope.email = '';
        $scope.departments = [];
        $scope.role = 5;
        $scope.departmentId = 0;
        $scope.user_permissions_approve_case_closure = 0;
        $scope.user_permissions_change_settings = 0;

        $scope.val_first_nm = false;
        $scope.val_last_nm = false;
        $scope.val_title_ds = false;
        $scope.val_email = false;
        $scope.val_departmentId = false;

        $scope.id = parseInt($location.absUrl().substring($location.absUrl().indexOf('user/') + 'user/'.length));
        $scope.id = isNaN($scope.id) ? 0 : $scope.id;

        SettingsUserEditService.get({ id: $scope.id }, function (data) {
            $scope.first_nm = data.model.first_nm;
            $scope.last_nm = data.model.last_nm;
            $scope.title_ds = data.model.title_ds;
            $scope.email = data.model.email;
            $scope.departments = data.departments;
            $scope.role = data.model.role;
            $scope.departmentId = data.model.departmentId;
            $scope.user_permissions_approve_case_closure = data.model.user_permissions_approve_case_closure.toString();
            $scope.user_permissions_change_settings = data.model.user_permissions_change_settings.toString();
        });

        $scope.validate = function (value, rv) {
            if (rv === undefined) {
                if ((value === null) || (value === undefined) || (value.trim() === '')) {
                    return false;
                }
            } else {
                if (value.trim() === '' || !rv.test(value.trim())) {
                    return false;
                }
            }
            return true;
        };

        $scope.post = function () {
            $scope.val_first_nm = !$scope.validate($scope.first_nm);
            $scope.val_last_nm = !$scope.validate($scope.last_nm);
            $scope.val_title_ds = !$scope.validate($scope.title_ds);
            $scope.val_email = !$scope.validate($scope.email, /^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$/);
            $scope.val_departmentId = $scope.departmentId == null || !$scope.validate($scope.departmentId.toString());

            if (!$scope.val_first_nm
                && !$scope.val_last_nm
                && !$scope.val_title_ds
                && !$scope.val_email
                && !$scope.val_departmentId
                ) {

                var model = {
                    id: $scope.id,
                    first_nm: $scope.first_nm,
                    last_nm: $scope.last_nm,
                    title_ds: $scope.title_ds,
                    email: $scope.email,
                    role_id: $scope.role,
                    company_department_id: $scope.departmentId,
                    user_permissions_approve_case_closure: $scope.user_permissions_approve_case_closure,
                    user_permissions_change_settings: $scope.user_permissions_change_settings,
                };
                SettingsUserEditService.post(model, function () {
                    if ($scope.id !== 0) {
                        window.location = window.location;
                    } else {
                        window.location = '/Settings/Mediators';
                    }
                });
            }
        };
    }
}());

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

(function () {

    'use strict';

    angular.module('EC')
        .service('EmployeeAwarenessPosterService', ['$resource', EmployeeAwarenessPosterService]);

    function EmployeeAwarenessPosterService($resource) {
        return $resource('/api/EmployeeAwarenessPoster', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
        });
    };
})();

(function () {

    'use strict';

    angular.module('EC')
        .service('EmployeeAwarenessService', ['$resource', EmployeeAwarenessService]);

    function EmployeeAwarenessService($resource) {
        return $resource('/api/EmployeeAwareness', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();

(function () {

    'use strict';

    angular.module('EC')
        .service('NewCaseCaseClosureReportService', ['$resource', NewCaseCaseClosureReportService]);

    function NewCaseCaseClosureReportService($resource) {
        return $resource('/api/NewCaseCaseClosureReport', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
        });
    };
})();

(function () {

    'use strict';

    angular.module('EC')
        .service('NewCaseInvestigationNotesService', ['$resource', NewCaseInvestigationNotesService]);

    function NewCaseInvestigationNotesService($resource) {
        return $resource('/api/NewCaseInvestigationNotes', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
        });
    };
})();

(function () {

    'use strict';

    angular.module('EC')
        .service('NewCaseReportService', ['$resource', NewCaseReportService]);

    function NewCaseReportService($resource) {
        return $resource('/api/NewCaseReport', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();

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

(function () {

    'use strict';

    angular.module('EC')
        .service('SettingsCompanyRoutingService', ['$resource', SettingsCompanyRoutingService]);

    function SettingsCompanyRoutingService($resource) {
        return $resource('/api/SettingsCompanyRouting', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
            delete: { method: 'POST', params: {}, isArray: false },
        });
    };
})();

(function () {

    'use strict';

    angular.module('EC')
        .service('SettingsCompanySecondaryTypeService', ['$resource', SettingsCompanySecondaryTypeService]);

    function SettingsCompanySecondaryTypeService($resource) {
        return $resource('/api/SettingsCompanySecondaryType', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
            delete: { method: 'POST', params: {}, isArray: false },
        });
    };
})();

(function () {

    'use strict';

    angular.module('EC')
        .service('SettingsUserEditService', ['$resource', SettingsUserEditService]);

    function SettingsUserEditService($resource) {
        return $resource('/api/SettingsUserEdit', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
        });
    };
})();
