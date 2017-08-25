(function () {

    'use strict';

    angular
        .module('EC')
        .controller('SettingsCompanyRoutingController',
            ['$scope', '$filter', 'orderByFilter', '$http', 'SettingsCompanyRoutingService', SettingsCompanyRoutingController]);

    function SettingsCompanyRoutingController($scope, $filter, orderByFilter, $http, SettingsCompanyRoutingService) {
        $scope.types = [];
        $scope.departments = [];
        $scope.users = [];
        $scope.scopes = [];
        $scope.items = [];
        $scope.files = [];
        $scope.uploadLine = 0;
        $scope.locations = [];
        $scope.locationItems = [];

        $scope.refresh = function (data) {
            $scope.types = data.types;
            $scope.departments = data.departments;
            $scope.users = data.users;
            $scope.scopes = data.scopes;
            $scope.items = data.items;
            $scope.files = data.files;
            $scope.departments.splice(0, 0, { id: 0, name_en: 'Select' });
            $scope.users.splice(0, 0, { id: 0, first_nm: 'Select', last_nm: '' });
            $scope.scopes.splice(0, 0, { id: 0, scope_en: 'Select' });
            $scope.locations = data.locations;
            $scope.locationItems = data.locationItems;
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

        $scope.locationName = function (id) {
            return $filter('filter')($scope.locations, { 'id': id })[0];
        };

        $scope.changeLine = function (line) {
            SettingsCompanyRoutingService.post({ model: line }, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.changeLineLocation = function (line) {
            SettingsCompanyRoutingService.post({ modelLocation: line }, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.upload = function (elem) {
            console.log(1);
            var data = new FormData();
            data.append('model', JSON.stringify($scope.uploadLine));
            data.append('file', elem.files[0]);

            $http({
                url: '/api/SettingsCompanyRouting',
                method: 'POST',
                data: data,
                headers: { 'Content-Type': undefined }, //this is important
                transformRequest: angular.identity //also important
            }).then(function (data) {
                $scope.refresh(data.data);
            }).catch(function(response){
            });
        };

        $scope.fileSelect = function (line) {
            console.log(0);
            $scope.uploadLine = line;
        };
    }
}());
