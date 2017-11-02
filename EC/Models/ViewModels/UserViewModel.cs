using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel()
        {

        }
        public UserViewModel(user user)
        {
            User = user;
        }

        public user User { get; set; }

        public string Detail { get; set; }
        public string FullNameWithDetail
        {
            get
            {
                if (String.IsNullOrEmpty(Detail))
                {
                    return $"{User.first_nm} {User.last_nm}";
                }
                return $"{User.first_nm} {User.last_nm} [{Detail}]";
            }
        }

        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", User.first_nm, User.last_nm);
            }
        }
    }
}