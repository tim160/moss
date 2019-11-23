using EC.Localization;
using EC.Models.Database;
using System.Linq;
using System;
using EC.Common.Util;
using EC.Utils;
using System.Data.Entity.Migrations;

namespace EC.Models
{
    public class LoginModel : BaseModel
    {
        public string restorePass(string token, string email)
        {
            GlobalFunctions glb = new GlobalFunctions();

            if (token != null && token.Length > 0)
            {
                if (email != null && email.Length > 0)
                {
                    if (m_EmailHelper.IsValidEmail(email.Trim()))
                    {
                        user_change_password _ucp = (db.user_change_password.Where(t => t.password_token.ToLower().Trim() == token.ToLower().Trim())).FirstOrDefault();
                        if (_ucp != null)
                        {
                            int user_id = _ucp.user_id;
                            UserModel um = new UserModel(user_id);
                            if (um._user.email.Trim().ToLower() != email.Trim().ToLower())
                            {
                                return LocalizationGetter.GetString("InvalidToken");
                            }
                            else
                            {
                                return "success";
                            }
                        }
                        else
                            return LocalizationGetter.GetString("InvalidToken");
                    }
                    else
                        return LocalizationGetter.GetString("EmailInvalid");
                }
                else
                    return LocalizationGetter.GetString("EmailInvalid");
            }
            else
                return LocalizationGetter.GetString("NoUserFound");
        }
        public string setNewPass(string email, string token, string password, string confirmPassword)
        {
            if (restorePass(token, email) == "success")
            {
                if (password == confirmPassword)
                {
                    string result = GlobalFunctions.IsValidPass(password);
                    if (result == "Success")
                    {
                        user_change_password _ucp = (db.user_change_password.Where(t => t.password_token.ToLower().Trim() == token.ToLower().Trim())).FirstOrDefault();
                        if (_ucp != null)
                        {
                            _ucp.password_updated = 1;
                            _ucp.updated_on = DateTime.Now;
                            _ucp.updated_user_ip = DomainUtil.GetUser_IP();
                            db.SaveChanges();
                            user uptUser = db.user.Where(item => item.id == _ucp.user_id).FirstOrDefault();
                            uptUser.password = password;
                            db.SaveChanges();
                            return "Success";
                        }
                    }
                    else
                    {
                        return result;
                    }
                }
                else
                {
                    return LocalizationGetter.GetString("notMatchConfPassandPass");
                }
            }
            else
            {
                return LocalizationGetter.GetString("reEnterEmailToken");
            }
            return "";
        }

    public string SaveLoginChanges(int userId, string password)
    {
      if (userId > 0)
      {
        try
        {
          string result = GlobalFunctions.IsValidPass(password);
          if (result.ToLower() == "success")
          {

            user user = db.user.FirstOrDefault(item => (item.id == userId));
            if (user != null)
            {
              using (ECEntities adv = new ECEntities())
              {
                user.password = PasswordUtils.GetHash(password);
                user.last_update_dt = DateTime.Now;
                adv.user.AddOrUpdate(user);
                adv.SaveChanges();

                //  db.user.AddOrUpdate(user);
                //  db.SaveChanges();
              }
              return result;
            }
            else
            {
              return LocalizationGetter.GetString("NoUserFound");
            }
          }
          else
            return result;

        }
        catch (Exception ex)
        {
          logger.Error(ex.ToString());
          return "Cannot update password " + ex.ToString();// LocalizationGetter.GetString("ErrorSavingLoginPass", is_cc);
        }
      }
      else
      {
        return "Cannot update your password "; // LocalizationGetter.GetString("ErrorSavingLoginPass", is_cc);
      }
    }
  }
}