(function () {

    'use strict';

    angular
        .module('EC')
        .controller('AnalyticsRootCauseAnalysisController',
            ['$scope', 'AnalyticsRootCauseAnalysisService', AnalyticsRootCauseAnalysisController]);

    function AnalyticsRootCauseAnalysisController($scope, AnalyticsRootCauseAnalysisService) {

        $scope.secondaryType = { id: 0 };
        $scope.secondaryTypes = [];

        $scope.refresh = function () {
            AnalyticsRootCauseAnalysisService.get({ secondaryType: $scope.secondaryType.id }, function (data) {
                if ($scope.secondaryTypes.length === 0) {
                    $scope.secondaryTypes = data.SecondaryTypes;
                    $scope.secondaryType = $scope.secondaryTypes[0];
                }
                $scope.chartData1 = data.Behavioral;
                $scope.chartData2 = data.External;
                $scope.chartData3 = data.Organizational;
                //$scope.api1.refresh();
                //$scope.api2.refresh();
                //$scope.api3.refresh();
            });
        };

        $scope.refresh();

        $scope.selectSecondaryTypes = function (item) {
            $scope.secondaryType = item;
            $scope.refresh();
        };


        $scope.chart1 = {
            chart: {
                type: 'pieChart',
                donut: true,
                donutRatio: 1,
                x: function (d) {
                    return d.name;
                },
                y: function (d) {
                    return d.count;
                },
                height: 500,
                showLabels: false,
                duration: 500,
                labelThreshold: 0.01,
                labelSunbeamLayout: true,
                legend: {
                    margin: {
                        top: 5,
                        right: 35,
                        bottom: 5,
                        left: 0
                    }
                }
            }
        };
        $scope.chart2 = {
            chart: {
                type: 'pieChart',
                donut: true,
                donutRatio: 1,
                x: function (d) {
                    return d.name;
                },
                y: function (d) {
                    return d.count;
                },
                height: 500,
                showLabels: false,
                duration: 500,
                labelThreshold: 0.01,
                labelSunbeamLayout: true,
                legend: {
                    margin: {
                        top: 5,
                        right: 35,
                        bottom: 5,
                        left: 0
                    }
                }
            }
        };
        $scope.chart3 = {
            chart: {
                type: 'pieChart',
                donut: true,
                donutRatio: 1,
                x: function (d) {
                    return d.name;
                },
                y: function (d) {
                    return d.count;
                },
                height: 500,
                showLabels: false,
                duration: 500,
                labelThreshold: 0.01,
                labelSunbeamLayout: true,
                legend: {
                    margin: {
                        top: 5,
                        right: 35,
                        bottom: 5,
                        left: 0
                    }
                }
            }
        };
    }
}());
