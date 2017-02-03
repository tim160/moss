'use strict';
var FireEventsController = function ($scope) {
    var callbacks = {};
    var callbacksAll = [];
    //Events
    $scope.events = ["register", "ready"];

    $scope.fire = function (event, data) {
        var callbacksForEvent = callbacks[event];
        if (callbacksForEvent && callbacksForEvent.length) {
            $.each(callbacksForEvent, function (index, callback) {
                callback(data);
            });
        }
        if (callbacksAll.length) {
            $.each(callbacksAll, function (index, callback) {
                callback(event, data);
            });
        }
        //use jquery style chaining
        return this;
    };
    var validateEvent = function (eventName) {
        if (jQuery.inArray(eventName, $scope.events) === -1) {
            //TODO: better handling for debugging events
            if (console && console.log) {
                console.log(eventName); //this is not inside the list of events
            }
        }
    };
    $scope.unlisten = function (event, callback) {
        validateEvent(event);
        if (!callback) {
            //remove all callbacks
            callbacks[event] = [];
        } else {
            var index = callbacks[event].indexOf(callback);
            if (index !== -1) {
                callbacks[event].splice(index, 1);
            }
        }

        return this;
    };
    $scope.listenAll = function (callback) {
        callbacksAll.push(callback);

        return this;
    };
    $scope.hasListeners = function () { return !jQuery.isEmptyObject(callbacks) || (callbacksAll.length) > 1; }
    $scope.listen = function (event, callback) {
        validateEvent(event);

        var callbacksForEvent = callbacks[event];
        if (callbacksForEvent) {
            callbacksForEvent.push(callback);
        } else {
            callbacks[event] = [callback];
        }

        return this;
    };
};
FireEventsController.$inject = ['$scope'];