using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common
{
    class Incident_Status
    {
        public readonly static string Pending = "Pending";
        public readonly static string Review = "Review";
        public readonly static string Investigation = "Investigation";
        public readonly static string Resolution = "Resolution";
        public readonly static string Escalation = "Escalation";
        public readonly static string Closed = "Closed";
        public readonly static string Spam = "Spam";
        public readonly static string Closed_Not_Resolved = "Closed, not resolved";

        public enum Incident_Statuses_Values
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
