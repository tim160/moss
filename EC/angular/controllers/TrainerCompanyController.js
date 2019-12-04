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
                agenda: 'H:mm',
                views: {
                    week: {
                        timeFormat: 'H:mm'
                    }
                },
                height: 'auto',
                minTime: '07:00:00',
                maxTime: '19:00:00',
                scrollTime: '19:00:00',
                slotDuration: '01:00:00',
                axisFormat: 'hh:mm a',
                selectable: true,
                selectOverlap: true,
                select: function (start, end) {
                    var dt = new Date();
                    var d3 = moment(dt).add(3, 'days').utc().startOf('day');

                    var ds = moment(start);
                    var msg = 'Do you want to book training on ' + ds.format('MMM DD HH:00') + '?';

                    ds = ds.startOf('day');
                    if (d3.diff(ds, 'days') > 0) {
                        msg += ' WARNING. You cannot cancel training withing 3 days before start date.';
                    }
                    if (!confirm(msg)) {
                        uiCalendarConfig.calendars.calendarOne.fullCalendar('unselect');
                    } else {
                        TrainerService.addEvent({ DateFrom: start, DateTo: end }, function (data) {
                            $scope.refresh();
                            if (!data.Result) {
                                if (confirm(data.Message)) {
                                    if (data.booked_session !== '') {
                                        TrainerService.deleteCompanyTime({ Hour: data.booked_session.Hour }, function (data) {
                                            if (data.Result) {
                                                TrainerService.addEvent({ DateFrom: start, DateTo: end }, function (data) {
                                                    if (!data.Result) {
                                                        alert(data.Message);
                                                    }
                                                    $scope.refresh();
                                                });
                                            } else {
                                                alert(data.Message);
                                            }
                                        });
                                    }

                                }
                            }
                        });
                    }
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
                    if (view.name === 'agendaWeek') {
                        element.find('.fc-content').prepend('<a href=\"#\" style=\"float: right\" class=\"closeon\">X</span>');
                        element.find('.closeon').off('click').on('click', function () {
                            if (confirm('Do you want to cancel this training?')) {
                                TrainerService.deleteCompanyTime({ Hour: event.start.format('YYYY/MM/DD HH:00:00') }, function (data) {
                                    if (data.Result) {
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
            TrainerService.get({ DateFrom: moment($scope.period.start).format('YYYY/MM/DD'), DateTo: moment($scope.period.end).format('YYYY/MM/DD') }, function (data) {
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
                $scope.onboardingsRemaining = data.OnboardingsRemaining;
            });
        };
    }
}());
