'use strict';
components
.service('onLeaveService',['$injector', function ($injector) {
    var self = this;
    //Inheritance
    $injector.invoke(FireEventsController, self, { $scope: self });

    //Events supported for this service
    self.events = ["leave"];

    self.RegisterCallback = function (callback) {
        callbacks.push(callback);
        
        }

    var initialize = function () {
        window.onunload = FireLeave;

    };
    var FireLeave = function () {
        //self.fire("leave", e);
        for (var i = 0; i < callbacks.length; i++) {
            callbacks[i]();
        }
    }

    var callbacks = [];

    initialize();
}]);