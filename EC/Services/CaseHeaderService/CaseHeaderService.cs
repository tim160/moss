using EC.Constants;
using EC.Localization;
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
        private const string ACTIVE_CLASS_NAME = "active";

        //investigationstatus вынести в отедльный переменную

        public CaseHeaderService(int user_id, ECEntities db, ReportModel rm)
        {
            this.user_id = user_id;
            this.db = db;
            this.rm = rm;
        }


        public CaseHeaderViewModel getHeader()
        {

            var progressLineViewModel = new CaseHeaderViewModel();
            if (checkUserRole())
            {

            }
            progressLineViewModel.ProgressStepsCount = gettingProgressLineStepsCount();

            /*
             1) можно взять роль юзера ,и по ней определить надо ли ему грузить медиаторов или только вернуть колическтво закрашеных полосок
                1) метод для определния кол полосокж
                2) метод для генерации заполнения модалки + кнопки + ссылки
                3) список медиаторов меющих доступ к этой  [картинки медиторов и имена их]
             */

            return progressLineViewModel;
        }



        private List<DataProgressLine> gettingProgressLineStepsCount()
        {
            //передасть во вью массив с текстом и классом active или done

            var progressLine = new List<DataProgressLine>(ECGlobalConstants.ReportFlowStatusesList.Length);

            int investigationstatus = rm._investigation_status;

            if (investigationstatus == (int)CaseStatusConstants.CaseStatusValues.Completed)
            {
                investigationstatus = (int)CaseStatusConstants.CaseStatusValues.Resolution;
            }

            //investigation_status_id int from 1 to 9

            for (int i = 1; i <= ECGlobalConstants.ReportFlowStatusesList.Length; i++)
            {
                var newItem = new DataProgressLine();
                if (i <= investigationstatus)
                {
                    newItem.ProgressStepsClass = ACTIVE_CLASS_NAME;
                }
                else
                {
                    newItem.ProgressStepsClass = "";
                }

                newItem.ProgressStepsText = ECGlobalConstants.ReportFlowStatusesList[i-1];
                progressLine.Add(newItem);

            }

            return progressLine;
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
