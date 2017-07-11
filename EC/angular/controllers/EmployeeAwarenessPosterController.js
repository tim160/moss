(function () {

    'use strict';

    angular
        .module('EC')
        .controller('EmployeeAwarenessPosterController',
            ['$scope', '$filter', '$location', 'EmployeeAwarenessPosterService', EmployeeAwarenessPosterController]);

    function EmployeeAwarenessPosterController($scope, $filter, $location, EmployeeAwarenessPosterService) {
        $scope.SelectedSize = 1;
        $scope.SelectedLogo = 1;
        $scope.mainImage = '';
        $scope.id = $location.absUrl().substring($location.absUrl().indexOf('poster/') + 'poster/'.length);

        EmployeeAwarenessPosterService.get({ id: $scope.id }, function (data) {
            $scope.mainImage = data.mainImage;
        });

        $scope.Process = function () {
            return true;
        };
    }
}());
