$(document).ready(function () {
    //drop down list
    //first start
    updateGraphics();

    $('.liItem').on("click", function (event) {
        var temp = $(event.currentTarget);
        $(".clearAll").show();
        if (temp.hasClass('selected')) {
            temp.removeClass('selected');
        } else {
            var allOptions = temp.parents('ul');
            /*allOptions.find('li').each(function (index, element) {
                $(element).find('div.liItem').removeClass('selected');
            });*/
            temp.addClass('selected');
        }
        updateFilter();
        updateGraphics();
    });
    function updateFilter() {
        var tableFilter = $("#tableFilter").find('.sales');
        tableFilter.each(function (index, element) {
            $(element).closest('th').remove();
        });

        var arraySelected = $(".selected");
        var template = '';

        $(".selected").each(function (index, element) {
            var str = $.trim($(element).text());
            template = '<th><div class="sales">' + str + '<img src="/Content/Icons/Xanal.gif" /></div></th>';
            $(".filterUp").after(template);
        });
        $(".sales").on('click', function (event) {
            var temp = $(event.currentTarget);
            var name = $.trim(temp.text());
            $(".selected").each(function (index, element) {
                var currentElement = $(element);
                if ($.trim(currentElement.text()) == name) {
                    currentElement.removeClass('selected');
                }
            });
            temp.closest('th').remove();
            if ($(".sales").length == 0) {
                $(".clearAll").hide();
            }
        });
    }
    function updateGraphics() {
        var userId = $("#user_id");
        var companyId = $("#companyId");
        var types = {};
        $(".menuUl").each(function (indx, element) {
            var temp = $(element).find('.selected');
            if (temp.length > 0) {
                var index = temp.attr('namedropdown');
                types[index] = temp.attr('value');
            } else {
                temp = $($(element)[0]);
                temp = temp.find('.liItem')[0];
                temp = $(temp);
                var index = temp.attr('namedropdown');
                types[index] = " ";//temp.attr('value');
            }
        });
        sendAjax(userId.val(), companyId.val(), types);
    }

    //drop down list
    function sendAjax(userId, companyId, types) {
        if (userId && companyId) {
            $.ajax({
                method: "POST",
                url: "/Analytics/CompanyDepartmentReportAdvanced",
                //dataType: 'json',
                data: { companyId: companyId, userId: userId, types: types }
            }).done(function (data) {//data from server
                var temp = $.parseJSON(data);
                if (temp['LocationTable'] != null && temp['LocationTable'].length >= 1) {
                    _dtCompanyLocationReport(temp['LocationTable']);
                }
                if (temp['DepartmentTable'] != null && temp['DepartmentTable'].length > 1) {
                    _dtCompanyDepartmentReport(temp['DepartmentTable']);
                }
                if (temp["SecondaryTypeTable"] != null && temp["SecondaryTypeTable"].length > 1) {
                    var parentBlock = $(".blockTypeOfIncident");
                    blockTypeOfIncident(parentBlock, temp["SecondaryTypeTable"]);
                    TypeOfIncident_Reporter('.blockTypeOfIncident');
                }
                if (temp["RelationTable"] != null && temp["RelationTable"].length > 1) {
                    var parentBlock = $(".blockReporter");
                    blockTypeOfIncident(parentBlock, temp["RelationTable"]);
                    TypeOfIncident_Reporter('.blockReporter');
                }
                //if() top _dtAnalyticsTimeline
                //_dtCompanyLocationReport($.parseJSON(data)); 



                var averOfDays = temp['AverageStageDaysTable'];
                var preReview = averOfDays[0].val;
                var review = averOfDays[1].val;
                var investigation = averOfDays[2].val;
                var resolution = averOfDays[3].val;
                var escalation = averOfDays[4].val;

                AverageOfDaysPerStageBackUp();
                init(review, investigation, resolution, escalation);
                addNumberCenter();
            }).fail(function (error) {
                // alert(data);
            });
        }

    }
    function blockTypeOfIncident(parentBlock, data) {
        if (parentBlock) {
            if (parentBlock.find(".allBlockLines .blockLinesItem").length > 1) {
                parentBlock.find(".allBlockLines").html("");
            }
            var str = "";
            data.forEach(function (item, index) {
                str += '<div class="blockLinesItem">';
                str += "<p>" + item.name + "</p>";

                str += '<div class="linesShow">';
                str += '<div class="blockLineFirst"></div>';
                str += "<label>" + item.value + "</label>";
                if (item.value > item.prev) {
                    var diff = item.value - item.prev;
                    str += "<span>+" + diff + "<span>";
                } else {
                    str += "<span>(-)</span>";
                }
                str += '<div class="blockLineSecond"></div>';
                str += '</div></div>';
            });
            
            parentBlock.find(".allBlockLines").html(str);
        }
    }
    function maxForArray(arr) {
        var max = 0;
        for (var i = 0; i < arr.length; i++) {
            if (max < arr[i]) {
                max = arr[i];
            }
        }
        return max;
    }
    function WidthHeaderLine(elementName, AllDay, day) {
        var elementWidth = 70;
        var widthX = (day * elementWidth) / AllDay;

        $(elementName).css('width', widthX + '%');
    }

    function TypeOfIncident_Reporter(NameClass) {
        var allDay = [], day = [];
        for (var i = 0; i < allDay.length; i++) {
            allDay[i] = 0;
        }
        var maxLenght;
        $(NameClass + ' .blockLinesItem label').each(function (indx, element) {
            allDay[indx] = parseFloat($(element).text());
        });
        $(NameClass + ' .blockLinesItem span').each(function (indx, element) {
            if ($(element).text().indexOf("-") > -1) {
                day[indx] = allDay[indx] - 0;
            }
            else {
                temp_days = $(element).text();
                if(isNaN(temp_days))
                {
                    temp_days = temp_days.replace("(", "").replace(")", "").replace("+", "").trim(); 
                }

                day[indx] = allDay[indx] -  parseInt(temp_days);
                if(isNaN( day[indx]))
                    day[indx] = 0;
            }
        });
        maxLenght = maxForArray(allDay);
        $(NameClass + ' .blockLineFirst').each(function (indx, element) {
            WidthHeaderLine($(element), maxLenght, allDay[indx]);
        });
        $(NameClass + ' .blockLineSecond').each(function (indx, element) {
            WidthHeaderLine($(element), maxLenght, day[indx]);
        });
    }


    function AverageOfDaysPerStageBackUp() {
        var table = $(".wrapper table");
        var backup = $("#base");
        backup.hide();
        if (backup.length > 1) {
            table.html(backup.html());
            backup.html("");
        }
    }
    ///addNumberCenter();
    function addNumberCenter() {
        $(".table tr").each(function (indx, element) {
            var el = $(element);
            var arrayBrownDiv = el.find('.redDiv');
            if (arrayBrownDiv.length == 0) {
                arrayBrownDiv = el.find('.greenDiv');
            }
            var num = arrayBrownDiv.length;
            if (num % 2 == 0) {
                //четное число
                var temp = $(arrayBrownDiv[num / 2]).parents('td');
                temp.append('<span class="even">' + num + '</span>');
            } else {
                //
                var temp = $(arrayBrownDiv[(num - 1) / 2]).parents('td');
                temp.append('<span class="notEven">' + num + '</span>');
            }
        });
    }
    var buffer = 0;
    function init(review, investigation, resolution, escalation) {
        //var review = 3;
        //var investigation = 21;
        //var resolution = 1;
        //var escalation = 2;
        if ((review + investigation + resolution + escalation) > 28) {
            review = Math.round(review / 10);
            investigation = Math.round(investigation / 10);
            resolution = Math.round(resolution / 10);
            escalation = Math.round(escalation / 10);
        }


        var table = $(".wrapper table");
        var backup = $("#base");
        backup.html("");
        backup.html(table.html());


        var reviewTr = $(table.find('tr')[1]);
        var investigationTr = $(table.find('tr')[2]);
        var resolutionTr = $(table.find('tr')[3]);
        var escalationTr = $(table.find('tr')[4]);

        putBlock(buffer, review, reviewTr, 1);
        putBlock(buffer, investigation, investigationTr, 2);
        putBlock(buffer, resolution, resolutionTr, 3);
        putBlock(buffer, escalation, escalationTr, 4);

    }
    function putBlock(start, end, block, indexBlock) {
        buffer += end;
        for (var i = start + 1; i <= buffer; i++) {
            if (block) {
                var temp = $(block.find('td')[i]);
                if (temp) {
                    if (i == start + 1) {
                        addBorderHead(temp, i, 'startRedLine');
                    }
                    if (i == buffer) {
                        addBorderHead(temp, buffer, 'endRedLine');
                    }
                    var parentDiv = temp.find('.parentDiv');
                    var className = "redDiv";
                    if (indexBlock >= 3) {
                        className = "greenDiv";
                    }
                    if (parentDiv.length > 0) {
                        parentDiv.append("<div class='" + className + "'></div>");
                    } else {
                        var parentDivHtml = "<div class='parentDiv'><div class='" + className + "'></div></div>"
                        temp.append(parentDivHtml);
                    }
                }
            }
        }
    }
    function addBorderHead(currentItem, index, nameClass) {
        var listItems = $(".wrapper table tr");
        var buff = listItems.index(currentItem.parents('tr'));
        var heightOF = 4;
        if (currentItem && buff > 0 && index > 0 && nameClass == 'startRedLine') {
            for (var i = buff; i > 0; i--) {
                var temp = $(listItems[i]);
                heightOF += temp.height();
            }
            currentItem.append('<div class="startRedLineDiv" style="height: ' + heightOF + 'px"></div>');
        }
        if (nameClass == "endRedLine") {
            var temp = $(listItems[0]).find('td')[index];
            if (temp) {
                if (buff != 4) {
                    if (index >= 10) {
                        $(temp).append('<div class="parentDivHeader"><div class="headerBrown2 darker">' + index + '</div></div>');
                    } else {
                        $(temp).append('<div class="parentDivHeader"><div class="headerBrown darker">' + index + '</div></div>');
                    }
                } else {
                    $(temp).append('<div class="base"><div class="letters">' + index + '</div></div>');
                    heightOF = 4;
                    for (var i = buff; i > 0; i--) {
                        var temp = $(listItems[i]);
                        heightOF += temp.height();
                    }
                    currentItem.append('<div class="endRedLineDiv" style="height: ' + heightOF + 'px"></div>');
                }
            }
        }
    }
    //------------------------------------- START All cases -----------------------------------------------

    function _dtAnalyticsTimeline(data) {
        //var data = [
        //    { month: '', pending: 0, review: 0, investigation: 0, resolution: 0, escalation: 0, closed: 0 },// начало января
        //    { month: 'January', pending: 4, review: 3, investigation: 2.5, resolution: 2, escalation: 1.5, closed: 1 },// конец января - начало февраля
        //    { month: 'February', pending: 7, review: 5.5, investigation: 4, resolution: 3.5, escalation: 3, closed: 2 },// конец февраля - начало марта
        //    { month: 'March', pending: 9, review: 6, investigation: 5.5, resolution: 5, escalation: 5, closed: 2.5 },
        //    { month: 'April', pending: 9, review: 8.5, investigation: 7, resolution: 6, escalation: 4.5, closed: 3 },
        //    { month: 'May', pending: 9, review: 9, investigation: 6, resolution: 5, escalation: 4, closed: 3 },
        //    { month: 'June', pending: 10, review: 7, investigation: 5, resolution: 4, escalation: 3.5, closed: 3.5 },
        //    { month: 'July', pending: 10, review: 8, investigation: 5, resolution: 3, escalation: 2, closed: 3 },
        //    { month: 'August', pending: 12, review: 10, investigation: 8, resolution: 9, escalation: 7, closed: 4 },
        //    { month: 'September', pending: 10, review: 9, investigation: 8, resolution: 6, escalation: 5, closed: 4 },
        //    { month: 'October', pending: 12, review: 10, investigation: 10, resolution: 7, escalation: 6, closed: 4 },
        //    { month: 'November', pending: 14, review: 13, investigation: 12, resolution: 11, escalation: 10, closed: 8 },
        //    { month: 'December', pending: 15, review: 14.5, investigation: 13, resolution: 12, escalation: 10, closed: 9 },// конец ноября - начало декабря
        //    { month: ' ', pending: 15, review: 14.5, investigation: 13, resolution: 12, escalation: 10, closed: 9 }// конец декабря
        //];
        var pieChart = $("#chartContainer");
        if (pieChart.length > 1) {
            pieChart = $("#chartContainer").dxPieChart('instance');
            pieChart.clearSelection();
        }
        $("#chartContainer").dxChart({
            dataSource: data,
            series: [
                {
                    argumentField: 'month',
                    valueField: 'pending',
                    type: 'area',
                    color: '#D6F9F6'
                },
                {
                    argumentField: 'month',
                    valueField: 'review',
                    type: 'area',
                    color: '#c0ece8'
                },
                {
                    argumentField: 'month',
                    valueField: 'investigation',
                    type: 'area',
                    color: '#82dad0'
                },
                {
                    argumentField: 'month',
                    valueField: 'resolution',
                    type: 'area',
                    color: '#44c8b9'
                },
                {
                    argumentField: 'month',
                    valueField: 'escalation',
                    type: 'area',
                    color: '#05b5a2'
                },
                {
                    argumentField: 'month',
                    valueField: 'closed',
                    type: 'area',
                    color: '#e5eeee',
                    opacity: '0.8'/*,
                    hoverMode: "none"*/
                }
            ],
            legend: { visible: false },
            argumentAxis: {
                valueMarginsEnabled: false
            }
        });
    }
    
    //------------------------------------- END All cases -----------------------------------------------

    //----------------------------- START Departments ----------------------------
    function _dtCompanyDepartmentReport(data) {
        //example data
        //var dataSource = [{
        //    name: "Sales",
        //    val: 10
        //}, {
        //    name: "Accounting",
        //    val: 18
        //}, {
        //    name: "Marketing",
        //    val: 13
        //}, {
        //    name: "R&D",
        //    val: 3
        //}];
        var pieChart = $("#containerDepartments");
        if (pieChart.length > 1) {
            pieChart = $("#containerDepartments").dxPieChart('instance');
            pieChart.clearSelection();
        }
        $("#containerDepartments").dxPieChart({
            dataSource: data,
            tooltip: {
                enabled: true,
                percentPrecision: 2,
                customizeTooltip: function (arg) {
                    return {
                        text: arg.valueText + " - " + arg.percentText
                    };
                }
            },
            legend: {
                horizontalAlignment: "left",
                verticalAlignment: "top",
                margin: 0
            },
            series: [{
                type: "doughnut",
                argumentField: "name",
                label: {
                    visible: false,
                    connector: {
                        visible: true
                    }
                }
            }]
        });
    }


    //------------------------- END Departments ------------------------------------------
    //depens on one line
    //-------------------------- START Locations ----------------------------------------
    function _dtCompanyLocationReport(data) {
        //example data
        //var dataLocation = [{
        //    name: "Sales",
        //    val: 10
        //}, {
        //    name: "Accounting",
        //    val: 18
        //}, {
        //    name: "Marketing1",
        //    val: 13
        //}, {
        //    name: "Marketing2",
        //    val: 13
        //}, {
        //    name: "Marketing3",
        //    val: 13
        //}, {
        //    name: "R&D",
        //    val: 3
        //}];


        var pieChart = $("#containerLocation");
        if (pieChart.length > 1) {
            pieChart = $("#containerLocation").dxPieChart('instance');
            pieChart.clearSelection();
        }
        
        $("#containerLocation").dxPieChart({
            dataSource: data,
            tooltip: {
                enabled: true,
                percentPrecision: 2,
                customizeTooltip: function (arg) {
                    return {
                        text: arg.valueText + " - " + arg.percentText
                    };
                }
            },
            legend: {
                horizontalAlignment: "left",
                verticalAlignment: "top",
                margin: 0
            },
            series: [{
                type: "doughnut",
                argumentField: "name",
                label: {
                    visible: false,
                    connector: {
                        visible: true
                    }
                }
            }]
        });
    }
    //------------------------ END Locations ------------------------------------------

    var configObject = {
        autoClose: false,
        format: 'YYYY-MM-DD',
        separator: ' to ',
        language: 'en',
        startOfWeek: 'sunday',// or monday
        getValue: function()
        {
            return $(this).val();
        },
        setValue: function(s)
        {
            if(!$(this).attr('readonly') && !$(this).is(':disabled') && s != $(this).val())
            {
                $(this).val(s);
            }
        },
        startDate: false,
        endDate: false,
        time: {
            enabled: false
        },
        minDays: 0,
        maxDays: 0,
        showShortcuts: false,
        shortcuts:
        {
            //'prev-days': [1,3,5,7],
            //'next-days': [3,5,7],
            //'prev' : ['week','month','year'],
            //'next' : ['week','month','year']
        },
        customShortcuts : [],
        inline:false,
        container:'body',
        alwaysOpen:false,
        singleDate:false,
        lookBehind: false,
        batchMode: false,
        duration: 200,
        stickyMonths: false,
        dayDivAttrs: [],
        dayTdAttrs: [],
        applyBtnClass: '',
        singleMonth: 'auto',
        hoveringTooltip: function(days, startTime, hoveringTime)
        {
            return days > 1 ? days + ' ' + lang('days') : '';
        },
        showTopbar: true,
        swapTime: false,
        selectForward: false,
        selectBackward: false,
        showWeekNumbers: false,
        getWeekNumber: function(date) //date will be the first day of a week
        {
            return moment(date).format('w');
        }
    }
    $('#daterange').dateRangePicker(configObject);
    $('.daterange.menuItem').click(function (evt) {
        evt.stopPropagation();
        $("#daterange").show();
        $('#daterange').data('dateRangePicker').open();
        $("#daterange").hide();
        $(".prev").html("");
        $(".next").html("");
    });
    $(".clearAll").on('click', function (event) {
        $(event.currentTarget).hide();
        $(".selected").each(function (index, element) {
            $(element).removeClass('selected');
        });
        $(".sales").each(function (index, element) {
            var temp = $(element);
            temp.closest('th').remove();
        });
        updateGraphics();
    })
});