//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Principal;
//using System.Web;
//using EC.Models.ECModel;
//
//namespace EC.Utils.Auth
//{
//    public class UserIndentity : IIdentity
//    {
//        public User User { get; set; }
//
//        public string AuthenticationType
//        {
//            get { return typeof (User).ToString(); }
//        }
//
//        public bool IsAuthenticated
//        {
//            get { return User != null; }
//        }
//
//        public string Name
//        {
//            get
//            {
//                if (User != null)
//                {
//                    return User.Email;
//                }
//                //иначе аноним
//                return "anonym";
//            }
//        }
//
//        public void Init(string email, Repo repository)
//        {
//            if (!string.IsNullOrEmpty(email))
//            {
//                User = repository.GetUser(email);
//            }
//        }
//    }
//}