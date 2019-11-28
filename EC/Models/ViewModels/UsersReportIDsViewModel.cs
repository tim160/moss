using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.ViewModels
{
  public class UsersReportIDsViewModel
  {
    public List<int> all_report_ids { get; set; }

    public List<int> all_active_report_ids { get; set; }

    public List<int> all_pending_report_ids { get; set; }

    public List<int> all_spam_report_ids { get; set; }

    public List<int> all_completed_report_ids { get; set; }

    public List<int> all_closed_report_ids { get; set; }
  }

  public class UserInCaseActivity
  {
    public int caseTasksQuantity { get; set; }
    public int caseMessagesQuantity { get; set; }
    public int caseActionsQuantityNoCheck { get; set; }
  }
}