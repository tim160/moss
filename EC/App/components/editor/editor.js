'use strict';
components
    .directive("editor",
     function () {
         return {
             require: '?ngModel',
             scope: {},
             controller: ['$scope', '$injector', function ($scope, $injector) {
                 //Inheritance
                 $injector.invoke(FireEventsController, this, { $scope: $scope });

                 //Events
                 $scope.events = $scope.events.concat(["change"]);
             }],
             link: function ($scope, $element, $attrs, ngModel) {
                 $scope.name = "editor";

                 var inline = false;
                 var ckeditor = null;
                 var ckeditorData = null;

                 //Functions 
                 $scope.get = function () {
                     return ckeditor.getData();
                 };

                 $scope.set = function (value) {
                     ckeditor.setData(value);
                     $scope.fire("change");
                 };

                 $scope.save = function () {
                     ngModel.$setViewValue(ckeditor.getData());
                 };

                 $scope.destroy = function () {

                     //un-register this component with the parent
                     $scope.$parent.unregister($scope.name, $scope, $attrs.editorId);

                     //try to destroy the ckeditor
                     try {
                         ngModel.$render = angular.noop;
                         if (ckeditor) {
                             ckeditor.removeAllListeners();
                             ckeditor.destroy(true);

                             ckeditor = null;
                         }

                     }
                     catch (e) { }
                 };

                 var initialize = function () {
                     if (angular.isDefined($attrs.editor)) {
                         if ($attrs.editor === "inline") {
                             inline = true;
                         }
                     }

                     var config = {
                         toolbar: [['Undo', 'Redo'], ['Bold', 'Italic', '-', 'RemoveFormat'], ['Link', 'Unlink'], ['NumberedList', 'BulletedList', 'Outdent', 'Indent'], ['Scayt']]
                     };
                     if (angular.isDefined($attrs.editorPlaceholder) && $attrs.editorPlaceholder === "true") {
                         //include the placeholder
                         config.toolbar.push(['CreatePlaceholder']);
                     }

                     if (inline) {
                         CKEDITOR.disableAutoInline = true;
                     }

                     ckeditor = CKEDITOR[(inline ? "inline" : "replace")]($element[0], config);

                     $scope.$on("$destroy", $scope.destroy);

                     ckeditor.on("change", function () {
                         $scope.$apply(function () {
                             var newData = ckeditor.getData();

                             if (!angular.isDefined($attrs.editorDelaySave)) {
                                 ngModel.$setViewValue(newData);
                             }

                             var isChange = (ckeditorData != newData);
                             var isInitialState = (!ckeditorData && newData == "");


                             if (isChange && !isInitialState) {
                                 $scope.fire("change");
                             }
                         });
                     });

                     if (ngModel) {

                         var updateCkeditor = function () {
                             //assumes we have jquery on the page
                             var existsOnPage = $(document).find(ckeditor.element.$).length;
                             //if it exists on the page update the viewValue
                             if (existsOnPage) {
                                 ckeditorData = ngModel.$viewValue;
                                 ckeditor.setData(ngModel.$viewValue);
                             } else {
                                 //try to destroy the editor
                                 $scope.$destroy();
                             }
                         };
                         ngModel.$render = function (value) {

                             if (ckeditor && ckeditor.status === "ready") {
                                 updateCkeditor();
                             } else {
                                 ckeditor.on('instanceReady', updateCkeditor);
                                 //check that the core is loaded
                                 ckeditor.loadFullCore && ckeditor.loadFullCore();
                             }

                         };
                     }

                     $scope.$parent.register($scope.name, $scope, $attrs.editorId);
                 };

                 initialize();
             }
         };
     });
