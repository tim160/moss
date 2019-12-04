(function () {

    'use strict';

    angular
        .module('EC')
        .controller('TrainerController',
            ['$scope', '$filter', '$location', 'TrainerService', 'uiCalendarConfig', TrainerController]);

    function TrainerController($scope, $filter, $location, TrainerService, uiCalendarConfig) {

        $scope.refresh = function () {
            TrainerService.getTrainer({ DateFrom: $scope.period.start, DateTo: $scope.period.end }, function (data) {
                uiCalendarConfig.calendars.calendarOne.fullCalendar('unselect');

                for (var e = 0; e < 2; e++) {
                    for (var i = 0; i < data.Events[e].events.length; i++) {
                        console.log(moment(data.Events[e].events[i].start).hasTime());
                        data.Events[e].events[i].start = moment(data.Events[e].events[i].start).toDate();
                        data.Events[e].events[i].end = moment(data.Events[e].events[i].end).toDate();
                    }
                }
                $scope.eventSources.splice(0, $scope.eventSources.length);
                $scope.eventSources.push(data.Events[0]);
                $scope.eventSources.push(data.Events[1]);
            });
        };

        $scope.eventSources = [];

        $scope.uiConfig = {
            calendar: {
                header: {
                    left: 'month,agendaWeek,agendaDay',
                    center: 'title',
                },
                agenda: 'H:mm',
                views: {
                    week: {
                        timeFormat: 'H:mm'
                    }
                },
                defaultView: 'agendaWeek',
                minTime: '06:00:00',
                maxTime: '18:00:00',
                slotDuration: '01:00:00',
                axisFormat: 'hh:mm a',
                selectable: true,
                select: function (start, end) {
                    TrainerService.addTime({ DateFrom: start, DateTo: end }, function (data) {
                        if (!data.Result) {
                            uiCalendarConfig.calendars.calendarOne.fullCalendar('unselect');
                            alert(data.Message);
                        } else {
                            $scope.refresh();
                        }
                    });
                    return false;
                },
                viewRender: function (view) {
                    $scope.period = {
                        start: view.start.toDate(),
                        end: view.end.toDate(),
                    };
                    $scope.refresh();
                },
                selectHelper: true,
                eventRender: function (event, element, view) {
                    if ((view.name === 'agendaWeek') && (!event.companyId)) {
                        element.find('.fc-content').prepend('<a href=\"#\" style=\"float: right\" class=\"closeon\">X</span>');
                        element.find('.closeon').off('click').on('click', function () {
                            TrainerService.deleteTime({ Hour: event.start.format('YYYY/MM/DD HH:00:00') }, function () {
                                uiCalendarConfig.calendars.calendarOne.fullCalendar('removeEvents', event._id);
                            });
                        });
                    }
                },
            },
        };
    }
}());
