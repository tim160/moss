using EC.COM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Localization;

namespace EC.COM.Models
{
    public class GlobalFunctions
    {

        public bool SaveEmailBeforeSend(int user_id_from, int user_id_to, int company_id, string to, string from, string cc, string subject, string msg, bool send, int email_type)
        {
            var email = new email
            {
                To = to,
                From = from,
                cc = cc,
                Title = subject,
                Body = msg,
                EmailType = email_type,
                is_sent = false,
                user_id_from = user_id_from,
                user_id_to = user_id_to,
                company_id = company_id,
                created_dt = DateTime.Now,
                isSSL = false
            };
            using (var db = new DBContext())
            {
                db.emails.Add(email);
                db.SaveChanges();
            }

            return true;
        }

        public string BookingECOnboardingSessionNotifications(company company, string orderNumber, long price, int sessionNumber, bool is_cc, HttpRequestBase requestBase)
        {
            Business.Actions.Email.EmailManagement em = new Business.Actions.Email.EmailManagement(false);
            Business.Actions.Email.EmailBody eb = new Business.Actions.Email.EmailBody(1, 1, requestBase.Url.AbsoluteUri.ToLower());
            user user = null;
            if (company != null)
            {
                using (var db = new DBContext())
                {
                    user = db.users.Where(us => us.company_id == company.id && us.role_id == 5).FirstOrDefault();
                }
            }
            string sessions = "";
            switch (sessionNumber)
            {
              case 1:
                sessions = "(one session)";
                break;
              case 2:
                sessions = "(two sessions)";
                break;
              case 3:
                sessions = "(three sessions)";
                break;
            }

            eb.BookingOnboardingSeparately((body) =>
            {
                body = body.Replace("[First Name]", user.first_nm);
                body = body.Replace("[Last Name]", user.last_nm);

                body = body.Replace("[orderNumber]", orderNumber);

                body = body.Replace("[price]", price.ToString());
                body = body.Replace("[sessionNumber]", sessions);

                body = body.Replace("[Company Name]", company.company_nm);

                return body;
            });

            string title = LocalizationGetter.GetString("BookingECOnboardingSessionNotifications", is_cc);

            SaveEmailBeforeSend(2, user.id, user.company_id, user.email, "employeeconfidential@employeeconfidential.com", null, title, eb.Body, false, 74);
            return "ok";
        }
        public string TimeslotHasBeenBooked(user user, company company, bool is_cc, HttpRequestBase requestBase)
        {
            Business.Actions.Email.EmailManagement em = new Business.Actions.Email.EmailManagement(false);
            Business.Actions.Email.EmailBody eb = new Business.Actions.Email.EmailBody(1, 1, requestBase.Url.AbsoluteUri.ToLower());
            //var trainerTimes = null;
            using (var db = new DBContext())
            {
              //  trainerTimes = db.TrainerTimes.where(c => c.company.id == company.id).FirstOrDefault();
            }
            //DateTime date = DateTime.Now();
            //var tempDate = date.Date;
            //var tempTime = date.TimeOfDay;

            //var testTime = db.TrainerTimes.Where(com => com.CompanyId == 2).FirstOrDefault();
            //var testTimeDate = testTime.Hour.Date.ToShortDateString();
            //var testTimeHours = testTime.Hour.TimeOfDay;
            eb.ConfirmationTimeslot((body) =>
            {
                body = body.Replace("[orderNumber]", "1");
                body = body.Replace("[sessionDate]", "");
                body = body.Replace("[sessionTime]", "");
                body = body.Replace("[First Name]", user.first_nm);
                body = body.Replace("[Last Name]", user.last_nm);
                body = body.Replace("[Contact]", "");
                return body;
            });

            string title = LocalizationGetter.GetString("TrainingTimeslotBooked", is_cc);
            SaveEmailBeforeSend(2, user.id, user.company_id, user.email, "employeeconfidential@employeeconfidential.com", null, title, eb.Body, false, 75);

            return "ok";

        }
    }
}