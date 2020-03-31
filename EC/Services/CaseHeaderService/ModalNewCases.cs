using EC.Constants;
using EC.Localization;
using EC.Models;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Services.CaseHeaderService
{
    public class ModalNewCases
    {
        private ECEntities db;
        private ReportModel rm;
        private int user_id;
        private string modalWindow = String.Empty;

        public ModalNewCases(ECEntities db, ReportModel rm, int user_id)
        {
            this.db = db;
            this.rm = rm;
            this.user_id = user_id;
        }

        //1) метод выбираем тип модального окна
        //2) соотв грузим данные для конкретного мод окна

        //метод какую модалку грузить. будет параметр за это отвечающий
        private void generateModalWindowData()
        {
            int investigationstatus = rm._investigation_status;

            if (investigationstatus == 9)
            {
                this.ReOpenModalNew();
            }
            else if (investigationstatus == (int)CaseStatusConstants.CaseStatusValues.Completed ||
                      investigationstatus == (int)CaseStatusConstants.CaseStatusValues.Resolution)
            {

            }



            var _user = db.user.Find(user_id);


            string Res_or_comp = LocalizationGetter.GetString("CloseCaseUp");//какой то параметр предается в модалку

            string normal_button_value = ((int)CaseStatusConstants.CaseStatusValues.Completed).ToString();




            if ((investigationstatus > 2 && investigationstatus < 7) || (investigationstatus == 9))
            {
                Res_or_comp = LocalizationGetter.GetString("CloseCaseUp");
                if ((investigationstatus == 4) || (investigationstatus == 6))
                {
                    Res_or_comp = "";
                }
                if ((investigationstatus == 9) && (_user.role_id == 4 || _user.role_id == 5))
                {
                    Res_or_comp = LocalizationGetter.GetString("Reopen");
                    normal_button_value = ((int)CaseStatusConstants.CaseStatusValues.Investigation).ToString();
                }
                if (((rm._investigation_status == (int)CaseStatusConstants.CaseStatusValues.Completed || rm._investigation_status == (int)CaseStatusConstants.CaseStatusValues.Resolution)))
                {
                    Res_or_comp = LocalizationGetter.GetString("Awaitingapproval");
                }
            }
        }

        private void ReOpenModalNew()
        {

        }

        private void AwaitingApprovalModalNew()
        {

        }

        private void ApproveDialogModalNew()
        {

        }
    }
}


//1) LayoutCasesNewReport   
//-- Delete section( if report_id > 0) - we don't use this section for newReport Views anymore. 
//-- delete section @if((hrefURL.Contains("/case/")) && (_report_id != 0))   
//-- for if (CurrentURL.Contains("settings/user"))  use urlService
//-- check if we need all js(textarea#txtresolve) , etc - I don't think so.
//2)LayoutNewCases
//-- if (hrefURL.Contains("/settings"))   - move it to urlService.
//# I will add logic in next comment

/*_NewCasesTopMenu.cshtml*/
/*LayoutNewCases.cshtml*/

//@if(investigationstatus == 9)
//{
//    @Html.Partial("~/Views/Shared/EditorTemplates/ReOpenModalNew.cshtml", new ViewDataDictionary {
//                               { "normal_button_promotion", normal_button_promotion },
//                               { "Res_or_comp", Res_or_comp },
//                               { "overlay_title", overlay_title },
//                               { "textbox_title", textbox_title },
//                               { "investigationstatus", investigationstatus },
//                               { "left_button_promotion", left_button_promotion },
//                               { "normal_button_value", normal_button_value },
//                               { "left_button_value", left_button_value },
//                               { "company_outcomes", company_outcomes },
//                               { "report_id", ViewBag.report_id },
//                               { "is_cc", is_cc }
//                        })
//    }
//    else if (((rm._investigation_status == (int) CaseStatusConstants.CaseStatusValues.Completed || rm._investigation_status == (int) CaseStatusConstants.CaseStatusValues.Resolution)))
//    {
//        @Html.Partial("~/Views/Shared/EditorTemplates/AwaitingApprovalModalNew.cshtml", new ViewDataDictionary {
//                               { "normal_button_promotion", normal_button_promotion },
//                               { "Res_or_comp", Res_or_comp },
//                               { "overlay_title", overlay_title },
//                               { "textbox_title", textbox_title },
//                               { "investigationstatus", investigationstatus },
//                               { "left_button_promotion", left_button_promotion },
//                               { "normal_button_value", normal_button_value },
//                               { "left_button_value", left_button_value },
//                               { "company_outcomes", company_outcomes },
//                               { "report_id", ViewBag.report_id },
//                               { "is_cc", is_cc },
//                               { "user_id", user_id}
//                        })
//        @Html.Partial("~/Views/Shared/EditorTemplates/ApproveDialogModalNew.cshtml", new ViewDataDictionary { })
//    }
//    else
//    {
//        @Html.Partial("~/Views/Shared/EditorTemplates/CreateTaskModalNew.cshtml", new ViewDataDictionary {
//                               { "normal_button_promotion", normal_button_promotion },
//                               { "Res_or_comp", Res_or_comp },
//                               { "overlay_title", overlay_title },
//                               { "textbox_title", textbox_title },
//                               { "investigationstatus", investigationstatus },
//                               { "left_button_promotion", left_button_promotion },
//                               { "normal_button_value", normal_button_value },
//                               { "left_button_value", left_button_value },
//                               { "company_outcomes", company_outcomes },
//                               { "report_id", ViewBag.report_id },
//                               { "is_cc", is_cc },
//                               { "user_id", user_id}
//                        })
//    }