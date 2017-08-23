(function () {

    'use strict';

    angular
        .module('EC')
        .controller('SettingsCompanySecondaryTypeController',
            ['$scope', '$filter', 'orderByFilter', 'SettingsCompanySecondaryTypeService', SettingsCompanySecondaryTypeController]);

    function SettingsCompanySecondaryTypeController($scope, $filter, orderByFilter, SettingsCompanySecondaryTypeService) {
        $scope.secondaryTypes = [];
        $scope.third_level_types = [];
        $scope.addMode = false;
        $scope.addName = '';
        $scope.addType = 0;

        $scope.refresh = function (data) {
            $scope.secondaryTypes = data.secondaryTypes;
            $scope.addType = data.secondaryTypes[0].id;
            $scope.third_level_types = data.third_level_types;
        };

        SettingsCompanySecondaryTypeService.get({}, function (data) {
            $scope.refresh(data);
        });

        $scope.add = function () {
            $scope.addMode = false;
            SettingsCompanySecondaryTypeService.post({ addName: $scope.addName, addType: $scope.addType }, function (data) {
                $scope.refresh(data);
            });
        };

        $scope.findST = function (id) {
            for (var i = 0; i < $scope.secondaryTypes.length; i++) {
                console.log(id, $scope.secondaryTypes[i].id, id);
                if ($scope.secondaryTypes[i].id === id) {
                    return $scope.secondaryTypes[i].secondary_type_en;
                }
            }
            return '';
        };

        $scope.delete = function (id) {
            $scope.addMode = false;
            SettingsCompanySecondaryTypeService.delete({ DeleteId: id }, function (data) {
                $scope.refresh(data);
            });
        };
    }
}());
