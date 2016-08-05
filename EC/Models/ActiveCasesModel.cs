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

        public report getReport(string reportName)
        {
            /*  SELECT * FROM [EC].[dbo].[report] WHERE id=xxxx*/
            return (report)(from ca in db.report
                            where (ca.display_name == reportName)
                            select ca);
        }

        public report getReport(int reportId)
        {
            /*  SELECT * FROM [EC].[dbo].[report] WHERE id=xxxx*/

            var r = db.report.Where(s => (s.id == reportId));

            if (r.First() == null)
            {
                return new report();
            }
            else
            {
                return r.First();
            }
        }
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