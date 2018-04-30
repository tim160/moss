(function () {

    'use strict';

    angular
        .module('EC')
        .controller('SettingsDisclaimerController',
            ['$scope', '$filter', '$location', 'Upload', 'SettingsDisclaimerService', SettingsDisclaimerController]);

    function SettingsDisclaimerController($scope, $filter, $location, Upload, SettingsDisclaimerService) {

        $scope.refresh = function () {
            SettingsDisclaimerService.get({}, function (data) {
                data.message_to_employeesChanged = false;
                data.message_about_guidelinesChanged = false;
                for (var i = 0; i < data.company_disclamer_uploads_dt.length; i++) {
                    data.company_disclamer_uploads[i].create_dt_s = data.company_disclamer_uploads_dt[i];
                }
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

        $scope.upload = function (files) {
            Upload.upload({
                url: '/api/SettingsDisclaimer/Upload',
                data: {
                    File: files,
                    Description: ''
                }
            }).then(function () {
                $scope.refresh();
            }, function () {
            }, function () {
            });
        };

        $scope.deleteFile = function (file) {
            SettingsDisclaimerService.deleteFile(file, function () {
                $scope.refresh();
            });
        };

        $scope.saveFile = function (file) {
            SettingsDisclaimerService.saveFile(file, function () {
                $scope.refresh();
            });
        };
    }
}());
