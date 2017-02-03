'use strict';
csmartApp
      .directive('addOffering', function () {
          return {
              scope: {
                  completeCallback: "@",
                  successCallback: "@",
                  cancelCallback: "@"
              },
              templateUrl: 'App/csmart/components/addEmployeeAwareness/addEmployeeAwareness.cshtml',
              controller:
                  ['$scope',
                  'ecDataService',
                  '$injector',
                  'handleError',
                  'orderResourceBookings',
                  'resourceBookingTypeEnum',
                  function ($scope,
                  ecDataService,
                  $injector,
                  handleError,
                  orderResourceBookings,
                  resourceBookingTypeEnum) {

                  //Inheritance
                  $injector.invoke(FireEventsController, this, { $scope: $scope });
                  $injector.invoke(ParentController, this, { $scope: $scope });


                  //Events
                  $scope.events = $scope.events.concat([
                      "expand",
                      "collapse",
                      "success",
                      "cancel",
                      "edit",
                      "operation"
                  ]);

                  //TODO: make these constants dependency injected
                  //Constants
                  var resourcePostponed = { Name: "Postponed", Postponed: true, Id: null, Availability: '' };
                  var resourceNone = { Name: "None", None: true, Id: null, Availability: '' };
                  var resourceExternal = { Name: "External", External: true, Id: null, Availability: '' };

                  //Constant Sub Views
                  $scope.possibleSubViews = {
                      "details": {
                          text: 'Event Details',
                          icon: 'icon-list'
                      },
                      "criteria": {
                          text: 'Criteria',
                          icon: 'icon-check'
                      },
                      "notes": {
                          text: 'Notes',
                          icon: 'icon-edit'
                      },
                      "calendar": {
                          text: 'Schedule',
                          icon: 'icon-calendar'
                      },
                      "courseDetails": {
                          text: 'Course Details',
                          icon: 'icon-list-ul'
                      }
                  };
                  $scope.allSubViews = Enumerable.From($scope.possibleSubViews).Select("$.Key").ToArray();

                  //State
                  $scope.show = false;
                  $scope.subPanel = false;
                  $scope.subPanelChanged = false;
                  $scope.changed = false;
                  $scope.resourceBookingTypeEnum = resourceBookingTypeEnum;
                  $scope.validTotal = true;

                  var currentEventTemplateBookingTimes = null;
                  var currentEventTemplate = null;
                  
                  /** 
                    * Create an offering based on the given template.
                    * NOTE: This code is currently ignoring 'bookingTime' and 'to' because event templates
                    * have a default length and a set of default booking times. In the future, we will
                    * allow the user to override these.
                    * @param {EventTemplate} eventTemplate The template to base the event on
                    * @param {DateTime} from The start date for the event
                    */
                  $scope.addOffering = function (eventTemplate, from, to, bookingTimes, client, uniqueIdentifier) {
                      $scope.success = false;
                      $scope.changed = false;
                      $scope.postingEditor = false;
                      $scope.postingEditorAsDefault = false;
                      //store the default booking times for later use with the week calendar
                      currentEventTemplateBookingTimes = eventTemplate.DefaultBookingTimes;
                      currentEventTemplate = eventTemplate;

                      //Round the number as if there is only one day then it comes up as 0.9
                      var numDays = Math.round(from.diffDays(to));
                      $scope.show = true;
                      $scope.scheduling = true;

                      ecDataService.ScheduleEvent(eventTemplate.Id, from)
                          .success(function (data) { setupResourceBookingSelections(data); })
                          .error(function (data) {
                              delete $scope.scheduling;
                              handleError($scope, data);
                          });
                  };

                  $scope.saveCalendar = function () {
                      if ($scope.subPanelChanged) { $scope.changed = true; }
                      //retrieve the update resource booking times
                      var resourceBookings = $scope.children("weekCalendar").getResourceBookingTimes();
                      var bookingTimes = $scope.children("weekCalendar").getBookingTimes();

                      //save the new states
                      $scope.event.Booking.BookingTimes = bookingTimes;
                      $scope.event.Booking.ResourcesBookings = resourceBookings;

                      $scope.toggleView();

                      setupResourceBookingSelections($scope.event);
                  };

                  //Load default details into the passed in control
                  $scope.loadDefaultDetails = function (control) {
                      $scope.children("editor")[control].set($scope.event.Template.Details);
                  };

                  //Load default criteria into the passed in control
                  $scope.loadDefaultCriteria = function (control) {
                      $scope.children("editor")[control].set($scope.event.Template.Criteria);
                  };

                  //Load default notes into the passed in control
                  $scope.loadDefaultNotes = function (control) {
                      $scope.children("editor")[control].set($scope.event.Template.Notes);
                  };

                  $scope.saveEditorAsDefault = function (control) {
                      $scope.postingEditorAsDefault = true;
                      var newDefaultText = $scope.children("editor")[control].get();

                      var httpPromise;
                      var successMessage = "details";

                      //save the state
                      switch (control) {
                          case "editorDetails":
                              httpPromise = ecDataService.UpdateTemplateDescription($scope.event.Template.Id, newDefaultText);
                              $scope.event.Template.Details = newDefaultText;
                              break;
                          case "editorCriteria":
                              successMessage = "criteria";
                              httpPromise = ecDataService.UpdateTemplateCritera($scope.event.Template.Id, newDefaultText);
                              $scope.event.Template.Criteria = newDefaultText;
                              break;
                          case "editorNotes":
                              successMessage = "notes";
                              httpPromise = ecDataService.UpdateTemplateNotes($scope.event.Template.Id, newDefaultText);
                              $scope.event.Template.Notes = newDefaultText;
                              break;
                          case "editorCourseDetails":
                              //This is s a special case!
                              //This field updates the template regardless of the event being created
                              //
                              //Please not this field does not follow the add event workflow
                              httpPromise = ecDataService.UpdateTemplateCourseDetails($scope.event.Template.Id, newDefaultText);
                              $scope.event.Template.CourseDetails = newDefaultText;
                              break;
                      }

                      httpPromise.success(function () {
                          //update message that details were sent to the server
                          $scope.fire("operation", {
                              type: "success",
                              title: "Updated default " + successMessage,
                              message: "Successfully updated the default " + successMessage + " of this event's template."
                          });
                          delete $scope.postingEditorAsDefault;

                      }).error(function (data) {
                          delete $scope.postingEditorAsDefault;
                          handleError($scope, data);
                      });
                  };

                  $scope.saveEditor = function (control) {
                      if ($scope.subPanelChanged) { $scope.changed = true; }

                      $scope.children("editor")[control].save();
                      $scope.toggleView();
                  };

                  $scope.toggleView = function (view) {
                      //discard any changes
                      delete $scope.subPanelChanged;
                      delete $scope.postingEditor;
                      delete $scope.postingEditorAsDefault;

                      if (!angular.isDefined(view) || $scope.subPanel === view) {
                          delete $scope.subPanel;
                      } else {
                          $scope.subPanel = view;
                      }
                      fireExpandedStateChange();

                      if ($scope.subPanel === 'calendar') {
                          delete $scope.calendarTitle;
                      }
                  };


                  $scope.addClient = function () {
                      $scope.changed = true;
                      $scope.resetRedBorder();
                      var newClient = {
                          OperatingLine: Enumerable.From($scope.uniqueClients()).First(),
                          Allocation: $scope.event.MaxSeats
                      };

                      if (!$scope.event.LineDetails) {
                          //init the array if there are not already an array of Line Details
                          $scope.event.LineDetails = [];
                      }
                      $scope.event.LineDetails.push(newClient);

                  };

                  $scope.resetRedBorder = function () {
                      $scope.validTotal = true;
                  }

                  $scope.uniqueClients = function (current) {
                      var listOfLines = Enumerable.From($scope.event.LineDetails);
                      var result = Enumerable.From(ecDataService.operatinglines).Where(function (x) {
                          if (current && x.Id === current.Id) {
                              return true;
                          } else {
                              return !listOfLines.Any(function (z) { return z.OperatingLine.Id === x.Id; });
                          }
                      }).ToArray();
                      return result;
                  };

                  $scope.removeClient = function (operatingLine) {
                      $scope.resetRedBorder();
                      $scope.event.LineDetails.splice($scope.event.LineDetails.indexOf(operatingLine), 1);
                  };

                  $scope.checkTotal= function() {
                      var sumSeats = 0;
                      $scope.validTotal = true;

                      Enumerable.From($scope.event.LineDetails).ForEach(function (lineDetail) {
                          sumSeats += lineDetail.Allocation;
                      });

                      if (sumSeats > $scope.event.MaxSeats) {
                          $scope.validTotal = false;
                      }

                      return $scope.validTotal;
                  }

                  $scope.createOffering = function () {
                      if (!$scope.validTotal) { return; }
                      $scope.posting = true;

                      ecDataService.CreateEvent($scope.event).success(function (data) {
                          $scope.show = false;

                          delete $scope.posting;

                          $scope.event.Id = data;
                          $scope.fire("success", $scope.event);

                          //after successful creating of the event let the person edit further details of the event
                          //fire to edit the parent to edit an offering

                          //$scope.fire("edit", $scope.event.Id );
                      }).error(function (data) {
                          delete $scope.posting;
                          //Handle the Error
                          handleError($scope, data);
                      });
                  };

                  /**
                  * Abandon the event being defined, and fire the 'cancel' event (which will
                  * cause the parent controller to close Add Event panel).
                  */
                  $scope.cancel = function () {
                      if ($scope.show) {
                          delete $scope.event;

                          $scope.show = false;
                          $scope.fire("cancel");
                      }
                  };

                  /**
                  * Format a date.
                  * NOTE: This should really be a filter that can be applied
                  */
                  $scope.getDayByDate = function (date, format) {
                      if (!date) { return "No Date"; }
                      return (new XDate(date)).toString(format);
                  };

                  /**
                  * Adds an ad-hoc resource binding on the booking
                  * @param {ResourceType} resourceType - the resource type to add to the booking
                  */
                  $scope.addResourceBooking = function (resourceType) {
                      $scope.changed = true;

                      if (!$scope.event.Booking.ResourcesBookings) {
                          $scope.event.Booking.ResourcesBookings = [];
                      }

                      // Create new resource booking and initialize possible resource selections...
                      var newResourceBooking = {
                          ResourceType: resourceType,
                          AdHoc: true,
                          //use the booking times from the current booking
                          BookingTimes: $scope.event.Booking.BookingTimes,
                          BookingType: resourceBookingTypeEnum.Default
                      };

                      //overwrite booking times with default resource type booking times if they exist
                      var matchResourceType = Enumerable.From($scope.event.Template.ResourceTypes).FirstOrDefault(null, function (rt) { return rt.Id === resourceType.Id; });
                      if (matchResourceType != null) {
                          ecDataService.GetDefaultResourceBookingTimesByResourceType($scope.event.Template.Id, $scope.event.Booking.BookingTimes[0].Start, resourceType.Id)
                              .success(function (data) {
                                  newResourceBooking.BookingTimes = data.BookingTimes;
                                  $scope.GetAvaliableResourcesAndUpdateResourceBooking(newResourceBooking);
                              })
                              .error(function (data) { handleError($scope, data); });
                      } else {
                          $scope.GetAvaliableResourcesAndUpdateResourceBooking(newResourceBooking);
                      }
                  };

                  $scope.GetAvaliableResourcesAndUpdateResourceBooking = function (newResourceBooking) {
                      updateResourceSelection(newResourceBooking);
                      $scope.event.Booking.ResourcesBookings.push(newResourceBooking);

                      ecDataService.GetAvaliableResourcesForResourceBooking(newResourceBooking)
                          .success(function (data) { onCompleted_GetAvaliableResourcesForResourceBooking(data, newResourceBooking); })
                          .error(function (data) { handleError($scope, data); });

                      //Update Resource Indexes on the 
                      //Resource Bindings
                      updateResourceBookingIndexes($scope.event);
                  };

                  $scope.resourceChanged = function (resourceBinding) {
                      if (resourceBinding.ResourceSelection) {
                          if (resourceBinding.AdHoc) {

                              //clear the state
                              resourceBinding.BookingType = resourceBookingTypeEnum.Default;
                              delete resourceBinding.External;

                              if (resourceBinding.ResourceSelection.None) {
                                  //remove this added resource
                                  $scope.event.Booking.ResourcesBookings.splice(
                                      $scope.event.Booking.ResourcesBookings.indexOf(resourceBinding), 1);

                                  //Update Resource Indexes on the 
                                  //Resource Bindings
                                  updateResourceBookingIndexes();
                              } else if (resourceBinding.ResourceSelection.Postponed) {
                                  resourceBinding.BookingType = resourceBookingTypeEnum.Postponed;
                                  delete resourceBinding.Resource;
                              } else if (resourceBinding.ResourceSelection.External) {
                                  resourceBinding.BookingType = resourceBookingTypeEnum.External;
                                  delete resourceBinding.Resource;
                              }
                              else {
                                  resourceBinding.Resource = resourceBinding.ResourceSelection;
                              }

                          } else {

                              //clear the state
                              resourceBinding.BookingType = resourceBookingTypeEnum.Default;
                              delete resourceBinding.External;

                              if (resourceBinding.ResourceSelection.Id) {
                                  //if there are no booking times the get the booking times from the current booking
                                  if (!resourceBinding.BookingTimes || !resourceBinding.BookingTimes.length) {
                                      resourceBinding.BookingTimes = $scope.event.Booking.BookingTimes;
                                  }
                                  resourceBinding.Resource = resourceBinding.ResourceSelection;
                              } else if (resourceBinding.ResourceSelection.None) {
                                  resourceBinding.BookingType = resourceBookingTypeEnum.None;
                                  delete resourceBinding.Resource;
                              } else if (resourceBinding.ResourceSelection.Postponed) {
                                  resourceBinding.BookingType = resourceBookingTypeEnum.Postponed;
                                  delete resourceBinding.Resource;
                              } else if (resourceBinding.ResourceSelection.External) {
                                  resourceBinding.BookingType = resourceBookingTypeEnum.External;
                                  delete resourceBinding.Resource;
                              }
                          }
                      } else {
                          delete resourceBinding.Resource;
                      }
                  };

                  /**
                   * This is called on success of the service call GetAvaliableResourcesForResourceBooking
                   * and will update the resourceBooking with the available resources.
                   *
                   * @param {List<Resource>} data List of available resources
                   * @param {ResourceBooking} resourceBooking resource booking to update according to 'data'
                   */

                  var onCompleted_GetAvaliableResourcesForResourceBooking = function (data, resourceBooking) {
                      resourceBooking.AvailableResources = data;
                      assignResourceAvailabilityForType(resourceBooking);
                      updateResourceSelection(resourceBooking);
                  };

                  /**
                   * Update resource selection of the resource booking.
                   * Available resources are listed in resourceBooking.AvailableResources
                   * Select the first available resource for the resource booking.
                   *
                   * @param {ResourceBooking} resourceBooking The resource booking to update
                   */

                  var updateResourceSelection = function (resourceBooking) {
                      /// <summary>Select the resource in the resource booking according to availability (if available).</summary>
                      /// <param name="resourceBooking" type="Object">resource booking</param>

                      resourceBooking.PossibleResourceSelections = getResourcesFromResourceBooking(resourceBooking); // Set possible resources.

                      // Select resource according to availability...
                      var availableResources = Enumerable.From(resourceBooking.PossibleResourceSelections).Where("$.Availability==='Available'").ToArray();
                      if (availableResources.length > 0) {
                          resourceBooking.ResourceSelection = availableResources[0];  // Select the first available element.
                      } else {
                          resourceBooking.ResourceSelection = resourceBooking.PossibleResourceSelections[0];  // Select the first element.
                      }

                      if (resourceBooking.ResourceSelection.Id) {
                          resourceBooking.Resource = resourceBooking.ResourceSelection;
                      } else {
                          //We assume that this will be external because getResourcesFromResourceBooking adds external as first item
                          resourceBooking.BookingType = resourceBookingTypeEnum.External;
                      }
                  };

                  var fireExpandedStateChange = function () {
                      $scope.fire($scope.subPanel ? "expand" : "collapse");
                  };

                  //store a reference to the csmart data service for the view
                  $scope.csmart = ecDataService;

                  var updateResourceBookingIndexes = function () {
                      /// <summary>Update indexes of ResourceBookings</summary>
                      Enumerable.From($scope.event.Booking.ResourcesBookings).
                          GroupBy("$.ResourceType.Id").
                          Where("$.Count() > 1").ForEach(function (x) {
                              var index = 1;
                              x.ForEach(function (m)
                              {
                                  m.Index = index;
                                  index++;
                              });
                          });
                  };

                  /**
                   *  Get all resources from a resource booking and add 'None' and optionally 'Postponed' at the end of the list.
                   *  The resources come from the resource type associated with the booking. Only add 'Postponed' if the booking 
                   *  is not AdHoc. This method is used for creating the resource selection drop-down.
                   *
                   *  @param {ResourceBooking} resourceBooking The resource booking with 'AdHoc' and 'ResourceType' assigned.
                   */
                  var getResourcesFromResourceBooking = function (resourceBooking) {
                      //TODO get the available resources

                      assignResourceAvailabilityForType(resourceBooking);
                      var result = resourceBooking.ResourceType.Resources.slice(0);

                      result.push(resourceExternal);
                      //postponed needs to be added as the second item so Adhoc
                      //resourcesBookings will default to external if there are no resources.
                      result.push(resourcePostponed);
                      result.push(resourceNone);
                      return result;
                  };

                  /**
                   * Extend each resource attached to the resource types in the given resource
                   * booking, with a field indicating whether that resource is available or not
                   * in the context of that booking. This information is used to partition the
                   * resource selection dropdowns into available/unavailable sections.
                   *
                   * @param {ResourceBooking} resourcebooking The resource booking to extend
                   */
                  var assignResourceAvailabilityForType = function (resourceBooking) {
                      Enumerable.From(resourceBooking.ResourceType.Resources)
                          .ForEach(function (resource) {
                              if (!resourceBooking.AvailableResources) {
                                  resource.Availability = '';
                              }
                              else if (resourceBooking.AvailableResources.length < 1) {
                                  resource.Availability = 'Unavailable';
                              }
                              else {
                                  var isResourceAvail = Enumerable.From(resourceBooking.AvailableResources).Any("$.Id == '" + resource.Id + "'");
                                  resource.Availability = isResourceAvail ? 'Available' : 'Unavailable';
                              }
                          });
                  };

                  /** 
                   * Create initial drop-down selections for each resource booking in the given
                   * event.
                   * @param {Event} event The event to process
                   */
                  var setupResourceBookingSelections = function (event) {
                      Enumerable.From(event.Booking.ResourcesBookings).ForEach(function (resourceBooking) {
                          resourceBooking.PossibleResourceSelections = getResourcesFromResourceBooking(resourceBooking);

                          if (resourceBooking.Resource) {
                              resourceBooking.ResourceSelection = Enumerable.From(resourceBooking.PossibleResourceSelections).First("$.Id=='" + resourceBooking.Resource.Id + "'");
                          }
                          else if (resourceBooking.BookingType === resourceBookingTypeEnum.None) {
                              resourceBooking.ResourceSelection = resourceNone;
                          }
                          else if (resourceBooking.BookingType === resourceBookingTypeEnum.Postponed) {
                              resourceBooking.ResourceSelection = resourcePostponed;
                          }
                      });

                      $scope.event = event;

                      // Update indexes of ResourceBookings
                      updateResourceBookingIndexes();

                      delete $scope.scheduling;

                  };

                  var initialize = function () {
                      $scope.listen("register", function (data) {
                          switch (data.type) {
                              case "weekCalendar":
                                  //Init of the week calendar

                                  //For all the booking times add to the list of days
                                  var days =
                                  Enumerable.From($scope.event.Booking.BookingTimes).Select(function (bookingtime) {
                                      return (new XDate(bookingtime.Start, true)).clearTime();
                                  }).ToArray();

                                  var booking = angular.copy($scope.event.Booking);

                                  data.child
                                      .listen("changed", function () { $scope.subPanelChanged = true; })
                                      .init(days, booking, currentEventTemplateBookingTimes, currentEventTemplate);

                                  $scope.calendarTitle = data.child.getTitle();
                                  break;
                              case "editor":
                                  data.child.listen("change", function () {
                                      $scope.subPanelChanged = true;
                                  });
                                  break;
                          }
                      });
                      $scope.orderResourceBookings = orderResourceBookings;
                      //Register this control with the parent control
                      $scope.$parent.register('addOffering', $scope);
                  };
                  initialize();
              }]
          };
      });