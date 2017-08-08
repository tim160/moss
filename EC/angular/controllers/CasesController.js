(function () {

    'use strict';

    angular
        .module('EC')
        .controller('CasesController',
            ['$scope', '$filter', 'CasesService', CasesController]);

    function CasesController($scope, $filter, CasesService) {
        $scope.showAsGrid = true;
        //$scope.showAsGrid = false;
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
                //WARNING DEBUG
                data.Reports[0].investigation_status_number = 3;
                data.Reports[1].investigation_status_number = 9;
                data.Reports[2].investigation_status_number = 4;
                data.Reports[3].investigation_status_number = 5;
                data.Reports[4].investigation_status_number = 6;
                data.Reports[5].investigation_status_number = 7;
                //WARNING DEBUG

                console.log(data.Users);

                for (var i = 0; i < data.Reports.length; i++) {
                    var r = $filter('filter')(data.ReportsAdv, { 'id': data.Reports[i].report_id });
                    data.Reports[i].AdvInfo = r;

                    var r = $filter('filter')(data.Users, { 'id': data.Reports[i].last_sender_id });
                    r = r.length === 0 ? null : r[0];
                    if (r === null) {
                        console.log(1, data.Reports[i].last_sender_id, r);
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
    }
}());
