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
                $scope.chartColors = data.Colors;
            });
        };

        $scope.chartColors = ['#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472'];

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
                color: $scope.chartColors,
                duration: 500,
                labelThreshold: 0.01,
                labelSunbeamLayout: true,
                //legendPosition: 'bottom',
                showLegend: false,
                title: {
                    enable: true,
                    text: '',
                },
                labels: {
                    mainLabel: {
                        fontSize: 20,
                    },
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
                color: $scope.chartColors,
                duration: 500,
                labelThreshold: 0.01,
                labelSunbeamLayout: true,
                showLegend: false,
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
                color: $scope.chartColors,
                duration: 500,
                labelThreshold: 0.01,
                labelSunbeamLayout: true,
                showLegend: false,
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

        $scope.print = function (elem, title) {
            var mywindow = window.open('', 'PRINT', 'width=' + screen.availWidth + ',height=' + screen.availHeight);

            mywindow.document.write('<html><head><title>' + title + '</title>');
            mywindow.document.write('<link rel="stylesheet" href="/Content/styleAnalitics.css" type="text/css" />');
            mywindow.document.write('<link rel="stylesheet" href="/Content/newCase.css" type="text/css" />');
            mywindow.document.write('</head><body onload="window.print(); window.close()">');
            mywindow.document.write('<h1>' + title + '</h1>');
            mywindow.document.write('<div class="container">');
            mywindow.document.write(document.getElementById(elem).innerHTML);
            mywindow.document.write('</div></body></html>');

            mywindow.document.close(); // necessary for IE >= 10
            mywindow.focus(); // necessary for IE >= 10*/

            return true;
        };
    }
}());
