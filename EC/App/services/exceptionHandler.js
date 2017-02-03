'use strict';
components
.provider('$exceptionHandler',  function () {
    this.$get = [ '$window','loggingService', function ($window, loggingService) {
        return function (exception, cause) {
            //integration with track js
            if ($window.trackJs) { $window.trackJs.track(exception); }

            var sTrace = printStackTrace();
            var stackTrace = sTrace.join("\n");
            loggingService.Error(exception + ' : ' + cause + ' - ' + exception.stack + "\n\n StackTrace: " + stackTrace);
        };
    }];
});