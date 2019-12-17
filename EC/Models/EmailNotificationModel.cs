using System;
using System.Web;
using System.Configuration;
using System.Data;
using EC.Models.Database;
using System.Linq;
using EC.Common.Interfaces;
using EC.Core.Common;
using EC.Common.Util;
using EC.Localization;
using EC.Constants;
using EC.Utils;

namespace EC.Models
{

  public class EmailNotificationModel : BaseModel
  {

    ///Email.EmailManagement = check if we need it   , add emailHelper. add this to BaseModel, controllers
    //replace with select2item - CompanyLocationViewModel, DepartmentsViewModel, InjuryDamageViewModel, ManagamentViewModel, ReportedOutsideViewModel
    // UserModel - ReportsSearch() - merge with ReportSearchID/ Remove UnreadReportQuantity,etc   -- CloseTask/OpenTask- Move to TaskModel?
    // reportModel  getCompanySecondaryType(3 func)  isCustomIncidentTypes(check,not needed) 
    //     LocalizationGetter.GetString_bkp
    //Unread_message_number_string1  +ReporterDashboardModel(save call)
    //  Add new models -  ReportStringPropertiesModel???    AnaslyticsModel?
    //      @Model._reporter_user.first_nm   @Model._reporter_user.last_nm  -- PDF!!!! - move 2 userDetails to function/Model. Rename 3 to Const
    public EmailNotificationModel()
    {

    }

    public void CampusSecurityAlertEmail(int user_from, report report, Uri uri, ECEntities db, string email)
    {
      ////   return;
      IEmailAddressHelper m_EmailHelper = new EmailAddressHelper();
      EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(true);
      EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, uri.AbsoluteUri.ToLower());

      var user_to = db.user.FirstOrDefault(x => x.role_id == 5 && x.company_id == report.company_id);
      if ((user_to != null) && (email.Trim().Length > 0) && m_EmailHelper.IsValidEmail(email.Trim()))
      {
        string phone = $"{user_to.phone}";
        if (string.IsNullOrEmpty(phone))
          phone = $"{user_to.email}";

        eb.CampusSecurityAlert(
            report.id.ToString(),
            report.display_name,
            $"{user_to.first_nm} {user_to.last_nm}",
            phone
            );
        SaveEmailBeforeSend(user_from, user_to.id, user_to.company_id, email.Trim(), ConfigurationManager.AppSettings["emailFrom"], "",
           LocalizationGetter.GetString("CampusSecurityAlert", true), eb.Body, false, 51);
      }
    }

    public bool SaveEmailBeforeSend(int user_id_from, int user_id_to, int company_id, string to, string from, string cc, string subject, string msg, bool send, int email_type)
    {

      if ((to.Trim().Length > 0) && m_EmailHelper.IsValidEmail(to.Trim()))
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
        db.email.Add(email);
        db.SaveChanges();

        return true;
      }
      else
        return false;
    }

    public string resendInvitation(string email, bool is_cc, HttpRequestBase Request, user AdminUser)
    {
      email = email.ToLower().Trim();
      var invite = db.invitation.Where(i => i.email == email).FirstOrDefault();

      if (invite != null)
      {
        var userSender = db.user.Find(invite.sent_by_user_id);
        var company = db.company.FirstOrDefault(x => x.id == userSender.company_id);

        IEmailAddressHelper m_EmailHelper = new EmailAddressHelper();

        if ((email.Length > 0) && m_EmailHelper.IsValidEmail(email))
        {
          EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
          EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, Request.Url.AbsoluteUri.ToLower());

          eb.MediatorInvited(AdminUser.first_nm, AdminUser.last_nm,
              userSender.first_nm, userSender.last_nm, company.company_nm, invite.code,
              DomainUtil.GetSubdomainLink(Request.Url.AbsoluteUri.ToLower(),
              Request.Url.AbsoluteUri.ToLower()) + "/new/?code=" + invite.code + "&email=" + email);

          SaveEmailBeforeSend(userSender.id, 0, 0, email.Trim(), System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "",
             LocalizationGetter.GetString("Email_Title_MediatorInvited", is_cc), eb.Body, false, 41);

          return LocalizationGetter.GetString("InvitationWasSent", is_cc);
        }
      }
      else
      {
        var newAdminUser = db.user.Where(u =>
            u.email.Equals(email) &&
            u.company_id == AdminUser.company_id &&
            u.status_id == ECStatusConstants.Pending_Value).FirstOrDefault();

        if (newAdminUser != null)
        {
          var company = db.company.Where(c => c.id == newAdminUser.company_id).FirstOrDefault();

          sendAddNewAdmin(AdminUser, company, newAdminUser, is_cc);

          return LocalizationGetter.GetString("InvitationWasSent", is_cc);
        }
      }
      return LocalizationGetter.GetString("ErrorResendingEmail", is_cc);
    }
    public void sendAddNewAdmin(user adminUser, company company, user newUser, bool is_cc)
    {
      var generateModel = new GenerateRecordsModel();

      var password = generateModel.GeneretedPassword();

      EC.Business.Actions.Email.EmailManagement em = new EC.Business.Actions.Email.EmailManagement(is_cc);
      EC.Business.Actions.Email.EmailBody eb = new EC.Business.Actions.Email.EmailBody(1, 1, HttpContext.Current.Request.Url.AbsoluteUri.ToLower());
      eb.NewMediator(
          $"{adminUser.first_nm} {adminUser.last_nm}",
          $"{company.company_nm}",
          HttpContext.Current.Request.Url.AbsoluteUri.ToLower(),
          HttpContext.Current.Request.Url.AbsoluteUri.ToLower(),
          $"{newUser.login_nm}",
          $"{password}");
      string body = eb.Body;
      SaveEmailBeforeSend(adminUser.id, newUser.id, adminUser.company_id,
          newUser.email, System.Configuration.ConfigurationManager.AppSettings["emailFrom"], "", "You have been added as a Case Administrator", body, false, 0);

      var newUserDb = db.user.Find(newUser.id);
      newUserDb.password = PasswordUtils.GetHash(password);
      db.SaveChanges();
    }
  }

}