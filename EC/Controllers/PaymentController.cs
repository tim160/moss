using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC.Models.Database;
using EC.Constants;
using EC.Models;

namespace EC.Controllers
{
    public class PaymentController : BaseController
    {
        // GET: History
        public ActionResult History()
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Start");

            UserModel um = new UserModel(user.id);

            List<company_payments> all_payments = db.company_payments.Where(t => t.company_id == um._user.company_id).ToList();
            ViewBag.payments = all_payments;
            ViewBag.user_id = user.id;
            return View();
        }

        public ActionResult Index()
        {
            return History();
        }

        public ActionResult Receipt(string id)
        {
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Start");

            UserModel um = new UserModel(user.id);
            ViewBag.user_id = user.id;
            Guid _id;
            if ((id != null) && (id != ""))
            {
                try
                {
                    _id = new Guid(id);
                }
                catch 
                {
                    return RedirectToAction("History", "Payment");
                }
            }
            else
                return RedirectToAction("History", "Payment");

            List<company_payments> all_payments = db.company_payments.Where(t => t.company_id == um._user.company_id && t.id == _id).ToList();
            
            if(all_payments.Count == 0)
                return RedirectToAction("History", "Payment");

            company_payments cp = all_payments[0];

            ViewBag.cp = cp;
            return View();
        }
    }
}