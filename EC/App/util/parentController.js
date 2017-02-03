'use strict';
var ParentController = function ($scope) {
    var children = {};
    //Events
    $scope.events = ["register", "ready"];
    $scope.components = null;

    $scope.children = function (id) {
        if (id) {
            return children[id];
        } else {
            //if no id was asked for then return all children
            return children;
        }
    };
    $scope.operateChild = function (child, callback) {
        //is there the child
        if (children[child]) {
            //is it an array?
            if ($.isArray(children[child]) || children[child].isListOfChildren) {
                $.each(children[child],
                    function (i, v) {
                        callback(v);
                    });
            } else {
                //if there is only
                callback(children[child]);
            }
        }
    };
    var loadedComponents = [];
    var evalulateLoadedComponents = function () {
        //see if all the components are ready
        if ($($scope.components).not(loadedComponents).length == 0 &&
        $(loadedComponents).not($scope.components).length == 0) {
            $scope.fire('ready', $scope.name);
        }
    };

    $scope.unregister = function (type, child, id) {
        if (!children[type]) { //Already Empty
        } else if ($.isArray(children[type])) { //Is Array
            children[type].splice(children[type].indexOf(child), 1);
        } else if (children[type].isListOfChildren) { //Is a List of Children
            delete children[type][id];
        } else { //Is Single Item
            delete children[type];
        }
        //TODO: if a required component goes away throw an exception
    };

    $scope.register = function (type, child, id) {
        child.$on("$destroy", function () {
            $scope.unregister(type, child, id);
        });

        if (!children[type]) { //Empty
            //if this item has an id on it then there are unique components
            if (angular.isDefined(id)) {
                children[type] = { isListOfChildren: true };
                children[type][id] = child;
            } else {
                children[type] = child;
            }
        } else if ($.isArray(children[type])) { //Is Array
            children[type].push(child);
        } else if (children[type].isListOfChildren) { //Is a List of Children
            children[type][id] = child;
        } else { //Is Single Item
            children[type] = [children[type], child];
        }
        if ($scope.fire) { //if this 
            $scope.fire('register', { type: type, child: child, id: id });
        }

        //child has components
        if ($scope.components) {
            if ($scope.components.indexOf(type) !== -1) { //is this one in the list of components
                if (!child.components) { //child can't fire the ready it has no components
                    loadedComponents.push(type);
                    evalulateLoadedComponents();
                } else {
                    var childIsReady = function () {
                        loadedComponents.push(type);
                        child.unlisten('ready', childIsReady);
                        evalulateLoadedComponents();
                    };
                    child.listen('ready', childIsReady);
                }
            }
        }
    };
};
ParentController.$inject = ['$scope'];