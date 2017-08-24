(function () {

    'use strict';

    angular
        .module('EC')
        .controller('SettingsCompanyRoutingController',
            ['$scope', '$filter', 'orderByFilter', 'SettingsCompanyRoutingService', SettingsCompanyRoutingController]);

    function SettingsCompanyRoutingController($scope, $filter, orderByFilter, SettingsCompanyRoutingService) {
        $scope.types = [];
        $scope.departments = [];
        $scope.users = [];
        $scope.scopes = [];
        $scope.items = [];

        $scope.refresh = function (data) {
            $scope.types = data.types;
            $scope.departments = data.departments;
            $scope.users = data.users;
            $scope.scopes = data.scopes;
            $scope.items = data.items;
            $scope.departments.splice(0, 0, { id: 0, name_en: 'Select' });
            $scope.users.splice(0, 0, { id: 0, first_nm: 'Select', last_nm: '' });
            $scope.scopes.splice(0, 0, { id: 0, scope_en: 'Select' });
        };

        SettingsCompanyRoutingService.get({}, function (data) {
            $scope.refresh(data);
        });

        $scope.delete = function (id) {
            SettingsCompanyRoutingService.delete({ DeleteId: id }, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.userName = function (item) {
            return item.first_nm + ' ' + item.last_nm;
        };

        $scope.typeName = function (id) {
            return $filter('filter')($scope.types, { 'id': id })[0];
        };

        $scope.changeLine = function (line) {
            SettingsCompanyRoutingService.post({ model: line }, function (data) {
                $scope.refresh(data);
            });
        };
    }
}());
