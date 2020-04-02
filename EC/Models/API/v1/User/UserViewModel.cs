using System.Collections.Generic;

namespace EC.Models.API.v1.User
{
    public class UserViewModel
  {
        public int Total { get; set; }
        public List<UserModel> Items { get; set; }
    }
}