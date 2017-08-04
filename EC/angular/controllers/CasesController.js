(function () {

    'use strict';

    angular
        .module('EC')
        .controller('CasesController',
            ['$scope', '$filter', 'CasesService', CasesController]);

    function CasesController($scope, $filter, CasesService) {
        $scope.mode = 1;
        $scope.reports = [];
        $scope.counts = {
            Active: 0,
            Completed: 0,
            Spam: 0,
            Closed: 0,
        };

        $scope.Refresh = function (mode) {
            CasesService.get({ ReportFlag: mode }, function (data) {
                $scope.reports = data.Reports;
                $scope.mode = data.Mode;
                $scope.counts = data.Counts;
            });
        };
        $scope.Refresh($scope.mode);
    }
}());
