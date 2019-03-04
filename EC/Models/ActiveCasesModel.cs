using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using EC.Models.Database;
using EC.Controllers.ViewModel;
using EC.Controllers.Utils;
using Microsoft.Ajax.Utilities;


namespace EC.Models.ECModel
{
    public class ActiveCasesModel : BaseModel
    {
        
        ActiveCasesViewModel activeCase = new ActiveCasesViewModel();

        public void HasTaskFile(TaskExtended task)
        {
            List<attachment> list = new List<attachment>();
            if (task.TaskID > 0)
            {
                list = db.attachment.Where(item => (item.report_task_id == task.TaskID)).ToList();
            }
            if(list.Count > 0)
            {
                task.file = true;
            }
            task.fileAttach = list;
        }
    }
}