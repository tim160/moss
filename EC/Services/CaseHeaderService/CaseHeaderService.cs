using EC.Constants;
using EC.Models;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Services.ProgressLineService
{
    public class CaseHeaderService
  {
        private int user_id;
        private ECEntities db;
        private ReportModel rm;
        public CaseHeaderService(int user_id, ECEntities db, ReportModel rm)
        {
            this.user_id = user_id;
            this.db = db;
            this.rm = rm;
        }


        public CaseHeaderViewModel getHeaderProgressLine()
        {

            var progressLineViewModel = new CaseHeaderViewModel();
            if (checkUserRole())
            {

            }


            /*
             1) можно взять роль юзера ,и по ней определить надо ли ему грузить медиаторов или только вернуть колическтво закрашеных полосок
                1) метод для определния кол полосокж
                2) метод для генерации заполнения модалки + кнопки + ссылки
                3) список медиаторов меющих доступ к этой  [картинки медиторов и имена их]
             */

            return progressLineViewModel;
        }

        private int gettingProgressStepsCount()
        {
            int investigationstatus = rm._investigation_status;

            if (investigationstatus == 6)
            {
                investigationstatus = 4;
            }

            return 0;
        }

        private bool checkUserRole()
        {
            var currentUser = db.user.Find(user_id);

            if (currentUser.role_id == ECLevelConstants.level_informant)
            {
                return false;
            }
            return true;
        }

    }
}
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