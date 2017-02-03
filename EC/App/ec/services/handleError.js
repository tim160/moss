'use strict';
csmartApp.factory('handleError', function () {
    var handleError = function (scope, data) {
        if (data && data.Error) {
            if (data.Unhandled) {

                if (data.HttpMethod && data.HttpMethod.toLowerCase() == "get") {
                    scope.fire("operation", {
                        sticky: true,
                        type: "error",
                        title: "Application Tampering",
                        message: "You might be behind a firewall or proxy that is preventing this application from running properly. Please check your system configuration or contact your technical support.",
                        details: JSON.stringify(data.Exception),
                    });
                }
                else {
                    scope.fire("operation", {
                        sticky: true,
                        type: "error",
                        title: "Unexpected Exception",
                        message: data.Message,
                        details: JSON.stringify(data.Exception),
                    });
                }

            } else if (data.Title && data.Message) {
                scope.fire("operation", {
                    sticky: true,
                    type: "error",
                    title: data.Title,
                    message: data.Message,
                    details: JSON.stringify(data.Exception),
                });
            }
        } else {
            scope.fire("operation", {
                sticky: true,
                type: "error",
                title: "Unexpected Response",
                message: "There was an unexpected response from the server when trying to make your request"
            });
        }
    };
    return handleError;
});