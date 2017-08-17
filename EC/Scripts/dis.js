(function () {
    ROOT = $('base').attr('href');

    'use strict';

    angular.module('EC', [
        'ngResource',

        'EC',
    ]);

    angular.module('EC').config([
        function () {
        }])
    .run(function () {
    });

    angular.module('EC').filter('getBy', function () {
        return function (arr, prop, val, valprop, array) {
            if (arr === undefined) {
                return null;
            }
            if (array) {
                array = [];
            }
            var i = 0, len = arr.length;
            for (; i < len; i++) {
                var cval;
                if ((prop === undefined) | (prop === '')) {
                    cval = arr[i];
                } else {
                    cval = arr[i][prop];
                }
                if (cval === val) {
                    if (valprop !== undefined) {
                        if (array !== undefined) {
                            array.push(arr[i][valprop]);
                        } else {
                            return arr[i][valprop];
                        }
                    } else {
                        if (array !== undefined) {
                            array.push(arr[i][valprop]);
                        } else {
                            return arr[i];
                        }
                    }
                }
            }
            if (array !== undefined) {
                return array;
            }
            return null;
        };
    });

})();


(function () {

    'use strict';

    angular
        .module('EC')
        .controller('CasesController',
            ['$scope', '$filter', 'orderByFilter', 'CasesService', CasesController]);

    function CasesController($scope, $filter, orderByFilter, CasesService) {
        $scope.showAsGrid = true;
        $scope.mode = 1;
        $scope.reports = [];
        $scope.sortColumn = 'case_number';
        $scope.sortColumnDesc = true;

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
                    if ((r != null) && (r.length > 0)) {
                        data.Reports[i].AdvInfo = r[0];
                        data.Reports[i].total_days = r[0].total_days;
                        data.Reports[i].case_dt_s = r[0].case_dt_s;
                        data.Reports[i].cc_is_life_threating = r[0].cc_is_life_threating;
                    }

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

        $scope.sort = function (column) {
            if (column === $scope.sortColumn) {
                $scope.sortColumnDesc = !$scope.sortColumnDesc;
            } else {
                $scope.sortColumn = column;
                $scope.sortColumnDesc = false;
            }

            $scope.reports = orderByFilter($scope.reports, $scope.sortColumn, $scope.sortColumnDesc);
            console.log($scope.reports);
        };
    }
}());

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('EmployeeAwarenessController',
            ['$scope', '$filter', 'EmployeeAwarenessService', EmployeeAwarenessController]);

    function EmployeeAwarenessController($scope, $filter, EmployeeAwarenessService) {
        $scope.posters = [];
        $scope.categories = [];
        $scope.messages = [];
        $scope.languages = [];
        $scope.avaibleFormats = [];

        EmployeeAwarenessService.get({}, function (data) {
            $scope.posters = data.posters;
            $scope.categories = data.categories;
            $scope.messages = data.messages;
            $scope.avaibleFormats = data.avaibleFormats;
            $scope.languages = data.languages;
        });

        $scope.categoryName = function (category) {
            if (category === null) {
                return '';
            }
            if (typeof (category) === 'object') {
                return category.industry_posters_en;
            } else {
                return $scope.categoryName($filter('getBy')($scope.categories, 'id', category));
            }
        };

        $scope.categoryById = function () {
            return category.industry_posters_en;
        };

        $scope.messageName = function (message) {
            return message.message_posters_en;
        };

        $scope.selectedItemsCount = function () {
            var count = 0;

            var isFilter = false;
            angular.forEach($scope.categories, function (category) {
                if (category.Selected) {
                    isFilter = true;
                }
            });
            angular.forEach($scope.messages, function (message) {
                if (message.Selected) {
                    isFilter = true;
                }
            });

            angular.forEach($scope.posters, function (poster) {
                if (!isFilter) {
                    poster.IsVisible = true;
                } else {
                    poster.IsVisible = false;

                    angular.forEach($scope.categories, function (category) {
                        if (category.Selected) {
                            if ($filter('getBy')(poster.posterCategoryNames, 'id', category.id) != null) {
                                poster.IsVisible = true;
                            }
                        }
                    });

                    angular.forEach($scope.messages, function (message) {
                        if (message.Selected) {
                            if (poster.poster.poster_message_posters_id === message.id) {
                                poster.IsVisible = true;
                            }
                        }
                    });
                }

                count += poster.IsVisible ? 1 : 0;
            });
            return count;
        };
    }
}());

(function () {

    'use strict';

    angular
        .module('EC')
        .controller('EmployeeAwarenessPosterController',
            ['$scope', '$filter', '$location', 'EmployeeAwarenessPosterService', EmployeeAwarenessPosterController]);

    function EmployeeAwarenessPosterController($scope, $filter, $location, EmployeeAwarenessPosterService) {
        $scope.SelectedSize = 1;
        $scope.SelectedLogo = 1;
        $scope.mainImage = '';
        $scope.id = parseInt($location.absUrl().substring($location.absUrl().indexOf('poster/') + 'poster/'.length));

        EmployeeAwarenessPosterService.get({ id: $scope.id }, function (data) {
            $scope.mainImage = data.mainImage;
        });

        $scope.Process = function () {
            console.log($scope.id, $scope.SelectedSize, $scope.SelectedLogo);
            EmployeeAwarenessPosterService.post({ type: $scope.id, size: $scope.SelectedSize, logo: $scope.SelectedLogo }, function (data) {
                window.location = data.file;
            });

            return true;
        };
    }
}());

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
        $scope.role = 5;
        $scope.departmentId = 0;

        $scope.val_first_nm = false;
        $scope.val_last_nm = false;
        $scope.val_title_ds = false;
        $scope.val_email = false;
        $scope.val_departmentId = false;

        $scope.id = parseInt($location.absUrl().substring($location.absUrl().indexOf('user/') + 'user/'.length));
        $scope.id = isNaN($scope.id) ? 0 : $scope.id;

        SettingsUserEditService.get({ id: $scope.id }, function (data) {
            $scope.first_nm = data.model.first_nm;
            $scope.last_nm = data.model.last_nm;
            $scope.title_ds = data.model.title_ds;
            $scope.email = data.model.email;
            $scope.departments = data.departments;
            $scope.role = data.model.role;
            $scope.departmentId = data.model.departmentId;
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

            if (!$scope.val_first_nm
                && !$scope.val_last_nm
                && !$scope.val_title_ds
                && !$scope.val_email
                && !$scope.val_departmentId
                ) {

                var model = {
                    id: $scope.id,
                    first_nm: $scope.first_nm,
                    last_nm: $scope.last_nm,
                    title_ds: $scope.title_ds,
                    email: $scope.email,
                    role_id: $scope.role,
                    company_department_id: $scope.departmentId,
                };
                SettingsUserEditService.post(model, function () {
                    if ($scope.id !== 0) {
                        window.location = window.location;
                    } else {
                        window.location = '/Settings/Mediators';
                    }
                });
            }
        };
    }
}());

(function () {

    'use strict';

    angular.module('EC')
        .service('CasesService', ['$resource', CasesService]);

    function CasesService($resource) {
        return $resource('/api/Cases', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();

(function () {

    'use strict';

    angular.module('EC')
        .service('EmployeeAwarenessPosterService', ['$resource', EmployeeAwarenessPosterService]);

    function EmployeeAwarenessPosterService($resource) {
        return $resource('/api/EmployeeAwarenessPoster', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
        });
    };
})();

(function () {

    'use strict';

    angular.module('EC')
        .service('EmployeeAwarenessService', ['$resource', EmployeeAwarenessService]);

    function EmployeeAwarenessService($resource) {
        return $resource('/api/EmployeeAwareness', {}, {
            get: { method: 'GET', params: {}, isArray: false },
        });
    };
})();

(function () {

    'use strict';

    angular.module('EC')
        .service('SettingsUserEditService', ['$resource', SettingsUserEditService]);

    function SettingsUserEditService($resource) {
        return $resource('/api/SettingsUserEdit', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
        });
    };
})();
