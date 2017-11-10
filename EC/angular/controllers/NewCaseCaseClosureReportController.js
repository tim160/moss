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
