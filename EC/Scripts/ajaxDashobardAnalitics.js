
(function () {

    'use strict';
    var app = angular.module('EC', ['nvd3']);


    app.controller('MenuCases', function ($scope, getMenuFilterCases) {
        var promiseObj = getMenuFilterCases.getData();
        promiseObj.then(function (response) {
            $scope.MenuCases = response.data;
        });
    });



    app.controller('BodyController', function ($scope, $http, getMenuFilterCases) {
        getMenuFilterCases
        //getCasesService.then(function createGraph(temp) {
        //    var a = 10;
        //});

        $http({ method: 'GET', url: '/api/AnalyticsRootCauseAnalysis?secondaryType=0' }).
            then(function successCallback(response) {
                $scope.data = response.data;

                $scope.chartData1 = $scope.data.Behavioral;
                $scope.chartData2 = $scope.data.External;
                $scope.chartData3 = $scope.data.Organizational;

                $scope.chartColors = ['#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472'];

                var chart = {
                    type: 'pieChart',
                    donut: true,
                    donutRatio: 0.55,
                    labelThreshold: .05,
                    x: function (d) {
                        return d.name;
                    },
                    y: function (d) {
                        return d.count;
                    },
                    height: 500,
                    showLabels: false,
                    color: $scope.chartColors,
                    duration: 500,
                    labelSunbeamLayout: true,
                    showLegend: false,
                };
                $scope.behavioralFactors = {
                    chart: chart
                };
                $scope.behavioralFactors.chart.title = "Behavioral Factors";
                $scope.externalInfluences = {
                    chart: chart
                };

                $scope.organizationalInfluences = {
                    chart: chart
                };

                $scope.externalInfluences.chart.title = "External Influences";
                $scope.organizationalInfluences.chart.title = "Organizational Influences";

            }, function errorCallback(response) {
                console.log('error');
            });
    });


    app.controller('CasesController', function ($scope, getCasesService) {

        var promiseObj = getCasesService.getData();
        promiseObj.then(function (response) {
            $scope.chartColors = ['#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472', '#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472', '#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472'];

            $scope.dataCases = JSON.parse(response);

            $scope.DepartmentsData = $scope.dataCases.DepartmentTable;
            $scope.LocationData = $scope.dataCases.LocationTable;
            $scope.TypesOfIncidentData = $scope.dataCases.SecondaryTypeTable;
            $scope.TypesOfReporterData = $scope.dataCases.RelationTable;

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
                showLabels: false,
                color: $scope.chartColors,
                duration: 500,
                labelSunbeamLayout: true,
                showLegend: false,
            };

            $scope.containerDepartments = {
                chart: chart
            };

            $scope.containerLocation = {
                chart: chart
            };

            $scope.containerTypesOfIncident = {
                chart: chart
            };

            $scope.containerTypesOfReporter = {
                chart: chart
            };

            $scope.containerDepartments.chart.title = "Departments";
            $scope.containerLocation.chart.title = "Location";
            $scope.containerTypesOfReporter.chart.title = "Type of reporter";
            $scope.containerTypesOfIncident.chart.title = "Type of incident";
        });
    });


    app.controller('CaseTime', function ($scope) {

    });

    app.factory('getCasesService', function ($http, $q) {
        return {
            getData: function (companyId, userId) {
                var deffered = $q.defer();
                $http({ method: 'POST', data: { companyId: companyId, userId: userId }, url: '/Analytics/CompanyDepartmentReportAdvanced' })
                    .then(function success(response) {
                        deffered.resolve(response.data);
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
                    }, function error(response) {
                        deffered.reject(response.status);
                    });
                return deffered.promise;
            }
        }
    });

}());
