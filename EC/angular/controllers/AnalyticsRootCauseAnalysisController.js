(function () {

    'use strict';

    angular
        .module('EC')
        .controller('AnalyticsRootCauseAnalysisController',
            ['$scope', 'AnalyticsRootCauseAnalysisService', 'AdditionalComp', AnalyticsRootCauseAnalysisController]);

    function AnalyticsRootCauseAnalysisController($scope, AnalyticsRootCauseAnalysisService, AdditionalComp) {

        $scope.displayCompanyName = document.querySelector('#ddListDefaultValue').value;
        var companyIdArray = Array();
        $scope.additionalCompanies = Array();
        $scope.showDDMenu = false;

        AdditionalComp.getData(document.querySelector('#company_id').value).then(function (response) {
            $scope.additionalCompanies = response.data.additionalCompanies;
            companyIdArray = $scope.additionalCompanies.map(function (item) {
                return item.id;
            });
            $scope.secondaryType = { id: 0 };
            $scope.secondaryTypes = [];

            $scope.refresh($scope.secondaryType, companyIdArray);
        });

        $scope.ddListClickedCompany = function (company_id, company_name) {
            if (company_name === undefined) {
                $scope.displayCompanyName = document.querySelector('#ddListDefaultValue').value;
                $scope.filterValue = null;
                $scope.refresh($scope.secondaryType.id, companyIdArray);
            } else {
                $scope.filterValue = company_id;
                $scope.displayCompanyName = company_name;
                $scope.refresh($scope.secondaryType.id, company_id);
            }
        };

        $scope.refresh = function (secondaryType, companyIdArray) {
            AnalyticsRootCauseAnalysisService.get({ secondaryType: secondaryType, companyId: companyIdArray }, function (data) {

                if ($scope.secondaryTypes.length === 0) {
                    $scope.secondaryTypes = data.SecondaryTypes;
                    $scope.secondaryType = $scope.secondaryTypes[0];
                }
                $scope.chartData1 = setPercentage(data.Behavioral);
                $scope.chartData2 = setPercentage(data.External);
                $scope.chartData3 = setPercentage(data.Organizational);
                $scope.chart1.chart.title = 'Behavioral Factors';
                $scope.chart2.chart.title = 'External Influences';
                $scope.chart3.chart.title = 'Organizational Influences';
                $scope.chartColors = data.Colors;
            });
        };
        function setPercentage(array) {
            if (toString.call(array) !== '[object Array]') {
                return false;
            }

            var sum = 0;
            array.forEach(function (element) {
                sum += element.count;
            });
            if (sum > 0) {
                array.forEach(function (element) {
                    element.percentage = Math.round((element.count * 100) / sum);
                    if (element.percentage === 0) {
                        element.percentage = 0.01;
                    }
                });
            }
            return array;
        }
        $scope.chartColors = ['#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472'];
        $scope.selectSecondaryTypes = function (item) {
            $scope.secondaryType = item;
            var companyIDSelected = '';
            if($scope.filterValue === undefined) {
                companyIDSelected = $scope.additionalCompanies.map(function (item) {
                    return item.id;
                });
            } else {
                companyIDSelected = $scope.filterValue;
            }
            $scope.refresh(item.id, companyIDSelected);
        };
        $scope.chart1 = {
            chart: {
                type: 'pieChart',
                donut: true,
                donutRatio: 0.72,
                labelThreshold: .05,
                x: function (d) {
                    return d.name;
                },
                y: function (d) {
                    return d.count;
                },
                width: 225,
                height: 300,
                format: '',
                showLabels: false,
                color: $scope.chartColors,
                duration: 500,
                labelSunbeamLayout: true,
                showLegend: false,
                dispatch: {
                    renderEnd: function (e) {
                        var label = d3.select('#chart1 text.nv-pie-title');
                        label.html('');
                        label.insert('tspan').text('Behavioral').attr('dy', -10).attr('x', 0);
                        label.insert('tspan').text('Factors').attr('dy', 20).attr('x', 0);
                    }
                }
            }
        };
        $scope.chart2 = {
            chart: {
                type: 'pieChart',
                donut: true,
                donutRatio: 0.72,
                labelThreshold: .05,
                x: function (d) {
                    return d.name;
                },
                y: function (d) {
                    return d.count;
                },
                width: 225,
                height: 300,
                format: '',
                showLabels: false,
                color: $scope.chartColors,
                duration: 500,
                labelSunbeamLayout: true,
                showLegend: false,
                dispatch: {
                    renderEnd: function (e) {
                        var label = d3.select('#chart2 text.nv-pie-title');
                        label.html('');
                        label.insert('tspan').text('External').attr('dy', -10).attr('x', 0);
                        label.insert('tspan').text('Influences').attr('dy', 20).attr('x', 0);
                    }
                }
            }
        };

        $scope.chart3 = {
            chart: {
                type: 'pieChart',
                donut: true,
                donutRatio: 0.72,
                labelThreshold: .05,
                x: function (d) {
                    return d.name;
                },
                y: function (d) {
                    return d.count;
                },
                width: 225,
                height: 300,
                format: '',
                showLabels: false,
                color: $scope.chartColors,
                duration: 500,
                labelSunbeamLayout: true,
                showLegend: false,
                dispatch: {
                    renderEnd: function (e) {
                        var label = d3.select('#chart3 text.nv-pie-title');
                        label.html('');
                        label.insert('tspan').text('Organizational').attr('dy', -10).attr('x', 0);
                        label.insert('tspan').text('Influences').attr('dy', 20).attr('x', 0);
                    }
                }
            }
        };

        $scope.print = function (elem, title) {
            setTimeout(function () {
                var mywindow = window.open('', 'PRINT', 'width=' + screen.availWidth + ',height=' + screen.availHeight);

                mywindow.document.write('<html><head><title>' + title + '</title>');
                mywindow.document.write('<link rel="stylesheet" href="/Content/styleAnalytics.css" type="text/css" />');
                mywindow.document.write('<link rel="stylesheet" href="/Content/newCase.css" type="text/css" />');
                mywindow.document.write('<link rel="stylesheet" href="/Content/RootcauseAnalisysPrint.css" type="text/css" />');
                mywindow.document.write('</head><body onload="window.print(); window.close()">');
                mywindow.document.write('<h1>' + title + '</h1>');
                mywindow.document.write('<div class="container">');
                mywindow.document.write(document.getElementById('templateForPrinting').innerHTML.trim());
                mywindow.document.write(document.getElementById(elem).innerHTML);
                mywindow.document.write('</div></body></html>');

                mywindow.document.close(); // necessary for IE >= 10
                mywindow.focus(); // necessary for IE >= 10*/

                return true;

            }, 250);
        };
    }
}());
