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
