//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Principal;
//using System.Web;
//
//namespace EC.Utils.Auth
//{
//    public class AuthUser : IPrincipal
//    {
//        public User user;
//
//        public AuthUser(User user)
//        {
//            this.user = user;
//        }
//
//        public bool IsInRole(string role)
//        {
//            return role == user.Role;
//        }
//
//        public IIdentity Identity
//        {
//            get { return new EsIIdentity(user != null ? user.Login : null); }
//        }
//    }
//
//    public class EsIIdentity : IIdentity
//    {
//        protected string name;
//        public EsIIdentity(string name)
//        {
//            this.name = name;
//        }
//
//        public string Name
//        {
//            get { return name ?? "anonym"; }
//        }
//
//        public string AuthenticationType
//        {
//            get { return "form"; }
//        }
//
//        public bool IsAuthenticated
//        {
//            get { return name != null; }
//        }
//    }
//}