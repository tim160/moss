using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common
{
    class UserRole
    {
        public readonly static string SuperUser = "SuperUser";
        public readonly static Int32 SuperUser_Value = 1;

        public readonly static string EC_Top_Mediator = "EC Top Mediator";
        public readonly static Int32 EC_Top_MediatorValue = 2;

        public readonly static string EC_Mediator = "EC Mediator";
        public readonly static Int32 EC_Mediator_Value = 3;

        public readonly static string Escalation_Mediator = "Escalation Mediator";
        public readonly static Int32 Escalation_Mediator_Value = 4;

        public readonly static string Admin_Mediator = "Admin Mediator";
        public readonly static Int32 Admin_Mediator_Value = 5;

        public readonly static string Mediator = "Mediator";
        public readonly static Int32 Mediator_Value = 6;

        public readonly static string Legal_Board_Rep = "Legal Board Representative";
        public readonly static Int32 Legal_Board_Rep_Value = 7;

        public readonly static string Reporter = "Reporter";
        public readonly static Int32 Reporter_Value = 8;
    }
}
