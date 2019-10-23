using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.ViewModels
{
    public class LoginUserViewModel
    {
        public user user { get; set; }
        public string ErrorMessage { get; set; }
    }
}