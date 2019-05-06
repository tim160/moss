
(function () {

    'use strict';
    var arraySelectedItems = {
        location: [],
        department: [],
        incident_type: [],//SecondaryTypesList
        reporter_type: [],//RelationTypesList
        data_range: [],
        dateStart: String,
        dateEnd: String
    };
    var app = angular.module('EC', ['nvd3', 'daterangepicker']);
    
    app.controller('CasesController', function ($scope, getCasesService, addPercentageRoundGraph, getMenuFilterCases, AnalyticsByDate) {
        var analiticsObj = AnalyticsByDate.getData();
        analiticsObj.then(function (response) {
            $scope._today_spanshot = response.data._today_spanshot;
        });
        $scope.datePicker = {
            date: { startDate: null, endDate: null },
            options: {
                "showDropdowns": true,
                "autoApply": true,
                ranges: {
                    'All': [moment(), moment()],
                    'Last 12 Months': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 6 Months': [moment().subtract(6, 'days'), moment()],
                    'Last 3 Months': [moment().subtract(29, 'days'), moment()],
                    'Last 30 days': [moment().startOf('month'), moment().endOf('month')],
                    'Last 7 days': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                },
                "linkedCalendars": false,
                "autoUpdateInput": false,
                "showCustomRangeLabel": false,
                "alwaysShowCalendars": true,
                //"startDate": "04/29/2019",
                //"endDate": "05/05/2019"
            }, function(start, end, label) {
                console.log('New date range selected: ' + start.format('YYYY-MM-DD') + ' to ' + end.format('YYYY-MM-DD') + ' (predefined range: ' + label + ')');
            }
        };
        $scope.$watch('datePicker.date', function (newDate) {
            arraySelectedItems.dateStart = newDate.startDate;
            arraySelectedItems.dateEnd = newDate.endDate;
            if (newDate.startDate != undefined && newDate.endDate != undefined) {
                angular.element('#selectedCasesDateRange').html(": <span class='bold'>Start Date: </span>" + moment(newDate.startDate).format("MMMM D, YYYY") + " <span class='bold'>End Date: </span>" + moment(newDate.endDate).format("MMMM D, YYYY"));
            }
            
            updateGraph();
        }, false);
        var promiseObjGetMenu = getMenuFilterCases.getData();
        promiseObjGetMenu.then(function (response) {
            $scope.MenuCases = response.data;

            $scope.selectedCasesFilters = 0;
            $scope.selectedItemClick = function ($event, clickedItemId, menu) {
                if (arraySelectedItems[menu].indexOf(clickedItemId) == -1) {
                    arraySelectedItems[menu].push(clickedItemId);
                    $scope.selectedCasesFilters++;
                } else {
                    arraySelectedItems[menu].splice(arraySelectedItems[menu].indexOf(clickedItemId), 1);
                    $scope.selectedCasesFilters--;
                }
                $event.currentTarget.classList.toggle('checked');
                updateGraph();
            }
            $scope.dataRangeClick = function ($event, clickedItemId) {
                angular.element('#selectedCasesDateRange').html(": " + $event.target.textContent.trim());
                //$scope.selectedCasesDateRange = 
                if (clickedItemId == 0) {
                    //$scope.selectedCasesDateRange = "";
                    angular.element('#selectedCasesDateRange').html("");
                }
                arraySelectedItems["data_range"] = clickedItemId;
                updateGraph();
            }
        });
        updateGraph();
        function updateGraph() {
            var promiseObj = getCasesService.getData(arraySelectedItems);
            promiseObj.then(function (response) {
                $scope.chartColors = ['#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472', '#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472', '#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472'];
                $scope.chartColorsFunction = function (index) {
                    if (index >= $scope.chartColors.length) {
                        return $scope.chartColors[index % $scope.chartColors.length];
                    } else {
                        return $scope.chartColors[index];
                    }
                };
                $scope.dataCases = JSON.parse(response);
                //console.log('$scope.dataCases CasesController ', $scope.dataCases);
                function returnGraph() {
                    var chart = {
                        type: 'pieChart',
                        donut: true,
                        donutRatio: 0.72,
                        labelThreshold: .05,
                        x: function (d) {
                            return d.name;
                        },
                        y: function (d) {
                            return d.val;
                        },
                        width: 220,
                        height: 300,
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

                    //promiseObjGetMenu.then(function (response) {
                    //    response.data.DepartmentsList.forEach(function (item, i, arr) {
                    //        $scope.DepartmentsData.push({ name: item.department_en, val: 0, percentage: 0 });
                    //    });
                    //});



                } else {
                    $scope.containerDepartmentsValues = false;
                    $scope.containerDepartments = {
                        chart: returnGraph()
                    };
                    $scope.containerDepartments.chart.title = "Departments";
                    $scope.containerDepartments.chart.noData = "No cases found";
                }
                if ($scope.LocationData.length == 0) {
                    $scope.locationDataValues = true;


                    //promiseObjGetMenu.then(function (response) {
                    //    response.data.LocationsList.forEach(function (item, i, arr) {
                    //        $scope.LocationData.push({ name: item.location_en, val: 0, percentage: 0 });
                    //    });
                    //});


                } else {
                    $scope.locationDataValues = false;
                    $scope.containerLocation = {
                        chart: returnGraph()
                    };
                    $scope.containerLocation.chart.title = "Location";
                    $scope.containerLocation.chart.noData = "No cases found";
                }
                if ($scope.TypesOfIncidentData.length == 0) {
                    $scope.typesOfIncidentDataValues = true;


                    //promiseObjGetMenu.then(function (response) {
                    //    response.data.SecondaryTypesList.forEach(function (item, i, arr) {
                    //        $scope.TypesOfIncidentData.push({ name: item.secondary_type_en, val: 0, percentage: 0 });
                    //    });
                    //});


                } else {
                    $scope.typesOfIncidentDataValues = false;
                    $scope.containerTypesOfIncident = {
                        chart: returnGraph()
                    };
                    $scope.containerTypesOfIncident.chart.title = "Type of incident";
                    $scope.containerTypesOfIncident.chart.noData = "No cases found";
                }
                if ($scope.TypesOfReporterData.length == 0) {
                    $scope.typesOfReporterDataValues = true;


                    //promiseObjGetMenu.then(function (response) {
                    //    response.data.RelationTypesList.forEach(function (item, i, arr) {
                    //        $scope.TypesOfReporterData.push({ name: item.relationship_en, val: 0, percentage: 0 });
                    //    });
                    //});


                } else {
                    $scope.typesOfReporterDataValues = false;
                    $scope.containerTypesOfReporter = {
                        chart: returnGraph()
                    };
                    $scope.containerTypesOfReporter.chart.title = "Type of reporter";
                    $scope.containerTypesOfReporter.chart.noData = "No cases found";
                }
            });
        };


        $scope.printGraphs = function (elem, title) {
            var printHtml = "";

            if (Array.isArray(elem)) {
                elem.forEach(function (element) {
                    printHtml += document.querySelector(element).innerHTML;
                });
            } else {
                printHtml = document.getElementById(elem).innerHTML;
            }

            setTimeout(function () {
                var mywindow = window.open('', 'PRINT', 'width=' + screen.availWidth + ',height=' + screen.availHeight);
                mywindow.document.write('<html><head><title>' + title + '</title>');
                mywindow.document.write('<link rel="stylesheet" href="/Content/analiticsDashboard.css" type="text/css"/>');
                mywindow.document.write('<link rel="stylesheet" href="/Content/analiticsDashboardPrint.css" type="text/css"/>');
                mywindow.document.write('</head><body onload="window.print(); window.close()">');
                //mywindow.document.write('</head><body>');
                //mywindow.document.write('<h1>' + title + '</h1>');
                mywindow.document.write('<div class="container">');
                mywindow.document.write(document.getElementById("templateForPrinting").innerHTML);
                mywindow.document.write(printHtml);
                mywindow.document.write('</div></body></html>');
                mywindow.document.close(); // necessary for IE >= 10
                mywindow.focus(); // necessary for IE >= 10*/

                return true;
            }, 250);
        }
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
            //barData = barData.sort();
            barData.forEach(function (element) {
                previousElement = previousElement + element;
                anotherBar.push([previousElement]);
            });
            $scope.chart1 = c3.generate({
                bindto: '#responseTime',
                data: {
                    columns: columnData,
                    type: 'bar',
                    labels: false,
                    groups: [['New Report', 'Report Review','Under Inves', 'Awaiting Sign-Off']],
                    order: null,
                    colors: {
                        'New Report': '#d47472',
                        'Report Review': '#ff9b42',
                        'Under Inves': '#3099be',
                        'Awaiting Sign-Off': '#64cd9b'
                    },
                },
                point: {
                    show: false
                },
                bar: {
                    width: {
                        ratio: 1 // this makes bar width 50% of length between ticks,
                    },
                    // or
                    //width: 100 // this makes bar width 100px
                },
                axis: {
                    rotated: true,
                    x: {
                        type: 'category',
                        padding: 0
                    }, 
                    y: {
                        tick: {
                            values: anotherBar,
                            outer: false,
                        },
                        padding: 0
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
                $http({
                    method: 'POST', data: {
                        "ReportsSecondaryTypesIDStrings": arraySelectedItems.reporter_type.join(),
                        "ReportsRelationTypesIDStrings": arraySelectedItems.incident_type.join(),
                        "ReportsDepartmentIDStringss": arraySelectedItems.department.join(),
                        "ReportsLocationIDStrings": arraySelectedItems.location.join(),
                        "data_range": arraySelectedItems.data_range,
                        "dateStart": arraySelectedItems.dateStart,
                        "dateEnd": arraySelectedItems.dateEnd
                    }, url: '/Analytics/CompanyDepartmentReportAdvanced'
                })
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
                        if (element.percentage == 0) {
                            element.percentage = 0.01;
                        }
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

    app.directive('clickcalendar', function () {
        return {
            restrict: 'A',
            link: function (scope, element) {
                element.bind('click', function (e) {
                    angular.element(document.querySelector('#calendarCustomDate')).triggerHandler('click');
                });
            }
        };
    });
}());
