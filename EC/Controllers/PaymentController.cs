using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC.Models.Database;
using EC.Constants;
using EC.Models;
using LavaBlast.Util.CreditCards;
using System.Configuration;

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
            CompanyModel cm = new CompanyModel(um._user.company_id);

            List<company_payments> all_payments = db.company_payments.Where(t => t.company_id == um._user.company_id).ToList();
            ViewBag.payments = all_payments;
            ViewBag.user_id = user.id;
            ViewBag.cm = cm;
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

        public ActionResult NewPayment()
        {
            // to make payment
            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return RedirectToAction("Index", "Start");

            UserModel um = new UserModel(user.id);
            CompanyModel cm = new CompanyModel(um._user.company_id);

            decimal _amount = 0;

            if (cm._company.next_payment_date.HasValue && cm._company.next_payment_amount.HasValue)
            {
                DateTime dt = cm._company.next_payment_date.Value;
                if (dt < DateTime.Today.AddMonths(1) || dt < DateTime.Today)
                {
                    _amount = cm._company.next_payment_amount.Value;
                }
            }

            ViewBag.user_id = user.id;
            ViewBag.cm = cm;
            ViewBag.amount = _amount;

            return View();
        }

        public string Pay( string amount, string cardnumber, string cardname, string csv, string selectedMonth, string selectedYear)
        {

            user user = (user)Session[ECGlobalConstants.CurrentUserMarcker];
            if (user == null || user.id == 0)
                return "Error - Login again";
            UserModel um = new UserModel(user.id);
            CompanyModel cm = new CompanyModel(um._user.company_id);

            decimal _amount = 0;
            if (!string.IsNullOrEmpty(amount) && amount != "0")
            {
                // amount more than 0 -> we have a registration with money involved
                decimal.TryParse(amount, out _amount);
            }

            if (_amount > 0)
            {
                if ((string.IsNullOrEmpty(cardnumber)) || (string.IsNullOrEmpty(cardname)) || (string.IsNullOrEmpty(csv)) || (string.IsNullOrEmpty(selectedMonth)) || (string.IsNullOrEmpty(selectedYear)))
                {
                    return App_LocalResources.GlobalRes.EmptyData;
                }
            }
            #region Credit Card
            string auth_code = "";
            string payment_auth_code = "";
            if (_amount > 0)
            {
                /// amount, string cardnumber, string cardname, string csv
                BeanStream.beanstream.TransactionProcessRequest tpr = new BeanStream.beanstream.TransactionProcessRequest();
                BeanStream.beanstream.TransactionProcessAuthRequest tpar = new BeanStream.beanstream.TransactionProcessAuthRequest();

                // to do - move constants to AppSettingsConstants
                BeanStreamProcessing bsp = new BeanStreamProcessing(ConfigurationManager.AppSettings["bs_merchant_id"]);
                string cc_error_message = "";

                int _month = 0;
                int _year = 0;
                if (selectedMonth.StartsWith("0"))
                    selectedMonth = selectedMonth[1].ToString();

                int.TryParse(selectedMonth, out _month);
                int.TryParse(selectedYear, out _year);

                if (_month == 0 || _year == 0)
                    return App_LocalResources.GlobalRes.EmptyData;

                var random = new Random();
                payment_auth_code = glb.GenerateInvoiceNumber(); // "INV_" + random.Next(10001, 99999).ToString(); 



                auth_code = payment_auth_code;
                /*    Dictionary<BeanStreamProcessing.RequestFieldNames, string> _dictionary = new Dictionary<BeanStreamProcessing.RequestFieldNames, string>();
                    bsp.ProcessRequest(_dictionary, out cc_error_message, out auth_code);
                    if (cc_error_message.Trim().Length > 0)
                    {
                        return cc_error_message;
                    }*/
            }

            #endregion

            #region Saving CC_Payment
            if (_amount > 0)
            {
                company_payments _cp = new company_payments();
                _cp.amount = _amount;
                _cp.auth_code = auth_code.Trim();
                _cp.local_invoice_number = payment_auth_code.Trim();
                _cp.cc_csv = Convert.ToInt32(csv);

                _cp.cc_month = Convert.ToInt32(1);
                _cp.cc_year = Convert.ToInt32(2017);

                _cp.cc_name = cardname.Trim();
                _cp.cc_number = glb.ConvertCCInfoToLast4DigitsInfo(cardnumber.Trim());

                _cp.company_id = user.company_id;
                _cp.payment_date = DateTime.Today;
                _cp.id = Guid.NewGuid();
                _cp.user_id = um._user.id;

                try
                {
                    db.company_payments.Add(_cp);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            #endregion
            Session["Auth"] = auth_code;
            Session["Amount"] = _amount;

            if (cm._company.next_payment_date.HasValue)
            {
                company _cm = db.company.FirstOrDefault(item => (item.id == cm._company.id));
                _cm.next_payment_date = cm._company.next_payment_date.Value.AddYears(1);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }

            return App_LocalResources.GlobalRes._Completed.ToLower();

            //  return user_id.ToString();

        }

    }
}