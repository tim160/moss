using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace EC.Utils.Auth
{
    public class User : IUser<string>
    {

        public string Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }

        public string Role {
            get { return "Test"; } 
           }

        string IUser<string>.Id
        {
            get { return Id; }
        }

        public string UserName
        {
            get { return Login; }
            set { Login = value; }
        }
    }
}