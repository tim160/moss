(function () {
    ROOT = $('base').attr('href');

    'use strict';

    angular.module('EC', [
        'ngResource',
        'ngAnimate',
        'ngSanitize',
        'nvd3',
        'ngFileUpload',

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
            var p = url.indexOf('#');
            if (p !== -1) {
                url = url.substring(0, p);
            }
            if (url.indexOf('?') === -1) {
                var s = url.split('/');
                return s[s.length - 1];
            }
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
            $(element).hover(
                function () {
                },
                function () {
                    scope.$apply(function () {
                        scope.expanded = false;
                    });
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

    angular.module('EC').directive('repeatDone', function () {
        return function (scope, element, attrs) {
            if (scope.$last) {
                scope.$eval(attrs.repeatDone);
            }
        };
    });
})();


(function () {

    'use strict';

    angular
        .module('EC')
        .controller('AnalyticsCACSController',
            ['$scope', 'AnalyticsCACSService', AnalyticsCACSController]);

    function AnalyticsCACSController($scope, AnalyticsCACSService) {

        $scope.cc_crime_statistics_categories = [];
        $scope.cc_crime_statistics_category = { id: 0 };
        $scope.totals = [];

        $scope.refresh = function () {
            AnalyticsCACSService.get({ category: $scope.cc_crime_statistics_category.id }, function (data) {
                if ($scope.cc_crime_statistics_categories.length === 0) {
                    $scope.cc_crime_statistics_categories = data.cc_crime_statistics_categories;
                    $scope.cc_crime_statistics_category = $scope.cc_crime_statistics_categories[0];
                }
                $scope.cacsChartData = data.report_cc_crime;
                $scope.api.refresh();

                $scope.totals = data.totals;
            });
        };

        $scope.refresh();

        $scope.selectCategory = function (item) {
            $scope.cc_crime_statistics_category = item;
            $scope.refresh();
        };

        $scope.cacsChart = {
            chart: {
                type: 'lineChart',
                height: 450,
                margin: {
                    top: 20,
                    right: 20,
                    bottom: 40,
                    left: 55
                },
                x: function (d) {
                    return d.x;
                },
                y: function (d) {
                    return d.y;
                },
                useInteractiveGuideline: true,
                dispatch: {
                    renderEnd: function(){
                        var wa = d3.select('.nv-legendWrap')[0][0].parentNode.getBBox().width;
                        var wl = d3.select('.nv-legendWrap')[0][0].getBBox().width;
                        var x = (wa - wl) / 2;
                        d3.select('.nv-legendWrap').attr('transform', 'translate(-' + x + ',-30)');
                    },
                },
                xAxis: {
                    axisLabel: 'Years'
                },
                yAxis: {
                    axisLabel: '',
                    tickFormat: function (d) {
                        return d3.format('d')(d);
                    },
                    axisLabelDistance: -10,
                    domain: [0, 1000],
                },
                legend: {
                    align: false
                },
                callback: function (chart) {
                },
                forceY: [0, 1],
            },
            title: {
                enable: false,
                text: ''
            },
            subtitle: {
                enable: false,
                text: '',
                css: {
                    'text-align': 'center',
                    'margin': '10px 13px 0px 7px'
                }
            },
            caption: {
                enable: false,
                html: '',
                css: {
                    'text-align': 'justify',
                    'margin': '10px 13px 0px 7px'
                }
            }
        };

        $scope.Total = function (type, id) {
            if (type === 1) {
                return $scope.totals.length <= id ? 0 : $scope.totals[id];
            }
            if (type === 2) {
                return $scope.totals.length <= id ? 0 : $scope.totals[id];
            }
        };
    }
}());

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('AnalyticsRootCauseAnalysisController',
            ['$scope', 'AnalyticsRootCauseAnalysisService', AnalyticsRootCauseAnalysisController]);

    function AnalyticsRootCauseAnalysisController($scope, AnalyticsRootCauseAnalysisService) {

        $scope.secondaryType = { id: 0 };
        $scope.secondaryTypes = [];

        $scope.refresh = function () {
            /*var types = $scope.secondaryTypes.map(function (v) {
                return v._selected;
            });*/
            AnalyticsRootCauseAnalysisService.get({ secondaryType: $scope.secondaryType.id }, function (data) {
                if ($scope.secondaryTypes.length === 0) {
                    $scope.secondaryTypes = data.SecondaryTypes;
                    $scope.secondaryType = $scope.secondaryTypes[0];
                }
                $scope.chartData1 = data.Behavioral;
                $scope.chartData2 = data.External;
                $scope.chartData3 = data.Organizational;
                $scope.chart1.chart.title = data.BehavioralTotal;
                $scope.chart2.chart.title = data.ExternalTotal;
                $scope.chart3.chart.title = data.OrganizationalTotal;
                $scope.chartColors = data.Colors;
            });
        };

        $scope.chartColors = ['#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472'];

        $scope.refresh();

        $scope.selectSecondaryTypes = function (item) {
            //item._selected = !item._selected;
            $scope.secondaryType = item;
            $scope.refresh();
        };


        $scope.chart1 = {
            chart: {
                type: 'pieChart',
                donut: true,
                donutRatio: 1,
                x: function (d) {
                    return d.name;
                },
                y: function (d) {
                    return d.count;
                },
                height: 500,
                showLabels: false,
                color: $scope.chartColors,
                duration: 500,
                labelThreshold: 0.01,
                labelSunbeamLayout: true,
                //legendPosition: 'bottom',
                showLegend: false,
                title: {
                    enable: true,
                    text: '',
                },
                labels: {
                    mainLabel: {
                        fontSize: 20,
                    },
                },
                legend: {
                    margin: {
                        top: 5,
                        right: 35,
                        bottom: 5,
                        left: 0
                    }
                }
            }
        };
        $scope.chart2 = {
            chart: {
                type: 'pieChart',
                donut: true,
                donutRatio: 1,
                x: function (d) {
                    return d.name;
                },
                y: function (d) {
                    return d.count;
                },
                height: 500,
                showLabels: false,
                color: $scope.chartColors,
                duration: 500,
                labelThreshold: 0.01,
                labelSunbeamLayout: true,
                showLegend: false,
                legend: {
                    margin: {
                        top: 5,
                        right: 35,
                        bottom: 5,
                        left: 0
                    }
                }
            }
        };

        $scope.chart3 = {
            chart: {
                type: 'pieChart',
                donut: true,
                donutRatio: 1,
                x: function (d) {
                    return d.name;
                },
                y: function (d) {
                    return d.count;
                },
                height: 500,
                showLabels: false,
                color: $scope.chartColors,
                duration: 500,
                labelThreshold: 0.01,
                labelSunbeamLayout: true,
                showLegend: false,
                legend: {
                    margin: {
                        top: 5,
                        right: 35,
                        bottom: 5,
                        left: 0
                    }
                }
            }
        };

        $scope.print = function (elem, title) {
            var mywindow = window.open('', 'PRINT', 'width=' + screen.availWidth + ',height=' + screen.availHeight);

            mywindow.document.write('<html><head><title>' + title + '</title>');
            mywindow.document.write('<link rel="stylesheet" href="/Content/styleAnalitics.css" type="text/css" />');
            mywindow.document.write('<link rel="stylesheet" href="/Content/newCase.css" type="text/css" />');
            mywindow.document.write('</head><body onload="window.print(); window.close()">');
            mywindow.document.write('<h1>' + title + '</h1>');
            mywindow.document.write('<div class="container">');
            mywindow.document.write(document.getElementById(elem).innerHTML);
            mywindow.document.write('</div></body></html>');

            mywindow.document.close(); // necessary for IE >= 10
            mywindow.focus(); // necessary for IE >= 10*/

            return true;
        };
    }
}());

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('CasesController',
            ['$scope', '$filter', '$location', 'orderByFilter', 'CasesService', CasesController]);

    function CasesController($scope, $filter, $location, orderByFilter, CasesService) {
        $scope.showAsGrid = true;
        $scope.mode = 1;
        $scope.reports = [];
        $scope.sortColumn = 'case_number';
        $scope.sortColumnDesc = true;
        var tab = $filter('parseUrl')($location.$$absUrl, 'mode').toLowerCase();
        $scope.mode = tab === 'active' ? 1 : $scope.mode;
        $scope.mode = tab === 'completed' ? 2 : $scope.mode;
        $scope.mode = tab === 'closed' ? 5 : $scope.mode;
        $scope.mode = tab === 'spam' ? 3 : $scope.mode;

        var titles = ['', 'Active Cases', 'Cases Awaiting Sign-off', 'Spam Cases', 'New Reports', 'Closed Cases'];

        $scope.counts = {
            Active: 0,
            Completed: 0,
            Spam: 0,
            Closed: 0,
        };

        $scope.sortClass = function (column) {
            if (column === $scope.sortColumn) {
                return !$scope.sortColumnDesc ? 'sortArrow_down' : 'sortArrow_up';
            }
            return '';
        };

        $scope.severityClass = function (report) {
            if (report.severity_id === 2) {
                return 'dashboard-col__severity-textLow';
            }
            if (report.severity_id === 5) {
                return 'dashboard-col__severity-textCritical';
            }
            if (report.severity_id === 3) {
                return 'dashboard-col__severity-textMedium';
            }
            if (report.severity_id === 4) {
                return 'dashboard-col__severity-textHigh';
            }
            return '';
        };

        $scope.isOwner = function (report, mediator) {
            for (var i = 0; i < report.owners.length; i++) {
                if (report.owners[i].user_id === mediator.id) {
                    return true;
                }
            }
            return false;
        };

        $scope.refresh = function (mode, preload) {
            CasesService.get({ ReportFlag: mode, Preload: preload }, function (data) {
                $('.headerBlockTextRight > span').text(data.Title);
                for (var i = 0; i < data.Reports.length; i++) {
                    var r = $filter('filter')(data.ReportsAdv, { 'id': data.Reports[i].report_id }, true);
                }

                $scope.reports = data.Reports;
                $scope.mode = data.Mode;
                $scope.counts = data.Counts;
                $('title').html(titles[$scope.mode]);
            });
        };
        $scope.refresh($scope.mode);

        $scope.openCase = function (id) {
            if ($scope.mode === 3 || $scope.mode === 4) {
                window.location = '/NewReport/' +id;
            } else {
                window.location = '/newCase/Index/' + id;
            }
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
                            if ($filter('getBy')(poster.posterCategoryNames, 'industry_posters_id', category.id) != null) {
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
        $scope.SelectedSize = 1620;
        $scope.SelectedLogo = 1;
        $scope.mainImage = '';
        $scope.id = parseInt($location.absUrl().substring($location.absUrl().indexOf('poster/') + 'poster/'.length));

        EmployeeAwarenessPosterService.get({ id: $scope.id }, function (data) {
            $scope.mainImage = data.mainImage;
        });

        $scope.Process = function () {
            EmployeeAwarenessPosterService.post({ posterId: $scope.id, type: 1, size: $scope.SelectedSize, logo1: $scope.SelectedLogo }, function (data) {
                //window.location = data.file;
                $scope.downloadLink = data.file;
                document.getElementById('downloadA').setAttribute('href', data.file);
                document.getElementById('downloadA').click();
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
            data.report_cc_crime.cc_is_clear_act_crime = '' + data.report_cc_crime.cc_is_clear_act_crime;

            data.report_case_closure_outcome1 = $filter('filter')(data.report_case_closure_outcome, function (value) {
                if ((value.mediator.role_in_report_id === 1) || (value.mediator.role_in_report_id === 2)) {
                    return true;
                }
            });
            data.report_case_closure_outcome2 = $filter('filter')(data.report_case_closure_outcome, function (value) {
                if ((value.mediator.role_in_report_id === 3)) {
                    return true;
                }
            });

            $scope.model = data;
        };

        NewCaseCaseClosureReportService.get({ report_id: $scope.report_id }, function (data) {
            $scope.refresh(data);
        });

        $scope.saveCrime = function () {
            NewCaseCaseClosureReportService.post({ report_id: $scope.report_id, report_cc_crime: $scope.model.report_cc_crime }, function (data) {
                $scope.editExecutiveSummary = false;
                $scope.refresh(data);
            });
        };

        $scope.saveCrimeCategory = function (item) {
            $scope.model.report_cc_crime.cc_crime_statistics_category_id = item.id;
            $scope.saveCrime();
        };

        $scope.saveCrimeLocation = function (item) {
            $scope.model.report_cc_crime.cc_crime_statistics_location_id = item.id;
            $scope.saveCrime();
        };

        $scope.getCrimeCategory = function (item) {
            if ($scope.model.cc_crime_statistics_categories) {
                var r = $filter('filter')($scope.model.cc_crime_statistics_categories, { 'id': item }, true);
                if (r.length !== 0) {
                    return r[0].crime_statistics_category_en;
                }
            }
            return 'Crime Reporting';
        };

        $scope.getCrimeLocation = function (item) {
            if ($scope.model.cc_crime_statistics_locations) {
                var r = $filter('filter')($scope.model.cc_crime_statistics_locations, { 'id': item }, true);
                if (r.length !== 0) {
                    return r[0].crime_statistics_location_en;
                }
            }
            return 'Geographic Location Reporting';
        };

        $scope.saveOutcome = function (item, outcome) {
            if (outcome !== undefined) {
                item.outcome.outcome_id = outcome.id;
            }
            item.editNote = false;
            NewCaseCaseClosureReportService.post({ report_id: $scope.report_id, report_case_closure_outcome: item.outcome }, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.strBR = function (str) {
            str = str || '';
            return str.split('\n').join('<br/>');
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

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('NewCaseMessagesController',
            ['$scope', '$filter', 'orderByFilter', '$location', 'NewCaseMessagesService', NewCaseMessagesController]);

    function NewCaseMessagesController($scope, $filter, orderByFilter, $location, NewCaseMessagesService) {
        $scope.report_id = $filter('parseUrl')($location.$$absUrl, 'report_id');

        $scope.activeTab = $location.$$absUrl.toLowerCase().indexOf('/reporter') === -1 ? 1 : 2;
        $scope.activeMessage = {};

        $scope.mediators = [];
        $scope.mediatorsUnreaded = 0;
        $scope.reporters = [];
        $scope.reportersUnreaded = 0;
        $scope.currentUser = {};

        $scope.newMessage = '';
        $scope.newMessageReporter = '';

        $scope.refresh = function () {
            NewCaseMessagesService.get({ id: $scope.report_id }, function (data) {
                $scope.mediators = data.mediators;
                $scope.reporters = data.reporters;
                $scope.currentUser = data.currentUser;
                $scope.mediatorsUnreaded = $scope.unreaded(data.mediators);
                $scope.reportersUnreaded = $scope.unreaded(data.reporters);
            });
        };

        $scope.sendMessage = function () {
            NewCaseMessagesService.post({ mode: 1, report_id: $scope.report_id, newMessage: $scope.newMessage }, function () {
                $scope.newMessage = '';
                $scope.refresh();
            });
        };

        $scope.sendMessageReporter = function () {
            NewCaseMessagesService.post({ mode: 2, report_id: $scope.report_id, newMessage: $scope.newMessageReporter }, function () {
                $scope.newMessageReporter = '';
                $scope.refresh();
            });
        };

        $scope.unreaded = function (list) {
            var count = 0;
            for (var i = 0; i < list.length; i++) {
                count += list[i].isReaded ? 0 : 1;
            }
            return count;
        };

        $scope.refresh();
    }
}());

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('NewCaseReportController',
            ['$scope', '$filter', 'orderByFilter', '$location', 'NewCaseReportService', NewCaseReportController]);

    function NewCaseReportController($scope, $filter, orderByFilter, $location, NewCaseReportService) {
        //$scope.report_id = parseInt(parseInt($location.absUrl().substring($location.absUrl().indexOf('report_id=') + 'report_id='.length)));
        $scope.report_id = $filter('parseUrl')($location.$$absUrl, 'report_id');

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
        .controller('NewCaseTeamController',
            ['$scope', '$filter', 'orderByFilter', '$location', 'NewCaseTeamService', NewCaseTeamController]);

    function NewCaseTeamController($scope, $filter, orderByFilter, $location, NewCaseTeamService) {
        $scope.report_id = $filter('parseUrl')($location.$$absUrl, 'report_id');

        $scope.roleNames = ['', '', '', '', 'Case Reviewer', 'Platform Manager', 'Case Investigator', 'Legal counsel'];
        $scope.roleClasses = ['', '', '', '', 'teamBlock-body__profileSpec_reviewer', 'teamBlock-body__profileSpec_owner', '', ''];

        $scope.makeCircles = function () {
            var canvasTeam = $('.teamBlock').find('.canvasCircle');
            for (var i = 0; i < canvasTeam.length; i++) {
                var x = 1.25;
                if (x > 0.5) {
                    x = x - 0.5;
                }
                var context = canvasTeam[i].getContext('2d');

                context.beginPath();
                context.lineWidth = 1;
                context.strokeStyle = '#0ab6a4';
                context.arc(30, 30, 29, 1.5 * Math.PI, x * Math.PI);
                context.stroke();

                context.beginPath();
                context.lineWidth = 1;
                context.strokeStyle = '#ba83b8';
                context.arc(30, 30, 29, (x + 0.03) * Math.PI, 1.47 * Math.PI);
                context.stroke();
            }
        };

        $scope.refresh = function (data) {
            $scope.addMoreMediators = false;
            $scope.involved_mediators_user_list = data.involved_mediators_user_list;
            //$scope.mediators_whoHasAccess_toReport = data.mediators_whoHasAccess_toReport;
            $scope.available_toAssign_mediators = data.available_toAssign_mediators;
            $scope.currentInfo = data.currentInfo;
            $scope.mediators_whoHasAccess_toReportG = [];
            for (var i = 0; i < data.mediators_whoHasAccess_toReport.length; i++) {
                if (i % 3 === 0) {
                    $scope.mediators_whoHasAccess_toReportG.push([]);
                }
                $scope.mediators_whoHasAccess_toReportG[$scope.mediators_whoHasAccess_toReportG.length - 1].push(data.mediators_whoHasAccess_toReport[i]);
            }
        };

        NewCaseTeamService.get({ id: $scope.report_id }, function (data) {
            $scope.refresh(data);
        });

        $scope.closeBtn = function (item) {
            NewCaseTeamService.post({ removeFromTeam: item.user.id, report_id: $scope.report_id }, function (data) {
                $scope.refresh(data);
                if (!data.success) {
                    $scope.messageModal = {
                        title: 'Error',
                        text: data.message,
                    };
                    $('#messageModal').modal();
                }
            });
        };

        $scope.addToTeam = function (item) {
            NewCaseTeamService.post({ addToTeam: item.user.id, report_id: $scope.report_id }, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.makeCaseOwner = function (item) {
            NewCaseTeamService.post({ makeCaseOwner: item.user.id, report_id: $scope.report_id }, function (data) {
                $scope.refresh(data);
            });
        };
    }
}());

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('NewCaseTopMenuController',
            ['$scope', '$filter', '$location', 'NewCaseTopMenuService', NewCaseTopMenuController]);

    function NewCaseTopMenuController($scope, $filter, $location, NewCaseTopMenuService) {
        $scope.report_id = $filter('parseUrl')($location.$$absUrl, 'report_id');

        $scope.refresh = function () {
            NewCaseTopMenuService.get({ reportId: $scope.report_id }, function (data) {
                $scope.state = data;
            });
        };

        $scope.init = function (id) {
            $scope.report_id = id || $scope.report_id;
            $scope.refresh();
        };

        $scope.setIsLifeThreating = function (isLifeThreating) {
            if (isLifeThreating) {
                if (confirm('Does this Case or Subject pose an ongoing threat to the safety or health of the students and employees on campus? A Campus Alert will be requested if you select "OK"')) {
                    NewCaseTopMenuService.setLifeThreating({ reportId: $scope.report_id, isLifeThreating: isLifeThreating }, function (data) {
                        $scope.refresh();
                    });
                }
            }
        };
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
        .controller('SettingsDisclaimerController',
            ['$scope', '$filter', '$location', 'Upload', 'SettingsDisclaimerService', SettingsDisclaimerController]);

    function SettingsDisclaimerController($scope, $filter, $location, Upload, SettingsDisclaimerService) {

        $scope.refresh = function () {
            SettingsDisclaimerService.get({}, function (data) {
                data.message_to_employeesChanged = false;
                data.message_about_guidelinesChanged = false;
                for (var i = 0; i < data.company_disclamer_uploads_dt.length; i++) {
                    data.company_disclamer_uploads[i].create_dt_s = data.company_disclamer_uploads_dt[i];
                    data.company_disclamer_uploads[i].display_name2 = data.company_disclamer_uploads[i].display_name;
                }
                if ((!data.company_disclamer_page) || (!data.company_disclamer_page.message_to_employees) || (!data.company_disclamer_page.message_about_guidelines)) {
                    $('#DisclaimersExclamationmarkImg').show();
                } else {
                    $('#DisclaimersExclamationmarkImg').hide();
                }
                $scope.model = data;
            });
        };

        $scope.save = function (mode) {
            if (mode === 1) {
                SettingsDisclaimerService.save1({ message: $scope.model.company_disclamer_page.message_to_employees }, function () {
                    $scope.refresh();
                });
            }
            if (mode === 2) {
                SettingsDisclaimerService.save2({ message: $scope.model.company_disclamer_page.message_about_guidelines }, function () {
                    $scope.refresh();
                });
            }
        };

        $scope.upload = function (files) {
            Upload.upload({
                url: '/api/SettingsDisclaimer/Upload',
                data: {
                    File: files,
                    Description: ''
                }
            }).then(function () {
                $scope.refresh();
            }, function () {
            }, function () {
            });
        };

        $scope.deleteFile = function (file) {
            SettingsDisclaimerService.deleteFile(file, function () {
                $scope.refresh();
            });
        };

        $scope.saveFile = function (file) {
            SettingsDisclaimerService.saveFile(file, function () {
                $scope.refresh();
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
        $scope.locations = [];
        $scope.role = 5;
        $scope.departmentId = 0;
        $scope.locationId = 0;
        $scope.user_permissions_approve_case_closure = 0;
        $scope.user_permissions_change_settings = 0;
        $scope.status_id = 3;
        $scope.user_role = 0;
        $scope.photo_path = '/Content/Icons/settingsPersonalNOPhoto.png';

        $scope.val_first_nm = false;
        $scope.val_last_nm = false;
        $scope.val_title_ds = false;
        $scope.val_email = false;
        $scope.val_departmentId = false;
        $scope.val_locationId = false;

        $scope.id = parseInt($location.absUrl().substring($location.absUrl().indexOf('user/') + 'user/'.length));
        $scope.id = isNaN($scope.id) ? 0 : $scope.id;

        SettingsUserEditService.get({ id: $scope.id }, function (data) {
            $scope.first_nm = data.model.first_nm;
            $scope.last_nm = data.model.last_nm;
            $scope.title_ds = data.model.title_ds;
            $scope.email = data.model.email;
            $scope.departments = data.departments;
            $scope.locations = data.locations;
            $scope.role = data.model.role;
            $scope.departmentId = data.model.departmentId;
            $scope.status_id = data.model.status_id;
            $scope.locationId = data.model.locationId;
            $scope.user_permissions_approve_case_closure = data.model.user_permissions_approve_case_closure.toString();
            $scope.user_permissions_change_settings = data.model.user_permissions_change_settings.toString();
            $scope.user_role = data.user.role;
            $scope.photo_path = data.model.photo_path;
            $scope.canEditUserProfiles = data.user.CanEditUserProfiles;
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
            $scope.val_locationId = $scope.locationId == null || !$scope.validate($scope.locationId.toString());

            if (!$scope.val_first_nm
                && !$scope.val_last_nm
                && !$scope.val_title_ds
                && !$scope.val_email
                && !$scope.val_departmentId
                && !$scope.val_locationId
                ) {

                var model = {
                    id: $scope.id,
                    first_nm: $scope.first_nm,
                    last_nm: $scope.last_nm,
                    title_ds: $scope.title_ds,
                    email: $scope.email,
                    role_id: $scope.role,
                    company_department_id: $scope.departmentId,
                    company_location_id: $scope.locationId,
                    user_permissions_approve_case_closure: $scope.user_permissions_approve_case_closure,
                    user_permissions_change_settings: $scope.user_permissions_change_settings,
                    status_id: $scope.status_id,
                };
                SettingsUserEditService.post(model, function (data) {
                    if (data.ok) {
                        if ($scope.id !== 0) {
                            window.location = window.location;
                        } else {
                            window.location = '/Settings/Mediators';
                        }
                    } else {
                        alert(data.message);
                    }
                });
            }
        };
    }
}());

(function () {

    'use strict';

    angular.module('EC')
        .service('AnalyticsCACSService', ['$resource', AnalyticsCACSService]);

    function AnalyticsCACSService($resource) {
        return $resource('/api/AnalyticsCACS', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();

(function () {

    'use strict';

    angular.module('EC')
        .service('AnalyticsRootCauseAnalysisService', ['$resource', AnalyticsRootCauseAnalysisService]);

    function AnalyticsRootCauseAnalysisService($resource) {
        return $resource('/api/AnalyticsRootCauseAnalysis', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();

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
        .service('NewCaseMessagesService', ['$resource', NewCaseMessagesService]);

    function NewCaseMessagesService($resource) {
        return $resource('/api/NewCaseMessages', {}, {
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
        .service('NewCaseTeamService', ['$resource', NewCaseTeamService]);

    function NewCaseTeamService($resource) {
        return $resource('/api/NewCaseTeam', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
        });
    };
})();

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
        .service('SettingsDisclaimerService', ['$resource', SettingsDisclaimerService]);

    function SettingsDisclaimerService($resource) {
        return $resource('/api/SettingsDisclaimer', {}, {
            get: { url: '/api/SettingsDisclaimer/Get', method: 'GET', params: {}, isArray: false },
            save1: { url: '/api/SettingsDisclaimer/Save1', method: 'POST', params: {}, isArray: false },
            save2: { url: '/api/SettingsDisclaimer/Save2', method: 'POST', params: {}, isArray: false },
            deleteFile: { url: '/api/SettingsDisclaimer/DeleteFile', method: 'POST', params: {}, isArray: false },
            saveFile: { url: '/api/SettingsDisclaimer/SaveFile', method: 'POST', params: {}, isArray: false },
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
