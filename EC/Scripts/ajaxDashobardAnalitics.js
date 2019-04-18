﻿
(function () {

    'use strict';
    var arraySelectedItems = {
        location: [],
        department: [],
        incident_type: [],
        reporter_type: []
    };
    var app = angular.module('EC');



    app.controller('Today_spanshot', function ($scope, AnalyticsByDate) {
        //var promiseObj = AnalyticsByDate.getData();
        //promiseObj.then(function (response) {
        //    $scope._today_spanshot = response.data._today_spanshot;
        //});
    });
    
    app.controller('CasesController', function ($scope, getCasesService, addPercentageRoundGraph, getMenuFilterCases) {
        var promiseObj = getMenuFilterCases.getData();
        promiseObj.then(function (response) {
            $scope.MenuCases = response.data;
            $scope.selectedItemClick = function ($event, clickedItemId, menu) {
                if (arraySelectedItems[menu].indexOf(clickedItemId) == -1) {
                    arraySelectedItems[menu].push(clickedItemId);
                } else {
                    arraySelectedItems[menu].splice(arraySelectedItems[menu].indexOf(clickedItemId), 1);
                }
                
                $event.currentTarget.classList.toggle('checked');
                updateGraph();
            }

        });
        (function updateGraph() {
            var promiseObj = getCasesService.getData(arraySelectedItems);
            promiseObj.then(function (response) {
                $scope.chartColors = ['#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472', '#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472', '#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472'];

                $scope.dataCases = JSON.parse(response);
                console.log('$scope.dataCases CasesController ', $scope.dataCases);
                function returnGraph() {
                    var chart = {
                        type: 'pieChart',
                        donut: true,
                        donutRatio: 0.55,
                        labelThreshold: .05,
                        x: function (d) {
                            return d.name;
                        },
                        y: function (d) {
                            return d.val;
                        },
                        height: 500,
                        format: "",
                        showLabels: false,
                        color: $scope.chartColors,
                        duration: 500,
                        labelSunbeamLayout: true,
                        showLegend: false,
                    };
                    return chart;
                }
                $scope.DepartmentsData = addPercentageRoundGraph.setPercentage($scope.dataCases.DepartmentTable);
                $scope.LocationData = addPercentageRoundGraph.setPercentage($scope.dataCases.LocationTable);
                $scope.TypesOfIncidentData = addPercentageRoundGraph.setPercentage($scope.dataCases.SecondaryTypeTable);
                $scope.TypesOfReporterData = addPercentageRoundGraph.setPercentage($scope.dataCases.RelationTable);
                if ($scope.DepartmentsData.length == 0) {
                    $scope.containerDepartmentsValues = true;
                } else {
                    $scope.containerDepartments = {
                        chart: returnGraph()
                    };
                    $scope.containerDepartments.chart.title = "Departments";
                    $scope.containerDepartments.chart.noData = "No cases found";
                }
                if ($scope.LocationData.length == 0) {
                    $scope.locationDataValues = true;
                } else {
                    $scope.containerLocation = {
                        chart: returnGraph()
                    };
                    $scope.containerLocation.chart.title = "Location";
                    $scope.containerLocation.chart.noData = "No cases found";
                }
                if ($scope.TypesOfIncidentData.length == 0) {
                    $scope.typesOfIncidentDataValues = true;
                } else {
                    $scope.containerTypesOfIncident = {
                        chart: returnGraph()
                    };
                    $scope.containerTypesOfIncident.chart.title = "Type of incident";
                    //$scope.containerTypesOfIncident.chart.insert('tspan').text('Type of').attr('dy', 0).attr('x', 0).attr('class', 'big-font');
                    //$scope.containerTypesOfIncident.chart.insert('tspan').text('incident').attr('dy', 20).attr('x', 0);

                    $scope.containerTypesOfIncident.chart.noData = "No cases found";
                }
                if ($scope.TypesOfReporterData.length == 0) {
                    $scope.typesOfReporterDataValues = true;
                } else {
                    $scope.containerTypesOfReporter = {
                        chart: returnGraph()
                    };
                    $scope.containerTypesOfReporter.chart.title = "Type of reporter";
                    $scope.containerTypesOfReporter.chart.noData = "No cases found";
                }
            });
        })();

    });

    app.controller('CaseManagamentTime', function ($scope, getTurnAroundTime) {
        var promiseObj = getTurnAroundTime.getData();
        promiseObj.then(function (response) {
            $scope.turnaroundTime = response.data.resultAroundTime;
            $scope.CaseManagamentTime = response.data.CaseManagamentTime;
            var columnData = [];
            var barData = [0];
            var anotherBar = [];
            var previousElement = 0;
            $scope.CaseManagamentTime.forEach(function (element) {
                columnData.push([element.Name, element.value]);
                barData.push(element.value);
            });
            barData = barData.sort();
            barData.forEach(function (element) {
                previousElement = previousElement + element;
                anotherBar.push([previousElement]);
            });
            //console.log("response.data.CaseManagamentTime " + response.data.CaseManagamentTime);
            //console.log("barData " + barData);
            $scope.chartColors = ['#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472', '#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472', '#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472'];

            $scope.chart1 = c3.generate({
                bindto: '#responseTime',
                data: {
                    columns: $scope.CaseManagamentTime.Name,
                    columns: columnData,
                    type: 'bar',
                    labels: false,
                    groups: [['Under Inves', 'Report Review', 'New Report', 'Awaiting Sign-Off']],
                },
                point: {
                    show: false
                },
                bar: {
                    width: {
                        ratio: 1 // this makes bar width 50% of length between ticks
                    }
                    // or
                    //width: 100 // this makes bar width 100px
                },
                axis: {
                    rotated: true,
                    x: {
                        type: 'category',

                    }, 
                    y: {
                        tick: {
                            values: anotherBar
                        }
                    }
                },
                color: $scope.chartColors,
                size: {
                    height: 50
                },
                legend: {
                    show: false
                },
                tooltip: {
                    show: false
                }
            });

        });

    });

    app.factory('getCasesService', function ($http, $q) {
        return {
            getData: function (arraySelectedItems) {
                var deffered = $q.defer();
                var types = {
                    ReportsDepartmentIDStringss:[],
                    ReportsLocationIDStrings:[],
                    dateStart:[],
                    dateEnd:[]
                };
                var ReportsDepartmentIDStringss = arraySelectedItems.department;

                //types['ReportsDepartmentIDStringss'] = 
                types['ReportsLocationIDStrings'] = arraySelectedItems.location;
                types['dateStart'] = [];
                types['dateEnd'] = [];

                $http({ method: 'POST', data: { ReportsDepartmentIDStringss  }, url: '/Analytics/CompanyDepartmentReportAdvanced' })
                    .then(function success(response) {
                      deffered.resolve(response.data);
                      //console.log('getCasesService ', response.data);

                    }, function error(response) {
                        deffered.reject(response.status);
                    });
                return deffered.promise;
            }
        }
    });
    app.factory('getMenuFilterCases', function ($http, $q) {
        return {
            getData: function () {
                var deffered = $q.defer();
                $http({
                    method: 'POST', data: {},
                    url: '/api/AnalyticsDashboardAPI/GetMenuDashboard'
                })
                    .then(function success(response) {
                      deffered.resolve(response);
                      //console.log('GetMenuDashboard ', response.data);
                    }, function error(response) {
                        deffered.reject(response.status);
                    });
                return deffered.promise;
            }
        }
    });
    app.factory('AnalyticsByDate', function ($http, $q) {
        return {
            getData: function () {
                var deffered = $q.defer();
                $http({
                    method: 'GET',
                    url: '/api/AnaliticsDashboardCases/AnalyticsByDate'
                })
                    .then(function success(response) {
                        deffered.resolve(response);
                        //console.log('GetMenuDashboard ', response.data);
                    }, function error(response) {
                        deffered.reject(response.status);
                    });
                return deffered.promise;
            }
        }
    });
    app.factory('addPercentageRoundGraph', function () {
        return {
            setPercentage: function (array) {
                if (toString.call(array) !== "[object Array]")
                    return false;

                let sum = 0;
                array.forEach(function (element) {
                    sum += element.val;
                });
                if (sum > 0) {
                    array.forEach(function (element) {
                        element.percentage = Math.round((element.val * 100) / sum);
                    });
                }
                return array;
            }
        }
    });
    app.factory('getTurnAroundTime', function ($http, $q) {
        return {
            getData: function () {
                var deffered = $q.defer();
                $http({
                    method: 'POST',
                    url: '/api/AnaliticsDashboardCases/getTurnAroundTime'
                })
                    .then(function success(response) {
                        deffered.resolve(response);
                        //console.log('GetMenuDashboard ', response.data);
                    }, function error(response) {
                        deffered.reject(response.status);
                    });
                return deffered.promise;
            }
        }
    });
    app.directive("ngToggleClass", function () {
        return {
            restrict: 'A',
            compile: function (element, attr) {
                var classes = attr.ngToggleClass.split(',');
                element.bind('click', function () {
                    angular.forEach(classes, function (value) {
                        (element.hasClass(value)) ? element.removeClass(value) : element.addClass(value);
                    });
                });
            }
        }
    });
}());
