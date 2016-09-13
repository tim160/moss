using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Constants
{
    class CaseStatusConstants
    {
        public const string Pending = "Pending";
        public const string Review = "Review";
        public const string Investigation = "Investigation";
        public const string Resolution = "Resolution";
        public const string Escalation = "Escalation";
        public const string Closed = "Closed";
        public const string Spam = "Spam";
        public const string Closed_Not_Resolved = "Closed, not resolved";

        public enum CaseStatusValues
        {
            Pending = 1,
            Review = 2,
            Investigation = 3,
            Resolution = 4,
            Escalation = 5,
            Closed = 6,
            Spam = 7,
            Closed_Not_Resolved = 8,

        }
        /*    public enum Incident_Statuses
            {
                Pending = "Pending",
                Review = "Review",
                Investigation = "Investigation",
                Resolution = "Resolution",
                Escalation = "Escalation",
                Closed = "Closed",
                Spam = "Spam",
                Closed_Not_Resolved = "Closed, not resolved",

            }*/
    }
}
