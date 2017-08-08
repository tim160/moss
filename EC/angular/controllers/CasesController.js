(function () {

    'use strict';

    angular
        .module('EC')
        .controller('CasesController',
            ['$scope', '$filter', 'CasesService', CasesController]);

    function CasesController($scope, $filter, CasesService) {
        $scope.showAsGrid = true;
        $scope.mode = 1;
        $scope.reports = [];

        $scope.counts = {
            Active: 0,
            Completed: 0,
            Spam: 0,
            Closed: 0,
        };

        $scope.refresh = function (mode, preload) {
            CasesService.get({ ReportFlag: mode, Preload: preload }, function (data) {
                for (var i = 0; i < data.Reports.length; i++) {
                    var r = $filter('filter')(data.ReportsAdv, { 'id': data.Reports[i].report_id });
                    data.Reports[i].AdvInfo = r;

                    var r = $filter('filter')(data.Users, { 'id': data.Reports[i].last_sender_id });
                    r = r.length === 0 ? null : r[0];
                    if (r === null) {
                        r = {
                            photo_path: '/Content/Icons/noPhoto.png',
                            first_nm: '',
                            last_nm: '',
                        };
                    }
                    r.sb_full_name = (r.first_nm + ' ' + r.last_nm).replace(' ', '_');
                    data.Reports[i].Last_sender = r;

                    var r = $filter('filter')(data.Users, { 'id': data.Reports[i].previous_sender_id });
                    r = r.length === 0 ? null : r[0];
                    if (r === null) {
                        r = {
                            photo_path: '/Content/Icons/noPhoto.png',
                            first_nm: '',
                            last_nm: '',
                        };
                    }
                    r.sb_full_name = (r.first_nm + ' ' + r.last_nm).replace(' ', '_');
                    data.Reports[i].Previous_sender = r;
                }

                $scope.reports = data.Reports;
                $scope.mode = data.Mode;
                $scope.counts = data.Counts;
            });
        };
        $scope.refresh($scope.mode);

        $scope.openCase = function (id) {
            window.location ='/Case/Index/' + id;
        };
    }
}());
