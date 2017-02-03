'use strict';

csmartApp
.service('csmartDataService', ['$http', 'loggingService', '$injector', 'roleEnum', 'handleError', '$q', '$safeApply',
    function ($http, loggingService, $injector, roleEnum, handleError, $q, $safeApply) {
        var self = this;
        $injector.invoke(FireEventsController, self, { $scope: self });

        self.events = self.events.concat(["ready"]);

        self.GetEventById = function (eventId, detailLevel) {
            return callService('GetEventById', { eventId: eventId, detailLevel: detailLevel }, defaultConfig);
        };
        self.GetBooking = function (id, detailLevel) {
            return callService('GetBooking', { id: id, detailLevel: detailLevel });
        };
        self.GetBillingEntries = function (startDate, endDate, userContractTypeInt, operatingLineId) {
            return callService('GetBillingEntries', { startDate: startDate, endDate: endDate, userContractTypeInt: userContractTypeInt, operatingLineId: operatingLineId });
        };
        self.ScheduleEvent = function (templateId, from) {
            return callService('ScheduleEvent', { templateId: templateId, from: from });
        };
        self.GetResourcesByType = function (resourceTypeId, detailLevel) {
            return callService('GetResourcesByType', { resourceTypeId: resourceTypeId, detailLevel: detailLevel });
        };
        self.GetHolidays = function (year) {
            return callService('GetHolidays', { year: year });
        };
        self.GetAllResourceTypes = function (detailLevel) {
            return callService('GetAllResourceTypes', { detailLevel: detailLevel });
        };
        self.GetAllResources = function (includeResourcesWithoutResourceTypes, detailLevel) {
            return callService('GetAllResources', { includeResourcesWithoutResourceTypes: includeResourcesWithoutResourceTypes, detailLevel: detailLevel });
        };
        self.GetBusyTimesForResource = function (resourceId, bookingId, start, end) {
            return callService('GetBusyTimesForResource', { resourceId: resourceId, bookingId: bookingId, start: start, end: end });
        };
        self.GetDefaultResourceBookingTimes = function (templateId, startDate) {
            return callService('GetDefaultResourceBookingTimes', { templateId: templateId, startDate: startDate });
        };
        self.GetDefaultResourceBookingTimesByResourceType = function (templateId, startDate, resourceTypeId) {
            return callService('GetDefaultResourceBookingTimesByResourceType', { templateId: templateId, startDate: startDate, resourceTypeId: resourceTypeId });
        };

        self.GetAllTemplates = function (includeHidden, detailLevel) {
            return callService('GetAllTemplates', { includeHidden: includeHidden, detailLevel: detailLevel });
        };
        self.GetAllEventFilters = function (detailLevel) {
            return callService('GetAllEventFilters', {detailLevel: detailLevel });
        };
        self.GetAllOperatingLines = function (detailLevel) {
            return callService('GetAllOperatingLines', { detailLevel: detailLevel });
        };
        self.GetAllOwnerOnlyOperatingLines = function (detailLevel) {
            return callService('GetAllOwnerOnlyOperatingLines', { detailLevel: detailLevel });
        };
        self.GetAllRawOperatingLines = function (detailLevel) {
            return callService('GetAllRawOperatingLines', { detailLevel: detailLevel });
        };
        self.CreateEvent = function (eventInfo) {
            return callService('CreateEvent', { eventInfo: eventInfo });
        };
        self.UpdateEvent = function (eventInfo) {
            return callService('UpdateEvent', { eventInfo: eventInfo });
        };
        self.UpdateEventCritera = function (eventId, newCriteria) {
            return callService('UpdateEventCritera', { eventId: eventId, newCriteria: newCriteria });
        };
        self.UpdateEventDescription = function (eventId, newDescription) {
            return callService('UpdateEventDescription', { eventId: eventId, newDescription: newDescription });
        };
        self.AssignParticipant = function (eventId, userId, allocationLineId, assigningLineId) {
            return callService('AssignParticipant', { eventId: eventId, userId: userId, assignToLineId: allocationLineId, assignedByLineId: assigningLineId });
        };
        self.RemoveParticipantAssignments = function (eventId, userIds) {
            return callService('RemoveParticipantAssignments', { eventId: eventId, userIds: userIds });
        };
        self.RecordParticipantAgreement = function (eventId, userId) {
            return callService('RecordParticipantAgreement', { eventId: eventId, userId: userId });
        };
        self.RecordCriteraSatisfied = function (eventId, lineId) {
            return callService('RecordCriteraSatisfied', { eventId: eventId, lineId: lineId });
        };
        self.AbandonAllocation = function (eventId, lineId) {
            return callService('AbandonAllocation', { eventId: eventId, lineId: lineId });
        };
        self.GetBookings = function (from, to, lineId, templateId, resourceTypeId, resourceId, participantId, detailLevel) {
            return callService('GetBookings', { from: from, to: to, lineId: lineId, templateIds: templateId, resourceTypeId: resourceTypeId, resourceId: resourceId, participantId: participantId, detailLevel: detailLevel});
        };
        self.GetDetailedBookings = function (from, to, lineId, templateIds, resourceTypeId, resourceId, participantId, detailLevel, showPublishedOnly) {
            return callService('GetDetailedBookings', { from: from, to: to, lineId: lineId, templateIds: templateIds, resourceTypeId: resourceTypeId, resourceId: resourceId, participantId: participantId, detailLevel: detailLevel, showPublishedOnly: showPublishedOnly });
        };
        self.CreateNonEventBooking = function (when, resourceId, notes) {
            return callService('CreateNonEventBooking', { when: when, resourceId: resourceId, notes: notes });
        };
        self.DeleteEvent = function (eventId) {
            return callService('DeleteEvent', { eventId: eventId });
        };
        self.DeleteNonEventBooking = function (bookingId) {
            return callService('DeleteNonEventBooking', { bookingId: bookingId });
        };
        self.UpdateResourceBookingTimes = function (bookingId, resourceBookings, bookingTimes) {
            return callService('UpdateResourceBookingTimes', { bookingId: bookingId, resourceBookings: resourceBookings, bookingTimes: bookingTimes });
        };
        self.UpdateBookingNotes = function (bookingId, newNotes) {
            return callService('UpdateBookingNotes', { bookingId: bookingId, newNotes: newNotes });
        };
        self.FindViableTimes = function (templateId, from, eventLength, numWeeks) {
            return callService('FindViableTimes', { templateId: templateId, from: from, eventLength: eventLength, numWeeks: numWeeks });
        };
        self.CreateUser = function (newUser) {
            return callService('CreateUser', { newUser: newUser });
        };
        self.ActivateUser = function (userId, password) {
            return callService('ActivateUser', { userId: userId, password: password });
        };
        self.UpdateUser = function (userInfo) {
            return callService('UpdateUser', { userInfo: userInfo });
        };
        self.GetUserByEmail = function (emailAddress, detailLevel) {
            return callService('GetUserByEmail', { emailAddress: emailAddress, detailLevel: detailLevel });
        };
        self.SendAdHocEmailByUser = function (to, body, subject, details, isParticipants, carbonCopy) {
            return callService('SendAdHocEmailByUser', { to: to, body: body, subject: subject, details: details, isParticipants: isParticipants, carbonCopy: carbonCopy });
        };
        self.GetCurrentUserDetails = function (detailLevel) {
            return callService('GetCurrentUserDetails', { detailLevel: detailLevel });
        };
        //self.accordion = function (templateId, details) {
        //    return callService('accordion', { templateId: templateId, details: details });
        //};
        self.UpdateTemplateCourseDetails = function (templateId, courseDetails) {
            return callService('UpdateTemplateCourseDetails', { templateId: templateId, courseDetails: courseDetails });
        };
        self.UpdateTemplateDescription = function (templateId, details) {
            return callService('UpdateTemplateDescription', { templateId: templateId, details: details });
        };
        self.UpdateTemplateCritera = function (templateId, criteria) {
            return callService('UpdateTemplateCriteria', { templateId: templateId, criteria: criteria });
        };
        self.UpdateTemplateNotes = function (templateId, notes) {
            return callService('UpdateTemplateNotes', { templateId: templateId, notes: notes });
        };
        self.UpdateOperatingLineDetails = function (eventId, operatingLineId, details) {
            return callService('UpdateOperatingLineDetails', { eventId: eventId, operatingLineId: operatingLineId, details: details });
        };
        self.UpdateOperatingLineOpenDetailsForLines = function (eventId, operatingLineId, openDetailsForLines) {
            return callService('UpdateOperatingLineOpenDetailsForLines', { eventId: eventId, operatingLineId: operatingLineId, openDetailsForLines: openDetailsForLines });
        };
        self.GetResourceAvailabilityForEvent = function (eventId, startDate) {
            return callService('GetResourceAvailabilityForEvent', { eventId: eventId, startDate: startDate });
        };
        self.UpdateTemplateEmailForParticipants = function (templateId, emailTemplate) {
            return callService('UpdateTemplateEmailForParticipants', { templateId: templateId, emailTemplate: emailTemplate });
        };
        self.UpdateTemplateEmailForOperatingLines = function (templateId, emailTemplate) {
            return callService('UpdateTemplateEmailForOperatingLines', { templateId: templateId, emailTemplate: emailTemplate });
        };
        self.UpdateParticipantAssignment = function (participantAssignment) {
            return callService('UpdateParticipantAssignment', { participantAssignment: participantAssignment });
        };
        self.GetArrivalReportEntries = function (arrivalStartDate, arrivalEndDate, detailLevel) {
            return callService('GetArrivalReportEntries', { arrivalStartDate: arrivalStartDate, arrivalEndDate: arrivalEndDate, detailLevel: detailLevel });
        };
        self.GetDepartureReportEntries = function (departureStartDate, departureEndDate, detailLevel) {
            return callService('GetDepartureReportEntries', { departureStartDate: departureStartDate, departureEndDate: departureEndDate, detailLevel: detailLevel });
        };
        self.GetCancellationReportEntries = function (cancellationStartDate, cancellationEndDate, detailLevel) {
            return callService('GetCancellationReportEntries', { cancellationStartDate: cancellationStartDate, cancellationEndDate: cancellationEndDate, detailLevel: detailLevel });
        };
        self.GetAvailableCoursesReportEntries = function (startDate, endDate,keyword, detailLevel) {
            return callService('GetAvailableCoursesReportEntries', { startDate: startDate, endDate: endDate, keyword: keyword, detailLevel: detailLevel });
        };
        self.GetTransportReportEntries = function (startDate, endDate) {
            return callService('GetTransportReportEntries', { startDate: startDate, endDate: endDate });
        };
        self.GetUserByEmployeeId = function (employeeId, operatingLineId, detailLevel) {
            return callService('GetUserByEmployeeId', { employeeId: employeeId, operatingLineId: operatingLineId, detailLevel: detailLevel });
        };
        self.GetAvaliableResourcesForResourceBooking = function (resourceBooking) {
            return callService('GetAvaliableResourcesForResourceBooking', { resourceBooking: resourceBooking });
        };
        self.GetAllocationReportEntries = function (StartDate, EndDate, OwnerId, detailLevel) {
            return callService('GetAllocationReportEntries', { StartDate: StartDate, EndDate: EndDate, OwnerId: OwnerId, detailLevel: detailLevel });
        };

        var callService = function (serviceName, data) {

            //Look For any Peloaded Requests on the page
            //This is rendered as json on Index.cshtml
            if (preloadedInitialRequests && preloadedInitialRequests[serviceName]) {
                var defer = $q.defer();
                //extend to look like $http
                defer.promise.success = function (fn) {
                    defer.promise.then(function (value) {
                        fn(value);
                    });
                    return defer.promise;
                };

                defer.promise.error = function (fn) {
                    defer.promise.then(null, function (value) {
                        fn(value);
                    });
                    return defer.promise;
                };
                defer.resolve(preloadedInitialRequests[serviceName]);
                delete preloadedInitialRequests[serviceName];
                return defer.promise;
            }

            return $http.post(
                dataController + serviceName,
                data,
                defaultConfig).error(function (data, status, headers, config, statusText) {
                    if (data && data.Handled) { return; }
                    if (status == 0) { return; } // status 0 indicates that the request was termianted on the client

                    var sTrace = printStackTrace();
                    var stackTrace = sTrace.join("\n");

                    loggingService.Error("Error : Service Name =" + serviceName + "\r\nData=" + angular.toJson(data, true) +
                        "\r\n Status=" + status + "\r\n statusText=" + statusText + "\r\n Config=" + angular.toJson(config, true) +
                        "\r\n StackTrace = \n" + stackTrace);
                });
        };

        self.resourcetypes = null;
        self.resources = null;
        self.visibleTemplates = null;
        self.visibleEventFilters = null;
        self.allTemplates = null;
        self.operatinglines = null;
        self.currentuser = null;
        self.alreadyLoaded = false;

        self.permissions = {
            admin: false,
            /*New Roles For CSMART Users*/
            courseAdmin: false,
            viewer: false,
            csmart: false, //if they are a csmart user = admin || courseAdmin || viewer
            /*New Roles For CSMART Users*/
            lineAdmin: false, //if the are any line admin = lineAdmin || externalLineAdmin
            internalLineAdmin: false,
            externalLineAdmin: false,
            participant: false,
            invalid: false
        };

        //Configuration
        var defaultConfig = {
            transformResponse: [function (data, headersGetter) {
                if (!data) {
                    return null;
                }

                //FF always returns a JSON Object
                if ($.isPlainObject(data)) {
                    return data;
                }

                if (typeof data === 'string' &&
                    //Isn't a json string
                    data.trim().indexOf("[") !== 0 &&
                    data.trim().indexOf("{") !== 0 &&
                    //This means there is a nested json string
                    data.trim().indexOf('"') !== 0
                    ) {
                    return data;
                }

                //try to part the string
                return angular.fromJson(data);
            }]
        };
        var dataController = "CSMART/Data/";

        var initialize = function () {
            //These are the preloaded initial requests
            //They need to match up with the returned object in the Index action in the Schedule Controller
            //PLEASE MAKE SURE any changes there are reflected here
            Promise.all([
                //Get all resource types
                self.GetAllResourceTypes('resources').success(function (data) {
                    self.resourcetypes = data;
                }),
                /**
                 * Initialize the cached set of resources. Cached resources are populated with
                 * the set of resource types that they belong to.
                 */
                self.GetAllResources(false, 'resourcetypes').success(function (data) {
                    self.resources = data;
                }),
                //Get all the event templates
                self.GetAllTemplates(true, 'resourcetypes resourcetypes.resources DefaultBookingTimes').success(function (data) {
                    self.allTemplates = data;
                    self.visibleTemplates = Enumerable.From(data).Where("$.IsHidden===false").ToArray();
                }),
                //Get all the event templates with filters
                self.GetAllEventFilters('resourcetypes resourcetypes.resources DefaultBookingTimes').success(function (data) {
                    self.visibleEventFilters = Enumerable.From(data).Where("$.IsHidden===false").ToArray();
                }),
                //Get all operating lines
                self.GetAllOperatingLines('coordinators').success(function (data) {
                    self.operatinglines = data;
                }),
                //Get all owner only operating lines
                self.GetAllOwnerOnlyOperatingLines('coordinators').success(function (data) {
                    self.owneronlyoperatinglines = data;
                }),
                //Get all raw operating lines
                self.GetAllRawOperatingLines('coordinators').success(function (data) {
                    self.rawoperatinglines = data;
                }),
                //Get current user details
                self.GetCurrentUserDetails('core role').success(function (data) {
                    self.currentuser = data;
                    
                    if (!self.currentuser || !self.currentuser.User) {
                        loggingService.Error("Error : GetCurrentUserDetails callback \r\nData=" + angular.toJson(data, true));
                        self.permissions.invalid = true;
                        handleError(self, {
                            Error: true,
                            Title: "Current user is not defined.",
                            Message: "There's a problem communicating with the server. Current user is not defined."
                        });
                    }
                    else{
                           switch (self.currentuser.User.Role) {
                                case roleEnum.CourseAdmin:
                                    self.permissions.csmart = true;
                                    self.permissions.courseAdmin = true;
                                    break;
                                case roleEnum.CSmartAdmin:
                                    self.permissions.admin = true;
                                    self.permissions.csmart = true;
                                    break;
                                case roleEnum.ExternalLineAdmin:
                                    self.permissions.externalLineAdmin = true;
                                    self.permissions.lineAdmin = true;
                                    break;
                                case roleEnum.LineAdmin:
                                    self.permissions.internalLineAdmin = true;
                                    self.permissions.lineAdmin = true;
                                    break;
                                case roleEnum.Viewer:
                                    self.permissions.viewer = true;
                                    self.permissions.csmart = true;
                                    break;
                                default:
                                    self.permissions.invalid = true;
                                    break;
                            }
                        }
                   
                 
                })
            ]).then(function () {
                //Finally fire that the csmartDataService is ready
                self.fire('ready');
                self.alreadyLoaded = true;
            });
        };
        initialize();
    }]);