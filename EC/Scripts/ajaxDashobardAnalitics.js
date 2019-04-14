
(function () {

    'use strict';
    var app = angular.module('EC', ['nvd3']);



    app.controller('MenuCases', function ($scope, getMenuFilterCases) {
        var promiseObj = getMenuFilterCases.getData();
        promiseObj.then(function (response) {
            $scope.MenuCases = response.data;


            //$(".case-DropDownList-title").click(function () {

            //    $(".case-DropDownList-title").not(this).next(".case-DropDownList-contentBlock").hide("fast");
            //    $(".case-DropDownList-title").not(this).parent(".case-TotalHeader__items").removeClass("selected");

            //    $(this).next(".case-DropDownList-contentBlock").toggle("fast");
            //    $(this).parent(".case-TotalHeader__items").toggleClass("selected");
            //});

        });
    });
    app.controller('Today_spanshot', function ($scope, AnalyticsByDate) {
        var promiseObj = AnalyticsByDate.getData();
        promiseObj.then(function (response) {
            $scope._today_spanshot = response.data._today_spanshot;
        });
    });
    app.controller('CaseTime', function ($scope) {

    });
    app.controller('BodyController', function ($scope, $http, addPercentageRoundGraph) {

        $http({ method: 'GET', url: '/api/AnalyticsDashboardAPI' }).
            then(function successCallback(response) {
                
                //console.log('AnalyticsRootCauseAnalysis?secondaryType=0', response.data);

                $scope.chartData1 = addPercentageRoundGraph.setPercentage(response.data.Behavioral);
                $scope.chartData2 = addPercentageRoundGraph.setPercentage(response.data.External);
                $scope.chartData3 = addPercentageRoundGraph.setPercentage(response.data.Organizational);

                $scope.chartColors = ['#3099be', '#ff9b42', '#868fb8', '#64cd9b', '#ba83b8', '#c6c967', '#73cbcc', '#d47472'];
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
                        showLabels: false,
                        color: $scope.chartColors,
                        duration: 500,
                        labelSunbeamLayout: true,
                        showLegend: false,
                    };
                    return chart;
                }

                if ($scope.chartData1.length == 0) {
                    $scope.behavioralFactorsValues = true;
                } else {
                    $scope.behavioralFactors = {
                        chart: returnGraph()
                    };
                    $scope.behavioralFactors.chart.title = "Behavioral Factors";
                    $scope.behavioralFactors.chart.noData = "No cases found";
                }

                if ($scope.chartData2.length == 0) {
                    $scope.externalInfluencesValues = true;
                } else {
                    $scope.externalInfluences = {
                        chart: returnGraph()
                    };
                    $scope.externalInfluences.chart.title = "External Influences";
                    $scope.externalInfluences.chart.noData = "No cases found";
                }

                if ($scope.chartData3.length == 0) {
                    $scope.organizationalInfluencesValues = true;
                } else {
                    $scope.organizationalInfluences = {
                        chart: returnGraph()
                    };
                    $scope.organizationalInfluences.chart.title = "Organizational Influences";
                    $scope.organizationalInfluences.chart.noData = "No cases found";
                }

            }, function errorCallback(response) {
                console.log('error');
            });
    });

    
    app.controller('CasesController', function ($scope, getCasesService, addPercentageRoundGraph) {

        var promiseObj = getCasesService.getData();
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

                    //valueFormat: function (d) {
                    //    d3.format('.3s');
                    //},
                    height: 500,
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
    });

    app.controller('CaseManagamentTime', function ($scope, getTurnAroundTime) {
        var promiseObj = getTurnAroundTime.getData();
        promiseObj.then(function (response) {
            $scope.turnaroundTime = response.data.resultAroundTime;
            $scope.CaseManagamentTime = response.data.CaseManagamentTime;
            var columnData = [];
            $scope.CaseManagamentTime.forEach(function (element) {
                columnData.push([element.Name, element.value]);
            });
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
                        ratio: 0.35 // this makes bar width 50% of length between ticks
                    }
                    // or
                    //width: 100 // this makes bar width 100px
                },
                axis: {
                    rotated: true,
                    x: {
                        type: 'category',
                    },
                },
                size: {
                    height: 100
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
            getData: function (companyId, userId) {
                var deffered = $q.defer();
                $http({ method: 'POST', data: { companyId: companyId, userId: userId }, url: '/Analytics/CompanyDepartmentReportAdvanced' })
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
