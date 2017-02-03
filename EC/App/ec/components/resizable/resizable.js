'use strict';
csmartApp
    .directive('resizable', function () {
        return {
            scope: {},
            //Transclude to include template from the parent
            transclude: true,
            template: "<div class='inner' ng-transclude ></div>",
            controller: ['$scope', '$injector',function ($scope, $injector) {
                //State
                $scope.gridWidth = $scope.$parent.gridWidth;

                //Inheritance
                $injector.invoke(FireEventsController, this, { $scope: $scope });
                $injector.invoke(ParentController, this, { $scope: $scope });

                $scope.name = "resizable";

                //Events
                $scope.events = $scope.events.concat([
                    "update"
                ]);

                //Functions
                //$scope.resize Defined in Link
                //$scope.limit Defined in Link

                //Initialize
                var initialize = function () {
                    $scope.$parent.register($scope.name, $scope);
                };
                initialize();
            }],
            link: function ($scope, $element, $attrs) {
                //Private State
                var el = $($element);
                var p = el.parent();
                var oldLeft = el.position().left;
                var oldWidth = el.width();

                //Functions
                $scope.resize = function (ui) {
                    oldLeft = ui.current.position;
                    oldWidth = ui.current.size;
                    el.css({
                        left: oldLeft,
                        width: oldWidth
                    });
                };

                $scope.limit = function (ui) {
                    //get the current state
                    oldLeft = el.position().left;
                    oldWidth = el.width();

                    var updated = false;
                    var compatibilityOffset = 2;
                    var minLeft = ui.old.position - compatibilityOffset;
                    var maxLeft = ui.old.position + compatibilityOffset;

                    var minRight = (ui.old.position + ui.old.size) - compatibilityOffset;
                    var maxRight = (ui.old.position + ui.old.size) + compatibilityOffset;

                    var hadSameLeft = (oldLeft >= minLeft && oldLeft <= maxLeft);
                    var hadSameRight = (((oldWidth + oldLeft) >= minRight) && ((oldWidth + oldLeft) <= maxRight));

                    //get 
                    if (hadSameRight) {
                        //sticky right side
                        var limitRight = ui.current.position + ui.current.size;
                        var currentRight = oldWidth + oldLeft;
                        oldWidth = limitRight - oldLeft;
                        updated = true;
                    }


                    if (hadSameLeft) {
                        //sticky left side
                        oldLeft = ui.current.position;
                        var positionDif = ui.current.position - ui.old.position;
                        oldWidth = oldWidth - positionDif;

                        updated = true;
                    }


                    if (updated) {
                        var data = {
                            index: parseInt($attrs.index, 10),
                            //new
                            current: {
                                position: oldLeft,
                                size: oldWidth
                            }
                        };


                        el.css({
                            left: oldLeft,
                            width: oldWidth
                        });

                        //notify that the was a change in the time
                        $scope.fire("update", data);
                    }
                };


                //Initialization
                el.draggable({
                    axis: 'x',
                    containment: p,
                    grid: [$scope.gridWidth],
                    start: function () {

                        oldLeft = el.position().left;
                        oldWidth = el.width();

                    },
                    drag: function (event, ui) {
                        //
                        var data = {
                            index: parseInt($attrs.index, 10),
                            //old
                            old: {
                                position: oldLeft,
                                size: oldWidth
                            },
                            //new
                            current: {
                                position: ui.position.left,
                                size: oldWidth
                            }
                        };
                        $scope.$apply(function () {
                            $scope.fire("update", data);
                        });
                        oldLeft = ui.position.left;
                    }
                }).
                resizable({
                    handles: "e, w",
                    containment: p,
                    grid: [$scope.gridWidth],
                    minWidth: $scope.gridWidth,
                    start: function () {

                        oldLeft = el.position().left;
                        oldWidth = el.width();

                    },
                    resize: function (event, ui) {

                        var data = {
                            index: parseInt($attrs.index, 10),
                            //old
                            old: {
                                position: oldLeft,
                                size: oldWidth
                            },
                            //new
                            current: {
                                position: ui.position.left,
                                size: ui.size.width
                            }
                        };
                        $scope.$apply(function () {
                            $scope.fire("update", data);
                        });
                        oldLeft = ui.position.left;
                        oldWidth = ui.size.width;
                    }
                });

            }
        };
    });