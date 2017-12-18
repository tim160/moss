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
            $scope.involved_mediators_user_list = data.involved_mediators_user_list;
            $scope.mediators_whoHasAccess_toReport = data.mediators_whoHasAccess_toReport;
            $scope.available_toAssign_mediators = data.available_toAssign_mediators;
            $scope.currentInfo = data.currentInfo;
        };

        NewCaseTeamService.get({ id: $scope.report_id }, function (data) {
            $scope.refresh(data);
        });

        $scope.closeBtn = function (item) {
            NewCaseTeamService.post({ removeFromTeam: item.user.id, report_id: $scope.report_id }, function (data) {
                $scope.refresh(data);
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
