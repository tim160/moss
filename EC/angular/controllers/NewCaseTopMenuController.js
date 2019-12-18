(function () {

    'use strict';

    angular
        .module('EC')
        .controller('NewCaseTopMenuController',
            ['$scope', '$filter', '$location', 'NewCaseTopMenuService', NewCaseTopMenuController]);

    function NewCaseTopMenuController($scope, $filter, $location, NewCaseTopMenuService) {
        $scope.report_id = $filter('parseUrl')($location.$$absUrl, 'report_id');

        $scope.refresh = function () {
            NewCaseTopMenuService.get({ id: $scope.report_id }, function (data) {
                $scope.state = data;
            });
        };

        $scope.init = function (id) {
            $scope.report_id = id || $scope.report_id;
            $scope.refresh();
        };

        $scope.setIsLifeThreating = function (isLifeThreating) {
            if (isLifeThreating) {
                /*eslint max-len: [2, 200, 4]*/
                if (confirm('Does this Case or Alleged Offender pose an ongoing threat to the safety or health of the students and employees on campus?' +
                    ' A Campus Alert will be requested if you select "OK"')) {
                    NewCaseTopMenuService.setLifeThreating({ id: $scope.report_id, isLifeThreating: isLifeThreating }, function () {
                        $scope.refresh();
                    });
                }
            }
        };
    }
}());
