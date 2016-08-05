using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces
{
    public enum CaseMessagesType
    {
        All = 0,
        WithReporter = 1,
        BetweenMediators = 2,
        WithLegal = 3
    }

    public enum UserRoles
    {
        All = 0,
        level_superuser = 1,
        level_ec_top_mediator = 2,
        level_ec_mediator = 3,
        level_escalation_mediator = 4,
        level_supervising_mediator = 5,
        level_mediator = 6,
        level_administrator = 7,
        level_informant = 8
    }

    public enum AnonymityLevel
    {
        All = 0,
        Completely_Anonymous = 1,
        Anonymous_to_Company_Only = 2,
        Shared_info = 3
   }

    public enum CaseInvestigationStatus
    {
        All = 0,
        Pending = 1,
        Review = 2,
        Investigation = 3,
        Resolution = 4,
        Escalation = 5,
        Completed = 6,
        Closed = 9,
        Spam = 7
   
    }

}
