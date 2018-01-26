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

        $scope.refresh();

        $scope.setIsLifeThreating = function (isLifeThreating) {
            if (isLifeThreating) {
                if (!confirm('Mark case as a threat to the health or safety of students or employees on the campus?')) {
                    return;
                }
            } else {
                if (!confirm('Mark case as a non-threat to the health or safety of students or employees on the campus?')) {
                    return;
                }
            }
            NewCaseTopMenuService.setLifeThreating({ reportId: $scope.report_id, isLifeThreating: isLifeThreating }, function (data) {
                $scope.refresh();
            });
        };
    }
}());
