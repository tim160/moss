$(document).ready(function () {
    $(".contentBlock").focus(function (event) {
        var temp = $(event.currentTarget);
        if (temp.parents('.rightBlock').hasClass('rightBlock_cc')) {
            temp.css("border-color", "#4da456");
        }
        else {
            temp.css("border-color", "rgb(5, 181, 162)");
        }
    })
        .on('focusout', function (event) {
            var temp = $(event.currentTarget);
            temp.css("border-color", "transparent transparent #E5EBEB transparent");
        });
    $(".contentBlock textarea").focus(function (event) {
        var temp = $(event.currentTarget);

        if (temp.parents('.rightBlock').hasClass('rightBlock_cc')) {
            temp.parents('.contentBlock').css("border", "3px solid #4da456");
        }
        else {
            temp.parents('.contentBlock').css("border", "3px solid rgb(5, 181, 162)");
        }
    })
        .on('focusout', function (event) {
            var temp = $(event.currentTarget);
            temp.css("border", "3px solid transparent transparent #E5EBEB transparent");
        });


    var generalInfo = $('.tab.generalInfo');
    var passiveCircle = $('.tab.partiesInvolved');
    var caseInformation = $('.tab.caseInformation');
    var mediatorsAssigned = $('.tab.mAssigned');
    var caseSummary = $('.tab.caseSummary');
    var gInfoBtn = $('.tabOption.gInfo');
    var pInvolvedBtn = $('.tabOption.pInvolved');
    var cInformationBtn = $('.tabOption.cInformation');
    var mAssignedBtn = $('.tabOption.mAssignedButton');
    var cSummaryBtn = $('.tabOption.cSummary');
    var submitCaseSummary = caseSummary.find('#SubmitCase');
    var caseSubmitted = caseInformation.find('.result');

    var verticalMenu = $('.verticalMenu li');

    var currentStep = $('.steps .selectedStep');

    var avaible = ['1'];
    var tmplStoredge = $('#tmpl');

    var attachmentsFiles = [];

    function base() {
        vl.init($('form'));
        //   cmp.CustomSelect(generalInfo.find('select.reportFrom'));
    }

    function setOpacity() {
        verticalMenu.each(function () {
            if (avaible.indexOf($(this).attr('data-id')) == -1) {
                $(this).css('opacity', '0.2');
            }
            else {
                $(this).css('opacity', '1');
            }
        });

        currentStep.each(function () {
            if (avaible.indexOf($(this).attr('data-val')) == -1) {
                $(this).css('opacity', '0.2');
            }
            else {
                $(this).css('opacity', '1');
            }
        });
    }

    function stepChecked(step) {
        verticalMenu.each(function () {
            if (avaible.indexOf($(this).attr('data-id')) == step) {
                $(this).addClass('stepDone');
            }
        });
    }

    function addToAvaible(id) {
        for (var i = 1; i <= id; i++) {
            if (avaible.indexOf('' + i) == -1) {
                avaible.push('' + i);
            }
        }

    }
    function removeFromAvaible(id) {

        if (avaible.indexOf('' + id) != -1) {
            avaible.pop('' + id);
        }
    }


    function generalInfoProcess() {
        var yesBlock = generalInfo.find('.yesBlock');
        var noBlock = generalInfo.find('.noBlock');
        var caseDescription = generalInfo.find('.caseDescription');
        var partiesInvolvedBtn = generalInfo.find('.partiesInvolvedBtn');
        var reporterType = generalInfo.find('.reporterType');
        var reportFrom = generalInfo.find('.reportFrom');
        var anType = generalInfo.find('.anType');
        var locationsOfIncident = generalInfo.find('.locationsOfIncident');
        var departmentInvolvedContainer = generalInfo.find('.departmentInvolvedContainer');
        var otherIncident = generalInfo.find('input[name="radioName"]');
        $("input[name=isReportRefered][value=Other]").parents('.freeBlock').on('click', function (event) {
            var temp = $(event.currentTarget).find('input');
            temp.attr("checked", "checked");
            //temp.find("input[type=radio]").click();
        });
        generalInfo.find('.validate').change(function () {
            var button = generalInfo.find('.nextSepButton.tabOption.pInvolved');// go to next step
            if (vl.check(generalInfo)) {
                button.addClass('active');
            } else {
                button.removeClass('active');
            }
        });

        otherIncident.on('change', function () {
            if (($(this).val().toLowerCase() == "other") || ($(this).val().toLowerCase().indexOf("former") > -1)) {
                $('.explainReportType').addClass('validate');
                $('.contentBlock.incident').show();
                $('.contentBlock.incident').find('textarea').focus();
            }
            else {
                $('.explainReportType').removeClass('validate');
                $('.explainReportType').removeClass('vlError');
                $('.contentBlock.incident').hide();
            }
        });
        generalInfo.find('.reportFrom').change(function () {
            var report = generalInfo.find('#ReportingFromSelect');
            report.empty();
            var array = generalInfo.find('.countries');
            array.hide();

            var countryId = generalInfo.find(".reportFrom").val();
            var activeElement2 = generalInfo.find(".reportFrom :selected").val();

            if (countryId) {

                if (countryId == 198 || countryId == 171) {
                    /* to emu anonimnuu opciu ( Completely Confidential)  nado ubrat ( eto luchshe) ili disable ( eto huje). */
                    var temp = $(".rowBlock.anType");
                    temp.removeClass("vlCorrect");
                    temp.find("input name[incident_anonymity_id]").removeClass("vlCorrect");
                    temp.find("div[data-val=1]").hide();
                    $('.anType div[data-val=2] input').click();
                } else {
                    $(".rowBlock.anType div[data-val=1]").show();
                }
                array.each(function (indx, element) {
                    element = $(element);
                    if (element.attr('data-value') === countryId) {
                        element.show();
                        $('.description').append(element);
                        if (element.html().length > 100) {
                            $(".description").css({ "width": "95%", "max-width": "none" });
                        } else {
                            $(".description").css({ "width": "85%", "max-width": "410px" });
                        }
                        return false;
                    }
                });
            }
            /* change Confidential, Confidential to Stark, Private Info.... */
            $.ajax(
                {
                    method: "POST",
                    cache: false,
                    data: { countryId: countryId },
                    url: "/Report/getAjaxCountry",
                }
            ).done(function (data) {//data from server
            }).fail(function (error) {
                console.log(error);
            });
            /**/

        });

        caseDescription.change(function () {
            if (caseDescription.val() !== '') {
                partiesInvolvedBtn.show();
                generalInfo.find('.isPreviusReported').hide();
            }
        });
        cmp.OptionSelect(generalInfo.find('.isPreviusReported'), function (val) {
            if (val == 'true') {
                yesBlock.show();
                noBlock.hide();
                partiesInvolvedBtn.show();
            }
            else {
                noBlock.show();
                yesBlock.hide();
                generalInfo.find('.isPreviusReported').hide();
                partiesInvolvedBtn.show();
            }
        });

        cmp.OptionSelect(reporterType);
        partiesInvolvedBtn.hide();
        cmp.OptionSelect(anType);
        var lastDepartmentNum = 1;
        var departmenBuff = tmplStoredge.find('#departmentInvolvedTmpl');
        if (departmenBuff.length > 0) {
            departmenBuff = departmenBuff.html().trim();
        }

            
        var departmentInvolvedNum = generalInfo.find('.departmentInvolvedNum');
        function delecteDeertmentSelect(self) {
            if (departmentInvolvedContainer.find('select').length == 1) return;
            self.parent().remove();
            departmentInvolvedContainer.find('select').unbind('change');
            departmentInvolvedContainer.find('select:last').change(function () {
                processDepertmeptSelect($(this));
            });
        }

        function processDepertmeptSelect(self) {
            self.unbind('change');
            var newItem = $(departmenBuff);
            newItem.change(function () {
                processDepertmeptSelect($(this));
            });
            //newItem.find('.selected').text('Add another department');
            vl.init(newItem);
            newItem.find('.delete').click(function () {
                delecteDeertmentSelect($(this));
            });
            newItem.find('input').attr('name', newItem.find('input').attr('name') + lastDepartmentNum);
            departmentInvolvedNum.val(lastDepartmentNum);
            lastDepartmentNum++;
            departmentInvolvedContainer.append(newItem);
            setDropdown();
        }

        var firstdepartmentselect = generalInfo.find('.departmentInvolved');
        firstdepartmentselect.parent().find('.delete').click(function () {
            delecteDeertmentSelect($(this));
        });
        firstdepartmentselect.change(function () {
            processDepertmeptSelect($(this));
        });
        firstdepartmentselect.attr('name', firstdepartmentselect.attr('name') + lastDepartmentNum);
        lastDepartmentNum++;
        departmentInvolvedNum.val(lastDepartmentNum);

        reportFrom.on('change', function () {
            // need to change anonymity here.
            var country_id = $(this).val();
            var currentCompanyId = $(document.getElementById('currentCompanyId')).val();

            var str_anon = $(document.getElementById('str_anon'));
            var str_anon_val = getAnon(currentCompanyId, country_id);
            str_anon.val(str_anon_val);
            //         anonymityStatus.empty();
            //          anonymityStatus.append(generalInfo.find("input:radio[name=stay]:checked").val());


        });


    }

    function taSwitcher() {
        function hideTabs() {
            generalInfo.hide();
            passiveCircle.hide();
            caseInformation.hide();
            caseSummary.hide();
            mediatorsAssigned.hide();
            gInfoBtn.removeClass('active');
            pInvolvedBtn.removeClass('active');
            cInformationBtn.removeClass('active');
            cSummaryBtn.removeClass('active');
        }

        function menuItemClicked(id) {
            id = '' + id;
            verticalMenu.each(function () {
                if ($(this).attr('data-id') == id) {
                    var liElement = $(this);
                    var content = mediatorsAssigned.find('.content');
                    $('.tab').each(function () {

                        if ($(this).attr('data-id') == liElement.attr('data-id') && (avaible.indexOf(liElement.attr('data-id')) != -1)) {
                            if (liElement.attr('data-id') == '4') {
                                //Choose confidentiality level:
                                var displayBlock = mediatorsAssigned.find("#hideIfConfidential");
                                var value = generalInfo.find("input[name=incident_anonymity_id]").val();
                                if (value === 1) {
                                    displayBlock.hide();
                                } else if (value == 3) {

                                }

                                //General Information
                                var temp = content.find('#reportingFrom');
                                temp.empty();
                                temp.append($('.reportingFrom span').text().replace(/</g, "&lt;").replace(/>/g, "&gt;"));

                                var anonymityStatus = content.find('#anonymityStatus');
                                anonymityStatus.empty();
                                anonymityStatus.append(generalInfo.find("input:radio[name=stay]:checked").val().replace(/</g, "&lt;").replace(/>/g, "&gt;"));

                                var formalRole = content.find('#formalRole');
                                formalRole.empty();
                                formalRole.append(generalInfo.find("input:radio[name=radioName]:checked").siblings('.radioTitle').text().replace(/</g, "&lt;").replace(/>/g, "&gt;"));
                                var generalInfoTher = generalInfo.find("input:radio[name=radioName]:checked").val().toLowerCase().replace(/</g, "&lt;").replace(/>/g, "&gt;");

                                if (generalInfoTher == "other" ||
                                    generalInfoTher == "former employee") {
                                    var html = $('.explainReportType').val().replace(/</g, "&lt;").replace(/>/g, "&gt;");
                                    formalRole.append(', ' + html);
                                }

                                var happenedIn = content.find('#incedentHappenedIn');
                                happenedIn.empty();
                                if ($('.location span').text() == "Not listed") {
                                    happenedIn.append("Not listed : " + $('.otherLocation textarea').val().replace(/</g, "&lt;").replace(/>/g, "&gt;"));
                                } else
                                    happenedIn.append($('.location span').text().replace(/</g, "&lt;").replace(/>/g, "&gt;"));

                                //departments
                                var departments = content.find('#departments');
                                departments.empty();
                                var elem = 0;
                                $('.departmentInvolved').each(function () {
                                    elem++;
                                    if ($(this).parent().find('span').text() != "Select department") {
                                        if (elem > 1) departments.append(", ");
                                        departments.append($(this).parent().find('span').text().replace(/</g, "&lt;").replace(/>/g, "&gt;"));
                                    }
                                });

                                //Parties involved
                                var isManagementKnow = content.find('#isManagementKnow');
                                isManagementKnow.empty();
                                isManagementKnow.append($(' input[name=managementKnow]:checked').siblings(".radioTitle").text().replace(/</g, "&lt;").replace(/>/g, "&gt;"));
                                if ($(' input[name=managementKnow]:checked').val() == "Yes" ||
                                    $(' input[name=managementKnow]:checked').val() == "Do not want to involve") {
                                    var html = $('.managementIsKnown').val().replace(/</g, "&lt;").replace(/>/g, "&gt;");
                                    isManagementKnow.append(".<br />" + html);
                                }
                                var report_by_myself = content.find("#report_by_myself");
                                report_by_myself.empty();
                                report_by_myself.append($('input[name=report_by_myself]:checked').siblings(".radioTitle").text().replace(/</g, "&lt;").replace(/>/g, "&gt;"));
                                //if (generalInfo.find("input:radio[name=managementKnow]:checked").val() === "No") {
                                if ($("input:radio[name=managementKnow]:checked").val() === "No") {
                                    content.find('#managementFalse').show();
                                }
                                else {
                                    content.find('#managementTrue').show();
                                }

                                var witnessPersone = $('.witnessPersone');
                                $('.partiesInvolvedBlock').empty();
                                witnessPersone.each(function (indx, element) {
                                    var temp = $(element);
                                    var fname = temp.find('.fName').val().replace(/</g, "&lt;").replace(/>/g, "&gt;");
                                    var lName = temp.find('.lName').val().replace(/</g, "&lt;").replace(/>/g, "&gt;");
                                    var witnessRole = temp.find('.selectedRoleInReport option:selected').text().replace(/</g, "&lt;").replace(/>/g, "&gt;");
                                    var witnessTitle = temp.find('.witnessTitle').val().replace(/</g, "&lt;").replace(/>/g, "&gt;");

                                    if (temp.parent().hasClass('addPersonContainer')
                                        && fname != "" && lName != "" && witnessRole != "" && witnessTitle != "") {
                                        $('.partiesInvolvedBlock')
                                            .append("<div class='FirstLastNames'>" +
                                                fname + '&nbsp;' +
                                                lName + "</div>");

                                        $('.partiesInvolvedBlock')
                                            .append("<div class='titleReport'>" +
                                                witnessTitle + "</div>");

                                        $('.partiesInvolvedBlock')
                                            .append("<div class='reportText'>" +
                                                witnessRole + "</div>");
                                    }
                                });


                                var isReportedOutside = content.find('#isReportedOutside');
                                isReportedOutside.empty();
                                var isReportRefered = $(' input[name=isReportRefered]:checked').val();
                                if (isReportRefered) {
                                    isReportedOutside.append(isReportRefered.replace(/</g, "&lt;").replace(/>/g, "&gt;"));
                                }
                                if ($(".whoKnow .active input").val() != "No") {
                                    var html = $('.isReportedOutside').val().replace(/</g, "&lt;").replace(/>/g, "&gt;");
                                    isReportedOutside.append($(".whoKnow .active input").val().replace(/</g, "&lt;").replace(/>/g, "&gt;") + "<br />" + html);
                                } else {
                                    isReportedOutside.append("<br /> No");
                                }

                                var isReportUrgent = content.find('#isReportUrgent');
                                isReportUrgent.empty();
                                isReportUrgent.append($('.isReportUrgent input[name=isUrgent]:checked').siblings('.radioTitle').text().replace(/</g, "&lt;").replace(/>/g, "&gt;"));

                                //Case Information
                                $('.addedFilesList').empty();
                                for (var i = 0; i < attachmentsFiles.length; i++) {
                                    $('.addedFilesList').append("<img src=/Content/Icons/generic-file.png><span id=attachedFiles>" + attachmentsFiles[i].name.replace(/</g, "&lt;").replace(/>/g, "&gt;") + "</span><br>");
                                }
                                if (attachmentsFiles.length == 0) {
                                    $(".attachedFilesTitle").hide();
                                } else {
                                    $(".attachedFilesTitle").show();
                                }
                                var reportAbout = content.find('#reportAbout');
                                reportAbout.empty();
                                //reportAbout.append($("input:radio[name=whatHappened]:checked").val());
                                var str = "";
                                $("input:checkbox[name=whatHappened]:checked").each(function (indx, element) {
                                    if (indx + 1 == $("input:checkbox[name=whatHappened]:checked").length) {
                                        str += element.id;
                                    } else {
                                        str += element.id + ', ';
                                    }
                                });
                                reportAbout.append(str);

                                var incidentHappendDate = content.find('#incidentHappendDate');
                                incidentHappendDate.empty();
                                incidentHappendDate.append($('#datepicker').val().replace(/</g, "&lt;").replace(/>/g, "&gt;"));


                                var isIncidentOngoing = content.find('#isIncidentOngoing');
                                isIncidentOngoing.empty();
                                isIncidentOngoing.append($(' input[name=isIncidentOngoing]:checked').siblings('.radioTitle').text().replace(/</g, "&lt;").replace(/>/g, "&gt;"));

                                if ($(' input[name=isIncidentOngoing]:checked').val() == "Yes") {
                                    var html = $('.incidentOngoingDescription').val().replace(/</g, "&lt;").replace(/>/g, "&gt;");
                                    isIncidentOngoing.append(".<br />" + html);
                                }

                                var hasInjury = content.find('#hasInjury');
                                hasInjury.empty();
                                hasInjury.append($(' input[name=resultInjury]:checked').siblings('.radioTitle').text().replace(/</g, "&lt;").replace(/>/g, "&gt;"));

                                if ($(' input[name=resultInjury]:checked').val() == "Yes") {
                                    hasInjury.append(".<br />" + $('#injury_damage').val().replace(/</g, "&lt;").replace(/>/g, "&gt;"));
                                }

                                var incidentDescription = content.find('#incidentDescription');
                                incidentDescription.empty();
                                //incidentDescription.append(generalInfo.find('#describeHappened').val());
                                incidentDescription.append($('#describeHappened').val().replace(/</g, "&lt;").replace(/>/g, "&gt;"));

                                //content.find('#reportingFrom').
                                /*
                                if (content.find('.contentBlock').length == 0) {
                                    content.append(generalInfo.find('.contentBlock')[1]);
                                    content.append(generalInfo.find('.contentBlock')[0]);
                                    content.append(passiveCircle.find('.contentBlock')[1]);
                                    content.append(passiveCircle.find('.contentBlock')[0]);
                                    content.append(caseInformation.find('.contentBlock')[1]);
                                    content.append(caseInformation.find('.contentBlock')[0]);
                                    
                                }*/
                            }
                            else {
                                //validateTab($(this));
                                setOpacity();
                                if (content.find('.contentBlock').length > 0) {
                                    /*generalInfo.find('.content').prepend(content.find('.contentBlock')[0]);
                                    generalInfo.find('.content').prepend(content.find('.contentBlock')[0]);

                                    passiveCircle.find('.content').prepend(content.find('.contentBlock')[0]);
                                    passiveCircle.find('.content').prepend(content.find('.contentBlock')[0]);

                                    caseInformation.find('.content').prepend(content.find('.contentBlock')[0]);
                                    caseInformation.find('.content').prepend(content.find('.contentBlock')[0]);*/
                                }
                            }
                            hideTabs();
                            $(this).show();
                            $('.titlePage p').text(liElement.text());

                            $('.selectedStep').css({ 'border-color': 'rgb(132, 138, 139)', 'color': 'rgb(132, 138, 139)' });
                            $('.selectedStep').removeClass('clicked');

                            $('.selectedStep[data-val=' + $(this).attr('data-id') + ']').css({ 'border-color': 'rgb(5, 181, 162)', 'color': 'rgb(5, 181, 162)' });
                            $('.selectedStep[data-val=' + $(this).attr('data-id') + ']').addClass('clicked');

                            verticalMenu.removeClass('clicked');
                            liElement.addClass('clicked');

                        }
                    });

                }
            });

        }
        verticalMenu.click(function () {
            menuItemClicked($(this).attr('data-id'));
        });

        currentStep.click(function () {
            menuItemClicked($(this).attr('data-val'));
        });

        $('.goToStep1').on('click', function () { menuItemClicked(1) });
        $('.goToStep2').on('click', function () { menuItemClicked(2) });
        $('.goToStep3').on('click', function () { menuItemClicked(3) });


        gInfoBtn.click(function () {
            /**здесь было закомментировано
             * /
             */
            addToAvaible(generalInfo.attr('data-id'));
            menuItemClicked(generalInfo.attr('data-id'));
            setOpacity();
            gInfoBtn.addClass('active');
            /*
            */
        });
        function addErrors(tab) {
            tab.find('.validate').each(function (indx, element) {

                var self = $(element);
                if (self.hasClass('vlCorrect')) {
                    addToAvaible(parseInt(tab.attr('data-id')) + 1);
                    return true
                }
                if (self.data("vltarget")) {
                    self.parent(self.data("vltarget")).addClass("vlError");
                } else {
                    if (self.length > 1 || !self.is(":visible")) {
                        self.addClass('vlCorrect');
                        addToAvaible(parseInt(tab.attr('data-id')) + 1);
                        return true
                    } else {
                        self.addClass("vlError");
                    }

                }

            });
        }
        function validateTab(tab) {
            var containerWithness = $(".addPersonContainer");
            if (containerWithness.find('.witnessPersone').length != 0) {
                containerWithness.find('.sampleContainer').each(function (indx, element) {
                    element = $(element);
                    if (element.find('input').val() == "") {
                        element.addClass('vlError');
                        element.on('change', function (event) {
                            var event = $(event.currentTarget);
                            if (event.find('input').val() != "") {
                                event.removeClass('vlError');
                            }
                        });
                    }
                });
            }
            addErrors(tab);
            $('.tab').each(function () {
                if ($(this).find('.vlError').length > 0) {
                    removeFromAvaible(parseInt($(this).attr('data-id')) + 1);
                }
            });

            if (tab.find('.vlError').length == 0) {
                //addToAvaible(parseInt(tab.attr('data-id')) + 1);
                return true;
            }
            else return false;
        }
        function nextTab(tab) {
            var button = tab.find('input.nextSepButton.tabOption');
            if (button.hasClass("active")) {
                menuItemClicked(parseInt(tab.attr('data-id')) + 1);
                setOpacity();
            }
            window.scrollTo(0, 0);
        }

        pInvolvedBtn.click(function () {

            if (validateTab(generalInfo) == true) {
                stepChecked(0);
                pInvolvedBtn.addClass('active');
            }

            else pInvolvedBtn.removeClass('active');
            setOpacity();
            nextTab(generalInfo);

        });
        cInformationBtn.click(function () {
            //need to add validation custom
            if (validateTab(passiveCircle) == true) {
                stepChecked(1);
                cInformationBtn.addClass('active');
            }
            else cInformationBtn.removeClass('active');
            setOpacity();
            nextTab(passiveCircle);
        });
        mAssignedBtn.click(function () {
            if (validateTab(caseInformation) == true) {
                mAssignedBtn.addClass('active');
                stepChecked(2);
            }
            else mAssignedBtn.removeClass('active');
            setOpacity();
            nextTab(caseInformation);
        });

    }


    function partiesInvolvedProcess() {
        var managementIncident = passiveCircle.find('.managementIncident');
        cmp.OptionSelect(managementIncident);
        var report_by_myself = passiveCircle.find('.report_by_myself');
        cmp.OptionSelect(report_by_myself);
        $("#report_by_myselfContentBlock").click(function (event) {
            $(event.currentTarget).css("border-color", "rgb(5, 181, 162)");
            $(event.currentTarget).mouseleave(function (element) {
                $(element.currentTarget).css("border-color", "transparent transparent #E5EBEB transparent");
            });
        });
        $("#Myself").click();
        var whoKnow = passiveCircle.find('.whoKnow');
        cmp.OptionSelect(whoKnow);
        var isReportUrgent = passiveCircle.find('.isReportUrgent');
        cmp.OptionSelect(isReportUrgent);
        var addPersonContainer = passiveCircle.find('.addPersonContainer');
        var personNum = passiveCircle.find('input.personNum');
        var addPersonTempl = tmplStoredge.find('#addPersonTmpl');
        if (addPersonTempl.length > 0) {
            addPersonTempl = addPersonTempl.html().trim();
        }
        var num = 0;
        var personCount = 0;
        var urgentCheckBox = passiveCircle.find('#isUrgent');
        var urgenRadio = passiveCircle.find('input[name="isUrgent"]');

        var personNameNum = passiveCircle.find('.personNameNum');
        var personLastNameNum = passiveCircle.find('.personLastNameNum');
        var personTitleNum = passiveCircle.find('.personTitleNum');
        var personRoleNum = passiveCircle.find('.personRoleNum');

        var managementKnow = passiveCircle.find('input:radio[name="managementKnow"]');
        var isReportRefered = passiveCircle.find('input:radio[name="isReportRefered"]');



        managementKnow.on('change', function () {
            if ($("input:radio[name=managementKnow]:checked").val() == 'Yes' || $("input:radio[name=managementKnow]:checked").val() == 'Do not want to involve') {
                $('.managementIsKnown').addClass('validate');
                $('.contentBlock.managementKnownExplaintion').show();
                $('.contentBlock.managementKnownExplaintion').find('textarea').focus();
            }
            else {
                $('.managementIsKnown').removeClass('validate');
                $('.managementIsKnown').removeClass('vlError');
                $('.contentBlock.managementKnownExplaintion').hide();
            }
        });

        isReportRefered.on('change', function () {
            if ($("input:radio[name=isReportRefered]:checked").val() != 'No') {
                $('.isReportedOutside').addClass('validate');
            }
            else {
                $('.isReportedOutside').removeClass('validate');
                $('.isReportedOutside').removeClass('vlError');
            }
        });

        passiveCircle.find('.addPerson').click(function () {
            personCount++;


            if (personCount > 0) {
                passiveCircle.find('.display.button').text($("#AddAnotherPerson").val());
                passiveCircle.find('.display.button').css({ "border-color": "#AEB5B7", "color": "#AEB5B7", "background": "transparent" });
            }
            else {
                passiveCircle.find('.display.button').text($("#AddPerson").val());
            }

            //var buff = tmpl.render(addPersonTempl, { 'num': ++num });
            var buff = tmpl.render(addPersonTempl, { 'num': '' });
            personNameNum.val(num);
            personLastNameNum.val(num);
            personTitleNum.val(num);
            personRoleNum.val(num);

            buff.find('.delete').click(function () {
                $(this).closest('#persone').remove();
                personCount--;

                if (personCount > 0) {
                    passiveCircle.find('.display.button').text($("#AddAnotherPerson").val());
                }
                else {
                    passiveCircle.find('.display.button').text($("#AddPerson").val());
                }
            });

            addPersonContainer.append(buff);
            personNum.val(num);

            cm.setInput(personNum, num);
            var wid = $($(".contentBlock")[0]).width();
        });
        passiveCircle.find('input.validate').change(function () {
            if (vl.check(passiveCircle)) {
                var button = passiveCircle.find('.nextSepButton.tabOption.cInformation');
                button.addClass('active');
            }
        });
        urgenRadio.change(function () {
            if ($(this).val() == "1") {
                passiveCircle.find('#emergency').show();
            } else {
                passiveCircle.find('#emergency').hide();
            }
        });
    }

    function caseInformationProcess() {
        var caseInformationReport = caseInformation.find('.caseInformationReport');
        var incidentResultReport = caseInformation.find('.incidentResultReport');
        var isAccidentOngoing = caseInformation.find('.isAccidentOngoing');
        var adviceManagement = caseInformation.find('#AdviceManagement');
        var anyQuestions = caseInformation.find('#AnyQuestions');
        var isOnGoing = caseInformation.find('#isOnGoing');
        var witnessFlag = caseInformation.find('#witnessFlag');
        var otherFlag = caseInformation.find('input[name=caseInformationReportDetail]');
        var injuryDescription = caseInformation.find('input:radio[name="resultInjury"]');
        var isIncidentOngoing = caseInformation.find('input:radio[name="isIncidentOngoing"]');

        cmp.OptionSelect(caseInformationReport);
        cmp.OptionSelect(incidentResultReport);
        cmp.OptionSelect(isAccidentOngoing);
        
        caseInformationReport.on('change', function () {
            if ($("input:radio[name=whatHappened]:checked").val() == 'Other') {
                $('.reportAbout').addClass('validate');
                $('.contentBlock.reportingAboutArea').show();
                $('.contentBlock.reportingAboutArea').find('textarea').focus();
            }
            else {
                $('.reportAbout').removeClass('validate');
                $('.reportAbout').removeClass('vlError');
                $('.contentBlock.reportingAboutArea').hide();
            }
        });
        $(".caseInformationReport .inputRadio").click(function (event) {
            $("#ProgrammingConsiderations").hide();
            $("input:checkbox[name=whatHappened]:checked").each(function (indx, element) {
              if (element.id.toLowerCase() == "sexual harassment" || element.id.toLowerCase() == "sexual abuse")
                $("#ProgrammingConsiderations").show();
            });


        });

        injuryDescription.on('change', function () {
            if ($("input:radio[name=resultInjury]:checked").val() == 'Yes') {
                $('#injury_damage').addClass('validate');
                $('.contentBlock.discribeInjuryDamage').show();
                $('.contentBlock.discribeInjuryDamage').find('textarea').focus();
            }
            else {
                $('#injury_damage').removeClass('validate');
                $('.contentBlock.discribeInjuryDamage').hide();
            }
        });


        isIncidentOngoing.on('change', function () {
            if ($("input:radio[name=isIncidentOngoing]:checked").val() == 'Yes') {
                $('.incidentOngoingDescription').addClass('validate');
                $('.contentBlock.incidentOngoing').show();
                $('.contentBlock.incidentOngoing').find('textarea').focus();
            }
            else {
                $('.incidentOngoingDescription').removeClass('validate');
                $('.contentBlock.incidentOngoing').hide();
            }
        });


        $("#datepicker").datepicker({
            inline: true,
            maxDate: "+1D",
            dateFormat: "mm/dd/yy"
        });
        $("#datepicker").on('click', function () {
            var temp = $(event.currentTarget).parents('.contentBlock');
            temp.css("border-color", "rgb(5, 181, 162)");
        })
            .on('focusout', function () {
                var temp = $(event.currentTarget).parents('.contentBlock');
                temp.css("border-color", "transparent transparent #E5EBEB transparent");
            });
        adviceManagement.on('click', function (events) {
            adviceManagement.css({ "width": "750px", "height": "155px" });
            adviceManagement.find('textarea').show();
            adviceManagement.find('div').css({ "padding-left": "49px", "text-align": "initial" });
        });

        anyQuestions.on('click', function (events) {
            var anyQuestionsTextArea = $('#AnyQuestionsTextArea');
            anyQuestionsTextArea.css({ "width": "750px", "height": "155px" });
            anyQuestionsTextArea.show();
        });
        isOnGoing.change(function () {
            if ($(this).is(":checked")) {
                caseInformation.find('.incidentFrequency').show();
                isOnGoing.val(1);
            } else {
                caseInformation.find('.incidentFrequency').hide();
                isOnGoing.val(0);
            }
        });
        caseInformation.find('input.validate').change(function () {
            if (vl.check(caseInformation)) {
                var button = caseInformation.find('.nextSepButton.tabOption.cSummary');
                button.addClass('active');
            }
        });
        witnessFlag.change(function () {
            if ($(this).is(":checked")) {
                caseInformation.find('#witnessNames').show();
                witnessFlag.val("true");
            } else {
                caseInformation.find('#witnessNames').hide();
                witnessFlag.val("false");
            }
        });

        otherFlag.on("click", function () {
            $("#Other").prop('checked', true);
        });
        var counterFiles = 0;
        var tableFile = $(".attachedFilesTitle").clone();
        $(".addFileButton").on('click', function (event) {
            var table = tableFile.clone();
            //tableFile.clone().appendTo(".attach");
            var index = 'attachDocuments[' + counterFiles + ']';
            var input = table.find('input').attr('name', index);
            var newElement = table.appendTo(".attach");
            input.click();
            
            newElement.attr('style', "display:none"); 
            newElement.on('change', function (event) {
                changeInput(event);
            });
            counterFiles++;
        });
        function changeInput(event) {
            var current = $(event.currentTarget);
            current.attr('style', "display:block"); 

            current.find('.disableInputFile').bind('click', function (event) {
                var element = $(event.currentTarget);
                element.parents('.attachedFilesTitle').remove();
            });

            Array.from(current.find('input')[0].files).forEach(function (file) {
                current.find('.fileName').text(file.name);
            });
        }
    }
    function caseSummaryProcess() {
        caseSummary.find('input.validate').change(function () {
            if (vl.check(caseInformation)) {
                submitCaseSummary.addClass('active');
            }
        });

        $(".option.mediator").on('click', function (event) {
            var allItems = $(".option.mediator").length;
            var checkedItems = $(".option.mediator input:checked").length;
            if (allItems === checkedItems) {
                event.preventDefault();
            }
        });
    }

    function loginInformationProcess() {

    }

    function summaryPage() {
        //$("#userName").unbind('change');
        function setHoverBlock(item) {
            $(this).parent().find('img, input').addClass('hover');
        }

        function setUnHoverBlock(item) {
            $(this).parent().find('img, input').removeClass('hover');
        }

        var root = $('#summareResponce');
        var summaryButtons = root.find('.summaryButtons');
        var summaryImages = root.find('.summaryImages');
        summaryButtons.mouseenter(setHoverBlock)
        summaryImages.mouseenter(setHoverBlock);

        summaryButtons.mouseout(setUnHoverBlock)
        summaryImages.mouseout(setUnHoverBlock);
    }

    function setDefaultVaues() {
        var toStay = $('.anType div[data-val=1] input');
        /*var staff = $('.reporterType div[data-val=1] input');*/
        var staff = $('.reporterType .fourBlock label')[0];
        var managmantKnown = $('.managementIncident div[data-val=2] input');
        var reportAbout = $('.caseInformationReport div[data-val=BreachofLegalObligations] input');
        var isUrgent = $('.urgent .freeBlock label')[1];
        var isreferredOutside = $('.referredOutside .freeBlock label')[0];
        var isOngoing = $('.isOngoing .freeBlock label')[0];
        var isInjury = $('.incidentResultReport .freeBlock label')[0];

        $('.rowBlock.anType input').on('change', function () {
            if ($('.rowBlock.anType input').val() == 1) {
                $('#hideIfConfidential').hide()
                $('.summaryContentTitle').css("margin-top", "0px");
            }
            else {
                $('#hideIfConfidential').show()
                $('.summaryContentTitle').css("margin-top", "75px");
            }
        });


        $('.dropdown').on('change', function (event) {
            if ($('.dropdown .locationsOfIncident').val() == "Not listed") {
                $('.otherLocation').show();
                $(".selectBlock.location").removeClass('vlCorrect');
                $(".locationsOfIncident.selectBlock").removeClass('vlCorrect');
                $("textarea[name=locationOfIncidentInput]").on('change', function (event) {
                    if (event.currentTarget.value.length >= 2) {
                        $(".dropdown.selectBlock").removeClass('vlError');
                        $(".locationsOfIncident.selectBlock").removeClass('validate');
                        $(".locationsOfIncident.selectBlock").addClass('vlCorrect');
                        $(".selectBlock.location").addClass('vlCorrect');
                    }
                });

            }
            else {
                $('.otherLocation').hide();
                var temp = $(event.currentTarget);
                temp.find('span.selected').css("font-family", "OpenSans-Semibold");
            }
        });

        managmantKnown.click();
        toStay.click();
        if (staff != undefined) {
            staff.click();
        }
        reportAbout.click();
        if (isUrgent != undefined) {
            isUrgent.click();
        }
        if (isreferredOutside != undefined) {
            isreferredOutside.click();
        }
        if (isOngoing != undefined) {
            isOngoing.click();
        }
        if (isInjury != undefined) {
            isInjury.click();
        }

        setDropdown();
        $(".check").add(".sendUpdates").add('.checked').on('click', function () {

            if ($('#checkBox').prop("checked")) {
                $('#checkBox').click();
                $('.floatLeftClass').val('false')
                $('.checked').hide();
                $('.check').show();
            }

            else {
                $('#checkBox').click();
                $('.floatLeftClass').val('true')
                $('.checked').show();
                $('.check').hide();
            }
        });

        $(".buttonSubmit").on('click', function () {
           
            var incidentAnonymityIdVal = $('.incidentAnonymityId').val();
            if (incidentAnonymityIdVal != 1) {
                if ($("#userName").hasClass("vlCorrect") &&
                    $("#userLastName").hasClass("vlCorrect") &&
                    $("#reporterFirstName").hasClass("vlCorrect")) {
                    $('#loading').show();
                }
                else {
                    if (!$("#userName").hasClass("vlCorrect"))
                        $("#userName").addClass('vlError');

                    if (!$("#userLastName").hasClass("vlCorrect"))
                        $("#userLastName").addClass('vlError');

                    if (!$("#reporterFirstName").hasClass("vlCorrect"))
                        $("#reporterFirstName").addClass('vlError');
                    //if (!$("#userTelephone").hasClass("vlCorrect"))
                    //    $("#userTelephone").addClass('vlError');
                    return false;
                }
            }
        });
    };


    function setDropdown() {
        $('.dropdown').unbind('click');
        $('.dropdown ul li').unbind('click');

        $('.dropdown')
            .on('click',
                function () {
                    $(this).children('ul').slideToggle(150);
                    if ($(this).hasClass('open')) {
                        $(this).removeClass('open');
                        return false;
                    } else {
                        $(this).addClass('open');
                        return false;
                    }

                    return false;
                })

            .hover(
                function () {

                },
                function () {
                    $(this).children('ul').slideUp(150);
                    $(this).removeClass('open');
                });

        $('.dropdown ul li')
            .click(
                function () {
                    var sitem = $(this).html();
                    var sid = $(this).attr('value');
                    if ($(this).attr('data-id') == "Not Listed") {
                        $(this).siblings('li').removeClass('selected');
                        $(this).addClass('selected');
                        $(this).parent('ul').siblings('span.selected').html(sitem);
                        $(this).parent('ul').siblings('input').val(sid);
                        $(this).parent('ul').parent('.subject').change();
                        $(this).parent('ul').parent('.subject').find('.validate').addClass('vlCorrect');
                        return;
                    }
                    if ( $(this).text() != 'Select department' && $(this).val() !== 0 && $(this).attr('data-id') != undefined) {
                        var newItem = $(".dropdown.subject.selectBlock.department.open").siblings(".delete.redText.deleteDepartmentPosition");
                        newItem.css("display", "block");
                    }
                    $(this).siblings('li').removeClass('selected');
                    $(this).addClass('selected');
                    $(this).parent('ul').siblings('span.selected').html(sitem);
                    $(this).parent('ul').siblings('input').val(sid);
                    $(this).parent('ul').parent('.subject').change();
                    $(this).parent('ul').parent('.subject').find('.validate').change();
                });
    }

    function getAnon(company_id, country_id) {
        $.ajax({
            method: "POST",
            url: "/Report/GetAnon",
            data: { company_id: company_id, country_id: country_id }
        }).done(function (data) {//data from server
            return data;
            if (data != '') {
            }
        }).fail(function (error) {
            console.log(error);
        });
    }

    function PrintReport() {
        //Creating new page
        var pp = window.open();
        //Adding HTML opening tag with <HEAD> … </HEAD> portion 
        pp.document.writeln('<HTML><HEAD><title>Print Preview</title><LINK href=Styles.css  type="text/css" rel="stylesheet">')
        pp.document.writeln('<LINK href=PrintStyle.css  type="text/css" rel="stylesheet" media="print"><base target="_self"></HEAD>')
        //Adding Body Tag
        pp.document.writeln('<body MS_POSITIONING="GridLayout" bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0">');
        //Adding form Tag
        pp.document.writeln('<form  method="post">');
        //Creating two buttons Print and Close within a table
        pp.document.writeln('<TABLE width=100%><TR><TD></TD></TR><TR><TD align=right><INPUT ID="PRINT" type="button" value="Print" onclick="javascript:location.reload(true);window.print();"><INPUT ID="CLOSE" type="button" value="Close" onclick="window.close();"></TD></TR><TR><TD></TD></TR></TABLE>');
        //Writing print area of the calling page
        pp.document.writeln(document.getElementById(print_area).innerHTML);
        //Ending Tag of </form>, </body> and </HTML>
        pp.document.writeln('</form></body></HTML>');
    }
    base();
    taSwitcher();
    generalInfoProcess();
    partiesInvolvedProcess();
    caseInformationProcess();
    caseSummaryProcess();
    loginInformationProcess();
    summaryPage();
    gInfoBtn.click();
    setOpacity();

    setDefaultVaues();
});



angular.module('EC', []);