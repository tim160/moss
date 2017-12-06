(function () {

    'use strict';

    angular
        .module('EC')
        .controller('NewCaseMessagesController',
            ['$scope', '$filter', 'orderByFilter', '$location', 'NewCaseMessagesService', NewCaseMessagesController]);

    function NewCaseMessagesController($scope, $filter, orderByFilter, $location, NewCaseMessagesService) {
        $scope.report_id = $filter('parseUrl')($location.$$absUrl, 'report_id');

        $scope.activeTab = 1;
        $scope.activeMessage = {};

        $scope.mediators = [];
        $scope.reporters = [];
        $scope.currentUser = {};

        $scope.newMessage = '';

        $scope.refresh = function () {
            NewCaseMessagesService.get({ id: $scope.report_id }, function (data) {
                $scope.mediators = data.mediators;
                $scope.reporters = data.reporters;
                $scope.currentUser = data.currentUser;
            });
        };

        $scope.refresh();

        $scope.sendMessage = function () {
            NewCaseMessagesService.post({ report_id: $scope.report_id, newMessage: $scope.newMessage }, function () {
                $scope.newMessage = '';
                $scope.refresh();
            });
        };
    }
}());
