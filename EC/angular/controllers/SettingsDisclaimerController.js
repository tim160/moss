﻿(function () {

    'use strict';

    angular
        .module('EC')
        .controller('SettingsDisclaimerController',
            ['$scope', '$filter', '$location', 'SettingsDisclaimerService', SettingsDisclaimerController]);

    function SettingsDisclaimerController($scope, $filter, $location, SettingsDisclaimerService) {

        $scope.refresh = function () {
            SettingsDisclaimerService.get({}, function (data) {
                data.message_to_employeesChanged = false;
                data.message_about_guidelinesChanged = false;
                $scope.model = data;
            });
        };

        $scope.save = function (mode) {
            if (mode === 1) {
                SettingsDisclaimerService.save1({ message: $scope.model.company_disclamer_page.message_to_employees }, function () {
                    $scope.refresh();
                });
            }
            if (mode === 2) {
                SettingsDisclaimerService.save2({ message: $scope.model.company_disclamer_page.message_about_guidelines }, function () {
                    $scope.refresh();
                });
            }
        };
    }
}());
