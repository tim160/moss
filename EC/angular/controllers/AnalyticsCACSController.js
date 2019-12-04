(function () {

    'use strict';

    angular
        .module('EC')
        .controller('AnalyticsCACSController',
            ['$scope', 'AnalyticsCACSService', AnalyticsCACSController]);

    function AnalyticsCACSController($scope, AnalyticsCACSService) {

        $scope.cc_crime_statistics_categories = [];
        $scope.cc_crime_statistics_category = { id: 0 };
        $scope.totals = [];

        $scope.refresh = function () {
            AnalyticsCACSService.get({ category: $scope.cc_crime_statistics_category.id }, function (data) {
                if ($scope.cc_crime_statistics_categories.length === 0) {
                    $scope.cc_crime_statistics_categories = data.cc_crime_statistics_categories;
                    $scope.cc_crime_statistics_category = $scope.cc_crime_statistics_categories[0];
                }
                $scope.cacsChartData = data.report_cc_crime;
                $scope.cacsChartData2 = data.report_cc_crime;
                $scope.api.refresh();
                $scope.totals = data.totals;
            });
        };

        $scope.printGraphs = function (elem, title) {
            var offsetWidthOld = document.querySelectorAll('.borderAnalyt')[0].offsetWidth;
            $scope.cacsChart.chart.width = 700;
            var printHtml = '';

            setTimeout(function () {

                if (Array.isArray(elem)) {
                    elem.forEach(function (element) {
                        printHtml += document.querySelector(element).innerHTML;
                    });
                } else {
                    printHtml = document.getElementById(elem).innerHTML;
                }

                $scope.cacsChart.chart.width = offsetWidthOld;
                var mywindow = window.open('', 'PRINT', 'width=' + screen.availWidth + ',height=' + screen.availHeight);

                mywindow.document.write('<html><head><title>' + title + '</title>');
                mywindow.document.write('<link rel="stylesheet" href="/Content/newCase.css" type="text/css" />');
                mywindow.document.write('<link rel="stylesheet" href="/Content/RootcauseAnalisysPrint.css" type="text/css" />');
                mywindow.document.write('<link rel="stylesheet" href="/Libs/nvd3/build/nv.d3.min.css" type="text/css" />');
                mywindow.document.write('</head><body onload="window.print(); window.close()">');
                mywindow.document.write('<div class="container">');
                mywindow.document.write(document.getElementById('templateForPrinting').innerHTML.trim());
                mywindow.document.write(printHtml);
                mywindow.document.write('</div></body></html>');
                $scope.refresh();
                mywindow.document.close(); // necessary for IE >= 10
                mywindow.focus(); // necessary for IE >= 10*/
                return true;
            }, 550);
        };

        $scope.refresh();

        $scope.selectCategory = function (item) {
            $scope.cc_crime_statistics_category = item;
            $scope.refresh();
        };

        $scope.cacsChart = {
            chart: {
                type: 'lineChart',
                height: 450,
                margin: {
                    top: 20,
                    right: 20,
                    bottom: 40,
                    left: 55
                },
                x: function (d) {
                    return d.x;
                },
                y: function (d) {
                    return d.y;
                },
                useInteractiveGuideline: true,
                dispatch: {
                    renderEnd: function () {
                        var wa = d3.select('.nv-legendWrap')[0][0].parentNode.getBBox().width;
                        var wl = d3.select('.nv-legendWrap')[0][0].getBBox().width;
                        var x = (wa - wl) / 2;
                        d3.select('.nv-legendWrap').attr('transform', 'translate(-' + x + ',-30)');
                    },
                },
                xAxis: {
                    axisLabel: 'Years'
                },
                yAxis: {
                    axisLabel: '',
                    tickFormat: function (d) {
                        return d3.format('d')(d);
                    },
                    axisLabelDistance: -10,
                    domain: [0, 1000],
                },
                legend: {
                    align: false
                },
                callback: function () {
                },
                forceY: [0, 1],
            },
            title: {
                enable: false,
                text: ''
            },
            subtitle: {
                enable: false,
                text: '',
                css: {
                    'text-align': 'center',
                    'margin': '10px 13px 0px 7px'
                }
            },
            caption: {
                enable: false,
                html: '',
                css: {
                    'text-align': 'justify',
                    'margin': '10px 13px 0px 7px'
                }
            }
        };

        $scope.Total = function (type, year) {
            if (type === 1) {
                return $scope.totals.length <= year ? 0 : $scope.totals[year];
            }
            if (type === 2) {
                return $scope.totals.length <= year ? 0 : $scope.totals[year];
            }
        };
    }
}());
