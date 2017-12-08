(function () {

    'use strict';

    angular
        .module('EC')
        .controller('NewCaseTeamController',
            ['$scope', '$filter', 'orderByFilter', '$location', 'NewCaseTeamService', NewCaseTeamController]);

    function NewCaseTeamController($scope, $filter, orderByFilter, $location, NewCaseTeamService) {
        $scope.report_id = $filter('parseUrl')($location.$$absUrl, 'report_id');

        $scope.roleNames = ['', '', '', '', 'Case Reviewer', 'Platform Manager', 'Case Investigator', 'Legal counsel'];
        $scope.roleClasses = ['', '', '', '', 'escalation', 'admin', '', ''];

        $scope.refresh = function () {
            NewCaseTeamService.get({ id: $scope.report_id }, function (data) {
                $scope.involved_mediators_user_list = data.involved_mediators_user_list;
                $scope.mediators_whoHasAccess_toReport = data.mediators_whoHasAccess_toReport;
                $scope.available_toAssign_mediators = data.available_toAssign_mediators;
                $scope.currentInfo = data.currentInfo;
            });
        };

        $scope.refresh();

        $scope.sendMessage = function () {
        };
    }
}());
