define([
    ], function () {
    "use strict";
    var CaseMessagesModel = function (initialData, type, viewSettingModifiedCallback) {
        var _self = this;
  //      this.sortModified = ko.observable(false);
   //     this.BgIds = ko.observableArray();     
    }
            CaseMessagesModel.clearMessage = function () {
                var self = this;
                if (this.initializing) {
                    return;
                }

                this.body_tx('');
            };

            CaseMessagesModel.saveMessage = function () {
                var self = this;
                self.addMessage(self, function saveSuccess() {
                    self.clearMessage();
                });
            };


        return {
            CaseMessagesModel: CaseMessagesModel
        }
    });