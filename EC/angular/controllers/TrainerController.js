(function () {

    'use strict';

    angular
        .module('EC')
        .controller('TrainerController',
            ['$scope', '$filter', '$location', 'TrainerService', TrainerController]);

    function TrainerController($scope, $filter, $location, TrainerService) {

        $scope.uiConfig = {
            calendar: {
                header: {
                    left: 'month,agendaWeek,agendaDay',
                    center: 'title',
                },
            },
        };
    }
}());
