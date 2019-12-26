(function () {

    'use strict';

    angular
        .module('EC')
        .factory('AdditionalComp', ['$http', '$q', function ($http, $q) {
            return {
                getData: function (id) {
                    var deffered = $q.defer();
                    $http({
                        method: 'GET',
                        url: '/api/AdditionalCompanies/' + id
                    })
                        .then(function success(response) {
                            deffered.resolve(response);
                        }, function error(response) {
                            deffered.reject(response.status);
                        });
                    return deffered.promise;
                }
            };
        }]);
})();
