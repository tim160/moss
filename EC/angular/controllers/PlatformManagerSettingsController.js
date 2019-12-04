(function () {

    'use strict';

    angular
        .module('EC')
        .controller('PlatformManagerSettingsController',
            ['$scope', 'validateSettingsUser', PlatformManagerSettingsController]);
    function PlatformManagerSettingsController($scope, validateSettingsUser) {

        $scope.val_first_nm = false;
        $scope.val_last_nm = false;
        $scope.val_email = false;

        $scope.update = function (event) {

            $scope.val_first_nm = !validateSettingsUser.validate(angular.element(document.querySelector('#first_nm')).val());
            $scope.val_last_nm = !validateSettingsUser.validate(angular.element(document.querySelector('#last_nm')).val());
            $scope.val_email = !validateSettingsUser.validate(angular.element(document.querySelector('#email')).val(), /^([a-zA-Z0-9_-]+\.)*[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)*\.[a-zA-Z]{2,6}$/);
            if ($scope.val_first_nm || $scope.val_last_nm || $scope.val_email) {
            } else {
                angular.element('#submitPlatformManager').click();
            }
        };
    }
}());
