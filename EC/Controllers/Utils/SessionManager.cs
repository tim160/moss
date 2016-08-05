using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using EC.Models.Database;

namespace EC.Controllers.Utils
{
    public class SessionManager
    {
        public static readonly SessionManager inst = new SessionManager();
        protected readonly ThreadLocal<string> langCode = new ThreadLocal<string>();
        protected readonly ThreadLocal<user> user = new ThreadLocal<user>();
                   //userKey(cookies)         //UserValues
        //Dictionary<string, Dictionary<string, object>> sessionMap;

        protected SessionManager()
        {
        }

        public user User 
        {
            get { return user.Value; }
            set { user.Value = value; }
        }

        public string Lang
        {
            get { return langCode.Value; }
            set { langCode.Value = value; }
        }

    }
}