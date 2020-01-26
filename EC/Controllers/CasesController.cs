using System.Collections.Generic;
using System.Web.Mvc;
using EC.Models;
using EC.Models.Database;
using EC.Constants;
using EC.Models.ViewModels;

namespace EC.Controllers
{
    public class CasesController : BaseController
    {
        public ActionResult Index()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Login", "Service");

            UserModel um = new UserModel(user.id);
            UsersReportIDsViewModel vmAllIDs = um.GetAllUserReportIdsLists();

            var pending_report_ids = vmAllIDs.all_pending_report_ids;
            List<int> all_pending_reports_ids = new List<int>();
            if (pending_report_ids != null && pending_report_ids.Count > 0)
            {
                pending_report_ids.Sort();
                all_pending_reports_ids.Add(pending_report_ids[0]);
            }
            ViewBag.pending_report_ids = all_pending_reports_ids;



            //ViewBag.um = um;
            ViewBag.user_id = user.id;


            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

            return View();
        }

        public ActionResult Completed()
        {
            return RedirectToAction("Index", "Cases", new { mode = "completed" });
        }

        public ActionResult Closed()
        {
            return RedirectToAction("Index", "Cases", new { mode = "closed" });
        }

        public ActionResult Spam()
        {

            return RedirectToAction("Index", "Cases", new { mode = "spam" });

        }
        public ActionResult New()
        {
            return RedirectToAction("Index", "Cases", new { mode = "new" });
        }

    }
}