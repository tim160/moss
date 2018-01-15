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
            /*var types = $scope.secondaryTypes.map(function (v) {
                return v._selected;
            });*/
            AnalyticsRootCauseAnalysisService.get({ secondaryType: $scope.secondaryType.id }, function (data) {
                if ($scope.secondaryTypes.length === 0) {
                    $scope.secondaryTypes = data.SecondaryTypes;
                    $scope.secondaryType = $scope.secondaryTypes[0];
                }
                $scope.chartData1 = data.Behavioral;
                $scope.chartData2 = data.External;
                $scope.chartData3 = data.Organizational;
                $scope.chart1.chart.title = data.BehavioralTotal;
                $scope.chart2.chart.title = data.ExternalTotal;
                $scope.chart3.chart.title = data.OrganizationalTotal;
            });
        };

        $scope.chartColors = d3.scale.category20().range();

        $scope.refresh();

        $scope.selectSecondaryTypes = function (item) {
            //item._selected = !item._selected;
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
                legendPosition: 'bottom',
                title: {
                    enable: true,
                    text: '',
                },
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
