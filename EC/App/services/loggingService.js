'use strict';
components
.service('loggingService', ['onLeaveService',function (onLeaveService) {
    //Overwrite the Configuration Options For the Extensions Log Service
    LogService['DEBUG_URL'] = 'Ext/Services/Log/Debug';
    LogService['INFO_URL'] = 'Ext/Services/Log/Info';
    LogService['ERROR_URL'] = 'Ext/Services/Log/Error';

    onLeaveService.listen("leave", function () { _disabledLogging = true; });

    var _disabledLogging = false;
    var _isDebug = function () {
        return typeof IsDebug != 'undefined' && /* check if the variable is defined */ IsDebug; /* check if the variable is true */
    };
    var _logOnBrowser = function (msg) {
        if (console && console.log) {
            console.log(msg);
        }
    };

    this.Info = function (msg) {
        //Log the error to trackJs
        if (window.trackJs) { trackJs.track(msg); }

        if (!_disabledLogging) {
            //Remote Logging
            LogService.Info(msg, null, function () { _logOnBrowser("Log.Info: Error"); });
            _logOnBrowser(msg);
        }
    };
    this.Debug = function (msg) {
        //Log the error to trackJs
        if (window.trackJs) { trackJs.track(msg); }

        if (!_disabledLogging) {
            //Remote Logging
            LogService.Debug(msg, null, function () { _logOnBrowser("Log.Debug: Error"); });
            if (_isDebug()) {
                alert(msg);
            } else {
                _logOnBrowser(msg);
            }
        }
    };
    this.Error = function (msg) {
        //Log the error to trackJs
        if (window.trackJs) { trackJs.track(msg); }

        if (!_disabledLogging) {
            //Remote Logging
            LogService.Error(msg, null, function () { _logOnBrowser("Log.Error: Error"); });
            if (_isDebug()) {
                alert(msg);
            } else {
                _logOnBrowser(msg);
            }
        }
    };
}]);