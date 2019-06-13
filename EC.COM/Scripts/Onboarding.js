'use sctrict'
var purchaseApp = angular.module("EC", []);
purchaseApp.controller("onboarding", function ($scope, productService) {
    $scope.priceT = 500;
    $scope.switchTrainingSession = 'Training session X 1';
    $scope.priceSwitchTrainingSession = "$500";
    $scope.checkboxItem = function (clickedItem) {
        switch (clickedItem) {
            case 'one':
                $scope.priceT = 500;
                $scope.validationButtons = false;
                $scope.switchTrainingSession = "Training session X 1";
                $scope.priceSwitchTrainingSession = "$500";
                break;
            case 'two':
                $scope.priceT = 1000;
                $scope.validationButtons = false;
                $scope.switchTrainingSession = "Training session X 2";
                $scope.priceSwitchTrainingSession = "$1,000";
                break;
            case 'three':
                $scope.priceT = 1500;
                $scope.validationButtons = false;
                $scope.switchTrainingSession = "Training session X 3";
                $scope.priceSwitchTrainingSession = "$1,500";
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
        if (result == false) {
            angular.element('#formDrop').submit();
        }
    }
}); 


purchaseApp.controller("onboardingPayment", function ($scope, productService) {
    var priceT = angular.element(document.querySelector('#priceT')).val();
    productService.addSelectedItem({ priceT: priceT})
    $scope.NumberSessions = productService.getSelectedItem().sessions;

    $scope.SubmitOnboardPayment = function () {
        var result = false;
 
        //if ($scope.CardNumber == undefined) {
        //    $scope.validationCardNumber = true;
        //    result = true;
        //}
        //if ($scope.CardExpire == undefined) {
        //    $scope.validationCardExpire = true;
        //    result = true;
        //}
        //if ($scope.cvc == undefined) {
        //    $scope.validationcvc = true;
        //    result = true;
        //}
        if (!result) {
            angular.element('#formDrop').submit();
        }
    }
});


purchaseApp.factory('productService', function () {
    var selectedItem = {
        priceT : 1500,
        priceSwitchTrainingSession: "$1,500",
        sessions: ''
    };
    var getSelectedItem = function () {
        return selectedItem;
    };
    var addSelectedItem = function (newObj) {
        selectedItem.priceT = newObj.priceT;
        selectedItem.priceSwitchTrainingSession = newObj.priceSwitchTrainingSession;
        switch (newObj.priceT) {
            case "500":
                selectedItem.sessions = 'Up to one session';
                break;
            case "1000":
                selectedItem.sessions = 'Up to two sessions';
                break;
            case "1500":
                selectedItem.sessions = 'Up to three sessions';
                break;
        }
    };
    return {
        addSelectedItem: addSelectedItem,
        getSelectedItem: getSelectedItem
    };
});