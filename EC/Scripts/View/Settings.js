$(document).ready(function () {
    /*global functions for SETTINGS PAGE*/
    function blockActivityHeight() {
        if ($('#menu').height() < 50) {
            $('.positionActivityIcon').height(89);
        }
        else {
            $('.positionActivityIcon').height($('#casesHeared').height());
        }
    }
    $('.mainTitle').click(function () {
        $('.mainTitle + div').toggle();
        blockActivityHeight();
    });
    /*end global functions*/


    /*page cases begin*/
    updateTOTALTIMEONCASE();
    var isValidMed = "@is_valid_mediator";
    if (isValidMed != "False") {
        $(".show").on('click', function (event) {
            var clickedDrop = $(event.currentTarget);
            var dropDownList = clickedDrop.siblings('.hide');
            clickedDrop.addClass('activeDropDown');
            dropDownList.addClass('activeDropDown');
            dropDownList.on('click', function (event) {
                var clickedDropDown = $(event.currentTarget);
                var valueClicked = clickedDropDown.text().trim();
                valueClicked += '<img src="/Content/img/arrowSelect.png">';
                var main = clickedDropDown.siblings('.show');
                main.html(valueClicked);
                main.removeClass('activeDropDown');
                dropDownList.removeClass('activeDropDown');
                dropDownList.unbind('click');
                sendAjaxItems();
            });
        });
    }

    function updateTOTALTIMEONCASE() {
        var counter = 0;
        $(".someDropdowns .show").each(function (indx, element) {
            counter += Number($(element).text().trim());
        });
        $('#totalTime').text(counter + " days"); // where 60 days - update with total
    }
    function sendAjaxItems() {
        var array = {};
        $(".show").each(function (indx, element) {
            array[indx] = $(element).text().trim();
        });
        var step1_delayMin = array[0];
        var step2_delayMin = array[1];
        var step3_delayMin = array[2];
        var step4_delayMin = array[3];
        var company_id = $("#companyId").val();
        $.ajax({
            method: "POST",
            url: "/Settings/UpdateDelays",
            data: {
                company_id: company_id,
                step1_delayMin: step1_delayMin,
                step2_delayMin: step2_delayMin,
                step3_delayMin: step3_delayMin,
                step4_delayMin: step4_delayMin
            }
        }).done(function (data) {//data from server
            $('#updateButton').css('display', 'none');
            $('#saveButton').css('display', 'block');
            updateTOTALTIMEONCASE();
        }).fail(function (error) {
            console.log(error);
        });
    }
    $(".text3").on('click', function (event) {
        var temp = $(event.currentTarget);
        window.location.href = temp.siblings('.header').find('a').attr('href');
    });
    /*page cases end*/


    /*page company*/

    /*this function works for index page - update user foto too*/
    /*for upload files add foto*/
    $('.newImageBtn').click(function () {
            $("#_file").click();
    });
    $("#_file").on('change', function (e) {

        var urlAjaxUploadFiles = $("#urlAjaxUploadFiles").val();
        var from = $("#urlAjaxUploadFiles").attr("from");
        var files = e.target.files;

        if (files.length > 0) {
            var fd = new FormData();
            if (from != "") {
                fd.append("from", from);
            } else {
            }

            var file = document.getElementById('_file');
            for (var i = 0; i < file.files.length; i++) {
                fd.append('_file', file.files[i]);
            }

            $.ajax({
                type: "POST",
                url: urlAjaxUploadFiles,
                contentType: false,
                processData: false,
                data: fd,
                success: function (result) {
                    var from = $("#urlAjaxUploadFiles").attr("from");
                    if (from == "User") {
                        $("#logoUser").attr("src", result);
                    } else {
                        $("#logoCompany").attr("src", result);
                    }

                },
                error: function (xhr, status, p3, p4) {
                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] == "{")
                        err = JSON.parse(xhr.responseText).Message;
                    console.log(err);
                }
            });
        } else {
            alert("This browser doesn't support HTML5 file uploads!");
        }
    });

        //function blockActivityHeight() {
        //    if ($('#menu').height() < 50) {
        //        $('.positionActivityIcon').height(89);
        //    }
        //    else {
        //        $('.positionActivityIcon').height($('#casesHeared').height());
        //    }
        //}

        //function miniMenu() {
        //    $('.mainTitle').click(function () {
        //        $('.mainTitle + div').toggle();
        //        blockActivityHeight();
        //    });
        //}
        var contentCompanyProfile = $('.contentCompanyProfile');

        $(".menuCompanyProfile .settingMenuItems .menuItem").click(function (element) {
            $(".menuItem.active").removeClass("active");
            $(element.target).addClass("active");
            $("#menu .caseesTab:nth-child(3)").addClass("active");
        });


        function hideBlock() {
            $('.blockLanguages').hide();
            $('.blockLocations').hide();
            $('.blockDepartments').hide();
            $('.blockIncidentTypes').hide();
            $('.blockReporterTypes').hide();
            $('.blockAnonymity').hide();
            $('.blockOutcomes').hide();
            $('.blockRootCauses').hide();
            $('.blockSecondaryType').hide();
            $('.blockCaseAdminDepartment').hide();
            $('.blockCaseRouting').hide();
        }

        function contentCompanyProfileShow() {
            $('.menuItem:nth-child(8)').click(function () {
                hideBlock();
                $('.blockRootCauses').show();
            });
            $('.menuItem:nth-child(7)').click(function () {
                hideBlock();
                $('.blockOutcomes').show();
            });
            $('.menuItem:nth-child(6)').click(function () {
                hideBlock();
                $('.blockAnonymity').show();
            });
            $('.menuItem:nth-child(5)').click(function () {
                hideBlock();
                $('.blockReporterTypes').show();
            });
            $('.menuItem:nth-child(4)').click(function () {
                hideBlock();
                angular.element($('.blockCaseRouting').children().eq(0)).scope().onShow();
                $(".blockCaseRouting").show();
            });
            $('.menuItem:nth-child(3)').click(function () {
                hideBlock();
                $('.blockIncidentTypes').show();
            });
            $('.menuItem:nth-child(2)').click(function () {
                hideBlock();
                $('.blockLocations').show();
            });
            $('.menuItem:nth-child(1)').click(function () {
                hideBlock();
                $('.blockDepartments').show();
            });
        }



        //miniMenu();
        contentCompanyProfileShow();

        /**
        Department
        Location
        Language
        IncidentType
        */
        function close(nameObject, data, id) {
            var nameTable = ".table" + nameObject;
            contentCompanyProfile.find(".table" + nameObject).prepend('<div style="display:flex"><p>' + data + '</p>' + '<div class="delete' + nameObject + '" data-value=' + id + '>' + '</div></div>');
            closeIcon(nameObject);
        }
        function closeIcon(nameObject) {
            contentCompanyProfile.find(".addNew" + nameObject + " input").val('');
            contentCompanyProfile.find(".addNew" + nameObject).hide();
            contentCompanyProfile.find(".add" + nameObject + "Btn").removeClass("inactive");
            contentCompanyProfile.find(".add" + nameObject + "Btn").css('float', 'none');
            contentCompanyProfile.find("#add" + nameObject.charAt(0) + nameObject.charAt(1) + nameObject.charAt(2) + "Btn").hide();
        }

        function initRootCauses() {
            $('#addNewRootCauses1a').off().on("click", function () {
                $('#addNewRootCauses1a').css('display', 'none');
                $('#newRootCauses1').css('display', '');
            });
            $('#addNewRootCauses2a').off().on("click", function () {
                $('#addNewRootCauses2a').css('display', 'none');
                $('#newRootCauses2').css('display', '');
            });
            $('#addNewRootCauses3a').off().on("click", function () {
                $('#addNewRootCauses3a').css('display', 'none');
                $('#newRootCauses3').css('display', '');
            });
            $('#newRootCauses1 .btnAdd').off().on("click", function () {
                var data = $('#newRootCauses1 input').val();
                if (data.length > 0) {
                    sendAjax("RootCauses1", data, function (json) {
                        renderRootCauses(json);
                        $('.newRootCauses').css('display', 'none');
                        $('#newRootCauses1 input').val('');
                    }, 'BlockRootCauses');
                } else {
                    $('#newRootCauses1').css('border-color', 'red');
                    $('#newRootCauses1').css('border-width', '2px');
                }
            });
            $('#newRootCauses2 .btnAdd').off().on("click", function () {
                var data = $('#newRootCauses2 input').val();
                if (data.length > 0) {
                    sendAjax("RootCauses2", data, function (json) {
                        renderRootCauses(json);
                        $('.newRootCauses').css('display', 'none');
                        $('#newRootCauses2 input').val('');
                    }, 'BlockRootCauses');
                } else {
                    $('#newRootCauses2').css('border-color', 'red');
                    $('#newRootCauses2').css('border-width', '2px');
                }
            });
            $('#newRootCauses3 .btnAdd').off().on("click", function () {
                var data = $('#newRootCauses3 input').val();
                if (data.length > 0) {
                    sendAjax("RootCauses3", data, function (json) {
                        renderRootCauses(json);
                        $('.newRootCauses').css('display', 'none');
                        $('#newRootCauses3 input').val('');
                    }, 'BlockRootCauses');
                } else {
                    $('#newRootCauses3').css('border-color', 'red');
                    $('#newRootCauses3').css('border-width', '2px');
                }
            });
            $('.deleteRootCauses').off().on('click', function () {
                data = $(this).data('id').toString();
                sendAjax('deleteRootCauses' + $(this).data('type'), data, function (json) {
                    renderRootCauses(json);
                    $('.newRootCauses').css('display', 'none');
                }, 'BlockRootCauses');
            });
        }
        initRootCauses();

        function renderRootCauses(data) {
            /*data = JSON.parse(data);
            var html = '';
            for (var i = 0; i < Math.max(data.arr1.length, data.arr2.length, data.arr3.length) ; i++) {
                var item1 = data.arr1.length > i ? data.arr1[i] : null;
                var item2 = data.arr2.length > i ? data.arr2[i] : null;
                var item3 = data.arr3.length > i ? data.arr3[i] : null;
                html += '<tr>';
                html += '<td width="33%"';
                html += item1 != null ? ' data-id="' + item1.id + '">' : '>';
                html += item1 != null ? '<p>' + item1.name_en + '</p>' : '';
                html += $('#tblRootCauses').data('delete') == 1 && item1 != null ? '<div class="deleteRootCauses"></div>' : '';
                html += '</td>';
                html += '<td width="33%"';
                html += item2 != null ? ' data-id="' + item2.id + '">' : '>';
                html += item2 != null ? '<p>' + item2.name_en + '</p>' : '';
                html += $('#tblRootCauses').data('delete') == 1 && item2 != null ? '<div class="deleteRootCauses"></div>' : '';
                html += '</td>';
                html += '<td width="33%"';
                html += item3 != null ? ' data-id="' + item3.id + '">' : '>';
                html += item3 != null ? '<p>' + item3.name_en + '</p>' : '';
                html += $('#tblRootCauses').data('delete') == 1 && item3 != null ? '<div class="deleteRootCauses"></div>' : '';
                html += '</td>';
                html += '</tr>';
            }
            $('#tblRootCauses').html(html);
            deleteRootCauses();*/
            $('#tblRootCauses').html(data);
            initRootCauses();
        }

        $('.addLanguageBtn p').click(function () {
            $(this).parent().addClass("inactive");
            $('.addNewLanguage').css('display', 'flex');
        });

        $('.addNewLanguage .closeIcon').click(function () {
            $(".addLanguageBtn").removeClass("inactive");
            $('.addNewLanguage').hide();
        });
        $('.addNewDepartment .closeIcon').click(function () {
            closeIcon("Department");
        });
        $('.addNewIncidentType .closeIcon').click(function () {
            closeIcon("IncidentType");
        });
        $('.addNewReporterType .closeIcon').click(function () {
            closeIcon("ReporterType");
        });
        $('.addNewOutcome .closeIcon').click(function () {
            closeIcon("Outcome");
        });
        $('.addNewLocation .closeIcon').click(function () {
            closeIcon("Location");
        });
        /* Adding Location*/
        $('#addInputLoc p').click(function () {
            var temp = $(this).parent();
            adding("Location", temp);
        });
        $('#addLocBtn').click(function () {
            var name = $('.addNewLocation input[name="newLocationName"]').val();
            var cc = $('.addNewLocation #location_cc_extended').val();
            cc = (cc === undefined) || (cc.length == 0) ? 'none' : cc;
            var data = '' + name + '\n' + cc;

            if ($('.addNewLocation input[name="newLocationName"]').val().length > 0) {
                sendAjax('Location', data, function (id) {
                    var s = '<div style="display:flex"><p>';
                    s += '<span class="location_name">' + name + '</span>';
                    if ($('#location_cc_extended').length != 0) {
                        s += '<select class="location_cc_extended_items" id="location_cc_extended_' + id + '" name="location_cc_extended_' + id + '"><option value="">Not selected</option>';
                        var elem = $('.addNewLocation #location_cc_extended').clone();
                        s += elem.html();
                        s += '</select>';
                    }
                    s += '<div data-value="' + id + '" class="deleteLocation"></div>';
                    contentCompanyProfile.find('.tableLocation').prepend(s + '</div>');
                    if (cc != 'none') {
                        $('#location_cc_extended_' + id).val(cc);
                    }
                    init_location_cc_extended();
                    closeIcon('Location');
                });
            } else {
                $('.addNewLocation').css('border-color', 'red');
            }
        });

        function init_location_cc_extended() {
            $('.location_cc_extended_items').off().on('change', function () {
                var name = $(this).parent().parent().find('.location_name').text();
                var cc = $(this).val();
                cc = cc === undefined ? 'none' : cc;
                var data = '' + name + '\n' + cc;

                sendAjax('Location', data, function (id) {
                });
            });
        }
        init_location_cc_extended();

        function adding(newSetting, temp) {
            temp.addClass("inactive");
            temp.css({ 'width': '20%', 'float': 'left' });
            temp.parent().find(".addNew" + newSetting).css('display', 'flex');
            var btn = document.getElementById("add" + newSetting.charAt(0) + newSetting.charAt(1) + newSetting.charAt(2) + "Btn");
            btn.style.display = 'block';
            btn.style.styleFloat = 'left';
        }
        /* Adding Departments*/

        $('#addInputDep p').click(function () {
            var temp = $(this).parent();
            adding("Department", temp);
        });

        $('#addInputIncType p').click(function () {
            var temp = $(this).parent();
            adding("IncidentType", temp);
        });
        $('#addInputRepType p').click(function () {
            var temp = $(this).parent();
            adding("ReporterType", temp);
        });
        $('#addDepBtn').click(function () {
            var data = $(".addNewDepartment input").val();
            if (data.length > 0) {
                sendAjax("Department", data, function (id) {
                    close("Department", data, id);
                });
            } else {
                $('.addNewDepartment').css('border-color', 'red');
            }
        });
        $('#addIncBtn').click(function () {
            var data = $(".addNewIncidentType input").val();
            if (data.length > 0) {
                sendAjax("IncidentType", data, function (id) {
                    close("IncidentType", data, id);
                });
            } else {
                $('.addNewIncidentType').css('border-color', 'red');
            }
        });
        /*add Reporter types*/
        $('#addRepBtn p').click(function () {
            var data = $(".addNewReporterType input").val();
            if (data.length > 0) {
                sendAjax("addReporterType", data, function (id) {
                    close("ReporterType", data, id);
                });
            } else {
                $('.addNewReporterType').css('border-color', 'red');
            }
        });
        function sendAjax(newSetting, data, action, partial) {
            if (newSetting && data.toLowerCase() != "other") {
                var companyId = $("#companyId").data('value');
                var userId = $("#userId").data('value');

                var result = false;
                var sendAjax = $("#sendAjax").val();
                if (data.length > 0) {
                    $.ajax({
                        data: { companyId: companyId, userId: userId, data: data, newSetting: newSetting, partial: partial },
                        url: sendAjax,
                        success: function (result) {
                            if (result !== "false") {
                                action(result);
                            } else {
                                alert('Error deleting');
                            }
                        }
                    });
                }
            }
        }//end function sendAjax()
        /*delete incident Type*/
        $(".tableIncidentType").on('click', '.deleteIncidentType', function (element) {
            var temp = $(element.target);
            var id = $(element.target).attr("data-value");
            sendAjax("deleteIncidentType", id, function () {
                temp.parent().hide(200);
            });
        });
        /*delete reporter Types*/
        $(".tableReporterType").on('click', '.deleteReporterType', function (element) {
            var temp = $(element.target);
            var id = temp.attr("data-value");
            sendAjax("deleteReporterType", id, function () {
                temp.parent().hide(200);
            });
        });
        /*delete deleteDepartment*/
        $(".tableDepartment").on('click', '.deleteDepartment', function (element) {
            var temp = $(element.target);
            var id = temp.attr("data-value");
            sendAjax("deleteDepartment", id, function () {
                temp.parent().hide(200);
            });
        });
        /*delete deleteLocation*/
        $(".tableLocation").on('click', '.deleteLocation', function (element) {
            var temp = $(element.target);
            var id = temp.attr("data-value");
            sendAjax("deleteLocation", id, function () {
                temp.parent().hide(200);
            });
        });
    /*end page company*/

    /*page index*/
    //----------------END Open mini menu for mobile---------------------------
        //$('.blockPersonalSettings input')
        //   .focus(function () {
        //       $('.blockPersonalSettings').css({ 'border-color': 'transparent transparent #e0e5e6 transparent' });
        //       $(this).parent('.blockPersonalSettings').css({ 'border-color': '#05b5a2' });
        //   })
        //    .focusout(function () {
        //        $(this).parent('.blockPersonalSettings').css({ 'border-color': 'transparent transparent #e0e5e6 transparent' });
        //    });
        var levelId = $("#levelId").val();


    //RadioButton

    /**/

        var radioBlock = $('.inputBlock');
        
        radioBlock.on('click', function (event) {
            var temp = $(event.currentTarget);
            $('.blockPersonalSettings.greenBorder').removeClass('greenBorder').addClass('greyBorder');

            temp.parents('.blockPersonalSettings').addClass('greenBorder');
        });
        //radioBlock.on('focusout',function (event) {
        //    var temp = $(event.currentTarget);
        //    temp.parents('.blockPersonalSettings').addClass('greyBorder').removeClass('greenBorder');
        //});


        if (levelId == 5) {
            radioBlock.click(function () {
                var self = $(this);
                var arrows = self.parent('.inputRadio').parent('.rowBlock');
                arrows.find('.inputRadio').find('.radioTitle').css('color', 'rgb(174, 181, 183)');
                self.find('.radioTitle').css('color', 'rgb(60, 62, 63)');
            });

            //RadioButton Mediator
            var RadioButton = $('.inputBlock');
            function OldClick(self) {
                var mediatorBtn = self.parent();
                var mediatorAllBtn = self.parent().parent();
                mediatorAllBtn.find('.inputRadio').removeClass('active');
                mediatorBtn.addClass('active');
                mediatorBtn.find("input").prop('checked', true);

            }
            RadioButton.click(function () {
                var self = $(this);
                //self.parents('.blockPersonalSettings').css({ 'border': '2px solid #05b5a2', 'width': '99.8%' });
                OldClick(self);
            });
        }
    /*end page index*/

    /*start mediators*/

    //START Open mini menu for mobile
        //function blockActivityHeight() {
        //    if ($('#menu').height() < 50) {
        //        $('.positionActivityIcon').height(89);
        //    }
        //    else {
        //        $('.positionActivityIcon').height($('#casesHeared').height());
        //    }
        //}
        //$('.mainTitle').click(function () {
        //    $('.mainTitle + div').toggle();
        //    blockActivityHeight();
        //});

        $(".closeIcon").on('click', function (event) {
            $("#sendEmail").val("");
        });
        $(".sendBtn").on('click', function (event) {
            var val = $("#sendEmail").val();
            if ($("#sendEmail").val().trim().length > 0) {
                $('.blockSendEmail input').click();
                var email = $("#sendEmail").val().trim();
                createInvitation();
            }
            else {
                $(event.currentTarget).parent('.blockSendEmail').addClass('redBorder');
            }
        });
        function createInvitation() {
            var InvitationSuccessfull = $("#InvitationSuccessfull").val();
            $.ajax({
                method: "POST",
                url: "/Settings/InviteMediator",
                data: {
                    email: $("#sendEmail").val().trim(),
                }
            }).done(function (data) {//data from server
                if (data != 'completed') {
                    alert(data);
                }
                else {
                    $("#sendEmail").val('');
                    alert(InvitationSuccessfull);
                }
            }).fail(function (error) {
                console.log(error);
            });
        }
    /*end mediators*/

    /*Password view start*/
        var status = $("#status").val();
        
        if (typeof status != 'undefined' && status != "") {
            alert(status);
        }
        $('.blockPersonalSettings input').on('focus', function (event) {
            var temp = $(event.currentTarget);
            $('.blockPersonalSettings.greenBorder').removeClass('greenBorder').addClass('greyBorder');
            temp.parent('.blockPersonalSettings').addClass('greenBorder').removeClass('greyBorder');
        });
        $('.blockPersonalSettings input').on('focusout', function (event) {
            var temp = $(event.currentTarget);
            temp.parent('.blockPersonalSettings').addClass('greyBorder').removeClass('greenBorder');
        });
        //$('.blockPersonalSettings input').focus(function () {
        //    $(this).parent('.blockPersonalSettings').addClass('greenBorder').removeClass('greyBorder');
        //});

        //$('.blockPersonalSettings input')
        //    .focus(function () {
        //        $(this).parent('.blockPersonalSettings').addClass('greenBorder').removeClass('greyBorder');
        //    })
        //    .focusout(function () {
        //        $(this).parent('.blockPersonalSettings').addClass('greyBorder').removeClass('greenBorder');
        //    });
    /*Password view end*/


    /*workFlowProcess*/


    /*workFlowProcessend*/

    /*user process*/

    //----------------END Open mini menu for mobile---------------------------

        //$('.blockPersonalSettings input')
        //   .focus(function () {
        //       $(this).parent('.blockPersonalSettings').css({ 'border': '2px solid #05b5a2', 'width': '99.8%' });
        //   })
        //    .focusout(function () {
        //        $(this).parent('.blockPersonalSettings').css({ 'border': '1px solid #e0e5e6', 'width': '100%' });
        //    });

    //START Open mini menu for mobile
        //function blockActivityHeight() {
        //    if ($('#menu').height() < 50) {
        //        $('.positionActivityIcon').height(89);
        //    }
        //    else {
        //        $('.positionActivityIcon').height($('#casesHeared').height());
        //    }
        //}
        //$('.mainTitle').click(function () {
        //    $('.mainTitle + div').toggle();
        //    blockActivityHeight();
        //});


        $('.newMessageBtn span').click(function () {
            if (($("#first_nm").val().trim.length > 0) && ($("#last_nm").val().trim.length > 0) && ($("#title_ds").val().trim.length > 0) && ($("#email").val().trim.length > 0)) {
                $('.newMessageBtn input').click();
            }
        });
        //RadioButton
        //pod katei vse knopki najimautsya. Mne nado, chtobi eta ne najimalas nikogda

        var levelId = $("#levelId").val();

        if (levelId == 5) {
            var RadioButton = $('.inputBlock2');
            RadioButton.click(function () {
                var mediatorBtn = $(this).parent();
                var mediatorAllBtn = $(this).parent().parent();
                mediatorAllBtn.find('.inputRadio').removeClass('active');
                mediatorBtn.addClass('active');
                mediatorBtn.find("input").prop('checked', true);

            });
            ////need to work
            var radioBlock = $('.inputBlock2');

            radioBlock.click(function () {

                var self = $(this);
                var arrows = self.parent('.inputRadio').parent('.rowBlock');
                arrows.find('.inputRadio').css('background', 'rgb(255, 255, 255)');
                arrows.find('.inputRadio').find('.radioTitle').css('color', 'rgb(174, 181, 183)');

                self.parent('.inputRadio').css('background', 'rgb(242, 247, 247)');
                self.find('.radioTitle').css('color', 'rgb(60, 62, 63)');
            });
        }
    /*user end process*/

    /*this is second page*/

        function selectblockMenu() {
            $('.itemFormat').click(function (event) {
                var self = $(event.currentTarget);
                //var parent = 
                self.parent().find('.itemFormat').removeClass('active');
                self.addClass('active');
                if (self.parents('.blockFormat').length > 0) {
                    var text = "download " + self.text().trim() + " poster (pdf)";
                    $(".downloadPdf").text(text);
                }
            });
        }

        selectblockMenu();

        /*$(".downloadPdf").on('click', function () {
            var availableFormat = $(".availableFormat .itemFormat.active").text().trim();
            var blockFormat = $(".blockOption.blockFormat .active").text().trim();
            if (availableFormat.length > 0 && blockFormat.length > 0) {
                $.ajax({
                    method: "POST",
                    url: "/Settings/UpdateDelays",
                    data: {
                        availableFormat: availableFormat,
                        blockFormat: blockFormat
                    }
                }).done(function (data) {//data from server
                    //$('#updateButton').css('display', 'none');
                    //$('#saveButton').css('display', 'block');
                    updateTOTALTIMEONCASE();
                }).fail(function (error) {
                    console.log(error);
                });
            }
        });*/
        //$("#EmployeeAwarenessBlock .")


    /**/
    /*outcomes */
        $('#addInputOutcome p').click(function () {
            var temp = $(this).parent();
            adding("Outcome", temp);
        });
        $('#addOutBtn').click(function () {
            var data = $(".addNewOutcome input").val();
            if (data.length > 0) {
                sendAjax("Outcome", data, function (id) {
                    close("Outcome", data, id);
                });
            } else {
                $('.addNewOutcome').css('border-color', 'red');
            }
        });

    /*delete deleteOutcome*/
        $(".tableOutcome").on('click', '.deleteOutcome', function (element) {
            var temp = $(element.target);
            var id = temp.attr("data-value");
            sendAjax("deleteOutcome", id, function () {
                temp.parent().hide(200);
            });
        });
    /*end outcomes */
});