using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.ViewModels
{
    public class UserStatusViewModel
    {
        public int active_button_status = 0;
        public int pending_button_status = 0;
        public int inactive_button_status = 0;
        public int current_user_status = -1; // ili inactive?
    }
}