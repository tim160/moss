
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

    app.controller('CasesController', function ($scope, getCasesService, addPercentageRoundGraph, getMenuFilterCases, AnalyticsByDate, HolderAdditionalCompanies) {
        var companyIdArray = Array();
        $scope.additionalCompanies = Array();
        $scope.showDDMenu = false;
        $scope.displayCompanyName = document.querySelector("#ddListDefaultValue").value;

        HolderAdditionalCompanies.setAdditionalCompanies();
        HolderAdditionalCompanies.getAdditionalCompaniesFactory().then(function (response) {
            $scope.additionalCompanies = response.data.additionalCompanies;
            companyIdArray = $scope.additionalCompanies.map((item) => { return item.id; });
            putDataToPage(companyIdArray);
        });

        function putDataToPage(companyIdArray) {
            makeTodaySnapshot(AnalyticsByDate, $scope, companyIdArray);
            makeMenuWithFilter(getMenuFilterCases, $scope, companyIdArray);
            updateGraph(getCasesService, $scope, arraySelectedItems, companyIdArray, addPercentageRoundGraph);
        }


        $scope.datePicker = {
            date: { startDate: null, endDate: null },
            options: {
                "showDropdowns": true,
                "autoApply": true,
                "opens": "left",
                ranges: {
                    'All': [moment().subtract(9, 'years'), moment()],
                    'Last 12 Months': [moment().subtract(12, 'months'), moment()],
                    'Last 6 Months': [moment().subtract(6, 'months'), moment()],
                    'Last 3 Months': [moment().subtract(3, 'months'), moment()],
                    'Last 30 days': [moment().subtract(30, 'days'), moment()],
                    'Last 7 days': [moment().subtract(7, 'days'), moment()]
                },
                "linkedCalendars": false,
                "autoUpdateInput": false,
                "showCustomRangeLabel": false,
                "alwaysShowCalendars": true,
                locale: {
                    cancelLabel: 'Clear'
                }
            }, function(start, end, label) {
                console.log('New date range selected: ' + start.format('YYYY-MM-DD') + ' to ' + end.format('YYYY-MM-DD') + ' (predefined range: ' + label + ')');
            }
        };
        $scope.$watch('datePicker.date', function (newDate) {
            arraySelectedItems.dateStart = newDate.startDate;
            arraySelectedItems.dateEnd = newDate.endDate;
            if (newDate.label == 'All') {
                angular.element('#selectedCasesDateRange').html("");
                angular.element('#calendarCustomDate').val('');
                angular.element(".daterangepicker .cancelBtn").click();
            } else if (newDate.startDate != undefined && newDate.endDate != undefined) {
                angular.element('#selectedCasesDateRange').html(": <span class='bold'>Start Date: </span>" + moment(newDate.startDate).format("MMMM D, YYYY") + " <span class='bold'>End Date: </span>" + moment(newDate.endDate).format("MMMM D, YYYY"));
            } else if (newDate.startDate != undefined) {
                $scope.selectedCasesDateRange = ": Start Date: " + moment(newDate.startDate).format("MMMM D, YYYY")
            } else if (newDate.endDate != undefined) {
                $scope.selectedCasesDateRange = ": End Date: " + moment(newDate.endDate).format("MMMM D, YYYY")
            }
        }, false);

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
        /* drop down list functionality */
        $scope.ddListClickedCompany = function (company_id, company_name) {
            if (company_name == undefined) {
                $scope.displayCompanyName = document.querySelector("#ddListDefaultValue").value;
                $scope.filterValue = null;
                putDataToPage(companyIdArray);
            } else {
                $scope.filterValue = company_id;
                $scope.displayCompanyName = company_name;
                putDataToPage([company_id]);
            }
        }

        /* end drop down list functionality */

        $scope.filterValue = null;
        $scope.returnListReports = function () {
            $scope.filterValue = null;
        }
        $scope.casesFilterFunction = (item) => {
            if ($scope.filterValue != null) {
                return item.company_id == $scope.filterValue;
            } else {
                return item;
            }
        };

        $scope.showDDlist = false;
        $scope.displayCompanyName = document.querySelector("#ddListDefaultValue").value;

    });

    app.controller('CaseManagamentTime', function ($scope, getTurnAroundTime, HolderAdditionalCompanies, responseTimeSettingsByStage) {
        var columnData = [];
        var barData = [0];
        var anotherBar = [];
        var previousElement = 0;

        HolderAdditionalCompanies.getAdditionalCompaniesFactory().then(function (response) {
            var additionalCompanies = response.data.additionalCompanies;
            //Number Of Cases That Exceeded Turnaround Time
            var promiseObj = getTurnAroundTime.getData(additionalCompanies);
            promiseObj.then(function (TurnAroundTime) {

                $scope.turnaroundTime = TurnAroundTime.data.resultAroundTime;
                $scope.CaseManagamentTime = TurnAroundTime.data.CaseManagamentTime;

                $scope.CaseManagamentTime.forEach(function (element) {
                    columnData.push([element.Name, element.value]);
                    barData.push(element.value);
                });
                barData.forEach(function (element) {
                    previousElement = previousElement + element;
                    anotherBar.push([previousElement]);
                });
                //Response Time Settings By Stage
                $scope.chart1 = responseTimeSettingsByStage.graph(columnData, anotherBar, $scope.chartColors);
            });
        });
    });




    function makeTodaySnapshot(AnalyticsByDate, $scope, companyIdArray) {
        var analiticsObj = AnalyticsByDate.getData(companyIdArray);

        analiticsObj.then(function (response) {
            $scope._today_spanshot = response.data._today_spanshot;
        });
    }

    function makeMenuWithFilter(getMenuFilterCases, $scope, companyIdArray) {
        var promiseObjGetMenu = getMenuFilterCases.getData(companyIdArray);
        promiseObjGetMenu.then(function (response) {
            $scope.MenuCases = response.data;
            $scope.selectedCasesFilters = 0;
            $scope.selectedCasesFilterString = '';
            $scope.selectedItemClick = function ($event, clickedItemId, menu) {
                if (arraySelectedItems[menu].indexOf(clickedItemId) == -1) {
                    arraySelectedItems[menu].push(clickedItemId);
                    $scope.selectedCasesFilters++;
                } else {
                    arraySelectedItems[menu].splice(arraySelectedItems[menu].indexOf(clickedItemId), 1);
                    $scope.selectedCasesFilters--;
                }
                if ($scope.selectedCasesFilters > 0) {
                    $scope.selectedCasesFilterString = ': [' + $scope.selectedCasesFilters + ']';
                }
                else { $scope.selectedCasesFilterString = ''; }
                $event.currentTarget.classList.toggle('checked');
                //updateGraph();
            }
            $scope.dataRangeClick = function ($event, clickedItemId) {
                angular.element('#selectedCasesDateRange').html(": " + $event.target.textContent.trim());
                if (clickedItemId == 0) {
                    angular.element('#selectedCasesDateRange').html("");
                }
                arraySelectedItems["data_range"] = clickedItemId;
                //updateGraph();
            }
        });
    }

    function updateGraph(getCasesService, $scope, arraySelectedItems, companyIdArray, addPercentageRoundGraph) {
        var promiseObj = getCasesService.getData(arraySelectedItems, companyIdArray);
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
    function calculateAverage(arrayInt) {
        var sum = 0;
        sum += arrayInt.map(item => item);
        return sum / arrayInt.length;
    }

    app.factory('getCasesService', function ($http, $q) {
        return {
            getData: function (arraySelectedItems, companyIdArray) {
                var deffered = $q.defer();
                $http({
                    method: 'POST', data: {
                        "ReportsSecondaryTypesIDStrings": arraySelectedItems.reporter_type.join(),
                        "ReportsRelationTypesIDStrings": arraySelectedItems.incident_type.join(),
                        "ReportsDepartmentIDStringss": arraySelectedItems.department.join(),
                        "ReportsLocationIDStrings": arraySelectedItems.location.join(),
                        "data_range": arraySelectedItems.data_range,
                        "dateStart": arraySelectedItems.dateStart,
                        "dateEnd": arraySelectedItems.dateEnd,
                        "CompanyIDStrings": 2,
                        "companyIdArray": companyIdArray
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
            getData: function (comapanyIdArray) {
                var deffered = $q.defer();
                var strVithResult = '?';
                comapanyIdArray.map((item) => {
                    strVithResult += '&id=' + item;
                });
                $http({
                    method: 'POST', data: {},
                    url: '/api/AnalyticsDashboardAPI/GetMenuDashboard/' + strVithResult
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
    app.factory('AnalyticsByDate', function ($http, $q) {
        return {
            getData: function (comapanyIdArray) {
                var strVithResult = '?';
                comapanyIdArray.map((item) => {
                    strVithResult += '&id=' + item;
                });
                var deffered = $q.defer();
                $http({
                    method: 'GET',
                    url: '/api/AnaliticsDashboardCases/AnalyticsByDate/' + strVithResult
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
            getData: function (arrayAdditionalCompanyId) {
                debugger;
                var deffered = $q.defer();
                $http({
                    method: 'POST',
                    url: '/api/AnaliticsDashboardCases/getTurnAroundTime',
                    params: arrayAdditionalCompanyId
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
    app.factory('getAdditionalCompanies', function ($http, $q) {
        return {
            getData: function (id) {
                var deffered = $q.defer();
                $http({
                    method: 'GET',
                    url: '/api/AdditionalCompanies/' + id
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

    app.factory('HolderAdditionalCompanies', function (getAdditionalCompanies) {
        var AdditionalCompanies = [];

        var setAdditionalCompanies = function () {
            AdditionalCompanies = getAdditionalCompanies.getData(document.querySelector("#company_id").value);
        }

        var getAdditionalCompaniesFactory = function () {
            return AdditionalCompanies;
        };

        return {
            setAdditionalCompanies: setAdditionalCompanies,
            getAdditionalCompaniesFactory: getAdditionalCompaniesFactory
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
                    var daterange = document.querySelector('#calendarCustomDate');
                    daterange.style.display = "block";
                    angular.element(daterange).triggerHandler('click');
                });
            }
        };
    });
    //responseTimeSettingsByStage
    app.factory('responseTimeSettingsByStage', function () {
        return {
            graph: function (columnData, anotherBar, chartColors) {
                c3.generate({
                    bindto: '#responseTime',
                    data: {
                        columns: columnData,
                        type: 'bar',
                        labels: false,
                        groups: [['New Report', 'Report Review', 'Under Inves', 'Awaiting Sign-Off']],
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
                    color: chartColors,
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
            }
        }
    });
}());
