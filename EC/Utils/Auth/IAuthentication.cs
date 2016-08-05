//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Principal;
//using System.Web;
//using EC.Models.ECModel;
//
//namespace EC.Utils.Auth
//{
//    public interface IAuthentication
//    {
//        /// <summary>
//        /// Конекст (тут мы получаем доступ к запросу и кукисам)
//        /// </summary>
//        HttpContext HttpContext { get; set; }
//
//        User Login(string login, string password, bool isPersistent);
//
//        User Login(string login);
//
//        void LogOut();
//
//        IPrincipal CurrentUser { get; }
//    }
//}