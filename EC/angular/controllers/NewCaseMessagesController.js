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
            NewCaseMessagesService.post({ mode: 1, report_id: $scope.report_id, newMessage: $scope.newMessage }, function () {
                $scope.newMessage = '';
                $scope.refresh();
            });
        };

        $scope.sendMessageReporter = function () {
            NewCaseMessagesService.post({ mode: 2, report_id: $scope.report_id, newMessage: $scope.newMessageReporter }, function () {
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
