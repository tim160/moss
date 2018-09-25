(function () {

    'use strict';

    angular
        .module('EC')
        .controller('TrainerCompanyController',
            ['$scope', '$filter', '$location', '$timeout', 'TrainerService', 'uiCalendarConfig', TrainerCompanyController]);

    function TrainerCompanyController($scope, $filter, $location, $timeout, TrainerService, uiCalendarConfig) {

        $scope.eventSources = [];

        $scope.uiConfig = {
            calendar: {
                header: {
                    left: 'month,agendaWeek,agendaDay',
                    center: 'title',
                },
                defaultView: 'agendaWeek',
                eventClick: function(date, jsEvent, view) {
                },
                agenda: 'H:mm',
                views: {
                    week: {
                        timeFormat: 'H:mm' //this will return 23:00 time format
                    }
                },
                minTime: '00:00:00',
                maxTime: '24:00:00',
                slotDuration: '01:00:00',
                axisFormat: 'HH:mm',
                selectable: true,
                select: function (start, end, allDay) {
                    if (!confirm(' Do you want to book this time for training?')) {
                        uiCalendarConfig.calendars.calendarOne.fullCalendar('unselect');
                    } else {
                        TrainerService.addEvent({ DateFrom: start, DateTo: end }, function (data) {
                            $scope.refresh();
                            if (!data.Result) {
                                alert(data.Message);
                            }
                        });
                    }
                },
                viewRender: function(view, element) {
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
                            if (confirm('Do you want to cancel this training?')) {
                                TrainerService.deleteCompanyTime({ Hour: event.start.format('YYYY/MM/DD HH:00:00') }, function (data) {
                                    if (data.Result) {
                                        //uiCalendarConfig.calendars.calendarOne.fullCalendar('removeEvents', event._id);
                                        $scope.refresh();
                                    } else {
                                        alert(data.Message);
                                    }
                                });
                            }
                        });
                    }
                },
            },
        };

        $scope.refresh = function () {
            TrainerService.get({ DateFrom: $scope.period.start, DateTo: $scope.period.end }, function (data) {
                uiCalendarConfig.calendars.calendarOne.fullCalendar('unselect');

                $scope.eventSources.splice(0, $scope.eventSources.length);
                for (var t = 0; t < data.AvailableTimes.length; t++) {
                    $scope.eventSources.push(data.AvailableTimes[t]);
                };

                for (var i = 0; i < data.Events.events.length; i++) {
                    data.Events.events[i].start = moment(data.Events.events[i].start).toDate();
                    data.Events.events[i].end = moment(data.Events.events[i].end).toDate();
                }
                $scope.eventSources.push(data.Events);
            });
        };
    }
}());
