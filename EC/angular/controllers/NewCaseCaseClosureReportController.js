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

            data.report_case_closure_outcome1 = $filter('filter')(data.report_case_closure_outcome, function (value, index, array) {
                if (value.mediator.role_in_report_id === 3) {
                    return true;
                }
            });
            data.report_case_closure_outcome2 = $filter('filter')(data.report_case_closure_outcome, function (value, index, array) {
                if (value.mediator.role_in_report_id === 1) {
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
    }
}());
