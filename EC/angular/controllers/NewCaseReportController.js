(function () {

    'use strict';

    angular
        .module('EC')
        .controller('NewCaseReportController',
            ['$scope', '$filter', 'orderByFilter', '$location', 'NewCaseReportService', NewCaseReportController]);

    function NewCaseReportController($scope, $filter, orderByFilter, $location, NewCaseReportService) {
        $scope.report_id = $filter('parseUrl')($location.$$absUrl, 'id');

        $scope.model = {
            reportingFrom: '',
            reporterWouldLike: '',
            reporterName: '',
            reporterIs: '',
            incidentHappenedIn: '',
            affectedDepartment: '',
            partiesInvolvedName: '',
            partiesInvolvedTitle: '',
            partiesInvolvedType: '',
            reportingAbout: '',
            incidentDate: '',
        };

        NewCaseReportService.get({ id: $scope.report_id }, function (data) {
            $scope.model = data;
        });
    }
}());
