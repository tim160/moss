using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using EC.Constants;

namespace EC.Utils
{
    public class PasswordUtils
    {
        public static string GetHash(string password)
        {
            return password;
            /*
            password = $"{password}{ConfigurationManager.AppSettings["PasswordSalt"]}{password}";

            byte[] data = System.Text.Encoding.Default.GetBytes(password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            var hash = System.Convert.ToBase64String(data);

            return hash;*/
        }

        public static bool Validate(string password, string hash)
        {
            var hash2 = GetHash(password);

            return hash == hash2;
        }

        public static string IsValidPass(string password)
        {
          if (password != "" && password.Length >= PasswordConstants.PASSWORD_MIN_LENGTH)
          {
            return "Success";
          }
          return String.Format("The password should be at least {0} characters long", PasswordConstants.PASSWORD_MIN_LENGTH.ToString());
        }

  }
}