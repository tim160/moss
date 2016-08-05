//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Security;
//using EC.Models;
//
//namespace EC.Utils.Auth
//{
//    public class EcRoleProvider : RoleProvider
//    {
//        protected UserModel userModel = new UserModel();
//        public override bool IsUserInRole(string username, string roleName)
//        {
//           return userModel.GetUser(username).Role == roleName;
//        }
//
//        public override string[] GetRolesForUser(string username)
//        {
//            return new string[]{userModel.GetUser(username).Role};
//        }
//
//        public override void CreateRole(string roleName)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override bool RoleExists(string roleName)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override string[] GetUsersInRole(string roleName)
//        {
//            throw new NotImplementedException();
//        }
//
//        // -- Snip --
//
//        public override string[] GetAllRoles()
//        {
//            return new string[]{"test","admin"};
//        }
//
//        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override string ApplicationName
//        {
//            get { throw new NotImplementedException(); }
//            set { throw new NotImplementedException(); }
//        }
//
//        // -- Snip --
//    }
//}