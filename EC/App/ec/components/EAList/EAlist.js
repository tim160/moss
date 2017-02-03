'use strict';

csmartApp
.directive('listEA', function () {
    return {
        scope: {},
        templateUrl: 'App/csmart/components/listEA/listEA.cshtml',

        controller: ['$scope', '$timeout', '$injector', 'csmartDataService', function ($scope, $timeout, $injector, csmartDataService) {
            /// <summary>
            /// Events:
            /// </summary>
            /// <param name="$scope" type="Object"></param>
            /// <param name="$timeout" type="Object"></param>
            /// <param name="csmartDataService" type="Object"></param>
            /// <param name="$injector" type="Object"></param>


            //Inheritance
            $injector.invoke(ParentController, this, { $scope: $scope });
            $injector.invoke(FireEventsController, this, { $scope: $scope });

            $scope.components =
                ["listEAFilter", "listEAMain"];

            //State
            $scope.show = false;

            $scope.name = "listEA";

            //Functions
            $scope.load = function () {
                //prep the report page
                $scope.show = true;
                $scope.children("listEAFilter").load();
                $scope.children("listEAMain").load();
            };

            $scope.csmart = csmartDataService;

            var initialize = function () {
                $scope.$parent.register("listEA", $scope);

                $scope.listen("register", function (data) {
                    switch (data.type) {
                        case "listEAFilter":
                            data.child.listen("filterChanged", function (filter) {
                                $scope.children("listEAMain").fetchCourseEntries($scope.children("listEAFilter").filter);
                            });
                            break;
                    }
                });
            };
            initialize();

        }]
    };
});