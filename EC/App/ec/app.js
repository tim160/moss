'use strict';
var csmartApp = angular.
    module("csmartApp", ['ngCookies', 'ngRoute', 'ngSanitize', 'ui.bootstrap', 'components', 'ui.validate', 'SafeApply']).
    config(['$routeProvider',
        function ($routeProvider) {
            $routeProvider
                .otherwise({
                    reloadOnSearch: false,
                    templateUrl: 'App/csmart/components/schedule/schedule.cshtml',
                    controller: 'ScheduleController',
                });
        }]).
    constant('resourceNone', 'RESOURCE_NONE').
    constant('resourcePostponed', 'RESOURCE_POSTPONED').
    //Role Enum - copied from the service definition MarineLMSExtensions.Constants.Roles
    constant('roleEnum', {
        CSmartAdmin: 0,
        CourseAdmin: 1,
        LineAdmin: 2,
        ExternalLineAdmin: 3,
        Viewer: 4,
        None: 5
    }).
    //Booking Type Enum - copied from the service definition MarineLMSExtensions.Constants.ResourceBookingTypes
    constant('resourceBookingTypeEnum', {
        Default: 0,
        None: 1,
        Postponed: 2,
        External: 3
    }).
constant('userContractTypeEnum', {
    Employee: 1,
    Contract: 2,
    Both: 3
});