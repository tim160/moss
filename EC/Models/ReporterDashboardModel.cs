using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models;
using EC.Models.Database;
using System.Text.RegularExpressions;

using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;

namespace EC.Models
{
    public class ReporterDashboardModel : BaseModel
    {
        public static readonly ReporterDashboardModel inst = new ReporterDashboardModel();
        public bool UpdateUser(HttpRequestBase request, user sessionUser, int incidentAnonymity)
        {
            user user = new user();
            string data = string.Empty;
            string field = string.Empty;
            try
            {
                user.id = Convert.ToInt32(request.QueryString["userId"]);
                if (user.id == sessionUser.id)
                    if (incidentAnonymity != 1)
                    {
                        string email = request.QueryString["data[email]"];
                        string lastName = request.QueryString["data[lastName]"];
                        string firstName = request.QueryString["data[firstName]"];
                        string pattern = @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z][a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";
                        Match match = Regex.Match(email.Trim(), pattern, RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            sessionUser.email = email;
                        } else
                        {
                            return false;
                        }
                        pattern = @"^[a-zA-Z][a-zA-Z0-9-_\.][^.]{1,20}$";
                        match = Regex.Match(firstName.Trim(), pattern, RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            sessionUser.first_nm = firstName;
                        }
                        else
                        {
                            return false;
                        }
                        match = Regex.Match(lastName.Trim(), pattern, RegexOptions.IgnoreCase);
                        if(match.Success)
                        {
                            sessionUser.last_nm = lastName;
                        } else
                        {
                            return false;
                        }

                    }
                sessionUser.last_update_dt = DateTime.Now;
                sessionUser.notification_new_reports_flag = Convert.ToInt32(request.QueryString["data[checkBox]"]);
                db.user.AddOrUpdate(sessionUser);
                db.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }
    }
}