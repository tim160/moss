'use strict';
csmartApp
    .directive('selectable', function () {
        return {
            controller: ['$scope', '$injector', function ($scope, $injector) {
                //Inheritance
                $injector.invoke(FireEventsController, this, { $scope: $scope });
                $injector.invoke(ParentController, this, { $scope: $scope });

            }],
            link: function ($scope, $element, $attrs) {

                //State
                var enabled = true;
                var el = $($element);
                var selectCallback = $scope.$parent[$attrs.selectionAction];

                $scope.getEnabled = function () {
                    return enabled;
                };

                $scope.setEnabled = function (value) {
                    enabled = value;
                    el.selectable(value ? 'enable' : 'disable');
                };

                var initialize = function () {
                    el.
                        selectable({
                            filter: $attrs.selectable,
                            stop: function (event, ui) {
                                $scope.$apply(function () {
                                    //check 
                                    if (enabled) {
                                        var selection = [];
                                        $(".ui-selected").each(function () {
                                            var el = $(this);
                                            var index = el.index();
                                            if (index >= 0) {
                                                var week = el.parent().attr('week');
                                                var currentWeek = Enumerable.From(selection).FirstOrDefault(null, '$.week == ' + week);
                                                if (!currentWeek) {
                                                    currentWeek = { week: parseInt(week), days: [index] };
                                                    selection.push(currentWeek);
                                                } else {
                                                    currentWeek.days.push(index);
                                                }
                                            }
                                        });

                                        if (!$.isEmptyObject(selection)) {
                                            selectCallback(selection);
                                        }
                                    }
                                });
                            }
                        });
                    $scope.$parent.register("selectable", $scope);
                };
                initialize();
            }
        };
    });