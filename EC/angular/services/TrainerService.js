(function () {

    'use strict';

    angular.module('EC')
        .service('TrainerService', ['$resource', TrainerService]);

    function TrainerService($resource) {
        return $resource('/api/Trainer', {}, {
            get: { url: '/api/Trainer/Get', method: 'GET', params: {}, isArray: false },
            addEvent: { url: '/api/Trainer/AddEvent', method: 'POST', params: {}, isArray: false },
        });
    };
})();
