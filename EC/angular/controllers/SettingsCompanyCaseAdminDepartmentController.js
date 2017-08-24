(function () {

    'use strict';

    angular
        .module('EC')
        .controller('SettingsCompanyCaseAdminDepartmentController',
            ['$scope', '$filter', 'orderByFilter', 'SettingsCompanyCaseAdminDepartmentService', SettingsCompanyCaseAdminDepartmentController]);

    function SettingsCompanyCaseAdminDepartmentController($scope, $filter, orderByFilter, SettingsCompanyCaseAdminDepartmentService) {
        $scope.case_admin_departments = [];
        $scope.addMode = false;
        $scope.addName = '';

        $scope.refresh = function (data) {
            $scope.case_admin_departments = data.case_admin_departments;
        };

        SettingsCompanyCaseAdminDepartmentService.get({}, function (data) {
            $scope.refresh(data);
        });

        $scope.add = function () {
            $scope.addMode = false;
            SettingsCompanyCaseAdminDepartmentService.post({ addName: $scope.addName, addType: $scope.addType }, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.delete = function (id) {
            $scope.addMode = false;
            SettingsCompanyCaseAdminDepartmentService.delete({ DeleteId: id }, function (data) {
                $scope.refresh(data);
            });
        };
    }
}());
