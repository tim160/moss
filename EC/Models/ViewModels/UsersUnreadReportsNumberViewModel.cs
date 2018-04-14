using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.ViewModels
{
    public class UsersUnreadReportsNumberViewModel
    {
        public int unread_active_reports { get; set; }

        public int unread_pending_reports { get; set; }

        public int unread_spam_reports { get; set; }

        public int unread_completed_reports { get; set; }

        public int unread_closed_reports { get; set; }
    }
}