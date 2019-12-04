(function () {

    'use strict';

    angular.module('EC')
        .service('NewCaseMessagesService', ['$resource', NewCaseMessagesService]);

    function NewCaseMessagesService($resource) {
        return $resource('/api/NewCaseMessages', {}, {
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
        });
    };
})();

