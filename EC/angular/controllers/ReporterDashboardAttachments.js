(function () {

    'use strict';

    angular
        .module('EC')
        .controller('ReporterDashboardAttachments',  ['$scope', 'Upload', '$timeout', ReporterDashboardAttachments]);

    function ReporterDashboardAttachments($scope, Upload, $timeout) {
        $scope.$watch('files', function () {
            $scope.upload($scope.files);
        });
        $scope.$watch('file', function () {
            if ($scope.file != null) {
                $scope.files = [$scope.file];
            }
        });
        $scope.log = '';
        var report_id = angular.element( document.querySelector( '#report_id' ) ).val();
        var mode = 'upload_rd';

        $scope.upload = function (files) {
            if (files && files.length) {
                var itemsProcessed = 0;
                for (var i = 0; i < files.length; i++) {
                    var file = files[i];
                    if (!file.$error) {
                        var fsizemb = file.size;
                        fsizemb = fsizemb / 1024;
                        fsizemb = fsizemb / 1024;
                        fsizemb = fsizemb.toFixed(3);
                        if (fsizemb > 4) {
                            angular.element('#openModalForFileSize').click();
                        } else {
                            Upload.upload({
                                url: '/api/ReporterDashboardAttachment',
                                data: {
                                    report_id: report_id,
                                    mode: mode,
                                    file: file
                                }
                            }).then(function (resp) {
                                itemsProcessed++;
                                if(files.length === itemsProcessed) {
                                    location.reload();
                                }
                                $timeout(function() {
                                });
                            }, null, function (evt) {
                            });
                        }
                    }
                }
            }
        };
    }
}());
