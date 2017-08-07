(function () {

    'use strict';

    angular
        .module('EC')
        .controller('CasesController',
            ['$scope', '$filter', 'CasesService', CasesController]);

    function CasesController($scope, $filter, CasesService) {
        $scope.showAsGrid = true;
        //$scope.showAsGrid = false;
        $scope.mode = 1;
        $scope.reports = [];
        $scope.reportsAdv = [];
        $scope.users = [];

        $scope.counts = {
            Active: 0,
            Completed: 0,
            Spam: 0,
            Closed: 0,
        };

        $scope.refresh = function (mode) {
            CasesService.get({ ReportFlag: mode }, function (data) {
                //WARNING DEBUG
                data.Reports[0].investigation_status_number = 3;
                data.Reports[1].investigation_status_number = 9;
                data.Reports[2].investigation_status_number = 4;
                data.Reports[3].investigation_status_number = 5;
                data.Reports[4].investigation_status_number = 6;
                data.Reports[5].investigation_status_number = 7;
                //WARNING DEBUG

                $scope.reports = data.Reports;
                $scope.reportsAdv = data.ReportsAdv;
                $scope.users = data.Users;
                $scope.mode = data.Mode;
                $scope.counts = data.Counts;
            });
        };
        $scope.refresh($scope.mode);

        $scope.advData = function (id, prop) {
            var r = $filter('filter')($scope.reportsAdv, { 'id': id });
            if ((r != null) && (r.length > 0)) {
                return r[0][prop];
            }

            return '';
        };

        $scope.userData = function (id, prop) {
            var r = $filter('filter')($scope.users, { 'id': id });
            if ((r != null) && (r.length > 0)) {
                return r[0][prop];
            }

            return '';
        };
    }
}());
