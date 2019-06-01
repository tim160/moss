'use sctrict'
var purchaseApp = angular.module("EC", []);
purchaseApp.controller("onboarding", function ($scope, productService) {
    $scope.priceT = 3000;
    $scope.switchTrainingSession = 'Training session X3';
    $scope.priceSwitchTrainingSession = "$3,000";
    $scope.checkboxItem = function (clickedItem) {
        switch (clickedItem) {
            case 'one':
                $scope.priceT = 500;
                $scope.validationButtons = false;
                $scope.switchTrainingSession = "Training session X1";
                $scope.priceSwitchTrainingSession = "$500";
                break;
            case 'two':
                $scope.priceT = 1000;
                $scope.validationButtons = false;
                $scope.switchTrainingSession = "Training session X2";
                $scope.priceSwitchTrainingSession = "$1,000";
                break;
            case 'three':
                $scope.priceT = 3000;
                $scope.validationButtons = false;
                $scope.switchTrainingSession = "Training session X3";
                $scope.priceSwitchTrainingSession = "$3,000";
                break;
        }
        productService.addSelectedItem({
            priceT : $scope.priceT,
            priceSwitchTrainingSession : $scope.priceSwitchTrainingSession
        });
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


purchaseApp.controller("onboardingPayment", function ($scope, productService) {
    $scope.priceT = productService.getSelectedItem().priceT;
    $scope.switchTrainingSession = productService.getSelectedItem().switchTrainingSession;

    $scope.SubmitOnboardPayment = function () {
        var result = false;
        if ($scope.NameOnCard == undefined) {
            $scope.validationNameOnCard = true;
            result = true;
        }
        if ($scope.CardNumber == undefined) {
            $scope.validationCardNumber = true;
            result = true;
        }
        if ($scope.CardExpire == undefined) {
            $scope.validationCardExpire = true;
            result = true;
        }
        if ($scope.cvc == undefined) {
            $scope.validationcvc = true;
            result = true;
        }
        if (!result) {
            angular.element('#formDrop').submit();
        }
    }
});


purchaseApp.factory('productService', function () {
    var selectedItem = {
        priceT : 3000,
        priceSwitchTrainingSession: "$3,000",
        sessions: ''
    };
    var getSelectedItem = function () {
        return selectedItem;
    };
    var addSelectedItem = function (newObj) {
        selectedItem.priceT = newObj.priceT;
        selectedItem.switchTrainingSession = newObj.switchTrainingSession;
        selectedItem.priceSwitchTrainingSession = newObj.priceSwitchTrainingSession;
        switch (newObj.priceT) {
            case 500:
                selectedItem.sessions = 'Up to one session';
                break;
            case 1000:
                selectedItem.sessions = 'Up to two sessions';
                break;
            case 3000:
                selectedItem.sessions = 'Up to three sessions';
                break;
        }
    };
    return {
        addSelectedItem: addSelectedItem,
        getSelectedItem: getSelectedItem
    };
});