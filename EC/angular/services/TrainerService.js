(function () {

    'use strict';

    angular.module('EC')
        .service('TrainerService', ['$resource', TrainerService]);

    function TrainerService($resource) {
        return $resource('/api/Trainer', {}, {
            get: { url: '/api/Trainer/Get', method: 'GET', params: {}, isArray: false },
            addEvent: { url: '/api/Trainer/AddEvent', method: 'POST', params: {}, isArray: false },
            getTrainer: { url: '/api/Trainer/GetTrainer', method: 'GET', params: {}, isArray: false },
            addTime: { url: '/api/Trainer/AddTime', method: 'POST', params: {}, isArray: false },
            deleteTime: { url: '/api/Trainer/DeleteTime', method: 'POST', params: {}, isArray: false },
            deleteCompanyTime: { url: '/api/Trainer/DeleteCompanyTime', method: 'POST', params: {}, isArray: false },
        });
    };
})();
