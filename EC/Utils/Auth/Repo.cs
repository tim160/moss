//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.WebPages;
//using EC.Controllers.Utils.Auth;
//
//namespace EC.Utils.Auth
//{
//    //Stub
//    public class Repo
//    {
//        Dictionary<string,User> users = new Dictionary<string, User>();
//
//        public Repo()
//        {
//            users.Add("test",new User(){Email = "test",Login = "test",Password = "test"});
//        }
//
//        public User Login(string login, string password)
//        {
//            return users[login];
//        }
//
//        public User registration(string name, string pasword)
//        {
//            users.Add(name, new User() { Email = name, Login = name, Password = pasword });
//            return users[name];
//        }
//       public User GetUser(string email)
//       {
//           if (!users.ContainsKey(email)) return null;
//            return users[email];
//        }
//    }
//}