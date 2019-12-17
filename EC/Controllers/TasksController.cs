using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using EC.Models;
using EC.Models.Database;
using EC.Models.ECModel;
using EC.Constants;

namespace EC.Controllers
{
    public class TasksController : BaseController
    {
        //
        // GET: /Tasks/
        public ActionResult AllMyTasks()
        {
            return View();
        }

        public ActionResult Index()
        {

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          
            int user_id = user.id;
            // user
            UserModel um = new UserModel(user_id);
            ReportModel rm = new ReportModel();

          //  List<task> tasks = um.UserTasks(0, null);
            List<task> tasks = um.UserTasks(1, null, true);
            List<TaskExtended> list_tsk = new List<TaskExtended>();
            int task_id = 0;
            int unread_tasks = 0;
            foreach (task _task in tasks)
            {
                task_id = _task.id;
                TaskExtended tsk = new TaskExtended(_task.id, user_id);
                
                if (!tsk.IsRead())
                    unread_tasks++;

                list_tsk.Add(tsk);
            }
            ViewBag.tasks = (list_tsk.OrderBy(m => m.TaskDueDate)).ToList();
            ViewBag.user_id = user_id;
            ViewBag.um = um;
            ViewBag.unread_tasks = unread_tasks;
            List<report> reports =  um.ReportsSearch(0, 0);
            ViewBag.reports = reports;

            return View();
        }

        public ActionResult Completed()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            int user_id = user.id;
            // user
            UserModel um = new UserModel(user_id);
            ReportModel rm = new ReportModel();

            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion



            //  List<task> tasks = um.UserTasks(0, null);
            List<task> tasks = um.UserTasks(2, null, true);
            List<TaskExtended> list_tsk = new List<TaskExtended>();
            int task_id = 0;
            foreach (task _task in tasks)
            {
                task_id = _task.id;
                TaskExtended tsk = new TaskExtended(_task.id, user_id);
                list_tsk.Add(tsk);
            }
            ViewBag.tasks = list_tsk;
            ViewBag.user_id = user_id;
            ViewBag.um = um;


            List<task> tasks_active = um.UserTasks(1, null, true);
            int unread_tasks = 0;

            //redo this shit
            foreach (task _task in tasks_active)
            {
                task_id = _task.id;
                TaskExtended tsk = new TaskExtended(_task.id, user_id);

                if (!tsk.IsRead())
                    unread_tasks++;
            }
            ViewBag.unread_tasks = unread_tasks;

            List<report> reports = um.ReportsSearch(0, 0);
            ViewBag.reports = reports;

            return View();
        }
	}
}