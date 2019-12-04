(function () {

    'use strict';

    angular.module('EC')
        .factory('uploadImage',['$http', '$q', function ($http, $q) {
            return {
                getData: function (fd) {
                    var deffered = $q.defer();
                    $http({
                        method: 'POST',
                        data: fd,
                        url: '/Settings/AddLogoCompany',
                        headers: { 'Content-Type': undefined },
                        transformRequest: angular.identity
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
