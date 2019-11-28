using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.ViewModels
{
  // https://docs.microsoft.com/en-us/azure/devops/pipelines/ecosystems/javascript?view=azure-devops


  public class UsersUnreadReportsNumberViewModel
  {
    public int unread_active_reports { get; set; }

    public int unread_pending_reports { get; set; }

    public int unread_spam_reports { get; set; }

    public int unread_completed_reports { get; set; }

    public int unread_closed_reports { get; set; }
  }
  public class UsersUnreadEntitiesNumberViewModel
  {
    public int unread_reports { get; set; }

    public int unread_messages { get; set; }

    public int unread_tasks { get; set; }
  }

  public class UsersUnreadReportViewModel
  {
    public int unread_tasks { get; set; }
    public int unread_messages
    {
      get
      {
        return mediatorsUnreaded + reportersUnreaded;
      }
    }
    public int mediatorsUnreaded { get; set; }
    public int reportersUnreaded { get; set; }
  }


}