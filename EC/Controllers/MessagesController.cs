using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EC.Models.Database;
using EC.Models.ECModel;
using EC.Controllers.ViewModel;
using EC.Models;
using EC.Constants;

namespace EC.Controllers
{
    public class MessagesController : BaseController
    {
        // GET: Messages
        public ActionResult Index()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Account");
            
            #region EC-CC Viewbag
            ViewBag.is_cc = is_cc;
            string cc_ext = "";
            if (is_cc) cc_ext = "_cc";
            ViewBag.cc_extension = cc_ext;
            #endregion

          
            int user_id = user.id;
            UserModel um = new UserModel(user_id);
            ViewBag.um = um;

            ViewBag.user_id = user_id;
            MessagesViewModel _messages = new MessagesViewModel(user.id);

            ViewBag.Message = _messages;
            //ViewBag.um = um;
            return View();
        }
    }
}