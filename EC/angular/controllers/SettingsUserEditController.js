(function () {

    'use strict';

    angular
        .module('EC')
        .controller('SettingsUserEditController',
            ['$scope', '$filter', '$location', 'SettingsUserEditService', SettingsUserEditController]);

    function SettingsUserEditController($scope, $filter, $location, SettingsUserEditService) {
        $scope.first_nm = '';
        $scope.last_nm = '';
        $scope.title_ds = '';
        $scope.email = '';
        $scope.departments = [];
        $scope.locations = [];
        $scope.role = 5;
        $scope.departmentId = 0;
        $scope.locationId = 0;
        $scope.user_permissions_approve_case_closure = 0;
        $scope.user_permissions_change_settings = 0;
        $scope.status_id = 3;
        $scope.user_role = 0;

        $scope.val_first_nm = false;
        $scope.val_last_nm = false;
        $scope.val_title_ds = false;
        $scope.val_email = false;
        $scope.val_departmentId = false;
        $scope.val_locationId = false;

        $scope.id = parseInt($location.absUrl().substring($location.absUrl().indexOf('user/') + 'user/'.length));
        $scope.id = isNaN($scope.id) ? 0 : $scope.id;

        SettingsUserEditService.get({ id: $scope.id }, function (data) {
            $scope.first_nm = data.model.first_nm;
            $scope.last_nm = data.model.last_nm;
            $scope.title_ds = data.model.title_ds;
            $scope.email = data.model.email;
            $scope.departments = data.departments;
            $scope.locations = data.locations;
            $scope.role = data.model.role;
            $scope.departmentId = data.model.departmentId;
            $scope.status_id = data.model.status_id;
            $scope.locationId = data.model.locationId;
            $scope.user_permissions_approve_case_closure = data.model.user_permissions_approve_case_closure.toString();
            $scope.user_permissions_change_settings = data.model.user_permissions_change_settings.toString();
            $scope.user_role = data.user.role;
        });

        $scope.validate = function (value, rv) {
            if (rv === undefined) {
                if ((value === null) || (value === undefined) || (value.trim() === '')) {
                    return false;
                }
            } else {
                if (value.trim() === '' || !rv.test(value.trim())) {
                    return false;
                }
            }
            return true;
        };

        $scope.post = function () {
            $scope.val_first_nm = !$scope.validate($scope.first_nm);
            $scope.val_last_nm = !$scope.validate($scope.last_nm);
            $scope.val_title_ds = !$scope.validate($scope.title_ds);
            $scope.val_email = !$scope.validate($scope.email, /^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$/);
            $scope.val_departmentId = $scope.departmentId == null || !$scope.validate($scope.departmentId.toString());
            $scope.val_locationId = $scope.locationId == null || !$scope.validate($scope.locationId.toString());

            if (!$scope.val_first_nm
                && !$scope.val_last_nm
                && !$scope.val_title_ds
                && !$scope.val_email
                && !$scope.val_departmentId
                && !$scope.val_locationId
                ) {

                var model = {
                    id: $scope.id,
                    first_nm: $scope.first_nm,
                    last_nm: $scope.last_nm,
                    title_ds: $scope.title_ds,
                    email: $scope.email,
                    role_id: $scope.role,
                    company_department_id: $scope.departmentId,
                    company_location_id: $scope.locationId,
                    user_permissions_approve_case_closure: $scope.user_permissions_approve_case_closure,
                    user_permissions_change_settings: $scope.user_permissions_change_settings,
                    status_id: $scope.status_id,
                };
                SettingsUserEditService.post(model, function (data) {
                    if (data.ok) {
                        if ($scope.id !== 0) {
                            window.location = window.location;
                        } else {
                            window.location = '/Settings/Mediators';
                        }
                    } else {
                        alert(data.message);
                    }
                });
            }
        };
    }
}());
