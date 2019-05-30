﻿'use sctrict'
var purchaseApp = angular.module("EC", []);
purchaseApp.controller("onboarding", function ($scope) {
    $scope.priceT = 0;

    $scope.checkboxItem = function (clickedItem) {
        switch (clickedItem) {
            case 'one':
                $scope.priceT = 500;
                $scope.validationButtons = false;
                break;
            case 'two':
                $scope.priceT = 1000;
                $scope.validationButtons = false;
                break;
            case 'three':
                $scope.priceT = 3000;
                $scope.validationButtons = false;
                break;
        }
    }
    $scope.submitForm = function () {
        var result = false;
        if ($scope.priceT == 0) {
            $scope.validationButtons = true;
            result = true;
        }
        if ($scope.invitationCode == undefined) {
            $scope.validationInput = true;
            result = true;
        }
        if (result == false) {
            angular.element('#formDrop').submit();
        }
    }
}); 