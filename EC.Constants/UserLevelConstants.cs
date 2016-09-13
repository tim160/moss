using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Constants
{
    class UserLevelConstants
    {
        public const string SuperUser = "SuperUser";
        public const Int32 SuperUser_Value = 1;

        public const string EC_Top_Mediator = "EC Top Mediator";
        public const Int32 EC_Top_MediatorValue = 2;

        public const string EC_Mediator = "EC Mediator";
        public const Int32 EC_Mediator_Value = 3;

        public const string Escalation_Mediator = "Escalation Mediator";
        public const Int32 Escalation_Mediator_Value = 4;

        public const string Admin_Mediator = "Admin Mediator";
        public const Int32 Admin_Mediator_Value = 5;

        public const string Mediator = "Mediator";
        public const Int32 Mediator_Value = 6;

        public const string Legal_Board_Rep = "Legal Board Representative";
        public const Int32 Legal_Board_Rep_Value = 7;

        public const string Reporter = "Reporter";
        public const Int32 Reporter_Value = 8;
    }
}
