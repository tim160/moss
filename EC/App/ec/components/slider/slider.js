'use strict';
csmartApp
    .directive("slider", function () {
        return {
            scope: {
                text: '@slider'
            },
            templateUrl: 'App/csmart/components/slider/slider.cshtml',
            controller: ['$scope', '$injector',function ($scope, $injector) {
                //Inheritance
                $injector.invoke(FireEventsController, this, { $scope: $scope });
                $injector.invoke(ParentController, this, { $scope: $scope });

                $scope.name = "slider";

                //Events
                $scope.events = $scope.events.concat([
                    "limit"
                ]);


                //Functions
                $scope.init = function (style, textOverride) {
                    $scope.style = style;

                    if (textOverride) {
                        //if you'd like to override the text specified in the view
                        $scope.text = textOverride;
                    }
                };

                $scope.resize = function (ui) {
                    $scope.children("resizable").resize(ui);
                };

                var initialize = function () {
                    //get the gridwidth from the parent to pass to the resizable
                    $scope.gridWidth = $scope.$parent.gridWidth;

                    $scope.listen("register", function (data) {
                        data.child.listen("update", function (data) {
                            $scope.fire("limit", data);
                        });
                    });

                    $scope.$parent.register($scope.name, $scope);
                };
                initialize();
            }]
        };
    });