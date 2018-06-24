$(document).ready(function () {
    //drop down list
    //first start
    updateGraphics();

    $('.liItem').on("click", function (event) {
        var temp = $(event.currentTarget);
        if (temp.hasClass('selected')) {
            temp.removeClass('selected');
        } else {
            var allOptions = temp.parents('ul');

            temp.addClass('selected');
        }
        //updateFilter(temp);
        updateGraphics();
    });

    function updateGraphics() {
        var userId = $("#user_id");
        var companyId = $("#companyId");
        var types = {};
        $("#filterTable").html('');
        $(".menuItem").each(function (indx, element) {
            var selected = $(element).find('.selected');
            if (selected.length >= 1) {
                var textDropDown = $(element).find(".dropMenuText").text();
                
                var namedropdown = selected[0].getAttribute("namedropdown");
                types[namedropdown] = "";
                selected.each(function (indx, element) {
                    var temp = $(element);
                    upadteFilterAgain(textDropDown, temp);
                    if (temp.length > 0) {
                        types[namedropdown] = concatArrayString(types[namedropdown], temp.attr('value'));
                    }
                });
            }
        });
        /*try find calendar*/
        types['dateStart'] = $("#dateStart").attr("value");
        types['dateEnd'] = $("#dateEnd").attr("value");
        
        sendAjax(userId.val(), companyId.val(), types);

        if (($("#dateStart").val() !== '') && ($("#dateEnd").val() !== '')) {
            var html = '<tr name="Date">';
            html += '<td>Date</td><td>';
            html += "" + moment(parseInt($("#dateStart").val())).format('DD/MM/YYYY');
            html += " - ";
            html += "" + moment(parseInt($("#dateEnd").val())).format('DD/MM/YYYY');
            html += '</td><td class="clearAll clearDate">Clear<img src="/Content/Icons/Xanal.gif" /></td> </tr>';

            if ($('#filterTable > tbody > tr:first').length == 0) {
                $("#filterTable").append(html);
            } else {
                $(html).insertBefore('#filterTable > tbody > tr:last');
            }
            $('.clearDate').off().on('click', function () {
                $("tr [name='Date']").remove();
                $("#dateStart").val('');
                $("#dateEnd").val('');
                updateGraphics();
            })
        }
    }
    function upadteFilterAgain(textDropDown, elementSelected) {
        var table = $("#filterTable");
        var flag = true;

        table.find('tr').each(function (indx, element) {
            var temp = $(element);
            if (temp.attr('name') == textDropDown) {
                flag = false;
                var some = temp.find('.sales');///найти куда вставлять паралельно с div class sales
                some = some.parent();
                some.append(addTdSales(elementSelected.text()));
                //здесь добавим 
            }
        });
        if (flag) {
            if ($("#clearAll").length > 0) {
                $("#clearAll").remove();
            }
            table.append(addTrNew(textDropDown, elementSelected) + '<tr><td><div id="clearAll" class="clearAll">Clear all<img src="/Content/Icons/Xanal.gif"></div></td></tr>');
        }
        $('.clearAll').unbind('click');
        $('.clearAll').on('click', function (event) {
            clearRowItems($(event.currentTarget));
        });
        $('.sales').unbind('click');
        $('.sales').on('click', function (event) {
            clearItemOne($(event.currentTarget));
        });
        $("#clearAll").on('click', function () {
            clearAll();
            $("#dateStart").val('');
            $("#dateEnd").val('');
            updateGraphics();
        });
    }
    function clearItemOne(item) {
        var nameCurrentItem = item.text().trim();
        var nameGroup = item.parents('tr').attr('name');
        if (nameCurrentItem != "" && nameGroup != "") {
            $(".dropMenuText").each(function (indx, element) {
                if (nameGroup == $(element).text().trim()) {
                    var menuItem = $(element).parents('.menuItem');
                    var ul = menuItem.find('.menuUl li');
                    ul.each(function (indx, element) {
                        if ($(element).text().trim() == nameCurrentItem) {
                            $(element).find('.liItem').removeClass('selected');
                        }
                    });
                }
            });
        }
        item.unbind('click');
        item.remove();
        if ($('.sales').length == 0) {
            $("#filterTable").html('');
        }
    }
    function clearRowItems(item) {
        var nameCurrentItem = item.text().trim();
        var nameGroup = item.parents('tr').attr('name');
        if (nameCurrentItem != "" && nameGroup != "") {
            $(".dropMenuText").each(function (indx, element) {
                if (nameGroup == $(element).text().trim()) {
                    var menuItem = $(element).parents('.menuItem');
                    menuItem.find('.selected').removeClass('selected');
                }
            });
        }
        item.parent().remove();
        if ($('.sales').length == 0) {
            $("#filterTable").html('');
        }
        updateGraphics();
    }
    function clearAll() {
        var allSelected = $(".selected");
        if (allSelected.length > 0) {
            allSelected.each(function (indx, element) {
                $(element).removeClass('selected');
            });
            $("#filterTable").html("");
        }
    }

    function addTrNew(textDropDown, elementSelected) {
        var html = '<tr name="' + textDropDown + '">';
        html += '<td>' + textDropDown + '</td><td>';
        html += addTdSales(elementSelected.text().trim());
        html += '</td><td class="clearAll">Clear<img src="/Content/Icons/Xanal.gif" /></td> </tr>';
        return html;
    }
    function addTdSales(name) {
        var str = '<div class="sales">';
        str += name;
        str += '<img src="/Content/Icons/Xanal.gif"></div>';
        return str;
    }
    function concatArrayString(str, param) {
        if (str != "" && str.length > 1) {
            str += ',' + param;
        } else {
            str = param;
        }
        return str;
    }
    let totalDaysAll = {};
    let totalDaysSettings = {};
    let totalDays = {};




    //drop down list
    function sendAjax(userId, companyId, types) {
        if (userId && companyId) {
            $.ajax({
                method: "POST",
                url: "/Analytics/CompanyDepartmentReportAdvanced",
                data: { companyId: companyId, userId: userId, types: types }
            }).done(function (data) {//data from server
                var temp = $.parseJSON(data);

                if (temp['LocationTable'] != null && temp['LocationTable'].length >= 1) {
                    _dtCompanyLocationReport(temp['LocationTable']);
                    addValDep($("#containerLocation tspan"), temp['LocationTable']);
                }
                if (temp['DepartmentTable'] != null && temp['DepartmentTable'].length > 1) {
                    _dtCompanyDepartmentReport(temp['DepartmentTable']);
                    addValDep($("#containerDepartments tspan"), temp['DepartmentTable']);
                }
                if (temp["SecondaryTypeTable"] != null && temp["SecondaryTypeTable"].length > 1) {
                    _dtCompanySecondaryTypeReport(temp['SecondaryTypeTable']);
                    addValDep($("#containerTypesOfIncident tspan"), temp['SecondaryTypeTable']);
                }
                if (temp["RelationTable"] != null && temp["RelationTable"].length > 1) {
                    _dtCompanyRelationTypeReport(temp['RelationTable']);
                    addValDep($("#containerTypeOfReporter tspan"), temp['RelationTable']);
                }
                if (temp["AnalyticsTimeline"] != null && temp["AnalyticsTimeline"].length > 1) {
                    _dtAnalyticsTimeline(temp["AnalyticsTimeline"]);
                }
                if (temp["MonthEndSnapshotTable"] != null && temp["MonthEndSnapshotTable"].length > 1 && temp["TodaySnapshotTable"] != null && temp["TodaySnapshotTable"].length > 1 && temp["MonthEndSnapshotTable"].length == temp["TodaySnapshotTable"].length) {
                    var items = $(".blockTodayContant .itemLegend");
                    for (var i = 0; i < temp["MonthEndSnapshotTable"].length; i++) {
                        var currentItem = $(items[i]);
                        var today = currentItem.find('label');
                        var currentMonthEnd = currentItem.find('.closed');
                        today.text(temp["TodaySnapshotTable"][i]);
                        currentMonthEnd.text(temp["MonthEndSnapshotTable"][i]);
                    }
                }
                var averOfDays = temp['AverageStageDaysTable'];

                var preReview = averOfDays[0].val;
                var review = averOfDays[1].val;
                var investigation = averOfDays[2].val;
                var resolution = averOfDays[3].val;
                var escalation = averOfDays[4].val;

                
                let totalDaysAll = {};
                let totalDaysSettings = {};
                let totalDays = {};

                let reviewSettings = parseInt($("#step1_delay").attr("value"));
                let reviewInvestigation = parseInt($("#step2_delay").attr("value"));
                let reviewResolution = parseInt($("#step3_delay").attr("value"));
                let reviewEscalation = parseInt($("#step4_delay").attr("value"));



                totalDaysSettings.reviewSettings = reviewSettings;
                totalDaysSettings.reviewInvestigation = reviewInvestigation;
                totalDaysSettings.reviewResolution = reviewResolution;
                totalDaysSettings.reviewEscalation = reviewEscalation;

                totalDays.preReview = preReview;
                totalDays.review = review;
                totalDays.investigation = investigation;
                totalDays.resolution = resolution;
                totalDays.escalation = escalation;

                totalDaysAll.totalDaysSettings = totalDaysSettings;
                totalDaysAll.totalDays = totalDays;

                AverageOfDaysPerStageBackUp();
                //var init = {
                //    "review": review,
                //    "investigation": investigation,
                //    "resolution": resolution,
                //    "escalation": escalation
                //};


                //init(review, investigation, resolution, escalation);
                init(totalDaysAll);
                initGreyBlocks(totalDaysAll);

                addNumberCenter();
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
    //initGreyBlocks();
    function initGreyBlocks(totalDaysAll) {
        //var review = 2;
        //var investigation = 19;
        //var resolution = 3;
        //var escalation = 3;

        var table = $(".wrapper table");
        var backup = $("#base");
        backup.html("");
        backup.html(table.html());

        var reviewTr = $(table.find('tr')[1]);
        var investigationTr = $(table.find('tr')[2]);
        var resolutionTr = $(table.find('tr')[3]);
        var escalationTr = $(table.find('tr')[4]);
        buffer = 0;
        putBlockGrey(buffer, totalDaysAll.totalDaysSettings.reviewSettings, reviewTr, 1);
        putBlockGrey(buffer, totalDaysAll.totalDaysSettings.reviewInvestigation, investigationTr, 2);
        putBlockGrey(buffer, totalDaysAll.totalDaysSettings.reviewResolution, resolutionTr, 3);
        putBlockGrey(buffer, totalDaysAll.totalDaysSettings.reviewEscalation, escalationTr, 4);
    }

    function putBlockGrey(start, end, block, indexBlock) {
        buffer += end;
        for (var i = start + 1; i <= buffer; i++) {
            if (block) {
                var temp = $(block.find('td')[i]);
                if (temp) {
                    let arrayTd = $(".wrapper table tr:first td");
                    //if (i == (start + 1)) {
                    //    //это первый блок нужна цифирка сверху
                    //    let first = $(arrayTd[i]);
                    //    first.append('<div class="parentDivHeader"><div class="headerBrown"> ' + i + ' </div></div>');
                    //}

                    //надеюсь последний блок
                    if (i == buffer) {
                        if (indexBlock == 4) {
                            let first = $(arrayTd[i]);
                            first.append('<div class="base greyBase"><div class="letters">' + buffer + '<span class="specialBorder">&nbsp;</span></div></div>');
                        } else {
                            let first = $(arrayTd[i]);
                            first.append('<div class="parentDivHeader"><div class="headerBrown"> ' + i + ' <span class="specialBorder">&nbsp;</span></div></div>');
                        }

                    }

                    /*что то связано полоски рисующиея в начале и конце оранжевых блоков*/
                    //if (i == start + 1) {
                    //    addBorderHead(temp, i, 'startRedLine');
                    //}
                    //if (i == buffer) {
                    //    addBorderHead(temp, buffer, 'endRedLine');
                    //}

                    /*здесь вставим в ячейку парент див и броун див*/
                    if (temp.find('.parentDiv').length > 0) {
                        temp.find('.parentDiv').append("<div class='brownDiv'></div>");
                    } else {
                        temp.append("<div class='parentDiv'><div class='brownDiv'></div></div>");
                    }
                    

                    
                    //var parentDiv = temp.find('.parentDiv');
                    //var className = "redDiv";
                    //if (indexBlock >= 3) {
                    //    className = "greenDiv";
                    //}
                    //if (parentDiv.length > 0) {
                    //    parentDiv.append("<div class='" + className + "'></div>");
                    //} else {
                    //    var parentDivHtml = "<div class='parentDiv'><div class='" + className + "'></div></div>"
                    //    temp.append(parentDivHtml);
                    //}
                }
            }
        }
    }

    function init(totalDaysAll) {

        let review = totalDaysAll.totalDays.review;
        let investigation = totalDaysAll.totalDays.investigation;
        let resolution = totalDaysAll.totalDays.resolution;
        let escalation = totalDaysAll.totalDays.escalation;

        


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
        buffer = 0;

        var reviewTr = $(table.find('tr')[1]);
        var investigationTr = $(table.find('tr')[2]);
        var resolutionTr = $(table.find('tr')[3]);
        var escalationTr = $(table.find('tr')[4]);
        //try set color of line 
        if (totalDaysAll.totalDaysSettings.reviewSettings >= review) {
            putBlock(buffer, review, reviewTr, 1, "greenDiv");
        } else {
            putBlock(buffer, review, reviewTr, 1, "redDiv");
        }

        if (totalDaysAll.totalDaysSettings.reviewInvestigation >= investigation) {
            putBlock(buffer, investigation, investigationTr, 2, "greenDiv");
        } else {
            putBlock(buffer, investigation, investigationTr, 2, "redDiv");
        }
        
        if (totalDaysAll.totalDaysSettings.reviewResolution >= resolution) {
            putBlock(buffer, resolution, resolutionTr, 3, "greenDiv");
        } else {
            putBlock(buffer, resolution, resolutionTr, 3, "redDiv");
        }

        if (totalDaysAll.totalDaysSettings.reviewEscalation >= escalation) {
            putBlock(buffer, escalation, escalationTr, 4, "greenDiv");
        } else {
            putBlock(buffer, escalation, escalationTr, 4, "redDiv");
        }
    }
    function putBlock(start, end, block, indexBlock, color) {
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
                    var className = color;
                    //var className = "redDiv";
                    //if (color == "green") {
                    //       className = "greenDiv";
                    //}
                    //var className = "redDiv";
                    //if (indexBlock >= 3) {
                    //    className = "greenDiv";
                    //}
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
                    let curItem = $(temp);
                    let arrayTd = $(".wrapper table tr:first td");
                    let base = arrayTd.find('.base .letters');
                    if (base.length > 0) {
                        let greyValue = parseInt(base.text());
                        if (index == greyValue) {
                            //если серая штука и зеленая совпадает, серую удалим
                            curItem.find('.base').removeClass("greyBase");
                        } else if (index < greyValue) {
                            //если вложились в срок
                            curItem.append('<div class="base"><div class="letters">' + index + '</div></div>');
                        } else if (index > greyValue) {
                            //Если не вложились
                            //curItem.find('.base').removeClass("greyBase").addClass("orangeBase");
                            curItem.append('<div class="base orangeBase"><div class="letters">' + index + '</div></div>');
                        }
                    }

                    
                    //if (curItem.find('.base').length > 0) {
                        
                    //} else {
                        
                    //}
                    
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
                    color: '#d47472'
                },
                {
                    argumentField: 'month',
                    valueField: 'review',
                    type: 'area',
                    color: '#ff9b42'
                },
                {
                    argumentField: 'month',
                    valueField: 'investigation',
                    type: 'area',
                    color: '#3099be'
                },
                {
                    argumentField: 'month',
                    valueField: 'resolution',
                    type: 'area',
                    color: '#64cd9b'
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
    function addValDep(dep, data) {
        for (var i = 0; i < dep.length; i++) {
            let temp = dep[i].textContent.trim();
            for (var j = 0; j < data.length; j++) {
                if (data[j].name.trim() === temp) {
                    dep[i].textContent = temp + " - " + data[j].val;
                    data.splice(j, 1);
                    break;
                }
            }
        }
    }

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
            export: {
                enabled: true,
            },
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
                columnCount: 1,
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
            }],
            size: {
                width: 600,
                height: 400,
            },
        });
    }


    //------------------------- END Departments ------------------------------------------
    //depens on one line
    //-------------------------- START Locations ----------------------------------------
    function _dtCompanyLocationReport(data) {
        var pieChart = $("#containerLocation");
        if (pieChart.length > 1) {
            pieChart = $("#containerLocation").dxPieChart('instance');
            pieChart.clearSelection();
        }
        
        $("#containerLocation").dxPieChart({
            dataSource: data,
            export: {
                enabled: true,
            },
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
                columnCount: 1,
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
            }],
            size: {
                width: 600,
                height: 400,
            },
        });
    }
    //------------------------ END Locations ------------------------------------------
    function _dtCompanySecondaryTypeReport(data) {
        console.log('sec', data);

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
        var pieChart = $("#containerTypesOfIncident");
        if (pieChart.length > 1) {
            pieChart = $("#containerTypesOfIncident").dxPieChart('instance');
            pieChart.clearSelection();
        }
        $("#containerTypesOfIncident").dxPieChart({
            dataSource: data,
            export: {
                enabled: true,
            },
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
                columnCount: 1,
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
            }],
            size: {
                width: 600,
                height: 400,
            },
        });
    }

    function _dtCompanyRelationTypeReport(data) {
        console.log('rel', data);
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
        var pieChart = $("#containerTypeOfReporter");
        if (pieChart.length > 1) {
            pieChart = $("#containerTypeOfReporter").dxPieChart('instance');
            pieChart.clearSelection();
        }
        $("#containerTypeOfReporter").dxPieChart({
            dataSource: data,
            export: {
                enabled: true,
            },
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
                columnCount: 1,
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
            }],
            size: {
                width: 600,
                height: 400,
            },
        });
    }

    var configObject = {
        parentEl: '.daterange.menuItem',
        container: '.daterange.menuItem',
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
        //hoveringTooltip: function(days, startTime, hoveringTime)
        //{
        //    return days > 1 ? days + ' ' + /*lang('days')*/123 : '';
        //},
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
        $("#daterange").bind('datepicker-change', function (event, obj) {
            $("#dateStart").attr("value", obj.date1.getTime());
            $("#dateEnd").attr("value", obj.date2.getTime());
            $("#daterange").data('dateRangePicker').close();
            updateGraphics();
        });
        /*var dateStart = "";
        var dateEnd = "";
        $(".month1 div").on('click', function (event) {
            var temp = $(event.currentTarget).attr('time');
            console.log(temp);
            if (dateStart == "" ) {
                dateStart = temp;
            } else if(dateStart != "" && dateStart < temp) {
                dateEnd = dateStart;
                dateStart = temp;
            } else if (dateStart != "" && dateStart > temp) {
                dateEnd = temp;
            }
            if (dateStart != "" && dateEnd != "") {
                $(".apply-btn").click();
                $("#dateStart").attr("value", dateStart);
                $("#dateEnd").attr("value", dateEnd);
                updateGraphics();
            }
        });*/
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

    function PrintElem(elem) {
        var mywindow = window.open('', 'PRINT', 'height=400,width=600');

        mywindow.document.write('<html><head><title>' + document.title + '</title>');
        mywindow.document.write('</head><body >');
        mywindow.document.write('<h1>' + document.title + '</h1>');
        mywindow.document.write(elem.innerHTML);
        mywindow.document.write('</body></html>');

        mywindow.document.close(); // necessary for IE >= 10
        mywindow.focus(); // necessary for IE >= 10*/

        mywindow.print();
        mywindow.close();

        return true;
    }
    $('#blockTypeOfIncidentPrint').on('click', function () {
        PrintElem($('.blockTypeOfIncident')[0]);
    });
});