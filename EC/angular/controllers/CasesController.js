﻿(function () {

    'use strict';

    angular
        .module('EC')
        .controller('CasesController',
            ['$scope', '$filter', '$location', 'orderByFilter', 'CasesService', CasesController]);

    function CasesController($scope, $filter, $location, orderByFilter, CasesService) {
        $scope.showAsGrid = true;
        $scope.mode = 1;
        $scope.reports = [];
        $scope.sortColumn = 'case_number';
        $scope.sortColumnDesc = true;
        var tab = $filter('parseUrl')($location.$$absUrl, 'mode').toLowerCase();
        $scope.mode = tab === 'active' ? 1 : $scope.mode;
        $scope.mode = tab === 'completed' ? 2 : $scope.mode;
        $scope.mode = tab === 'spam' ? 3 : $scope.mode;
        $scope.mode = tab === 'new' ? 4 : $scope.mode;
        $scope.mode = tab === 'closed' ? 5 : $scope.mode;


        var titles = ['', 'Active Cases', 'Cases Awaiting Sign-off', 'Spam Cases', 'New Reports', 'Closed Cases'];
        $('title').html(titles[$scope.mode]);

        $scope.counts = {
            Active: 0,
            Completed: 0,
            Spam: 0,
            Closed: 0,
        };

        $scope.sortClass = function (column) {
            if (column === $scope.sortColumn) {
                return !$scope.sortColumnDesc ? 'sortArrow_down' : 'sortArrow_up';
            }
            return '';
        };

        $scope.severityClass = function (report) {
            if (report.severity_id === 2) {
                return 'dashboard-col__severity-textLow';
            }
            if (report.severity_id === 5) {
                return 'dashboard-col__severity-textCritical';
            }
            if (report.severity_id === 3) {
                return 'dashboard-col__severity-textMedium';
            }
            if (report.severity_id === 4) {
                return 'dashboard-col__severity-textHigh';
            }
            return '';
        };

        $scope.isOwner = function (report, mediator) {
            for (var i = 0; i < report.owners.length; i++) {
                if (report.owners[i].user_id === mediator.id) {
                    return true;
                }
            }
            return false;
        };

        $scope.refresh = function (mode, preload) {
            CasesService.get({ ReportFlag: mode, Preload: preload }, function (data) {
                $('.headerBlockTextRight > span').text(data.Title);

                $scope.reports = data.Reports;
                $scope.reports.forEach(function (element) {
                    element.last_update_dt = new Date(element.last_update_dt);
                });
                $scope.mode = data.Mode;
                $scope.counts = data.Counts;
                $scope.Companies = data.Companies;
            });
        };
        $scope.refresh($scope.mode);

        $scope.openCase = function (id) {
            if ($scope.mode === 3 || $scope.mode === 4) {
                window.location = '/NewReport/' + id;
            } else {
                window.location = '/newCase/Index/' + id;
            }
        };

        $scope.sort = function (column) {
            if (column === $scope.sortColumn) {
                $scope.sortColumnDesc = !$scope.sortColumnDesc;
            } else {
                $scope.sortColumn = column;
                $scope.sortColumnDesc = false;
            }
            $scope.reports = orderByFilter($scope.reports, $scope.sortColumn, $scope.sortColumnDesc);

        };
        $scope.ddListClickedCompany = function (company_id, company_name) {
            if (company_name === undefined) {
                $scope.displayCompanyName = document.querySelector('#ddListDefaultValue').value;
                $scope.filterValue = null;
            } else {
                $scope.filterValue = company_id;
                $scope.displayCompanyName = company_name;
            }
        };
        $scope.filterValue = null;
        $scope.returnListReports = function () {
            $scope.filterValue = null;
        };
        $scope.casesFilterFunction = function (item) {
            if ($scope.filterValue != null) {
                return item.company_id === $scope.filterValue;
            } else {
                return item;
            }
        };


        $scope.showDDMenu = false;
        $scope.displayCompanyName = document.querySelector('#ddListDefaultValue').value;
    }
}());
