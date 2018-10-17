(function () {

    'use strict';

    angular
        .module('EC')
        .controller('EmployeeAwarenessPosterController',
            ['$scope', '$filter', '$location', 'EmployeeAwarenessPosterService', EmployeeAwarenessPosterController]);

    function EmployeeAwarenessPosterController($scope, $filter, $location, EmployeeAwarenessPosterService) {
        $scope.SelectedSize = 1620;
        $scope.SelectedLogo = 1;
        $scope.mainImage = '';
        $scope.id = parseInt($location.absUrl().substring($location.absUrl().indexOf('poster/') + 'poster/'.length));

        EmployeeAwarenessPosterService.get({ id: $scope.id }, function (data) {
            $scope.mainImage = data.mainImage;
        });

        $scope.Process = function () {
            var p = { posterId: $scope.id, type: 1, size: $scope.SelectedSize, logo1: $scope.SelectedLogo };
            EmployeeAwarenessPosterService.post(p, function (data) {
                //window.location = data.file;
                $scope.downloadLink = data.file;
                document.getElementById('downloadA').setAttribute('href', data.file);
                document.getElementById('downloadA').click();
            });

            return true;
        };
    }
}());
