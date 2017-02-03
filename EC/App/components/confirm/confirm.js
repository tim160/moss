'use strict';
components
    .directive('confirm', function () {
        return {
            priority: 100,
            restrict: 'A',
            controller: ['$scope', '$dialog', function ($scope, $dialog) {
                //get a reference to a dialog provider
                $scope.dialog = $dialog;
            }],
            link: function ($scope, $element, $attrs) {

                var confirmationTemplate = "<div class=\"modal-header\">\n" +
                "	<h3>{{ title }}</h3>\n" +
                "</div>\n" +
                "<div class=\"modal-body\">\n" +
                "	<p>{{ message }}</p>\n" +
                "</div>\n" +
                "<div class=\"modal-footer\">\n" +
                "	<button ng-repeat=\"btn in buttons\" ng-click=\"btn.action(dialog,btn)\" class=\"btn\" ng-class=\"btn.cssClass\" data-ng-disabled='btn.posting || dialog.posting'>" +
                "       <i data-ng-if='btn.posting' class='icon-spin icon-spinner'></i> {{ btn.label }}" +
                "   </button>\n" +
                "</div>\n" +
                "";


                //Private State
                var el = $($element);

                $element.bind('click', function (e) {
                    var isConfirmMessageDefined = angular.isDefined($attrs.confirm);
                    var isConfirmClickDefined = angular.isDefined($attrs.confirmClick);

                    var isConfirmEnabled = isConfirmMessageDefined && isConfirmClickDefined;

                    if (angular.isDefined($attrs.confirmOn)) {
                        isConfirmEnabled = isConfirmEnabled && $scope.$eval($attrs.confirmOn);
                    }

                    if (isConfirmEnabled) {

                        //setup the message and buttons for the confirmation
                        var message = $attrs.confirm;
                        var messageTitle = angular.isDefined($attrs.confirmTitle) ? $attrs.confirmTitle : "Are you sure?"
                        var buttons = [
                            {
                                label: 'Yes',
                                cssClass: 'btn-primary',
                                action: function (dialog, btn) {
                                    //if yes is selected then execute the confirm action
                                    var evalResult = $scope.$eval($attrs.confirmClick);

                                    if (evalResult && evalResult.then) {
                                        //this confirmation returns a promise
                                        
                                        //mark the button and dialog as posting
                                        btn.posting = true;
                                        dialog.posting = true;
                                        
                                        evalResult.then(function () {
                                            //on success of the event close the dialog
                                            delete btn.posting;
                                            delete dialog.posting;

                                            dialog.close();
                                        });
                                    } else {
                                        //if this is not a promise then just close the dialog
                                        dialog.close();
                                    }
                                }
                            },
                            {
                                label: 'No',
                                action: function (dialog) { dialog.close(); }
                            }
                        ];


                        $scope.$apply(function () {
                            //open up the confirmation dialog and open it
                            $scope.dialog.dialog({
                                template: confirmationTemplate,
                                controller: ['$scope', 'dialog', 'model', function ($scope, dialog, model) {
                                    $scope.title = model.title;
                                    $scope.message = model.message;
                                    $scope.buttons = model.buttons;
                                    $scope.dialog = dialog;
                                }],
                                resolve:
                                  {
                                      model: function () {
                                          return {
                                              title: messageTitle,
                                              message: message,
                                              buttons: buttons
                                          }
                                      }
                                  }
                            }).open();
                        });
                    } else if (isConfirmClickDefined) {
                        $scope.$apply($attrs.confirmClick);
                    }
                });
                var container = el.parent();
                var activeClass = "active";

                //Config
                if (angular.isDefined($attrs.activeHoverContainer)) {
                    container = el.closest($attrs.activeHoverContainer);
                }
                if (angular.isDefined($attrs.activeHoverClass)) {
                    activeClass = $attrs.activeHoverClass;
                }
                el.hover(function () {
                    container.addClass(activeClass);
                }, function () {
                    container.removeClass(activeClass);
                });
            }
        };
    });
