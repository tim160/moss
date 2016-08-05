using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models
{
    public class UserIdentifyModel : BaseModel
    {
        public readonly static UserIdentifyModel inst = new UserIdentifyModel();
        public string GetUserRole(user user)
        {
            return (from userTable in db.user
                    from userRole in db.user_role
                    where userTable.role_id == userRole.id &&
                    userTable.role_id == user.role_id
                    select userRole.role_en).FirstOrDefault();
        }
    }
}