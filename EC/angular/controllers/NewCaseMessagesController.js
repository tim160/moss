(function () {

    'use strict';

    angular
        .module('EC')
        .controller('NewCaseMessagesController',
            ['$scope', '$filter', 'orderByFilter', '$location', 'NewCaseMessagesService', NewCaseMessagesController]);

    function NewCaseMessagesController($scope, $filter, orderByFilter, $location, NewCaseMessagesService) {
        $scope.report_id = $filter('parseUrl')($location.$$absUrl, 'id');

        $scope.activeTab = $location.$$absUrl.toLowerCase().indexOf('/reporter') === -1 ? 1 : 2;
        $scope.activeMessage = {};

        $scope.mediators = [];
        $scope.mediatorsUnreaded = 0;
        $scope.reporters = [];
        $scope.reportersUnreaded = 0;
        $scope.currentUser = {};

        $scope.newMessage = '';
        $scope.newMessageReporter = '';

        $scope.refresh = function () {
            NewCaseMessagesService.get({ id: $scope.report_id }, function (data) {
                $scope.mediators = data.mediators;
                $scope.reporters = data.reporters;
                $scope.currentUser = data.currentUser;
                $scope.mediatorsUnreaded = $scope.unreaded(data.mediators);
                $scope.reportersUnreaded = $scope.unreaded(data.reporters);
            });
        };

        $scope.sendMessage = function () {
            if ($scope.newMessage.trim().length > 0) {
                NewCaseMessagesService.post({ mode: 1, id: $scope.report_id, newMessage: $scope.newMessage }, function () {
                    $scope.newMessage = '';
                    $scope.refresh();
                });
            } else {
                angular.element('textarea').addClass('error');
            }
        };

        $scope.change = function () {
            if ($scope.newMessage.trim().length > 0) {
                angular.element('textarea').removeClass('error');
            }
        };

        $scope.sendMessageReporter = function () {
            NewCaseMessagesService.post({ mode: 2, id: $scope.report_id, newMessage: $scope.newMessageReporter }, function () {
                $scope.newMessageReporter = '';
                $scope.refresh();
            });
        };

        $scope.unreaded = function (list) {
            var count = 0;
            for (var i = 0; i < list.length; i++) {
                count += list[i].isReaded ? 0 : 1;
            }
            return count;
        };

        $scope.refresh();
    }
}());
