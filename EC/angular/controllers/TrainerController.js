(function () {

    'use strict';

    angular
        .module('EC')
        .controller('TrainerController',
            ['$scope', '$filter', '$location', 'TrainerService', 'uiCalendarConfig', TrainerController]);

    function TrainerController($scope, $filter, $location, TrainerService, uiCalendarConfig) {

        $scope.refresh = function () {
            TrainerService.getTrainer({ DateFrom: $scope.period.start, DateTo: $scope.period.end }, function (data) {
                for (var i = 0; i < data.Events.length; i++) {
                    data.Events[i].start = moment(data.Events[i].start).toDate();
                    data.Events[i].end = moment(data.Events[i].end).toDate();
                }

                uiCalendarConfig.calendars.calendarOne.fullCalendar('unselect');
                $scope.eventSources.splice(0, $scope.eventSources.length);
                $scope.eventSources.push({
                    events: data.Events,
                });
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
                agenda: 'H:mm',
                views: {
                    week: {
                        timeFormat: 'H:mm' //this will return 23:00 time format
                    }
                },
                defaultView: 'agendaWeek',
                minTime: '00:00:00',
                maxTime: '24:00:00',
                slotDuration: '01:00:00',
                axisFormat: 'HH:mm',
                selectable: true,
                eventClick: function (date, jsEvent, view) {
                    console.log(date, jsEvent, view);
                },
                select: function (start, end, allDay) {
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
                viewRender: function (view, element) {
                    $scope.period = {
                        start: view.start.toDate(),
                        end: view.end.toDate(),
                    };
                    $scope.refresh();
                },
                selectHelper: true,
                eventRender: function (event, element, view) {
                    if (view.name === 'agendaWeek') {
                        element.find('.fc-content').prepend('<a href=\"#\" style=\"float: right\" class=\"closeon\">X</span>');
                        element.find('.closeon').off('click').on('click', function () {
                            TrainerService.deleteTime({ id: event.id }, function (data) {
                                uiCalendarConfig.calendars.calendarOne.fullCalendar('removeEvents', event._id);
                            });
                        });
                    }
                },
            },
        };
    }
}());
