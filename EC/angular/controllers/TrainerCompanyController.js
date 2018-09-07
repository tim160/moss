(function () {

    'use strict';

    angular
        .module('EC')
        .controller('TrainerCompanyController',
            ['$scope', '$filter', '$location', '$timeout', 'TrainerService', 'uiCalendarConfig', TrainerCompanyController]);

    function TrainerCompanyController($scope, $filter, $location, $timeout, TrainerService, uiCalendarConfig) {

        $scope.refresh = function () {
            TrainerService.get({ DateFrom: $scope.period.start, DateTo: $scope.period.end }, function (data) {
                for (var i = 0; i < data.Events.length; i++) {
                    data.Events[i].start = moment(data.Events[i].start).toDate();
                    data.Events[i].end = moment(data.Events[i].end).toDate();
                }

                $scope.eventSources.splice(0, $scope.eventSources.length);
                $scope.eventSources.push({
                    events: data.Events,
                });

                //$('#fullCalendar').fullCalendar('gotoDate', $scope.period.start);
                //uiCalendarConfig.calendars.calendarOne.fullCalendar('removeEvents');
                //uiCalendarConfig.calendars.calendarOne.fullCalendar('refetchEvents');
                //uiCalendarConfig.calendars.calendarOne.fullCalendar('render');
                //uiCalendarConfig.calendars.calendarOne.fullCalendar('updateEvents', $scope.eventSources);
                //uiCalendarConfig.calendars['calendarOne'].fullCalendar('rerenderEvents');
                //uiCalendarConfig.calendars['calendarOne'].fullCalendar('removeEventSource', $scope.eventSources);
                //uiCalendarConfig.calendars['calendarOne'].fullCalendar('addEventSource', $scope.eventSources.events);
            });
        };

        $scope.eventSources = [{
        }];

        $scope.uiConfig = {
            calendar: {
                header: {
                    left: 'month,agendaWeek,agendaDay',
                    center: 'title',
                },
                defaultView: 'agendaWeek',
                eventClick: function(date, jsEvent, view) {
                    console.log(date, jsEvent, view);
                },
                agenda: 'H:mm',
                views: {
                    week: {
                        timeFormat: 'H:mm' //this will return 23:00 time format
                    }
                },
                axisFormat: 'HH:mm',
                //timeFormat: 'HH:mm{ - HH:mm}',
                selectable: true,
                select: function (start, end, allDay) {
                    TrainerService.addEvent({ DateFrom: start, DateTo: end }, function (data) {
                        $scope.refresh();
                        if (!data.Result) {
                            alert(data.Message);
                        }
                    });
                },
                viewRender: function(view, element) {
                    $scope.period = {
                        start: view.start.toDate(),
                        end: view.end.toDate(),
                    };
                    $scope.refresh();
                },
                selectHelper: true,
            },
        };
    }
}());
