
(function () {

    'use strict';
    var app = angular.module('EC', ['nvd3']); //not drawing graph but error is epsent

    app.controller('BodyController', function ($scope, $http) {

        $http({ method: 'GET', url: '/api/AnalyticsRootCauseAnalysis?secondaryType=0' }).
            then(function successCallback(response) {
                $scope.data = response.data;

                $scope.chartData1 = $scope.data.Behavioral;
                $scope.chartData2 = $scope.data.External;
                $scope.chartData3 = $scope.data.Organizational;

                $scope.chartColors = ['#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472'];

                $scope.behavioralFactors = {
                    chart: {
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
                    }
                };
                $scope.behavioralFactors.chart.title = "Behavioral Factors";
                $scope.externalInfluences = {
                    chart: {
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
                    }
                };

                $scope.organizationalInfluences = {
                    chart: {
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
                    }
                };

                $scope.externalInfluences.chart.title = "External Influences";
                $scope.organizationalInfluences.chart.title = "Organizational Influences";

            }, function errorCallback(response) {
                console.log('error');
            });
    });


    app.controller('CasesController', function ($scope, $http) {
        var url = '/Analytics/CompanyDepartmentReportAdvanced';
        var data = {
            companyId: 3136,
            userId: 12503
        };
        $http.post(url, data).
            then(function successCallback(response) {
                $scope.chartColors = ['#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472', '#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472', '#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472'];

                $scope.dataCases = JSON.parse(response.data);

                $scope.DepartmentsData = $scope.dataCases.DepartmentTable;
                $scope.LocationData = $scope.dataCases.LocationTable;
                $scope.TypesOfIncidentData = $scope.dataCases.SecondaryTypeTable;
                $scope.TypesOfReporterData = $scope.dataCases.RelationTable;


                $scope.containerDepartments = {
                    chart: {
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
                    }
                };
                
                $scope.containerLocation = {
                    chart: {
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
                    }
                };

                $scope.containerTypesOfIncident = {
                    chart: {
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
                    }
                };

                $scope.containerTypesOfReporter = {
                    chart: {
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
                    }
                };

                $scope.containerDepartments.chart.title = "Departments";
                $scope.containerLocation.chart.title = "Location";
                $scope.containerTypesOfReporter.chart.title = "Type of reporter";
                $scope.containerTypesOfIncident.chart.title = "Type of incident";
                

            }, function errorCallback(response) {
                console.log('error');
            });

    });
}());
