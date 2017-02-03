'use strict';
    csmartApp.factory('orderResourceBookings', function () {
        var orderResourceBookings = function (resourceBookings) {

            var result = Enumerable.From(resourceBookings).OrderBy("$.ResourceType.DisplayOrder").ToArray();

            return result;
        };
        return orderResourceBookings;
    }).factory('orderResourceTypes', function () {
        var orderResourceTypes = function (resourceTypes) {

            var result2 = Enumerable.From(resourceTypes).OrderBy("$.DisplayOrder").ToArray();
            return result2;
        };
        return orderResourceTypes;
    }).factory('getYearNumberOfWeeks', function () {
        //// Find number of the weeks in one year base on iso 8601 standard
        //// if year is null as input, it returns current year number of weeks.
        var getYearNumberOfWeeks = function (year) {
            
            if (year != null) {
                var nextYear = year + 1;
            } else {

                var currentDate = new Date();
                var nextYear = currentDate.getUTCFullYear() + 1;
            }

            var firstDayOfTheWeekNextYear = new XDate();
            firstDayOfTheWeekNextYear.setUTCWeek(1, nextYear);
            var oneWeekBefore = firstDayOfTheWeekNextYear.addWeeks(-1);
            var numOfWeeks = oneWeekBefore.getUTCWeek();
            return numOfWeeks;
        };
        return getYearNumberOfWeeks;
    });

    